using UnityEngine;

namespace DotsClone {
    public class TwoTapReset : MonoBehaviour {

        private void Update() {
            if(Input.touchCount >= 2 || Input.GetKeyDown(KeyCode.R)) {
                Application.LoadLevel(Application.loadedLevel);
            }
        }

    }
}