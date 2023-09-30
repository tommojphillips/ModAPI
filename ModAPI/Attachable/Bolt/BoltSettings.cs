using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents bolt settings eg => the size of ther bolt.
    /// </summary>
    public class BoltSettings : BaseBoltSettings
    {
        /// <summary>
        /// for initializing a new instance of bolt with custom name.
        /// </summary>
        public string name = null;
        /// <summary>
        /// Represents the bolts type.
        /// </summary>
        public BoltType type = BoltType.shortBolt;
        /// <summary>
        /// If <see langword="true"/>, the rachet can be used to tighten and loosen the fastener.
        /// </summary>
        public bool canUseRachet = true;
        /// <summary>
        /// Represents the position step. (how quick the position of the bolt moves in <see cref="posDirection"/>. when un/tightening)
        /// </summary>
        public float posStep = 0.0005f;
        /// <summary>
        /// Represents the rot step. (how quick the bolt rotates when un/tightening.
        /// </summary>
        public float rotStep = 30;
        /// <summary>
        /// Represents positional direction of bolt.
        /// </summary>
        public Vector3 posDirection = Vector3.forward;
        /// <summary>
        /// Represents the rotational direction of bolt.
        /// </summary>
        public Vector3 rotDirection = Vector3.forward;
        /// <summary>
        /// if true, highlights the bolt green when the bolt is activated.
        /// </summary>
        public bool highlightWhenActive = true;
        /// <summary>
        /// If true, Bolt is visible and logic is active when uninstalled.
        /// </summary>
        public bool activeWhenUninstalled = false;

        /// <summary>
        /// Initializes new instance of b settings with default values.
        /// </summary>
        public BoltSettings() { }
        /// <summary>
        /// inits new instance of bolt settings and copies instance values.
        /// </summary>
        /// <param name="s">the instance of bolt settings to copy.</param>
        public BoltSettings(BoltSettings s) : base(s)
        {
            if (s != null)
            {
                name = s.name;
                type = s.type;
                posStep = s.posStep;
                rotStep = s.rotStep;
                posDirection = s.posDirection;
                rotDirection = s.rotDirection;
                highlightWhenActive = s.highlightWhenActive;
                activeWhenUninstalled = s.activeWhenUninstalled;
                canUseRachet = s.canUseRachet;
            }
        }

        /// <summary>
        /// Copies field values to a new instance and returns.
        /// </summary>
        /// <returns>A new instance with the same values</returns>
        public new BoltSettings copy() 
        {
            return new BoltSettings(this);
        }
    }
}
