using Core.Faker;
using System;
using System.Collections.Generic;
using Xunit;

namespace FakerTests
{
    public class TestFakers
    {
        [Fact]
        public void Test()
        {
            IFaker faker = new FakerImpl();

            double value = faker.Create<double>();
            Assert.True(double.IsFinite(value));

            var list = faker.Create<List<List<int>>>();
            Assert.NotNull(list);
            Assert.NotEmpty(list);


            double[] numbers = faker.Create<double[]>();
            Assert.NotNull(numbers);
            Assert.NotEmpty(numbers);
        }
    }
}
