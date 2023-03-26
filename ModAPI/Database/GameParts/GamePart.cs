using System;
using UnityEngine;
using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    /// <summary>
    /// Represents a game part.
    /// </summary>
    public class GamePart
    {

        /// <summary>
        /// reps if the game part is bolted
        /// </summary>
        public FsmBool bolted { get; set; }
        /// <summary>
        /// reps if the game part is installed
        /// </summary>
        public FsmBool installed { get; set; }
        /// <summary>
        /// reps if the game part is damaged
        /// </summary>
        public FsmBool damaged { get; set; }
        /// <summary>
        /// reps the ingame gameobject reference.
        /// </summary>
        public FsmGameObject thisPart { get; set; }
        /// <summary>
        /// reps the 'not broken' mesh for the game part.
        /// </summary>
        public FsmGameObject newMesh { get; set; }
        /// <summary>
        /// reps the 'broken' mesh for the game part.
        /// </summary>
        public FsmGameObject damagedMesh { get; set; }
        /// <summary>
        /// reps the trigger gameobject for the game part.
        /// </summary>
        public FsmGameObject trigger { get; set; }

        /// <summary>
        /// inits a new instance of the game part and assigns data based on the input.
        /// </summary>
        /// <param name="data">the data to input</param>
        /// <exception cref="NullReferenceException">thrown if data is null</exception>
        public GamePart(PlayMakerFSM data)
        {
            if (!data)
                throw new NullReferenceException("[GamePart Constructor] param, 'data' cannot be null.]");

            bolted = data.FsmVariables.GetFsmBool("Bolted");
            installed = data.FsmVariables.GetFsmBool("Installed");
            damaged = data.FsmVariables.GetFsmBool("Damaged");
            thisPart = data.FsmVariables.GetFsmGameObject("ThisPart");
            newMesh = data.FsmVariables.GetFsmGameObject("New");
            damagedMesh = data.FsmVariables.GetFsmGameObject("Damaged");
            trigger = data.FsmVariables.GetFsmGameObject("Trigger");
        }

        /// <summary>
        /// gets the <see cref="GamePart.thisPart"/> <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gp"></param>
        public static implicit operator GameObject(GamePart gp) => gp.thisPart.Value;
    }
}
