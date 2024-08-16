using System.Collections.Generic;

namespace Tracers
{
    public interface ITracer
    {
        void StartTrace();

        void StopTrace();

        IEnumerable<IThreadTrace> GetResult();
    }
}
