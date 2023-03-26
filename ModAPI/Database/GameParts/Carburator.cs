using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    public class Carburator : GamePart
    {
        public FsmFloat dirt { get; set; }
        public FsmFloat idleAdjust { get; set; }
        public FsmFloat adjustMax { get; set; }
        public FsmFloat adjustMin { get; set; }

        public Carburator(PlayMakerFSM data) : base(data)
        {
            dirt = data.FsmVariables.GetFsmFloat("Dirt");
            idleAdjust = data.FsmVariables.GetFsmFloat("IdleAdjust");
            adjustMax = data.FsmVariables.GetFsmFloat("Max");
            adjustMin = data.FsmVariables.GetFsmFloat("Min");
        }
    }
}
