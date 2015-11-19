using System.Collections.Generic;
using UnityEngine;

namespace DotsClone {
    public class ConnectorLines : MonoBehaviour {
        public GameObject linePrefab;
        public List<Vector2> points = new List<Vector2>();

        List<LineRenderer> lines = new List<LineRenderer>();
        PrefabPool pool;

        private void Awake() {
            pool = new PrefabPool(linePrefab, transform, 5);
            TouchSystem.TouchHit += TouchSystem_TouchHit;
            TouchSystem.DragEnd += TouchSystem_DragEnd;
        }

        private void TouchSystem_DragEnd() {
            points.Clear();
        }

        private void TouchSystem_TouchHit(Collider2D collider) {
            points.Add(collider.transform.position);
        }

        private void Update() {
            while(lines.Count != points.Count) {
                if(lines.Count < points.Count) {
                    lines.Add(pool.Get().GetComponent<LineRenderer>());
                }
                else if(lines.Count > points.Count) {
                    var line = lines[lines.Count - 1];
                    lines.RemoveAt(lines.Count - 1);
                    pool.Return(line.gameObject);
                }
            }

            for(var i = 0; i < points.Count - 1; i++) {
                var line = lines[i];
                line.SetPosition(0, points[i]);
                if(i == points.Count - 2) {
                    line.SetPosition(1, TouchSystem.get.pointerWorldPosition);
                }
                else {
                    line.SetPosition(1, points[i + 1]);
                }
            }
        }
    }
}