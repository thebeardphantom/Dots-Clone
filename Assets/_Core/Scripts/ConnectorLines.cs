using System.Collections.Generic;
using UnityEngine;

namespace DotsClone {
    public class ConnectorLines : MonoBehaviour {
        public GameObject linePrefab;
        public List<Dot> connections = new List<Dot>();

        List<LineRenderer> lines = new List<LineRenderer>();
        PrefabPool pool;

        Color currentDrawColor;
        Dot.Type currentType;

        private void Awake() {
            pool = new PrefabPool(linePrefab, transform, 5);
            TouchSystem.TouchHit += TouchSystem_TouchHit;
            TouchSystem.DragEnd += TouchSystem_DragEnd;
        }

        private void TouchSystem_DragEnd() {
            connections.Clear();
            foreach(var l in lines) {
                ReturnLine(l);
            }
            lines.Clear();
        }

        private void TouchSystem_TouchHit(Dot dot) {
            var isValid = false;

            if(connections.Count == 0) {
                currentType = dot.dotType;
                currentDrawColor = Game.get.selectedTheme.FromDotType(currentType);
                isValid = true;
            }
            else if(connections[connections.Count - 1] == dot) {
                connections.RemoveAt(connections.Count - 1);
                ReturnLine(lines[lines.Count - 1]);
            }
            else {
                isValid = connections[connections.Count - 1].IsValidNeighbor(dot) && !connections.Contains(dot);
            }
            if(isValid) {
                connections.Add(dot);
                lines.Add(pool.Get().GetComponent<LineRenderer>());
            }
        }

        private void ReturnLine(LineRenderer line) {
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);
            pool.Return(line.gameObject);
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