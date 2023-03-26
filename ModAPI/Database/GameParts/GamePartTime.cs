using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    /// <summary>
    /// reps a game part that has a time member.
    /// </summary>
    public class GamePartTime : GamePart
    {
        /// <summary>
        /// time
        /// </summary>
        public FsmFloat time { get; set; }

        /// <summary>
        /// inits
        /// </summary>
        /// <param name="data"></param>
        public GamePartTime(PlayMakerFSM data) : base(data)
        {
            time = data.FsmVariables.GetFsmFloat("Time");
        }
    }
}
