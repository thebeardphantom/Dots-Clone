namespace DotsClone {
    public class Game : Singleton<Game> {
        public GameSession session;
        public DotsTheme selectedTheme = DotsTheme.defaultTheme;

        protected override void Awake() {
            base.Awake();
            session = new GameSession();
        }
    }
}