using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DotsClone
{
    [RequireComponent(typeof(ConnectionSystem))]
    public class DotsGrid : MonoBehaviour
    {
        private const float DOT_PIXEL_SIZE = 32f;


        [SerializeField]
        private GameObject dotPrefab;
        [SerializeField]
        private byte columns = 6;
        [SerializeField]
        private byte rows = 6;
        [SerializeField]
        private float dotSpacing = 5f;
        [SerializeField]
        private float dotPPU = 100f;

        private List<Dot> dots = new List<Dot>();
        private ConnectionSystem connectionSystem;

        public float dotSize { get { return DOT_PIXEL_SIZE / dotPPU; } }

        private void Awake()
        {
            connectionSystem = gameObject.GetComponent<ConnectionSystem>();
            DotTouchIO.SelectionEnded += ClearSelectedDots;
            CreateDotObjects();
        }

        private void Start()
        {
            ExecuteDotOperation((dot) =>
            {
                var targetPosition = GetPositionForCoordinates(dot.coordinates);
                dot.Spawn(targetPosition, 0.5f);
            });
        }

        private void CreateDotObjects()
        {
            // Need to manually loop for dot creation
            for (byte row = 0; row < rows; row++)
            {
                for (byte column = 0; column < columns; column++)
                {
                    var dot = Instantiate(dotPrefab).GetComponent<Dot>();
                    dot.transform.parent = transform;
                    dot.coordinates = new GridCoordinates(column, row);
                    dots.Add(dot);
                }
            }
        }

        private Vector2 GetPositionForCoordinates(GridCoordinates position)
        {
            var adjustedDotSize = dotSize + dotSpacing;
            var worldPosition = Vector2.zero;

            // Set to "zero position" (bottom left dot position)
            worldPosition.x = -adjustedDotSize * ((columns - 1) / 2f);
            worldPosition.y = -adjustedDotSize * ((rows - 1) / 2f);

            // Add offset from zero position via dot coordinate
            worldPosition.x += adjustedDotSize * position.column;
            worldPosition.y += adjustedDotSize * position.row;

            return worldPosition;
        }

        private void ClearSelectedDots()
        {
            if (connectionSystem.activeConnections.Count < 2)
            {
                return;
            }

            var dotsRemovedInColumn = new byte[columns];

            // Run square behavior
            if (connectionSystem.isSquare)
            {
                connectionSystem.activeConnections.Clear();
                foreach (var d in dots)
                {
                    if (d.dotType == connectionSystem.currentType)
                    {
                        connectionSystem.activeConnections.Add(d);
                    }
                }
            }

            Game.get.session.dotsCleared += connectionSystem.activeConnections.Count;

            // 1. Mark all connected dots
            foreach (var dot in connectionSystem.activeConnections)
            {
                var dotCoord = dot.coordinates;
                dotsRemovedInColumn[dotCoord.column]++;
                dot.ClearDot(); // Clear dot status
            }

            // 2. Set all affected dots in affected columns to move to new position
            for (byte c = 0; c < columns; c++)
            {
                if (dotsRemovedInColumn[c] == 0)
                {
                    continue;
                }
                ExecuteDotOperation(c, (dot) =>
                {
                    if (dot.coordinates.row != 0 && dot.dotType != DotType.Cleared)
                    {
                        var fallDist = GetBlankDotsUnderneath(dot);
                        dot.coordinates = new GridCoordinates(c, (byte)(dot.coordinates.row - fallDist));
                        dot.MoveToPosition(GetPositionForCoordinates(dot.coordinates), 0f);
                    }
                });
            }

            // 3. For each column, recycle dots
            for (byte c = 0; c < columns; c++)
            {
                var removedCount = dotsRemovedInColumn[c];
                for (byte r = 0; r < removedCount; r++)
                {
                    // The lowest empty row
                    var row = (byte)(rows - (removedCount - r));
                    var lastDotIndex = connectionSystem.activeConnections.Count - 1;
                    var dot = connectionSystem.activeConnections[lastDotIndex];

                    connectionSystem.activeConnections.RemoveAt(lastDotIndex);
                    dot.coordinates = new GridCoordinates(c, row);
                    dot.Spawn(GetPositionForCoordinates(dot.coordinates), 0f);
                }
            }

            // Sanity check
            connectionSystem.activeConnections.Clear();
        }

        private byte GetBlankDotsUnderneath(Dot dot)
        {
            byte count = 0;
            ExecuteDotOperation(dot.coordinates.column, (other) =>
            {
                if (other.dotType == DotType.Cleared && other.coordinates.row < dot.coordinates.row)
                {
                    count++;
                }
            });
            return count;
        }


        /// <summary>
        /// Calls callback on all dots
        /// </summary>
        delegate void OnDotOperation(Dot dot);
        private void ExecuteDotOperation(OnDotOperation callback)
        {
            foreach (var d in dots)
            {
                callback(d);
            }
        }

        /// <summary>
        /// Calls callback on all dots in column
        /// </summary>
        private void ExecuteDotOperation(byte column, OnDotOperation callback)
        {
            foreach (var d in dots)
            {
                if (d.coordinates.column == column)
                {
                    callback(d);
                }
            }
        }
    }
}