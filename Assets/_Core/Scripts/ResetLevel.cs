using UnityEngine;

namespace DotsClone {
    public class ResetLevel : MonoBehaviour {

        public void DoLevelReset() {
            Application.LoadLevel(Application.loadedLevel);
        }

    }
}