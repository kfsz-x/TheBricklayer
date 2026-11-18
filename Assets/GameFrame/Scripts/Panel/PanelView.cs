using DG.Tweening;
using UnityEngine;

namespace UnityCore
{
    [DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup))]
    public sealed class PanelView : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float showDuration = 0.2f;
        [SerializeField, Min(0f)] private float hideDuration = 0.15f;

        private CanvasGroup group;
        private Tween transition;
        private bool visible;

        /// <summary>The requested visibility, including while a fade is in progress.</summary>
        public bool IsVisible => group != null ? visible : gameObject.activeSelf;

        public void Show() => Show(showDuration);

        public void Show(float duration) => SetVisibility(true, duration);

        public void Hide() => Hide(hideDuration);

        public void Hide(float duration) => SetVisibility(false, duration);

        private void Awake() => Initialize();

        private void Initialize()
        {
            if (group != null) return;
            group = GetComponent<CanvasGroup>();
            visible = gameObject.activeSelf;
            ApplyVisibility();
        }

        private void SetVisibility(bool show, float duration)
        {
            Initialize();
            CancelTransition();
            visible = show;
            if (show) gameObject.SetActive(true);

            var alpha = show ? 1f : 0f;
            if (duration <= 0f || !isActiveAndEnabled || Mathf.Approximately(group.alpha, alpha))
            {
                ApplyVisibility();
                return;
            }

            SetInteractive(false);
            transition = DOTween.To(() => group.alpha, value => group.alpha = value, alpha, duration)
                .SetUpdate(UpdateType.Normal, true)
                .SetEase(Ease.OutQuad)
                .SetAutoKill(true)
                .SetRecyclable(false)
                .OnComplete(() =>
                {
                    transition = null;
                    ApplyVisibility();
                });
        }

        private void ApplyVisibility()
        {
            group.alpha = visible ? 1f : 0f;
            SetInteractive(visible);
            if (!visible) gameObject.SetActive(false);
        }

        private void SetInteractive(bool value)
        {
            group.interactable = value;
            group.blocksRaycasts = value;
        }

        private void CancelTransition()
        {
            transition?.Kill();
            transition = null;
        }

        private void OnDisable()
        {
            if (transition == null) return;
            CancelTransition();
            ApplyVisibility();
        }
    }
}
