using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Tracers;

namespace TRSerializer
{
    public class JsonSerializer : ISerializer
    {
        public void Save(Stream output, IEnumerable<IThreadTrace> traceResult)
        {
            using var writer = new Utf8JsonWriter(output);
            System.Text.Json.JsonSerializer.Serialize(writer, traceResult);
        }
    }
}
