using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    /// <summary>
    /// reps a game part that has a wear member.
    /// </summary>
    public class GamePartWear : GamePartTime
    {
        /// <summary>
        /// wear
        /// </summary>
        public FsmFloat wear { get; set; }

        /// <summary>
        /// inits
        /// </summary>
        public GamePartWear(PlayMakerFSM data) : base(data)
        {
            wear = data.FsmVariables.GetFsmFloat("Wear");
        }
    }
}
