using UnityEngine;

namespace DotsClone
{
    /// <summary>
    /// Stores current game stats
    /// Potential stats to add:
    ///     Abilities left
    ///     Score
    ///     Timer
    ///     Moves left
    /// </summary>
    public class GameSession
    {
        public int dotsCleared { get; set; }
    }
}