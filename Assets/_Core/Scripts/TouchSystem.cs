using UnityEngine;

namespace DotsClone {
    public class TouchSystem : Singleton<TouchSystem> {
        public delegate void OnTouchHit(Collider2D collider);
        public static event OnTouchHit TouchHit;

        public delegate void OnDragEnd();
        public static event OnDragEnd DragEnd;

        public bool isDragging;

        public Vector2 pointerWorldPosition {
            get {
                if(Application.isEditor) {
                    return Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else if(Input.touchCount > 0) {
                    return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                }
                else {
                    return Vector2.zero;
                }
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitSingleton() {
            DummyCreate();
        }

        private void Update() {
            if(isDragging && ((Application.isEditor && !Input.GetMouseButtonDown(0)) || Input.touchCount == 0)) {
                isDragging = false;
                if(DragEnd != null) {
                    DragEnd();
                }
            }
            if(Application.isEditor && (Input.GetMouseButtonDown(0) || isDragging)) {
                CheckInputDown(Input.mousePosition);
            }
            else if(!Application.isEditor && Input.touchCount > 0) {
                CheckInputDown(Input.GetTouch(0).position);
            }
        }

        private void CheckInputDown(Vector2 screenPoint) {
            var collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(screenPoint));
            if(collider != null && TouchHit != null) {
                TouchHit(collider);
                isDragging = true;
            }
        }
    }
}