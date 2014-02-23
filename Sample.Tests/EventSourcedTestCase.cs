using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Domain.Shared;

namespace Sample.Tests
{
    public abstract class EventSourcedTestCase<T> where T : AggregateRoot
    {
        protected IGiven Given(Guid id, params DomainEvent[] events)
        {
            var instance = (T)Activator.CreateInstance(typeof(T), id, events.ToList());
            return new TestHelper(instance);
        }

        private class TestHelper : IGiven, IWhen, IAndWhen
        {
            private readonly T instance;
            private Exception exception;

            public TestHelper(T instance)
            {
                this.instance = instance;
            }

            public IWhen When(Action<T> action)
            {
                try
                {
                    action(instance);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                return this;
            }

            IAndWhen IWhen.Then(params DomainEvent[] events)
            {
                Assert.IsNull(exception, "Expected no exception");
                Assert.AreEqual(events.Length, instance.UncommittedEvents.Count(), "Expected {0} events", events.Length);
                Assert.IsTrue(events.Select(e => e.GetType()).SequenceEqual(instance.UncommittedEvents.Select(e => e.GetType())));

                return this;
            }

            public IAndWhen Then(Action<T> action)
            {
                action.Invoke(instance);
                return this;
            }

            public IAndWhen ThenItThrows<T>()
            {
                Assert.IsNotNull(exception, "Expected an exception");
                Assert.AreEqual(typeof(T), exception.GetType());
                return this;
            }

            public void And(params DomainEvent[] events)
            {
                Assert.IsNull(exception, "Expected no exception");
                Assert.AreEqual(events.Length, instance.UncommittedEvents.Count(), "Expected {0} events", events.Length);
                Assert.IsTrue(events.SequenceEqual(instance.UncommittedEvents));
            }

            public void And(Action<T> action)
            {
                action.Invoke(instance);
            }

            public void AndItThrows<T1>()
            {
                Assert.IsNotNull(exception, "Expected an exception");
                Assert.AreEqual(typeof(T), exception.GetType());
            }
        }

        protected interface IGiven
        {
            IWhen When(Action<T> action);
        }

        protected interface IWhen
        {
            IAndWhen Then(params DomainEvent[] events);
            IAndWhen Then(Action<T> action);
            IAndWhen ThenItThrows<T>();
        }

        protected interface IAndWhen
        {
            void And(params DomainEvent[] events);
            void And(Action<T> action);
            void AndItThrows<T>();
        }
    }
}