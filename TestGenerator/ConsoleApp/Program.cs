using System;
using System.Threading;
using TestGenerator;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new NUnitTestGenerator(10, 10, 10);
            generator.GenerateTestsAsync(@"C:\Users\Zahar\source\repos\bsuir\3_1\MPP\TestGenerator\ConsoleApp\input",
                @"C:\Users\Zahar\source\repos\bsuir\3_1\MPP\TestGenerator\ConsoleApp\output").Wait();
        }
    }
}