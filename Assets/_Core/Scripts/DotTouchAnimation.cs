using System;
using UnityEngine;

namespace DotsClone {
    /// <summary>
    /// Handles code-based lerp animation for dots when selected
    /// </summary>
    public class DotTouchAnimation : MonoBehaviour {
        public SpriteRenderer sprite { get; private set; }

        private void Awake() {
            sprite = GetComponent<SpriteRenderer>();
            ConnectionSystem.DotConnected += RunTouchAnimation;
        }

        private void RunTouchAnimation(Dot dot) {
            float time = 0.5f;
            if(dot.gameObject == transform.parent.gameObject) {
                transform.localScale = Vector3.one;
                LeanTween.cancel(gameObject);
                LeanTween.scale(gameObject, new Vector3(2f, 2f, 1f), time);
                LeanTween.alpha(gameObject, 0f, time).setOnComplete(ResetStyle);
            }
        }

        private void ResetStyle() {
            var color = sprite.color;
            color.a = 1f;
            sprite.color = color;
            transform.localScale = Vector3.one;
        }
    }
}