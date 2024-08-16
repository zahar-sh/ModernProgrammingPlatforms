using System.Threading;
using System.Threading.Tasks;

namespace TestGenerator
{
    public interface ITestGenerator
    {
        Task GenerateTestsAsync(string inputDirectory, string outputDirectory);
    }
}
