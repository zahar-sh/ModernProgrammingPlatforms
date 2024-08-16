using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Tracers
{
    class ThreadTrace : IThreadTrace
    {
        private Thread _thread;
        private TimeSpan _deltaTime;
        private IEnumerable<IMethodTrace> _methods;

        public ThreadTrace(Thread thread, TimeSpan deltaTime, IEnumerable<IMethodTrace> methods)
        {
            _thread = thread;
            _deltaTime = deltaTime;
            _methods = methods;
        }

        public Thread Thread => _thread;

        public TimeSpan DeltaTime => _deltaTime;

        public IEnumerable<IMethodTrace> Methods => _methods;
    }

    class Method : IMethodTrace
    {
        public int depth;
        public StackTrace stackTrace;
        public DateTime start, end;
        public List<Method> children;

        public Type Class => MethodBase.ReflectedType;

        public MethodBase MethodBase => stackTrace.GetFrame(depth).GetMethod();

        public TimeSpan DeltaTime => end - start;

        public IEnumerable<IMethodTrace> Methods => children;
    }

    class MethodTreaceHelper
    {
        public readonly Stack<Method> stack = new Stack<Method>();
        public readonly List<Method> methods = new List<Method>();
    }

    public class Tracer : ITracer
    {
        private readonly IDictionary<Thread, MethodTreaceHelper> pairs = new ConcurrentDictionary<Thread, MethodTreaceHelper>();
        private readonly int _depth;

        public Tracer(int depth = 1)
        {
            if (depth < 1)
                throw new ArgumentException(nameof(depth) + " should be >= 1");
            _depth = depth;
        }

        public void StartTrace()
        {
            var method = new Method();
            var thread = Thread.CurrentThread;
            MethodTreaceHelper node;
            if (pairs.ContainsKey(thread))
            {
                node = pairs[thread];
            }
            else
            {
                node = new MethodTreaceHelper();
                pairs.Add(thread, node);
            }

            var stack = node.stack;
            stack.Push(method);
            method.depth = _depth;
            method.stackTrace = new StackTrace();
            method.children = new List<Method>();
            method.start = DateTime.Now;
        }

        public void StopTrace()
        {
            var endTime = DateTime.Now;

            var thread = Thread.CurrentThread;
            var node = pairs[thread];

            var stack = node.stack;
            if (stack.Count == 0)
            {
                throw new Exception("Stop trace called without start trace method");
            }

            var method = stack.Peek();
            var startStackTrace = method.stackTrace;
            var currentStackTrace = new StackTrace();

            if (!Equals(startStackTrace.GetFrame(_depth)?.GetMethod(),
                currentStackTrace.GetFrame(_depth)?.GetMethod()))
            {
                throw new Exception($"Start trace and stop trace called in differen methods:\n{startStackTrace}\n{currentStackTrace}");
            }

            method = stack.Pop();
            method.end = endTime;

            if (stack.Count == 0)
            {
                node.methods.Add(method);
            }
            else
            {
                var parent = stack.Peek();
                parent.children.Add(method);
            }
        }

        public IEnumerable<IThreadTrace> GetResult()
        {
            return pairs.Select(pair =>
            {
                var elapsedTime = TimeSpan.Zero;
                var methods = pair.Value.methods;
                foreach (IMethodTrace method in methods)
                {
                    elapsedTime += method.DeltaTime;
                }
                return new ThreadTrace(pair.Key, elapsedTime, methods);
            }).ToArray();
        }

        public void ClearMyHistory()
        {
            var thread = Thread.CurrentThread;
            if (pairs.ContainsKey(thread))
            {
                var node = pairs[thread];
                node.stack.Clear();
                node.methods.Clear();
            }
        }
    }
}
