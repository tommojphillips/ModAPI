using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    /// <summary>
    /// reps the oil pan game part.
    /// </summary>
    public class OilPan : GamePart
    {
        /// <summary>
        /// reps the oil level of the engine.
        /// </summary>
        public FsmFloat oilLevel { get; set; }
        /// <summary>
        /// reps the oil contamination.
        /// </summary>
        public FsmFloat oilContamination { get; set; }
        /// <summary>
        /// reps the oil grade.
        /// </summary>
        public FsmFloat oilGrade { get; set; }

        /// <summary>
        /// inits
        /// </summary>
        /// <param name="data"></param>
        public OilPan(PlayMakerFSM data) : base(data)
        {
            oilLevel = data.FsmVariables.GetFsmFloat("Oil");
            oilContamination = data.FsmVariables.GetFsmFloat("OilContamination");
            oilGrade = data.FsmVariables.GetFsmFloat("OilGrade");
        }
    }
}
