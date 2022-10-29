using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents settings for assembly type, joint
    /// </summary>
    public class AssemblyTypeJointSettings
    {
        /// <summary>
        /// Represents a list of install point rigidbodies for when using assemblyType:Joint
        /// </summary>
        public Rigidbody[] installPointRigidbodies;
        /// <summary>
        /// Represents the break force to break this joint. NOTE: unbreakable joints are <see cref="float.PositiveInfinity"/>.
        /// </summary>
        public float breakForce = float.PositiveInfinity;
        /// <summary>
        /// Represents the breakforce min limit. Relevant only to boltable parts.
        /// </summary>
        public float breakForceMin = 0;
        /// <summary>
        /// represents if the bolts tightness effects the joints breakforce.
        /// </summary>
        public bool boltTightnessEffectsBreakforce = false;

        /// <summary>
        /// Inits new joint settings class with default values
        /// </summary>
        public AssemblyTypeJointSettings() { }
        /// <summary>
        /// Inits new joint settings class and assigns rigidbodies.
        /// </summary>
        public AssemblyTypeJointSettings(params Rigidbody[] rigidbodies)
        {
            installPointRigidbodies = rigidbodies;
        }
        /// <summary>
        /// Inits new joint settings class and assigns class variables.
        /// </summary>
        public AssemblyTypeJointSettings(float breakForce, params Rigidbody[] rigidbodies)
        {
            this.breakForce = breakForce;
            installPointRigidbodies = rigidbodies;
        }
        /// <summary>
        /// Inits new joint settings class and assigns all class variables.
        /// </summary>
        public AssemblyTypeJointSettings(float breakForce, float breakForceMin, bool boltTightnessEffectsBreakForce, params Rigidbody[] rigidbodies)
        {
            this.breakForce = breakForce;
            installPointRigidbodies = rigidbodies;
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
                installPointRigidbodies = atjs.installPointRigidbodies;
                breakForce = atjs.breakForce;
                breakForceMin = atjs.breakForceMin;
                boltTightnessEffectsBreakforce = atjs.boltTightnessEffectsBreakforce;
            }
        }
    }
}
