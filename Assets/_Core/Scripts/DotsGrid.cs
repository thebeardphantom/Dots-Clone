using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DotsClone {
    [RequireComponent(typeof(ConnectionSystem))]
    public class DotsGrid : MonoBehaviour {
        public const float Y_DOT_SPAWN = 8f;

        [SerializeField]
        GameObject dotPrefab;
        [SerializeField]
        byte columns = 6;
        [SerializeField]
        byte rows = 6;
        [SerializeField]
        float dotSpacing = 5f;
        [SerializeField]
        float dotPPU = 100f;

        public List<Dot> dots = new List<Dot>();
        ConnectionSystem connectionSystem;

        public float dotSize { get { return 32f / dotPPU; } }

        private void Awake() {
            connectionSystem = gameObject.GetComponent<ConnectionSystem>();
            DotTouchIO.SelectionEnded += ClearSelectedDots;
            CreateDotObjects();
        }

        private void CreateDotObjects() {
            // Need to manually loop for dot creation
            for(byte row = 0; row < rows; row++) {
                for(byte column = 0; column < columns; column++) {
                    var dot = Instantiate(dotPrefab).GetComponent<Dot>();
                    dot.transform.parent = transform;
                    dot.coordinates = new GridCoordinates(column, row);
                    dots.Add(dot);
                }
            }
        }

        private void Start() {
            ExecuteDotOperation((dot) => {
                var targetPosition = GetPositionForCoordinates(dot.coordinates);
                dot.Spawn(targetPosition, 0.5f);
            });
        }

        private Vector2 GetPositionForCoordinates(GridCoordinates position) {
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

        private void ClearSelectedDots() {
            if(connectionSystem.activeConnections.Count < 2) {
                return;
            }

            var dotsRemovedInColumn = new byte[columns];

            // Run square behavior
            if(connectionSystem.isSquare) {
                connectionSystem.activeConnections.Clear();
                foreach(var d in dots) {
                    if(d.dotType == connectionSystem.currentType) {
                        connectionSystem.activeConnections.Add(d);
                    }
                }
            }

            Game.get.session.dotsCleared += connectionSystem.activeConnections.Count;

            // 1. Mark all connected dots
            foreach(var dot in connectionSystem.activeConnections) {
                var dotCoord = dot.coordinates;
                dotsRemovedInColumn[dotCoord.column]++;
                dot.ClearDot(); // Clear dot status
            }

            // 2. Set all affected dots in affected columns to move to new position
            for(byte c = 0; c < columns; c++) {
                if(dotsRemovedInColumn[c] == 0) {
                    continue;
                }
                ExecuteDotOperation(c, (dot) => {
                    if(dot.coordinates.row != 0 && dot.dotType != DotType.Cleared) {
                        var fallDist = GetBlankDotsUnderneath(dot);
                        dot.coordinates = new GridCoordinates(c, (byte)(dot.coordinates.row - fallDist));
                        dot.MoveToPosition(GetPositionForCoordinates(dot.coordinates), 0f);
                    }
                });
            }

            // 3. For each column, recycle dots
            for(byte c = 0; c < columns; c++) {
                var removed = dotsRemovedInColumn[c];
                for(byte r = 0; r < removed; r++) {
                    var row = (byte)(rows - (removed - r));
                    var dot = connectionSystem.activeConnections[connectionSystem.activeConnections.Count - 1];
                    connectionSystem.activeConnections.RemoveAt(connectionSystem.activeConnections.Count - 1);
                    dot.coordinates = new GridCoordinates(c, row);
                    var targetPosition = GetPositionForCoordinates(dot.coordinates);
                    dot.Spawn(targetPosition, 0f);
                }
            }

            connectionSystem.activeConnections.Clear();
        }

        private byte GetBlankDotsUnderneath(Dot dot) {
            byte count = 0;
            ExecuteDotOperation(dot.coordinates.column, (other) => {
                if(other.dotType == DotType.Cleared && other.coordinates.row < dot.coordinates.row) {
                    count++;
                }
            });
            return count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dot">The current dot</param>
        /// <returns>True if should continue, else break loop</returns>
        delegate void OnDotOperation(Dot dot);
        private void ExecuteDotOperation(OnDotOperation callback) {
            foreach(var d in dots) {
                callback(d);
            }
        }

        private void ExecuteDotOperation(byte column, OnDotOperation callback) {
            foreach(var d in dots) {
                if(d.coordinates.column == column) {
                    callback(d);
                }
            }
        }
    }
}