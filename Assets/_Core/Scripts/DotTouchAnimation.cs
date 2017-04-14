using UnityEngine;

namespace DotsClone
{
    /// <summary>
    /// Handles code-based lerp animation for dots when selected
    /// </summary>
    public class DotTouchAnimation : MonoBehaviour
    {
        private const float ANIMATION_TIME = 0.5f;

        public SpriteRenderer sprite { get; private set; }

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            ConnectionSystem.DotConnected += RunTouchAnimation;
        }

        private void RunTouchAnimation(Dot dot)
        {
            if (dot.gameObject == transform.parent.gameObject)
            {
                ResetStyle();
                LeanTween.cancel(gameObject);
                LeanTween.scale(gameObject, new Vector3(2f, 2f, 1f), ANIMATION_TIME);
                LeanTween.alpha(gameObject, 0f, ANIMATION_TIME).setOnComplete(ResetStyle);
            }
        }

        /// <summary>
        /// Ensures touch animation object is reset to normal before animating
        /// </summary>
        private void ResetStyle()
        {
            var color = sprite.color;
            color.a = 1f;
            sprite.color = color;
            transform.localScale = Vector3.one;
        }
    }
}