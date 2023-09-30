using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    /// <summary>
    /// Represents the rocker shaft game part.
    /// </summary>
    public class RockerShaft : GamePart
    {
        /// <summary>
        /// The Max Exhaust.
        /// </summary>
        public FsmFloat maxExhaust { get; set; }
        /// <summary>
        /// The Max Intake.
        /// </summary>
        public FsmFloat maxIntake { get; set; }
        /// <summary>
        /// The Min Exhaust.
        /// </summary>
        public FsmFloat minExhaust { get; set; }
        /// <summary>
        /// The Min Intake.
        /// </summary>
        public FsmFloat minIntake { get; set; }
        /// <summary>
        /// The exhaust max setting.
        /// </summary>
        public FsmFloat settingExhaustMax { get; set; }
        /// <summary>
        /// The exhaust min setting.
        /// </summary>
        public FsmFloat settingExhaustMin { get; set; }
        /// <summary>
        /// The Intake max setting.
        /// </summary>
        public FsmFloat settingIntakeMax { get; set; }
        /// <summary>
        /// The intake min setting.
        /// </summary>
        public FsmFloat settingIntakeMin { get; set; }
        /// <summary>
        /// Cylinder 1 Exhaust
        /// </summary>
        public FsmFloat cyl1Ex { get; set; }
        /// <summary>
        /// Cylinder 1 Intake
        /// </summary>
        public FsmFloat cyl1In { get; set; }
        /// <summary>
        /// Cylinder 2 Exhaust
        /// </summary>
        public FsmFloat cyl2Ex { get; set; }
        /// <summary>
        /// Cylinder 2 Intake
        /// </summary>
        public FsmFloat cyl2In { get; set; }
        /// <summary>
        /// Cylinder 3 Exhaust
        /// </summary>
        public FsmFloat cyl3Ex { get; set; }
        /// <summary>
        /// Cylinder 3 Intake
        /// </summary>
        public FsmFloat cyl3In { get; set; }
        /// <summary>
        /// Cylinder 4 Exhaust
        /// </summary>
        public FsmFloat cyl4Ex { get; set; }
        /// <summary>
        /// Cylinder 4 Intake
        /// </summary>
        public FsmFloat cyl4In { get; set; }

        /// <summary>
        /// Inits rocker shaft
        /// </summary>
        /// <param name="data">the data needed to set up.</param>
        public RockerShaft(PlayMakerFSM data) : base(data)
        {
            maxExhaust = data.FsmVariables.GetFsmFloat("MaxExhaust");
            maxIntake = data.FsmVariables.GetFsmFloat("MaxIntake");
            minExhaust = data.FsmVariables.GetFsmFloat("MinExhaust");
            minIntake = data.FsmVariables.GetFsmFloat("MinIntake");
        }
    }
}
