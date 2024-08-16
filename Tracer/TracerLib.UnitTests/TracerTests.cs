using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tracers;
using Xunit;

namespace TracerLib.UnitTests
{
    public class Foo
    {
        private ITracer _tracer;

        public Foo(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void M1()
        {
            _tracer.StartTrace();
            Thread.Sleep(100);
            _tracer.StopTrace();
        }

        public void M2()
        {
            _tracer.StartTrace();
            Thread.Sleep(200);
            _tracer.StopTrace();
        }

        public void M3()
        {
            M1();
            M2();
        }

        public void M4()
        {
            _tracer.StartTrace();
            M1();
            _tracer.StopTrace();
        }
    }
    class MethodValidator
    {
        string ClassName { get; }
        string MethodName { get; }
        MethodValidator[] Children { get; }

        public MethodValidator(string className, string methodName, params MethodValidator[] children)
        {
            ClassName = className;
            MethodName = methodName;
            Children = children;
        }

        void AssertValid(IMethodTrace node)
        {
            Assert.Equal(ClassName, node.ClassName);
            Assert.Equal(MethodName, node.MethodName);
            var enumerator = Children.GetEnumerator();
            foreach (var actualMethod in node.Methods)
            {
                Assert.True(enumerator.MoveNext());
                ((MethodValidator)enumerator.Current).AssertValid(actualMethod);
            }
        }

        public static void AssertValid(MethodValidator[] expected, IEnumerable<IMethodTrace> actual)
        {
            var enumerator = expected.GetEnumerator();
            foreach (var actualMethod in actual)
            {
                Assert.True(enumerator.MoveNext());
                ((MethodValidator)enumerator.Current).AssertValid(actualMethod);
            }
            Assert.False(enumerator.MoveNext());
        }
    }

    public class TracerTests
    {
        [Fact]
        public void TestTracerInSingleThread()
        {
            var tracer = new Tracer();

            var foo = new Foo(tracer);
            foo.M3();

            MethodValidator[] expectedMethods = new MethodValidator[]
            {
                new MethodValidator("Foo", "M1"),
                new MethodValidator("Foo", "M2"),
            };

            IEnumerable<IThreadTrace> result = tracer.GetResult();
            IEnumerator<IThreadTrace> nodes = result.GetEnumerator();
            Assert.True(nodes.MoveNext());
            IThreadTrace node = nodes.Current;
            Assert.False(nodes.MoveNext()); //one thread
            Assert.Equal(node.Thread, Thread.CurrentThread);
            MethodValidator.AssertValid(expectedMethods, node.Methods);
        }

        [Fact]
        public void TestTracerInMultyThread()
        {
            var tracer = new Tracer();

            var foo = new Foo(tracer);

            var m1 = new MethodValidator("Foo", "M1");
            var m2 = new MethodValidator("Foo", "M2");
            var task = Task.WhenAll(
                Task.Run(() =>
                {
                    foo.M3();
                    return (Thread.CurrentThread, new MethodValidator[] { m1, m2 });
                }),
                Task.Run(() =>
                {
                    foo.M4();
                    return (Thread.CurrentThread, new MethodValidator[] { new MethodValidator("Foo", "M4", m1, m2) });
                }),
                Task.Run(() =>
                {
                    foo.M2();
                    return (Thread.CurrentThread, new MethodValidator[] { m2 });
                })
            );
            task.Wait();

            var expectedResult = task.Result;
            var actualResult = tracer.GetResult();
            foreach (var pair in expectedResult)
            {
                IThreadTrace node = null;
                foreach (var n in actualResult)
                {
                    if (Equals(pair.Item1, n.Thread))
                    {
                        node = n;
                        break;
                    }
                }
                Assert.NotNull(node);
                MethodValidator.AssertValid(pair.Item2, node.Methods);
            }
        }
    }
}
