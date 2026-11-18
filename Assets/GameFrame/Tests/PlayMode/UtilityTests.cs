using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityCore.Tests.PlayMode
{
    public sealed class UtilityTests
    {
        [UnityTest]
        public IEnumerator SchedulerRunsUnscaledDelaysWhileTimeIsPaused()
        {
            var host = new GameObject("Scheduler host");
            var scheduler = host.AddComponent<Scheduler>();
            var timeScale = Time.timeScale;
            try
            {
                Time.timeScale = 0f;
                var unscaledCalls = 0;
                var scaledCalls = 0;
                scheduler.Delay(0.01f, () => unscaledCalls++);
                scheduler.Delay(0.01f, () => scaledCalls++, unscaled: false);

                var deadline = Time.realtimeSinceStartup + 2f;
                while (unscaledCalls == 0 && Time.realtimeSinceStartup < deadline)
                    yield return null;

                Assert.That(unscaledCalls, Is.EqualTo(1));
                Assert.That(scaledCalls, Is.Zero);
            }
            finally
            {
                Time.timeScale = timeScale;
                Object.Destroy(host);
            }
        }

        [UnityTest]
        public IEnumerator SchedulerCancelsPendingCallbacks()
        {
            var host = new GameObject("Scheduler host");
            try
            {
                var scheduler = host.AddComponent<Scheduler>();
                var called = false;
                scheduler.Cancel(scheduler.Delay(0f, () => called = true));
                yield return null;
                yield return null;
                Assert.That(called, Is.False);
            }
            finally
            {
                Object.Destroy(host);
            }
        }

        [UnityTest]
        public IEnumerator SchedulerDestructionStopsPendingCallbacks()
        {
            var host = new GameObject("Scheduler host");
            try
            {
                var scheduler = host.AddComponent<Scheduler>();
                var called = false;
                scheduler.Delay(0.01f, () => called = true);
                Object.Destroy(scheduler);
                yield return new WaitForSecondsRealtime(0.05f);
                Assert.That(called, Is.False);
            }
            finally
            {
                Object.Destroy(host);
            }
        }
    }
}
