using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DotsClone {
    [RequireComponent(typeof(ConnectionSystem))]
    public class DotsGrid : MonoBehaviour {
        public const float Y_DOT_SPAWN_OFFSET = 8f;

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

        List<Dot> dots = new List<Dot>();
        ConnectionSystem connectionSystem;

        public float dotSize { get { return 32f / dotPPU; } }

        private void Awake() {
            connectionSystem = gameObject.GetComponent<ConnectionSystem>();
            DotTouchIO.SelectionEnded += DotTouchIO_SelectionEnded;
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
                dot.Spawn(targetPosition);
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

        private void DotTouchIO_SelectionEnded() {
            if(connectionSystem.activeConnections.Count < 2) {
                return;
            }

            byte[] dotsRemovedPerColumn = new byte[columns];
            byte[] startsAtRow = new byte[columns];

            if(connectionSystem.isSquare) {
                connectionSystem.activeConnections.Clear();
                foreach(var d in dots) {
                    if(d.dotType == connectionSystem.currentType) {
                        connectionSystem.activeConnections.Add(d);
                    }
                }
            }

            // Mark all connected dots
            foreach(var dot in connectionSystem.activeConnections) {
                var dotCoord = dot.coordinates;
                dotsRemovedPerColumn[dotCoord.column]++;
                startsAtRow[dotCoord.column] = (byte)Mathf.Max(startsAtRow[dotCoord.column], dotCoord.row);
                dot.ClearDot(); // Clear dot status
            }

            // Set all affected dots to move to new position
            for(byte c = 0; c < columns; c++) {
                var dotsRemoved = dotsRemovedPerColumn[c];
                if(dotsRemoved == 0) {
                    continue;
                }
                ExecuteDotOperation(c, (dot) => {
                    if(dot.coordinates.row <= startsAtRow[c]) {
                        return;
                    }
                    dot.coordinates = new GridCoordinates(c, (byte)(dot.coordinates.row - dotsRemoved));
                    dot.MoveToPosition(GetPositionForCoordinates(dot.coordinates), 0f);
                });
            }

            // For each column, recycle dots
            for(byte c = 0; c < columns; c++) {
                var removed = dotsRemovedPerColumn[c];
                for(byte r = 0; r < removed; r++) {
                    var row = (byte)(rows - (removed - r));
                    var dot = connectionSystem.activeConnections[connectionSystem.activeConnections.Count - 1];
                    dot.coordinates = new GridCoordinates(dot.coordinates.column, row);
                    var targetPosition = GetPositionForCoordinates(dot.coordinates);
                    dot.Spawn(targetPosition);
                }
            }

            connectionSystem.activeConnections.Clear();
            dots.Sort();
        }

        delegate void OnDotOperation(Dot dot);
        private void ExecuteDotOperation(OnDotOperation callback) {
            foreach(var d in dots) {
                if(d.gameObject.activeInHierarchy) {
                    callback(d);
                }
            }
        }

        private void ExecuteDotOperation(byte column, OnDotOperation callback) {
            foreach(var d in dots) {
                if(d.gameObject.activeInHierarchy && d.coordinates.column == column) {
                    callback(d);
                }
            }
        }
    }
}