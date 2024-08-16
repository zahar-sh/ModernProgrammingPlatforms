using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TRSerializer;
using Tracers;

namespace TracerApp
{
    public class Foo
    {
        private Bar _bar;
        private ITracer _tracer;

        internal Foo(ITracer tracer)
        {
            _tracer = tracer;
            _bar = new Bar(_tracer);
        }

        public void MyMethod()
        {
            _tracer.StartTrace();
            _bar.InnerMethod();
            _tracer.StopTrace();
        }
    }

    public class Bar
    {
        private ITracer _tracer;

        internal Bar(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void InnerMethod()
        {
            _tracer.StartTrace();
            Thread.Sleep(200);
            _tracer.StopTrace();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tracer = new Tracer();

            var bar = new Bar(tracer);
            var foo = new Foo(tracer);
            var task = Task.WhenAll(
               Task.Run(() => foo.MyMethod()),
               Task.Run(() => bar.InnerMethod()),
               Task.Run(() => foo.MyMethod())
           );

            task.Wait();

            var fileName = "TraceResult";
            SaveToJson(fileName, tracer);
            SaveToXML(fileName, tracer);
        }


        static void SaveToJson(string fileName, ITracer tracer)
        {
            using var fs = new FileStream(fileName + ".json", FileMode.OpenOrCreate);
            var saver = new JsonSerializer();
            saver.Save(fs, tracer.GetResult());
        }

        static void SaveToXML(string fileName, ITracer tracer)
        {
            using var fs = new FileStream(fileName + ".xml", FileMode.OpenOrCreate);
            var saver = new XmlSerializer();
            saver.Save(fs, tracer.GetResult());
        }
    }
}
