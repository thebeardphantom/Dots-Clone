using System.Collections.Generic;
using UnityEngine;

namespace DotsClone
{
    /// <summary>
    /// Uses information from the ConnectionSystem class
    /// to manage/draw lines between connected dots
    /// Uses an object pool for optimization
    /// </summary>
    public class ConnectorLines : MonoBehaviour
    {
        public GameObject linePrefab;

        ConnectionSystem connectionSystem;
        PrefabPool pool;

        /// <summary>
        /// Multiple lines are used instead of one
        /// due to inconsistent thickness of LineRenderer
        /// </summary>
        List<LineRenderer> activeLines = new List<LineRenderer>();

        private void Awake()
        {
            connectionSystem = FindObjectOfType<ConnectionSystem>();
            pool = new PrefabPool(linePrefab, transform, 5);
        }

        /// <summary>
        /// Resets line parameters and returns it to object pool
        /// </summary>
        private void ReturnLine(LineRenderer line)
        {
            line.startColor = Color.clear;
            line.endColor = Color.clear;
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);
            pool.Return(line.gameObject);
            activeLines.Remove(line);
        }

        private void Update()
        {
            var connections = connectionSystem.activeConnections;

            // Get/return lines from pool until we are at the correct amount
            while (connections.Count > activeLines.Count)
            {
                activeLines.Add(pool.Get().GetComponent<LineRenderer>());
            }
            while (connections.Count < activeLines.Count)
            {
                // TODO optimize to use last line instead of first
                // using [0] due to time consuming bug
                ReturnLine(activeLines[0]);
            }

            if (connections.Count > 0)
            {
                DrawConnections(connections);
            }
        }

        private void DrawConnections(List<Dot> connections)
        {
            // Keeps a reference to the last line
            // For usage outside of for loop
            LineRenderer line = null;
            var currentDrawColor = Game.get.selectedTheme.FromDotType(connectionSystem.currentType);


            for (var i = 0; i < connections.Count; i++)
            {
                line = activeLines[i];
                line.startColor = currentDrawColor;
                line.endColor = currentDrawColor;
                line.SetPosition(0, connections[i].transform.position);

                // If we're not at the last connection
                if (i != connections.Count - 1)
                {
                    // Set second position to next connection
                    line.SetPosition(1, connections[i + 1].transform.position);
                }
            }

            // Set the last line to draw it's final point to the pointer
            var pointer = GetPointerWorldPosition();
            line.SetPosition(1, pointer);
        }

        /// <summary>
        /// Get the current pointer position in world (z: 0)
        /// </summary>
        /// <returns>Mouse position or first touch position</returns>
        private Vector2 GetPointerWorldPosition()
        {
            var screen = Vector2.zero;
            if (Input.touchSupported)
            {
                screen = Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero;
            }
            else
            {
                screen = Input.mousePosition;
            }
            return Camera.main.ScreenToWorldPoint(screen);
        }
    }
}