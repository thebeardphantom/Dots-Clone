using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DotsClone {
    public class DotsGrid : MonoBehaviour {
        public const float Y_DOT_SPAWN_OFFSET = 8f;

        [SerializeField]
        GameObject dotPrefab;
        [SerializeField]
        sbyte columns = 6;
        [SerializeField]
        sbyte rows = 6;
        [SerializeField]
        float dotSpacing = 5f;
        [SerializeField]
        float dotPPU = 100f;

        List<Dot> dots = new List<Dot>();
        ConnectionSystem connectionSystem;

        public float dotSize { get { return 32f / dotPPU; } }

        private void Awake() {
            connectionSystem = gameObject.AddComponent<ConnectionSystem>();
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
                dot.Spawn(targetPosition, dot.coordinates);
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
            byte[] removedPerColumn = new byte[columns];
            byte[] startsAtRow = new byte[rows];

            foreach(var connection in connectionSystem.activeConnections) {
                var coordinates = connection.coordinates; // Store old coordinates
                removedPerColumn[coordinates.column]++;
                startsAtRow[coordinates.column] = (byte)Mathf.Max(startsAtRow[coordinates.column], coordinates.row);
                connection.ClearDot(); // Clear dot status
            }

            for(byte c = 0; c < columns; c++) {
                ExecuteDotOperation(c, (dot) => {
                    if(dot.coordinates.row <= startsAtRow[c]) {
                        return;
                    }
                    dot.coordinates = new GridCoordinates(c, (byte)(dot.coordinates.row - removedPerColumn[c]));
                    dot.MoveToPosition(GetPositionForCoordinates(dot.coordinates), 0f);
                });
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