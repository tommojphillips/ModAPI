using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    public class Distributor : GamePart
    {
        public FsmFloat maxAngle { get; set; }
        public FsmFloat sparkAngle { get; set; }

        public Distributor(PlayMakerFSM data) : base(data)
        {
            maxAngle = data.FsmVariables.GetFsmFloat("MaxAngle");
            sparkAngle = data.FsmVariables.GetFsmFloat("SparkAngle");
        }
    }
}
