using UnityEngine;
using UnityEngine.UI;

namespace DotsClone
{
    /// <summary>
    /// Updates the game's UI
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        public Text dotsClearedLabel;

        private int dotsCleared;
        private float dotsClearedLerp;

        private void Update()
        {
            // Could be more efficient using events, but good enough
            dotsClearedLerp = Mathf.Lerp(dotsClearedLerp, Game.get.session.dotsCleared, Time.deltaTime * 3f);
            dotsCleared = Mathf.CeilToInt(dotsClearedLerp);
            dotsClearedLabel.text = dotsCleared.ToString();
        }

    }
}
