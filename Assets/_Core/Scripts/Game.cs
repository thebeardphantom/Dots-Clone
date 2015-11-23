using UnityEngine;

namespace DotsClone {
    public class Game : Singleton<Game> {
        public int seed;
        public GameSession session { get; private set; }
        public DotsTheme selectedTheme = DotsTheme.defaultTheme;

        protected override void Awake() {
            base.Awake();
            if(seed != 0) {
                Random.seed = seed;
            }
            session = new GameSession();
        }
    }
}