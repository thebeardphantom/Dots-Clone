using UnityEngine;
using UnityEngine.UI;

namespace DotsClone {
    public class GameUI : MonoBehaviour {
        public Text dotsClearedLabel;

        int dotsCleared;
        float dotsClearedLerp;

        private void Update() {
            dotsClearedLerp = Mathf.Lerp(dotsClearedLerp, Game.get.session.dotsCleared, Time.deltaTime * 3f);
            dotsCleared = Mathf.CeilToInt(dotsClearedLerp);
            dotsClearedLabel.text = dotsCleared.ToString();
        }

    }
}
