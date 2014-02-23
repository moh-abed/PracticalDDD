using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample.Domain.Shared
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
    }

    [Serializable]
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; private set; }
        public DateTime At { get; private set; }
        public string By { get; private set; }

        protected DomainEvent(string by)
        {
            EventId = Guid.NewGuid();
            At = DateTime.Now;
            By = by;
        }
    }

    public static class UserProfile
    {
        public static string Name { get; set; }

        static UserProfile()
        {
            Name = "John";
        }
    }

    [Serializable]
    public abstract class AggregateRoot
    {
        public Guid Id { get; protected set; }

        public List<DomainEvent> UncommittedEvents { get; set; }
        internal List<DomainEvent> CommittedEvents { get; set; }

        protected AggregateRoot(Guid id)
        {
            Id = id;
            UncommittedEvents = new List<DomainEvent>();
            CommittedEvents = new List<DomainEvent>();
        }

        protected AggregateRoot(Guid id, List<DomainEvent> events)
            : this(id)
        {
            CommittedEvents = events;
            foreach (var @event in events)
            {
                (this as dynamic).Handle((dynamic)@event);
            }
        }

        protected void Apply(DomainEvent @event)
        {
            (this as dynamic).Handle((dynamic)@event);
            UncommittedEvents.Add(@event);
        }

        public virtual bool TryResolveConflicts(IEnumerable<DomainEvent> missingEvents)
        {
            return !missingEvents.Any();
        }
    }
}
