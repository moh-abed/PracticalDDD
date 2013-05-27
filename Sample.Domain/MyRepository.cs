using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample.Domain
{
    public static class Data
    {
        public static Dictionary<Type, List<dynamic>> DB = new Dictionary<Type, List<dynamic>>();
    }

    public class MyRepository<T> where T : class
    {
        public void Add(T item)
        {
            Printer.Print(ConsoleColor.Yellow);

            if (Data.DB.ContainsKey(typeof (T)))
            {
                Data.DB[typeof(T)].Add(item);
            }
            else
            {
                Data.DB.Add(typeof(T), new List<dynamic> { item });
            }

        }

        public void Update(T item)
        {
            Printer.Print(ConsoleColor.Yellow);

            var itemToUpdate = Data.DB[typeof(T)].Single(i => i.Id == ((dynamic)item).Id);
            Data.DB[typeof(T)].Remove(itemToUpdate);
            Data.DB[typeof(T)].Add(item);
        }

        public T Fetch(Guid id)
        {
            return Data.DB[typeof(T)].Single(i => i.Id == id);
        }

        public IEnumerable<T> FetchAll()
        {
            return Data.DB[typeof(T)].Select(i => (T)i);
        }
    }
}
