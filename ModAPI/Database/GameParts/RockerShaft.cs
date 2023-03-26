using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    public class RockerShaft : GamePart
    {
        public FsmFloat maxExhaust { get; set; }
        public FsmFloat maxIntake { get; set; }
        public FsmFloat minExhaust { get; set; }
        public FsmFloat minIntake { get; set; }
        public FsmFloat settingExhaustMax { get; set; }
        public FsmFloat settingExhaustMin { get; set; }
        public FsmFloat settingIntakeMax { get; set; }
        public FsmFloat settingIntakeMin { get; set; }
        public FsmFloat cyl1Ex { get; set; }
        public FsmFloat cyl1In { get; set; }
        public FsmFloat cyl2Ex { get; set; }
        public FsmFloat cyl2In { get; set; }
        public FsmFloat cyl3Ex { get; set; }
        public FsmFloat cyl3In { get; set; }
        public FsmFloat cyl4Ex { get; set; }
        public FsmFloat cyl4In { get; set; }

        public RockerShaft(PlayMakerFSM data) : base(data)
        {
            maxExhaust = data.FsmVariables.GetFsmFloat("MaxExhaust");
            maxIntake = data.FsmVariables.GetFsmFloat("MaxIntake");
            minExhaust = data.FsmVariables.GetFsmFloat("MinExhaust");
            minIntake = data.FsmVariables.GetFsmFloat("MinIntake");
        }
    }
}
