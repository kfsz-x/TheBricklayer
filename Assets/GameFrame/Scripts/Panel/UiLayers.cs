using UnityEngine;

namespace UnityCore
{
    /// <summary>Optional parent transforms for organizing a Canvas in sibling draw order.</summary>
    [DisallowMultipleComponent]
    public sealed class UiLayers : MonoBehaviour
    {
        [SerializeField] private Transform hud;
        [SerializeField] private Transform panels;
        [SerializeField] private Transform overlay;

        public Transform Hud => hud;
        public Transform Panels => panels;
        public Transform Overlay => overlay;
    }
}
