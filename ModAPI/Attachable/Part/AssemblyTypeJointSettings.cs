using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents settings for assembly type, joint
    /// </summary>
    public class AssemblyTypeJointSettings
    {
        /// <summary>
        /// Represents the breakforce min limit. Relevant only to boltable parts.
        /// </summary>
        private float m_breakForceMin = 0;
        /// <summary>
        /// Represents the break force to break this joint. NOTE: unbreakable joints are <see cref="float.PositiveInfinity"/>.
        /// </summary>
        public float breakForce = float.PositiveInfinity;
        /// <summary>
        /// represents if the bolts tightness effects the joints breakforce.
        /// </summary>
        public bool boltTightnessEffectsBreakforce = false;

        /// <summary>
        /// Represents the breakforce min limit. Relevant only to boltable parts that have the <see cref="AssemblyTypeJointSettings.boltTightnessEffectsBreakforce"/> setting set to <see langword="true"/>.
        /// Used as an inital <see cref="Joint.breakForce"/> value so that the part being installed doesn't fall off before the player can tighten bolts. 
        /// This value must be high enough to support the part's weight but lower then <see cref="breakForce"/>.
        /// </summary>
        public float breakForceMin 
        {
            get => m_breakForceMin;
            set => m_breakForceMin = Mathf.Clamp(value, 0, breakForce);
        }

        /// <summary>
        /// Inits new joint settings class with default values
        /// </summary>
        public AssemblyTypeJointSettings() { }
        /// <summary>
        /// Inits new joint settings class and assigns class variables.
        /// </summary>
        public AssemblyTypeJointSettings(float breakForce)
        {
            this.breakForce = breakForce;
        }
        /// <summary>
        /// Inits new joint settings class and assigns all class variables.
        /// </summary>
        public AssemblyTypeJointSettings(float breakForce, float breakForceMin, bool boltTightnessEffectsBreakForce)
        {
            this.breakForce = breakForce;
            this.breakForceMin = breakForceMin;
            boltTightnessEffectsBreakforce = boltTightnessEffectsBreakForce;
        }
        /// <summary>
        /// Inits new joint settings class and assigns instance variables.
        /// </summary>
        /// <param name="atjs"></param>
        public AssemblyTypeJointSettings(AssemblyTypeJointSettings atjs)
        {
            if (atjs != null)
            {
                breakForce = atjs.breakForce;
                breakForceMin = atjs.breakForceMin;
                boltTightnessEffectsBreakforce = atjs.boltTightnessEffectsBreakforce;
            }
        }
    }
}
