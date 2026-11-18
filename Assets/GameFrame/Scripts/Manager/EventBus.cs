using System;
using System.Collections.Generic;

namespace UnityCore
{
    public sealed class EventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public IDisposable Subscribe<T>(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            // A distinct delegate lets duplicate registrations be disposed independently.
            Action<T> registration = message => handler(message);
            var type = typeof(T);
            _handlers.TryGetValue(type, out var existing);
            _handlers[type] = Delegate.Combine(existing, registration);
            return new Subscription(() => Unsubscribe(type, registration));
        }

        public void Publish<T>(T message)
        {
            // Multicast delegates provide a snapshot even if handlers modify subscriptions.
            if (_handlers.TryGetValue(typeof(T), out var handlers))
                ((Action<T>)handlers)(message);
        }

        public void Clear() => _handlers.Clear();

        private void Unsubscribe(Type type, Delegate registration)
        {
            if (!_handlers.TryGetValue(type, out var existing))
                return;

            var remaining = Delegate.Remove(existing, registration);
            if (remaining == null)
                _handlers.Remove(type);
            else
                _handlers[type] = remaining;
        }

        private sealed class Subscription : IDisposable
        {
            private Action _unsubscribe;

            public Subscription(Action unsubscribe)
            {
                _unsubscribe = unsubscribe;
            }

            public void Dispose()
            {
                _unsubscribe?.Invoke();
                _unsubscribe = null;
            }
        }
    }
}
