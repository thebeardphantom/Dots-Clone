using System;
using UnityEngine;

namespace DotsClone {
    public class Dot : MonoBehaviour, IComparable<Dot> {
        private GridCoordinates _coordinates;

        public SpriteRenderer sprite { get; private set; }
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

        private void Awake() {
            sprite = GetComponent<SpriteRenderer>();
        }

        public void ClearDot() {
            LeanTween.scale(gameObject, Vector3.zero, 0.2f).onComplete += () => {
                gameObject.SetActive(false);
            };
        }

        public void Spawn(Vector2 targetPosition, GridCoordinates coordinates) {
            this.coordinates = coordinates;

            var types = Enum.GetValues(typeof(DotType));
            dotType = (DotType)types.GetValue(UnityEngine.Random.Range(1, types.Length));
            sprite.color = Game.get.selectedTheme.FromDotType(dotType);

            MoveToPosition(targetPosition, 0.5f);

            // Set start position above the screen so they can lerp down
            targetPosition.y += DotsGrid.Y_DOT_SPAWN_OFFSET;
            transform.localPosition = targetPosition;
            gameObject.SetActive(true);
        }

        public void MoveToPosition(Vector2 targetPosition, float delay) {
            LeanTween.moveLocal(gameObject, targetPosition, 0.4f).setEase(LeanTweenType.easeOutBounce).setDelay((0.05f * coordinates.row) + delay);
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