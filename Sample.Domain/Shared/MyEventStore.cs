using System;
using System.Collections.Generic;
using System.Linq;
using Sample.Domain.V6;

namespace Sample.Domain.Shared
{
    public class MyEventStore<T> where T : AggregateRoot
    {
        public void Add(T item)
        {
            Printer.Print(ConsoleColor.Yellow);

            var aggregate = Cloner.DeepClone(item);

            if (!Data.Events.ContainsKey(typeof(T)))
                Data.Events.Add(typeof(T), new Dictionary<Guid, List<DomainEvent>>());

            if (Data.Events[typeof(T)].ContainsKey(aggregate.Id))
            {
                if (aggregate.CommittedEvents.Count != Data.Events[typeof (T)][aggregate.Id].Count)
                {
                    var missingEvents = Data.Events[typeof (T)][aggregate.Id].Where(e => !aggregate.CommittedEvents.Any(ae => e.EventId == ae.EventId)).ToList();

                    if (!aggregate.TryResolveConflicts(missingEvents))
                    {
                        var eventsMissed = string.Join(", ", missingEvents.Select(e => e.GetType().Name + " by: " + e.By + " at: " + e.At));
                        var uncommittedEvents = string.Join(", ", aggregate.UncommittedEvents.Select(e => e.GetType().Name));
                        throw new Exception("Concurrency Exception, missed the following events: " + eventsMissed + " conflicted with: " + uncommittedEvents);
                    }
                    else
                    {
                        var eventsMissed = string.Join(", ", missingEvents.Select(e => e.GetType().Name + " by: " + e.By + " at: " + e.At));
                        var uncommittedEvents = string.Join(", ", aggregate.UncommittedEvents.Select(e => e.GetType().Name));
                        Printer.Print("Concurrency Resolved Successfully for events: " + eventsMissed + " merged with: " + uncommittedEvents, ConsoleColor.Yellow);
                    }
                }
                Data.Events[typeof(T)][aggregate.Id].AddRange(aggregate.UncommittedEvents);
            }
            else
            {
                Data.Events[typeof(T)].Add(aggregate.Id, aggregate.UncommittedEvents);
            }

            item.CommittedEvents.AddRange(item.UncommittedEvents);

            foreach (var uncommittedEvent in item.UncommittedEvents)
                DomainEvents.Publish(uncommittedEvent);

            item.UncommittedEvents.Clear();

        }

        public T Fetch(Guid id)
        {
            return (T)Activator.CreateInstance(typeof(T), id, Data.Events[typeof(T)][id]);
        }
    }
}