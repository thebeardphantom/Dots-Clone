using System;
using System.Collections;
using UnityEngine;

namespace DotsClone {
    public class Dot : MonoBehaviour, IComparable<Dot> {

        private GridCoordinates _coordinates;
        private DotTouchAnimation touchAnimation;
        private bool isLerping;
        private DotType _dotType;

        public SpriteRenderer[] sprites { get; private set; }
        public GridCoordinates coordinates {
            get {
                return _coordinates;
            }
            set {
                _coordinates = value;

            }
        }
        public DotType dotType {
            get {
                return _dotType;
            }
            set {
                _dotType = value;
                foreach(var s in sprites) {
                    s.color = Game.get.selectedTheme.FromDotType(dotType);
                }
            }
        }

        private void Awake() {
            sprites = GetComponentsInChildren<SpriteRenderer>();
            touchAnimation = GetComponentInChildren<DotTouchAnimation>();
        }

#if UNITY_EDITOR
        private void Update() {
            name = string.Format("Dot {0}x{1} {2}", coordinates.column, coordinates.row, dotType.ToString());
        }
#endif

        public void ClearDot() {
            isLerping = true;
            dotType = DotType.Cleared;
            LeanTween.scale(gameObject, Vector3.zero, 0.2f).onComplete += () => {
                isLerping = false;
            };
        }

        public void Spawn(Vector2 targetPosition, float delay) {
            StartCoroutine(DoSpawn(targetPosition, delay));
        }

        private IEnumerator DoSpawn(Vector2 targetPosition, float delay) {
            while(isLerping) {
                yield return null;
            }
            var types = Enum.GetValues(typeof(DotType));
            dotType = (DotType)UnityEngine.Random.Range(1, types.Length);

            transform.localScale = Vector3.one;

            MoveToPosition(targetPosition, delay);

            // Set start position above the screen so they can lerp down
            targetPosition.y = DotsGrid.Y_DOT_SPAWN;
            transform.localPosition = targetPosition;
            touchAnimation.transform.localScale = Vector3.one;
        }

        public void MoveToPosition(Vector2 targetPosition, float delay) {
            isLerping = true;
            LeanTween.moveLocal(gameObject, targetPosition, 0.4f).setEase(LeanTweenType.easeOutBounce).setDelay((0.075f * coordinates.row) + delay).onComplete += () => {
                isLerping = false;
            };
        }

        public bool IsValidNeighbor(Dot other) {
            if(this == other || other.dotType != dotType) {
                return false;
            }
            else {
                var rowDiff = Mathf.Abs(other.coordinates.row - coordinates.row);
                var columnDiff = Mathf.Abs(other.coordinates.column - coordinates.column);

                // In diagonal direction
                if(rowDiff > 0 && columnDiff > 0) {
                    return false;
                }
                // Not directly next to dot
                if(rowDiff > 1 || columnDiff > 1) {
                    return false;
                }
                return true;
            }
        }

        public int CompareTo(Dot other) {
            var rowCompare = coordinates.row.CompareTo(other.coordinates.row);
            if(rowCompare != 0) {
                return rowCompare;
            }
            else {
                return coordinates.column.CompareTo(other.coordinates.column);
            }
        }
    }
}