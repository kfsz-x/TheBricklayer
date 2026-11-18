using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityCore.Tests
{
    public class ResourceTests
    {
        [UnityTest]
        public IEnumerator ConcurrentLeasesKeepTheAssetAliveUntilLastRelease()
        {
            using var resources = new ResourceStore();
            var first = resources.AcquireAsync<GameObject>("sample/cube");
            var second = resources.AcquireAsync<GameObject>("sample/cube");
            yield return new WaitUntil(() => first.IsCompleted && second.IsCompleted);
            using var a = first.GetAwaiter().GetResult();
            using var b = second.GetAwaiter().GetResult();
            Assert.That(a.Asset, Is.SameAs(b.Asset));

            a.Dispose();
            using var c = resources.AcquireAsync<GameObject>("sample/cube").GetAwaiter().GetResult();
            Assert.That(c.Asset, Is.SameAs(b.Asset));
            Assert.Throws<ObjectDisposedException>(() => { _ = a.Asset; });
        }

        [UnityTest]
        public IEnumerator DisposingStoreRejectsPendingAndFutureAcquisitions()
        {
            var resources = new ResourceStore();
            var pending = resources.AcquireAsync<GameObject>("sample/cube");
            resources.Dispose();
            yield return new WaitUntil(() => pending.IsCompleted);
            if (pending.IsCompletedSuccessfully)
                Assert.Throws<ObjectDisposedException>(() => { _ = pending.Result.Asset; });
            else
                Assert.Throws<ObjectDisposedException>(() => pending.GetAwaiter().GetResult());

            var after = resources.AcquireAsync<GameObject>("sample/cube");
            Assert.Throws<ObjectDisposedException>(() => after.GetAwaiter().GetResult());
        }

        [UnityTest]
        public IEnumerator PreloadedLabelReturnsTheTwoSampleAudioClips()
        {
            using var resources = new ResourceStore();
            var load = resources.PreloadAsync<AudioClip>("sample-audio");
            yield return new WaitUntil(() => load.IsCompleted);
            using var lease = load.GetAwaiter().GetResult();
            CollectionAssert.AreEquivalent(new[] { "btn", "bgmmain" },
                System.Linq.Enumerable.Select(lease.Asset, clip => clip.name));
        }

        [UnityTest]
        public IEnumerator MissingKeyCanFailTwiceWithoutLeavingAPendingEntry()
        {
            using var resources = new ResourceStore();
            for (var attempt = 0; attempt < 2; attempt++)
            {
                LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("InvalidKeyException"));
                var load = resources.AcquireAsync<GameObject>("sample/invalid");
                yield return new WaitUntil(() => load.IsCompleted);
                Assert.That(load.IsFaulted, Is.True);
            }
        }
    }
}
