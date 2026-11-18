using System;
using System.Collections;
using UnityEngine;

namespace UnityCore
{
    public sealed class Scheduler : MonoBehaviour
    {
        public Coroutine Delay(float seconds, Action callback, bool unscaled = true)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            if (float.IsNaN(seconds) || float.IsInfinity(seconds))
                throw new ArgumentOutOfRangeException(nameof(seconds));

            return StartCoroutine(WaitAndInvoke(seconds, callback, unscaled));
        }

        public void Cancel(Coroutine routine)
        {
            if (this != null && routine != null)
                StopCoroutine(routine);
        }

        private void OnDestroy() => StopAllCoroutines();

        private static IEnumerator WaitAndInvoke(float seconds, Action callback, bool unscaled)
        {
            if (seconds <= 0f)
                yield return null;
            else if (unscaled)
                yield return new WaitForSecondsRealtime(seconds);
            else
                yield return new WaitForSeconds(seconds);

            callback();
        }
    }
}
