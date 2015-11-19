using System.Collections.Generic;
using UnityEngine;

namespace DotsClone {
    public class ConnectorLines : MonoBehaviour {
        public GameObject linePrefab;
        public List<Dot> connections = new List<Dot>();

        List<LineRenderer> lines = new List<LineRenderer>();
        PrefabPool pool;
        Color currentDrawColor;

        private void Awake() {
            pool = new PrefabPool(linePrefab, transform, 5);
            TouchSystem.TouchHit += TouchSystem_TouchHit;
            TouchSystem.DragEnd += TouchSystem_DragEnd;
        }

        private void TouchSystem_DragEnd() {
            connections.Clear();
            foreach(var l in lines) {
                l.SetPosition(0, Vector3.zero);
                l.SetPosition(1, Vector3.zero);
                pool.Return(l.gameObject);
            }
            lines.Clear();
        }

        private void TouchSystem_TouchHit(Collider2D collider) {
            var dot = collider.GetComponent<Dot>();
            if(connections.Count == 0) {
                currentDrawColor = Game.get.selectedTheme.FromDotType(dot.dotType);
            }
            if(!connections.Contains(dot)) {
                print("TouchSystem_TouchHit");
                connections.Add(dot);
                lines.Add(pool.Get().GetComponent<LineRenderer>());
            }
        }

        private void Update() {
            if(connections.Count == 0) {
                return;
            }

            LineRenderer line = null;
            for(var i = 0; i < connections.Count; i++) {
                line = lines[i];
                line.SetColors(currentDrawColor, currentDrawColor);
                line.SetPosition(0, connections[i].transform.position);
                if(i + 1 != connections.Count) {
                    line.SetPosition(1, connections[i + 1].transform.position);
                }
            }
            line.SetPosition(1, TouchSystem.get.pointerWorldPosition);
        }
    }
}