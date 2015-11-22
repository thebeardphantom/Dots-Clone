using System.Collections.Generic;
using UnityEngine;

namespace DotsClone {
    public class ConnectorLines : MonoBehaviour {
        public GameObject linePrefab;

        ConnectionSystem connectionSystem;
        List<LineRenderer> lines = new List<LineRenderer>();
        PrefabPool pool;

        private void Awake() {
            connectionSystem = FindObjectOfType<ConnectionSystem>();
            pool = new PrefabPool(linePrefab, transform, 5);
        }

        private void ReturnLine(LineRenderer line) {
            line.SetColors(Color.clear, Color.clear);
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);
            pool.Return(line.gameObject);
            lines.Remove(line);
        }

        private void Update() {
            var connections = connectionSystem.activeConnections;

            while(connections.Count > lines.Count) {
                lines.Add(pool.Get().GetComponent<LineRenderer>());
            }
            while(connections.Count < lines.Count) {
                ReturnLine(lines[0]);
            }
            if(connections.Count > 0) {
                DrawConnections(connections);
            }
        }

        private void DrawConnections(List<Dot> connections) {
            LineRenderer line = null;
            var currentDrawColor = Game.get.selectedTheme.FromDotType(connectionSystem.currentType);
            for(var i = 0; i < connections.Count; i++) {
                line = lines[i];
                line.SetColors(currentDrawColor, currentDrawColor);
                line.SetPosition(0, connections[i].transform.position);
                if(i + 1 != connections.Count) {
                    line.SetPosition(1, connections[i + 1].transform.position);
                }
            }

            var pointer = GetPointerWorldPosition();
            line.SetPosition(1, pointer);
        }

        private Vector2 GetPointerWorldPosition() {
            var screen = Vector2.zero;
            if(Application.isEditor) {
                screen = Input.mousePosition;
            }
            else if(Input.touchCount > 0) {
                screen = Input.GetTouch(0).position;
            }
            else {
                return Vector2.zero;
            }
            return Camera.main.ScreenToWorldPoint(screen);
        }
    }
}