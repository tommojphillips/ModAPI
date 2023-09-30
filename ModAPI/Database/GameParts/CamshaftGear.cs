using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    /// <summary>
    /// Represents the Cam Gear game part
    /// </summary>
    public class CamshaftGear : GamePart
    {
        /// <summary>
        /// Represents the cam gear angle
        /// </summary>
        public FsmFloat angle { get; set; }
        /// <summary>
        /// ?
        /// </summary>
        public FsmInt _int { get; set; }
        /// <summary>
        /// inits cam gear
        /// </summary>
        /// <param name="data">the data needed to set up.</param>
        public CamshaftGear(PlayMakerFSM data) : base(data)
        {
            angle = data.FsmVariables.GetFsmFloat("Angle");
            _int = data.FsmVariables.GetFsmInt("Int");
        }
    }
}
