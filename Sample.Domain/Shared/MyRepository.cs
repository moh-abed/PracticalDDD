using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sample.Domain.Shared
{
    public static class Data
    {
        public static Dictionary<Type, List<dynamic>> DB = new Dictionary<Type, List<dynamic>>();
        public static Dictionary<Type, Dictionary<Guid, List<DomainEvent>>> Events = new Dictionary<Type, Dictionary<Guid, List<DomainEvent>>>();
    }

    public class MyRepository<T> where T : class
    {
        public void Add(T item)
        {
            //Printer.Print(ConsoleColor.Yellow);

            if (Data.DB.ContainsKey(typeof(T)))
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
            //Printer.Print(ConsoleColor.Yellow);

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

    public static class Cloner
    {
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
