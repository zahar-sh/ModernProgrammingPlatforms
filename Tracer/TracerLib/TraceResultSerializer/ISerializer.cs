using System.Collections.Generic;
using System.IO;
using Tracers;

namespace TRSerializer
{
    public interface ISerializer
    {
        void Save(Stream output, IEnumerable<IThreadTrace> traceResult);
    }
}
