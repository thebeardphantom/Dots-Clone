using System;
using System.Collections;
using UnityEngine;

namespace DotsClone
{
    /// <summary>
    /// Code representation of a single dot.
    /// Manages all placement operations of dots
    /// </summary>
    public class Dot : MonoBehaviour, IComparable<Dot>
    {
        private const float Y_DOT_SPAWN = 8f;
        private const float PER_ROW_LERP_DELAY = 0.075f;

        public GridCoordinates coordinates;

        /// <summary>
        /// Helps with post clear spawn timing
        /// </summary>
        private bool isLerping;
        private DotType _dotType;

        public SpriteRenderer[] sprites { get; private set; }
        public DotType dotType
        {
            get
            {
                return _dotType;
            }
            // Using a property means we dont need an update loop
            // to set sprite colors. We only set colors on type change.
            set
            {
                _dotType = value;
                // Conditional fixes black dot issue
                if (dotType != DotType.Cleared)
                {
                    foreach (var s in sprites)
                    {
                        s.color = Game.get.selectedTheme.FromDotType(dotType);
                    }
                }
            }
        }

        private void Awake()
        {
            sprites = GetComponentsInChildren<SpriteRenderer>();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Gives us a better visual representation of dot status in Hierarchy
        /// Preprocessor directive used for build optimization
        /// </summary>
        private void Update()
        {
            name = string.Format("Dot {0}x{1} {2}", coordinates.column, coordinates.row, dotType.ToString());
        }
#endif

        public void ClearDot()
        {
            isLerping = true;
            dotType = DotType.Cleared;
            LeanTween.scale(gameObject, Vector3.zero, 0.2f).onComplete += () =>
            {
                isLerping = false;
            };
        }

        public void Spawn(Vector2 targetPosition, float delay)
        {
            StartCoroutine(DoSpawn(targetPosition, delay));
        }

        /// <summary>
        /// Spawning is a coroutine so we can wait for clear lerp to finish
        /// </summary>
        private IEnumerator DoSpawn(Vector2 targetPosition, float delay)
        {
            while (isLerping)
            {
                yield return null;
            }
            var types = Enum.GetValues(typeof(DotType));
            dotType = (DotType)UnityEngine.Random.Range(1, types.Length);

            // Fixes scale if post-clear
            transform.localScale = Vector3.one;

            // Queue dropdown lerp
            MoveToPosition(targetPosition, delay);

            // Set start position above screen so dot can lerp down
            targetPosition.y = Y_DOT_SPAWN;
            transform.localPosition = targetPosition;

            // Resets the DotTouchAnimation obj
            // And anything else that might exist
            foreach (Transform child in transform)
            {
                child.transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// LeanTweens a drop animation with a bounce ease
        /// Delay is per-row, plus additional optional delay
        /// </summary>
        public void MoveToPosition(Vector2 targetPosition, float delay)
        {
            isLerping = true;

            var lerp = LeanTween.moveLocal(gameObject, targetPosition, 0.4f);
            lerp.setEase(LeanTweenType.easeOutBounce).setDelay((PER_ROW_LERP_DELAY * coordinates.row) + delay);
            lerp.onComplete += () =>
            {
                isLerping = false;
            };
        }

        public bool IsValidNeighbor(Dot other)
        {
            if (this == other || other.dotType != dotType)
            {
                return false;
            }
            else
            {
                // ABS because we don't care about direction, easier compare
                var rowDiff = Mathf.Abs(other.coordinates.row - coordinates.row);
                var columnDiff = Mathf.Abs(other.coordinates.column - coordinates.column);

                // In diagonal direction
                if (rowDiff > 0 && columnDiff > 0)
                {
                    return false;
                }
                // Not directly next to dot
                if (rowDiff > 1 || columnDiff > 1)
                {
                    return false;
                }
                return true;
            }
        }

        public int CompareTo(Dot other)
        {
            var rowCompare = coordinates.row.CompareTo(other.coordinates.row);
            if (rowCompare != 0)
            {
                return rowCompare;
            }
            else
            {
                return coordinates.column.CompareTo(other.coordinates.column);
            }
        }
    }
}