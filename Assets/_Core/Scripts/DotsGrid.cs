using System.Collections;
using UnityEngine;

namespace DotsClone {
    public class DotsGrid : MonoBehaviour {
        public const float Y_DOT_SPAWN_OFFSET = 8f;

        [SerializeField]
        GameObject dotPrefab;
        [SerializeField]
        sbyte _xSize = 6;
        [SerializeField]
        sbyte _ySize = 6;
        [SerializeField]
        float dotSpacing = 5f;
        [SerializeField]
        float dotPPU = 32f;
        [SerializeField]
        Color[] _colors;

        Dot[,] dotArray;
        bool isRunningAsyncOperation;

        public Color[] colors { get { return _colors; } }
        public float dotSize { get { return 32f / dotPPU; } }
        public sbyte xSize { get { return _xSize; } }
        public sbyte ySize { get { return _ySize; } }

        private void Awake() {
            dotArray = new Dot[_xSize, _ySize];

            // Need to manually loop for dot creation
            for(sbyte y = 0; y < _ySize; y++) {
                for(sbyte x = 0; x < _xSize; x++) {
                    var dot = Instantiate(dotPrefab).GetComponent<Dot>();
                    dot.name = string.Format("Dot {0}x{1}", x, y);
                    dot.transform.parent = transform;
                    dotArray[x, y] = dot;
                }
            }
        }

        private void Start() {
            ExecuteDotOperation(false, (ref Dot dot, sbyte xGridPosition, sbyte yGridPosition) => {
                var targetPosition = GetPositionForCoordinates(xGridPosition, yGridPosition);
                dot.Spawn(targetPosition, xGridPosition, yGridPosition, colors);
            });
        }

        private Vector2 GetPositionForCoordinates(sbyte xGridPosition, sbyte yGridPosition) {
            var adjustedGridSize = new Vector2((_xSize - 1) / 2f, (_ySize - 1) / 2f);
            var adjustedDotSize = dotSize + dotSpacing;

            var position = Vector2.zero;

            // Set to "zero position" (bottom left dot position)
            position.x = -adjustedDotSize * adjustedGridSize.x;
            position.y = -adjustedDotSize * adjustedGridSize.y;

            // Add offset from zero position via dot coordinate
            position.x += adjustedDotSize * xGridPosition;
            position.y += adjustedDotSize * yGridPosition;

            return position;
        }

        delegate void OnDotOperation(ref Dot dot, sbyte xGridPosition, sbyte yGridPosition);
        private void ExecuteDotOperation(bool async, OnDotOperation callback) {
            if(!isRunningAsyncOperation) {
                StartCoroutine(DotOperation(async, callback));
            }
        }

        /// <summary>
        /// Executes callback on every dot in proper order
        /// </summary>
        private IEnumerator DotOperation(bool async, OnDotOperation callback) {
            isRunningAsyncOperation = true;
            for(sbyte y = 0; y < _ySize; y++) {
                for(sbyte x = 0; x < _xSize; x++) {
                    callback(ref dotArray[x, y], x, y);
                    if(async) {
                        yield return null;
                    }
                }
            }
            isRunningAsyncOperation = false;
        }
    }
}