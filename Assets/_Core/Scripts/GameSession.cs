using UnityEngine;

namespace DotsClone {
    public class GameSession {
        public int dotsCleared { get; private set; }

        public GameSession() {
            Camera.main.backgroundColor = Game.get.selectedTheme.backgroundColor;
        }

    }
}