using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GridDemo.Models
{
    /// <summary>
    /// Trivial repository of random data for demo purposes. Not for use in a real application!
    /// </summary>
    public class Repository
    {
        private static IQueryable<SomeEntity> _data;

        public IQueryable<SomeEntity> SelectAll()
        {
            return _data;
        }

        /// <summary>
        /// Demo data is static so that we can demonstrate sorting with the same data for the whole lifetime of the application.
        /// </summary>
        static Repository()
        {
            var data = new List<SomeEntity>();
            var rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                data.Add(new SomeEntity
                    {
                        Id = i,
                        IntProperty = rand.Next(99),
                        StringProperty = rand.Next(99).ToString(),
                        DateProperty = new DateTime(2000 + rand.Next(10), rand.Next(11) + 1, rand.Next(27) + 1)
                    });
            }
            _data = data.AsQueryable();
        }
    }
}
