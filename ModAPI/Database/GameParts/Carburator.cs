using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    /// <summary>
    /// Represents the Stock Carburator game part.
    /// </summary>
    public class Carburator : GamePart
    {
        /// <summary>
        /// How much dirt is in the caburator.
        /// </summary>
        public FsmFloat dirt { get; set; }
        /// <summary>
        /// The idle adjust value.
        /// </summary>
        public FsmFloat idleAdjust { get; set; }
        /// <summary>
        /// The max adjust value.
        /// </summary>
        public FsmFloat adjustMax { get; set; }
        /// <summary>
        /// The min adjust value
        /// </summary>
        public FsmFloat adjustMin { get; set; }

        /// <summary>
        /// inits caburator
        /// </summary>
        /// <param name="data">the data needed to set up.</param>
        public Carburator(PlayMakerFSM data) : base(data)
        {
            dirt = data.FsmVariables.GetFsmFloat("Dirt");
            idleAdjust = data.FsmVariables.GetFsmFloat("IdleAdjust");
            adjustMax = data.FsmVariables.GetFsmFloat("Max");
            adjustMin = data.FsmVariables.GetFsmFloat("Min");
        }
    }
}
