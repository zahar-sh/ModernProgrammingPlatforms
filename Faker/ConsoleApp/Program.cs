using Core.Faker;
using System;
using System.Collections.Generic;

namespace FakerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var faker = new FakerImpl();

                double value = faker.Create<double>();
                Console.WriteLine(value);
                Console.WriteLine();

                var list = faker.Create<List<List<int>>>();
                foreach (var values in list)
                {
                    foreach (var v in values)
                    {
                        Console.WriteLine(v);
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
