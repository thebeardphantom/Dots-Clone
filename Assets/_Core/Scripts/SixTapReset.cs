using UnityEngine;

namespace DotsClone {
    public class SixTapReset : MonoBehaviour {

        private void Update() {
            if(Input.touchCount >= 6 || Input.GetKeyDown(KeyCode.R)) {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }
}