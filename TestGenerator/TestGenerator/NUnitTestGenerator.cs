using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TestGenerator
{
    public class NUnitTestGenerator : ITestGenerator
    {
        private static readonly List<UsingDirectiveSyntax> DefaultLoadDirectiveList = new()
        {
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Linq")),
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Text")),
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("NUnit.Framework"))
        };

        private const string Whitespace = "\t";

        private readonly int maxRead;
        private readonly int maxGenerate;
        private readonly int maxWrite;

        public NUnitTestGenerator(int maxRead, int maxGenerate, int maxWrite)
        {
            this.maxRead = maxRead;
            this.maxGenerate = maxGenerate;
            this.maxWrite = maxWrite;
        }

        public Task GenerateTestsAsync(string inputDirectory, string outputDirectory)
        {
            if (!Directory.Exists(inputDirectory))
            {
                throw new ArgumentException($"Input directory: '{inputDirectory}' doesn't exist");
            }
            if (!Directory.Exists(outputDirectory))
            {
                throw new ArgumentException($"Output directory: '{outputDirectory}' doesn't exist");
            }

            var drOpt = new ExecutionDataflowBlockOptions();
            var directoryReader = new TransformManyBlock<string, string>(
                dir => Directory.EnumerateFiles(dir).ToArray(), drOpt);

            var frOpt = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxRead
            };
            var fileReader = new TransformBlock<string, string>(ReadFileAsync, frOpt);


            var gOpt = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxGenerate
            };
            var generator = new TransformManyBlock<string, (string, string)>(CreateTests, gOpt);

            
            var wOpt = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxWrite
            };
            var writer = new ActionBlock<(string, string)>(code => WriteTest(outputDirectory, code), wOpt);

            var linkOpt = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };
            directoryReader.LinkTo(fileReader, linkOpt);
            fileReader.LinkTo(generator, linkOpt);
            generator.LinkTo(writer, linkOpt);

            directoryReader.Post(inputDirectory);
            directoryReader.Complete();
            return writer.Completion;
        }

        private static Task<string> ReadFileAsync(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException("File doesn't exist");
            }

            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.Asynchronous);
            using var reader = new StreamReader(fs, Encoding.UTF8);
            return reader.ReadToEndAsync();
        }
        private static Task WriteFileAsync(string path, string text)
        {
            using var outputFile = new StreamWriter(path);
            return outputFile.WriteAsync(text);
        }

        private static IEnumerable<(string, string)> CreateTests(string code)
        {
            var classes = CSharpSyntaxTree.ParseText(code)
                .GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(@class => @class.Modifiers.Any(SyntaxKind.PublicKeyword))
                .Where(@class => !@class.Modifiers.Any(SyntaxKind.StaticKeyword)).ToArray();
            return classes.Select(@class => (@class.Identifier.Text, CreateTest(@class))).ToArray();
        }

        private static string CreateTest(TypeDeclarationSyntax classDeclaration)
        {
            var unit = SyntaxFactory.CompilationUnit();
            unit = DefaultLoadDirectiveList.Aggregate(unit,
                (current, loadDirective) => current.AddUsings(loadDirective));

            var @namespace = SyntaxFactory
                .NamespaceDeclaration(SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("GeneratedNamespace"),
                    SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(SyntaxFactory.TriviaList(), "Tests",
                        SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed)))))
                .WithNamespaceKeyword(SyntaxFactory.Token(
                    SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed,
                        SyntaxFactory.CarriageReturnLineFeed), SyntaxKind.NamespaceKeyword,
                    SyntaxFactory.TriviaList(SyntaxFactory.Space)))
                .WithOpenBraceToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.OpenBraceToken,
                    SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed)))
                .WithCloseBraceToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed),
                    SyntaxKind.CloseBraceToken, SyntaxFactory.TriviaList())).AddMembers(SyntaxFactory
                    .ClassDeclaration(SyntaxFactory.Identifier(SyntaxFactory.TriviaList(),
                        classDeclaration.Identifier.Text + "Test",
                        SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed)))
                    .WithAttributeLists(SyntaxFactory.SingletonList(SyntaxFactory
                        .AttributeList(SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("TestFixture"))))
                        .WithOpenBracketToken(SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed,
                                SyntaxFactory.Whitespace(Whitespace)), SyntaxKind.OpenBracketToken,
                            SyntaxFactory.TriviaList()))
                        .WithCloseBracketToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(),
                            SyntaxKind.CloseBracketToken,
                            SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed)))))
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(Whitespace)), SyntaxKind.PublicKeyword,
                        SyntaxFactory.TriviaList(SyntaxFactory.Space))))
                    .WithKeyword(SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.ClassKeyword,
                        SyntaxFactory.TriviaList(SyntaxFactory.Space)))
                    .WithOpenBraceToken(SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(Whitespace)), SyntaxKind.OpenBraceToken,
                        SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed))).WithCloseBraceToken(
                        SyntaxFactory.Token(SyntaxFactory.TriviaList(new[]
                        {
                            SyntaxFactory.Whitespace(Whitespace),
                        }), SyntaxKind.CloseBraceToken, SyntaxFactory.TriviaList()))
                    .AddMembers(AddTestMethods(classDeclaration)));

            return unit.NormalizeWhitespace().AddMembers(@namespace).ToFullString();
        }

        private static MemberDeclarationSyntax[] AddTestMethods(SyntaxNode classDeclaration)
        {
            var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(method => method.Modifiers.Any(SyntaxKind.PublicKeyword));

            return methods.Select(method => SyntaxFactory
                .MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.VoidKeyword,
                        SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                    SyntaxFactory.Identifier(method.Identifier.Text + "Test"))
                .WithAttributeLists(SyntaxFactory.SingletonList(SyntaxFactory
                    .AttributeList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Test"))))
                    .WithOpenBracketToken(SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(Whitespace + Whitespace)),
                        SyntaxKind.OpenBracketToken, SyntaxFactory.TriviaList()))
                    .WithCloseBracketToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.CloseBracketToken,
                        SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)))))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(
                    SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(Whitespace + Whitespace)),
                    SyntaxKind.PublicKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space))))
                .WithParameterList(SyntaxFactory.ParameterList().WithCloseParenToken(
                    SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.CloseParenToken,
                        SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))))
                .WithBody(SyntaxFactory.Block()
                    .WithOpenBraceToken(SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(Whitespace + Whitespace)),
                        SyntaxKind.OpenBraceToken, SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)))
                    .WithCloseBraceToken(SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(Whitespace + Whitespace)),
                        SyntaxKind.CloseBraceToken,
                        SyntaxFactory.TriviaList(SyntaxFactory.LineFeed, SyntaxFactory.Whitespace(""),
                            SyntaxFactory.LineFeed)))).AddBodyStatements(SyntaxFactory
                    .ExpressionStatement(SyntaxFactory
                        .InvocationExpression(SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(
                                SyntaxFactory.TriviaList(
                                    SyntaxFactory.Whitespace(Whitespace + Whitespace + Whitespace)), "Assert",
                                SyntaxFactory.TriviaList())), SyntaxFactory.IdentifierName("Fail"))).WithArgumentList(
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("autogenerated")))))))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.SemicolonToken,
                        SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))))).Cast<MemberDeclarationSyntax>().ToArray();
        }

        private static Task WriteTest(string outputDirectory, (string, string) nameTestPair)
        {
            return WriteFileAsync(Path.Combine(outputDirectory, nameTestPair.Item1 + ".cs"), nameTestPair.Item2);
        }
    }
}