using Core.Faker;
using System;
using Xunit;

namespace Core.UnitTests
{
    public class TestCyclicDependency
    {
        [Fact]
        public void Test()
        {
            CyclicDependency.Validate(typeof(long));
            Assert.Throws<Exception>(() => CyclicDependency.Validate(typeof(DateTime)));
        }
    }
}
