using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    public class CamshaftGear : GamePart
    {
        public FsmFloat angle { get; set; }
        public FsmInt _int { get; set; }

        public CamshaftGear(PlayMakerFSM data) : base(data)
        {
            angle = data.FsmVariables.GetFsmFloat("Angle");
            _int = data.FsmVariables.GetFsmInt("Int");
        }
    }
}
