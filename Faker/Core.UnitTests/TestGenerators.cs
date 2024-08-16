using Core.Generator;
using System;
using System.Collections.Generic;
using Xunit;

namespace FakerTests
{
    public class TestGenerators
    {
        [Fact]
        public void TestPrimitiveGenerator()
        {
            IGenerator generator = new PrimitiveGenerator();
            Assert.True(generator.CanGenerate(typeof(int)));
            Assert.True(generator.CanGenerate(typeof(double)));
            Assert.True(generator.CanGenerate(typeof(bool)));
            Assert.True(generator.CanGenerate(typeof(uint)));
            Assert.False(generator.CanGenerate(typeof(DateTime)));
        }

        [Fact]
        public void TestArrayGenerator()
        {
            IGenerator generator = new ArrayGenerator();
            Assert.True(generator.CanGenerate(typeof(double[][])));
            Assert.False(generator.CanGenerate(typeof(DateTime)));
        }

        [Fact]
        public void TestListGenerator()
        {
            IGenerator generator = new ListGenerator();
            Assert.True(generator.CanGenerate(typeof(List<List<int>>)));
            Assert.False(generator.CanGenerate(typeof(DateTime)));
        }
    }
}
