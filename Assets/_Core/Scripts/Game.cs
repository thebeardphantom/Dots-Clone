using UnityEngine;

namespace DotsClone
{
    /// <summary>
    /// Used to house widely used values and game configuration
    /// </summary>
    public class Game : Singleton<Game>
    {
        public bool useRandomSeed = true;
        public int seed;

        public GameSession session { get; private set; }
        public DotsTheme selectedTheme = DotsTheme.defaultTheme;

        protected override void Awake()
        {
            base.Awake();
            if (!useRandomSeed)
            {
                Random.InitState(seed);
            }
            session = new GameSession();
            Camera.main.backgroundColor = selectedTheme.backgroundColor;
        }
    }
}