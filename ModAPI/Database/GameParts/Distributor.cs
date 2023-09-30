using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    /// <summary>
    /// The Distributor game part.
    /// </summary>
    public class Distributor : GamePart
    {
        /// <summary>
        /// The max angle the distributor can be.
        /// </summary>
        public FsmFloat maxAngle { get; set; }
        /// <summary>
        /// The spark angle of the distributor
        /// </summary>
        public FsmFloat sparkAngle { get; set; }

        /// <summary>
        /// inits distributor.
        /// </summary>        
        /// <param name="data">the data needed to set up.</param>
        public Distributor(PlayMakerFSM data) : base(data)
        {
            maxAngle = data.FsmVariables.GetFsmFloat("MaxAngle");
            sparkAngle = data.FsmVariables.GetFsmFloat("SparkAngle");
        }
    }
}
