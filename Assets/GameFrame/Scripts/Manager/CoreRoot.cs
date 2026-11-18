using UnityEngine;

namespace UnityCore
{
    [DefaultExecutionOrder(-1000), DisallowMultipleComponent]
    public sealed class CoreRoot : MonoBehaviour
    {
        [SerializeField] private string storagePrefix = "UnityCoreTemplate";

        public static CoreRoot Current { get; private set; }
        public ResourceStore Resources { get; private set; }
        public LocalStore Storage { get; private set; }
        public EventBus Events { get; private set; }
        public PrefabPool Pool { get; private set; }
        public SceneLoader Scenes { get; private set; }
        public AudioService Audio { get; private set; }
        public Scheduler Timer { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetCurrent() => Current = null;

        public static CoreRoot GetOrCreate()
        {
            return Current != null ? Current : new GameObject(nameof(CoreRoot)).AddComponent<CoreRoot>();
        }

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(gameObject);
                return;
            }

            Current = this;
            DontDestroyOnLoad(gameObject);
            Storage = new LocalStore(storagePrefix);
            Events = new EventBus();
            Resources = new ResourceStore();
            var poolRoot = new GameObject("Pooled Objects").transform;
            poolRoot.SetParent(transform, false);
            Pool = new PrefabPool(poolRoot);
            Scenes = new SceneLoader();
            Timer = gameObject.AddComponent<Scheduler>();
            Audio = gameObject.AddComponent<AudioService>();
            Audio.Initialize(Resources, Storage);
        }

        private void OnDestroy()
        {
            if (Current != this) return;
            Audio?.Shutdown();
            Pool?.Dispose();
            Events?.Clear();
            Resources?.Dispose();
            Storage?.Save();
            Current = null;
        }
    }
}
