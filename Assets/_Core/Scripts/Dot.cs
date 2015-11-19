using System;
using UnityEngine;

namespace DotsClone {
    public class Dot : MonoBehaviour {
        public new SpriteRenderer renderer { get; private set; }

        public sbyte xGridPosition { get; private set; }
        public sbyte yGridPosition { get; private set; }
        public Type dotType { get; private set; }

        private void Awake() {
            renderer = GetComponent<SpriteRenderer>();
        }

        public void ClearDot() {

        }

        public void Spawn(Vector2 targetPosition, sbyte xGridPosition, sbyte yGridPosition, Color[] colors) {
            this.xGridPosition = xGridPosition;
            this.yGridPosition = yGridPosition;

            var types = Enum.GetValues(typeof(Type));
            dotType = (Type)types.GetValue(UnityEngine.Random.Range(1, types.Length));
            renderer.color = colors[(byte)dotType - 1];

            LeanTween.moveLocal(gameObject, targetPosition, 0.3f).setEase(LeanTweenType.easeOutBounce).setDelay((0.1f * yGridPosition) + 0.5f);

            // Set start position above the screen so they can lerp down
            targetPosition.y += DotsGrid.Y_DOT_SPAWN_OFFSET;
            transform.localPosition = targetPosition;
        }

        //public void Update() {

        //}

        public enum Type : byte {
            Cleared,
            DotA,
            DotB,
            DotC,
            DotE,
            DotF
        }
    }
}