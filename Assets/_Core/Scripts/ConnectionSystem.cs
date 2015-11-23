using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DotsClone {
    /// <summary>
    /// Subscribes to IO events from the Dots and follows
    /// rules to establish connections:
    /// 
    /// If there is no active connection, start one.
    /// If there is an active connection:
    ///     If dot is new and a valid neighbor, add it
    ///     If the dot is the last selected dot or the one before it, undo
    ///     If the dot is somewhere else in the chain, a square has been created
    /// </summary>
    public class ConnectionSystem : MonoBehaviour {
        public delegate void OnDotConnected(Dot dot);
        public static event OnDotConnected DotConnected;

        [HideInInspector]
        public List<Dot> activeConnections = new List<Dot>();

        public DotType currentType { get; private set; }
        public bool isSquare { get; private set; }

        private void Start() {
            DotTouchIO.DotSelected += HandleNewDot;
        }

        private void Update() {
            if(Input.GetMouseButtonUp(0)) {
                activeConnections.Clear();
            }
        }

        private void HandleNewDot(Dot dot) {
            var isValid = false;

            if(activeConnections.Count == 0) {
                isSquare = false;
                currentType = dot.dotType;
                isValid = true;
            }
            else if(activeConnections[activeConnections.Count - 1] == dot ||
                    activeConnections[Mathf.Clamp(activeConnections.Count - 2, 0, int.MaxValue)] == dot) {
                // Undoing a connection
                activeConnections.RemoveAt(activeConnections.Count - 1);
            }
            else {
                isValid = activeConnections[activeConnections.Count - 1].IsValidNeighbor(dot);
            }

            if(isValid) {
                activeConnections.Add(dot);
                if(DotConnected != null) {
                    DotConnected(dot);
                }
            }

            // TODO Optimize
            isSquare = activeConnections.Count != activeConnections.Distinct().Count();
        }
    }
}