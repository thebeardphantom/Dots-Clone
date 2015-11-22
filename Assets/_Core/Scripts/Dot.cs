using System;
using System.Collections;
using UnityEngine;

namespace DotsClone {
    public class Dot : MonoBehaviour, IComparable<Dot> {
        private GridCoordinates _coordinates;

        public SpriteRenderer[] sprites { get; private set; }
        public GridCoordinates coordinates {
            get {
                return _coordinates;
            }
            set {
                _coordinates = value;
                name = string.Format("Dot {0}x{1}", value.column, value.row);
            }
        }
        public DotType dotType { get; private set; }
        bool isLerping;

        private void Awake() {
            sprites = GetComponentsInChildren<SpriteRenderer>();
        }

        public void ClearDot() {
            isLerping = true;
            LeanTween.scale(gameObject, Vector3.zero, 0.2f).onComplete += () => {
                isLerping = false;
                gameObject.SetActive(false);
            };
        }

        public void Spawn(Vector2 targetPosition) {
            StartCoroutine(DoSpawn(targetPosition));
        }

        private IEnumerator DoSpawn(Vector2 targetPosition) {
            while(isLerping) {
                yield return null;
            }
            gameObject.SetActive(true);
            var types = Enum.GetValues(typeof(DotType));
            dotType = (DotType)UnityEngine.Random.Range(1, types.Length);

            foreach(var s in sprites) {
                s.color = Game.get.selectedTheme.FromDotType(dotType);
            }

            transform.localScale = Vector3.one;

            MoveToPosition(targetPosition, 0.5f);

            // Set start position above the screen so they can lerp down
            targetPosition.y += DotsGrid.Y_DOT_SPAWN_OFFSET;
            transform.localPosition = targetPosition;
        }

        public void MoveToPosition(Vector2 targetPosition, float delay) {
            isLerping = true;
            LeanTween.moveLocal(gameObject, targetPosition, 0.4f).setEase(LeanTweenType.easeOutBounce).setDelay((0.05f * coordinates.row) + delay).onComplete += () => {
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