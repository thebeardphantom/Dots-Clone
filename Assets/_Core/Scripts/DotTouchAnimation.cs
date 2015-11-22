using UnityEngine;

namespace DotsClone {
    /// <summary>
    /// Handles code-based lerp animation for dots when selected
    /// </summary>
    public class DotTouchAnimation : MonoBehaviour {
        Dot dot;
        SpriteRenderer sprite;

        private void Awake() {
            dot = GetComponentInParent<Dot>();
            sprite = GetComponent<SpriteRenderer>();
            ConnectionSystem.DotConnected += ConnectionSystem_DotConnected;
        }

        private void ConnectionSystem_DotConnected(Dot dot) {
            float time = 0.5f;
            if(dot == this.dot) {
                OnEnable();
                LeanTween.cancel(gameObject);
                LeanTween.scale(gameObject, new Vector3(2f, 2f, 1f), time);
                LeanTween.alpha(gameObject, 0f, time);
            }
        }

        private void OnEnable() {
            var color = sprite.color;
            color.a = 1f;
            sprite.color = color;
            transform.localScale = Vector3.one;
        }

    }
}