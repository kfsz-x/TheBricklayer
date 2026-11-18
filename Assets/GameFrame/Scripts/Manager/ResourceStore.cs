using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityCore
{
    /// <summary>Retains one reference to an asset or preload group until disposed.</summary>
    public sealed class AssetLease<T> : IDisposable
    {
        private readonly T asset;
        private readonly Func<bool> valid;
        private Action release;

        internal AssetLease(T asset, Func<bool> valid, Action release)
        {
            this.asset = asset;
            this.valid = valid;
            this.release = release;
        }

        public T Asset => release != null && valid()
            ? asset
            : throw new ObjectDisposedException(nameof(AssetLease<T>));

        public void Dispose()
        {
            var callback = release;
            release = null;
            callback?.Invoke();
        }
    }

    /// <summary>Shared Addressables cache. Use and dispose leases on the Unity main thread.</summary>
    public sealed class ResourceStore : IDisposable
    {
        private readonly Dictionary<(string Key, Type Result, bool Group), Entry> entries = new();
        private bool disposed;

        private sealed class Entry
        {
            public readonly TaskCompletionSource<object> Result = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public AsyncOperationHandle Handle;
            public int References;
            public bool Closed;
        }

        public Task<AssetLease<T>> AcquireAsync<T>(string address) where T : UnityEngine.Object =>
            Acquire(address, false, () => Addressables.LoadAssetAsync<T>(address));

        public Task<AssetLease<IList<T>>> PreloadAsync<T>(string label) where T : UnityEngine.Object =>
            Acquire(label, true, () => Addressables.LoadAssetsAsync<T>(label, null));

        private async Task<AssetLease<T>> Acquire<T>(string key, bool group, Func<AsyncOperationHandle<T>> start)
        {
            if (disposed) throw new ObjectDisposedException(nameof(ResourceStore));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("An address or label is required.", nameof(key));

            var id = (key, typeof(T), group);
            if (entries.TryGetValue(id, out var entry))
            {
                entry.References++;
            }
            else
            {
                var operation = start();
                entry = new Entry { Handle = operation, References = 1 };
                entries.Add(id, entry);
                operation.Completed += result =>
                {
                    if (entry.Closed) return;

                    if (result.Status == AsyncOperationStatus.Succeeded)
                        entry.Result.TrySetResult(result.Result);
                    else
                    {
                        entries.Remove(id);
                        var error = result.OperationException ??
                            new InvalidOperationException($"Could not load '{key}'.");
                        entry.Result.TrySetException(error);
                        Close(entry);
                    }
                };
            }

            try
            {
                var asset = (T)await entry.Result.Task;
                if (entry.Closed) throw new ObjectDisposedException(nameof(ResourceStore));
                return new AssetLease<T>(asset, () => !entry.Closed, () => DropReference());
            }
            catch
            {
                DropReference();
                throw;
            }

            void DropReference()
            {
                if (entry.Closed || --entry.References > 0) return;
                if (entries.TryGetValue(id, out var current) && ReferenceEquals(entry, current))
                    entries.Remove(id);
                Close(entry);
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            var active = new List<Entry>(entries.Values);
            entries.Clear();
            foreach (var entry in active) Close(entry);
        }

        private static void Close(Entry entry)
        {
            entry.Closed = true;
            entry.Result.TrySetException(new ObjectDisposedException(nameof(ResourceStore)));
            if (entry.Handle.IsValid()) Addressables.Release(entry.Handle);
        }
    }
}
