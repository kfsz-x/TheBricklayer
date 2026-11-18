using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityCore.Tests.EditMode
{
    public sealed class UtilityTests
    {
        [Test]
        public void PoolKeepsSameNamedPrefabsSeparate()
        {
            var parent = new GameObject("Pool parent");
            var firstPrefab = new GameObject("Shared name");
            var secondPrefab = new GameObject("Shared name");
            try
            {
                using var pool = new PrefabPool(parent.transform);
                var first = pool.Rent(firstPrefab);
                Assert.That(pool.Return(first), Is.True);
                Assert.That(first.activeSelf, Is.False);
                var second = pool.Rent(secondPrefab);
                Assert.That(second, Is.Not.SameAs(first));
                Assert.That(pool.Rent(firstPrefab), Is.SameAs(first));
                Assert.That(first.activeSelf, Is.True);
                Assert.That(first.transform.parent, Is.EqualTo(parent.transform));
            }
            finally
            {
                Object.DestroyImmediate(parent);
                Object.DestroyImmediate(firstPrefab);
                Object.DestroyImmediate(secondPrefab);
            }
        }

        [Test]
        public void PoolRejectsDuplicateForeignAndDestroyedReturns()
        {
            var prefab = new GameObject("Prefab");
            try
            {
                using var pool = new PrefabPool(null);
                var instance = pool.Rent(prefab);
                Assert.That(pool.Return(prefab), Is.False);
                Assert.That(pool.Return(instance), Is.True);
                Assert.That(pool.Return(instance), Is.False);
                Object.DestroyImmediate(instance);
                Assert.That(pool.Return(instance), Is.False);
                var replacement = pool.Rent(prefab);
                Assert.That(replacement == null, Is.False);
                Assert.That(replacement, Is.Not.SameAs(instance));
                Object.DestroyImmediate(replacement);
                Assert.That(pool.Return(replacement), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(prefab);
            }
        }

        [Test]
        public void PoolDisposalDestroysIdleAndActiveInstances()
        {
            var prefab = new GameObject("Prefab");
            try
            {
                using var pool = new PrefabPool(null);
                var active = pool.Rent(prefab);
                var idle = pool.Rent(prefab);
                pool.Return(idle);
                pool.Dispose();

                Assert.That(active == null, Is.True);
                Assert.That(idle == null, Is.True);
                Assert.That(prefab == null, Is.False);
                Assert.Throws<ObjectDisposedException>(() => pool.Rent(prefab));
            }
            finally
            {
                Object.DestroyImmediate(prefab);
            }
        }

        [Test]
        public void EventSubscriptionsAreTypedAndIndependentlyDisposable()
        {
            var bus = new EventBus();
            var received = new List<int>();
            Action<int> handler = received.Add;
            using var first = bus.Subscribe(handler);
            using var second = bus.Subscribe(handler);

            bus.Publish("other type");
            bus.Publish(5);
            first.Dispose();
            first.Dispose();
            bus.Publish(6);
            Assert.That(received, Is.EqualTo(new[] { 5, 5, 6 }));
        }

        [Test]
        public void EventSubscriptionChangesAffectSubsequentPublishes()
        {
            var bus = new EventBus();
            var received = new List<int>();
            using var first = bus.Subscribe<int>(_ => bus.Clear());
            using var second = bus.Subscribe<int>(received.Add);

            bus.Publish(1);
            using var replacement = bus.Subscribe<int>(received.Add);
            second.Dispose();
            bus.Publish(2);

            Assert.That(received, Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public void BindablePropertyNotifiesOnlyWhenValueChanges()
        {
            var property = new BindableProperty<int>(3);
            var observed = new List<int>();
            property.OnChangeBefore += () => observed.Add(property.Value);
            property.OnChange += () => observed.Add(property.Value);

            property.Value = 3;
            property.Value = 5;
            int current = property;
            Assert.That(current, Is.EqualTo(5));
            Assert.That(observed, Is.EqualTo(new[] { 3, 5 }));

            property.Clear();
            Assert.That(property.Value, Is.Zero);
            Assert.That(observed, Has.Count.EqualTo(2));
        }

        [Test]
        public void LocalStoreIsolatesPrefixesAndRestoresFallbacksAfterDeletion()
        {
            var prefix = Guid.NewGuid().ToString("N");
            var first = new LocalStore(prefix + ".first");
            var second = new LocalStore(prefix + ".second");
            try
            {
                first.SetInt("count", 42);
                first.SetFloat("scale", 1.5f);
                first.SetString("name", "Wall");
                first.SetBool("active", true);
                first.Save();

                var reopened = new LocalStore(prefix + ".first");
                Assert.That(reopened.GetInt("count"), Is.EqualTo(42));
                Assert.That(reopened.GetFloat("scale"), Is.EqualTo(1.5f));
                Assert.That(reopened.GetString("name"), Is.EqualTo("Wall"));
                Assert.That(reopened.GetBool("active"), Is.True);
                Assert.That(second.GetInt("count", 8), Is.EqualTo(8));
                Assert.That(second.GetFloat("scale", 2f), Is.EqualTo(2f));
                Assert.That(second.GetString("name", "Default"), Is.EqualTo("Default"));
                Assert.That(second.GetBool("active"), Is.False);

                reopened.Delete("active");
                Assert.That(first.GetBool("active"), Is.False);
                Assert.That(first.GetBool("active", true), Is.True);
            }
            finally
            {
                foreach (var key in new[] { "count", "scale", "name", "active" })
                    first.Delete(key);
                first.Save();
            }
        }
    }
}
