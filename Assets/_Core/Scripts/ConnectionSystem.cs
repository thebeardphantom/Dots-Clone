using UnityEngine;
using System.Collections.Generic;

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
        public DotType currentType { get; private set; }

        public List<Dot> activeConnections = new List<Dot>();

        private void Start() {
            DotTouchIO.DotSelected += DotTouchIO_DotSelection;
        }

        private void DotTouchIO_DotSelection(Dot dot) {
            var isValid = false;

            if(activeConnections.Count == 0) {
                currentType = dot.dotType;
                isValid = true;
            }
            else if(activeConnections[activeConnections.Count - 1] == dot ||
                    activeConnections[Mathf.Clamp(activeConnections.Count - 2, 0, int.MaxValue)] == dot) {
                // Undoing a connection
                activeConnections.RemoveAt(activeConnections.Count - 1);
                // TODO Remove line
            }
            else {
                isValid = activeConnections[activeConnections.Count - 1].IsValidNeighbor(dot);
            }

            if(isValid) {
                activeConnections.Add(dot);
                // TODO Add line
            }
        }
    }
}