using UnityEngine;

namespace DotsClone {
    public class Game : Singleton<Game> {
        public DotsTheme selectedTheme = DotsTheme.defaultTheme;
        //public DotsGrid currentGrid;
    }
}