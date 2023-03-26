using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    /// <summary>
    /// reps the block game part.
    /// </summary>
    public class Block : GamePart
    {
        /// <summary>
        /// reps if the block is currently in the hoist.
        /// </summary>
        public FsmBool inHoist { get; set; }

        /// <summary>
        /// inits
        /// </summary>
        /// <param name="data"></param>
        public Block(PlayMakerFSM data) : base(data)
        {
            inHoist = data.FsmVariables.GetFsmBool("InHoist");
        }
    }
}
