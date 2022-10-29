using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents settings for the part class.
    /// </summary>
    public class PartSettings
    {
        /// <summary>
        /// Represents the physic Material Settings.
        /// </summary>
        public CollisionSettings collisionSettings = new CollisionSettings();
        /// <summary>
        /// Represents the assemble type of the part.
        /// </summary>
        public AssembleType assembleType = AssembleType.static_rigidbodyDelete;
        /// <summary>
        /// Represents '<see cref="AssembleType.joint"/>' settings.
        /// </summary>
        public AssemblyTypeJointSettings assemblyTypeJointSettings = new AssemblyTypeJointSettings() { installPointRigidbodies = null, breakForce = float.PositiveInfinity };
        /// <summary>
        /// Represents the layer to send a part that is installed
        /// </summary>
        public LayerMasksEnum installedPartToLayer = LayerMasksEnum.Parts;
        /// <summary>
        /// Represents the layer to send a part that is not installed
        /// </summary>
        public LayerMasksEnum notInstalledPartToLayer = LayerMasksEnum.Parts;
        /// <summary>
        /// Represents if <see cref="Part.initPart(PartSaveInfo, PartSettings, Trigger[])"/> will set pos and rot of part if NOT installed.
        /// </summary>
        public bool setPositionRotationOnInitialisePart = true;
        /// <summary>
        /// Sets the parts colliders physics material if enabled.
        /// </summary>
        public bool setPhysicsMaterialOnInitialisePart = false;
        /// <summary>
        /// Represents the disassemble collider. collider must not be of IsTrigger. if null, logic uses Parts Gameobject to determine if part is being looked at. otherwise logic uses this collider to determine if part is being looked at.
        /// </summary>
        public Collider disassembleCollider = null;
        /// <summary>
        /// Represents the assemble collider. collider must be of IsTrigger. if null, logic uses <see cref="installPointColliders"/> to determine if part is in trigger. otherwise logic uses this collider to determine if part is in trigger.
        /// </summary>
        public Collider assembleCollider = null;
        /// <summary>
        /// Represents whether this part can be installed by: 1.) (false) holding this part in one of its triggers. OR 2.) (true) if the trigger is a child of another part. you can hold the root part to install aswell.
        /// </summary>
        public bool installEitherDirection = false;
        /// <summary>
        /// Represents the tightness threshold. 0.25f - 1. at what percent of all bolt tightness does the part trigger disable. 0 = triggers will disable at 25% of total tightness. 1 = triggers will disable at 100% of total tightness (tightness == maxTightness)
        /// </summary>
        public float tightnessThreshold = 0.3f;

        /// <summary>
        /// Initializes a new instance of part settings.
        /// </summary>
        public PartSettings() { }
        /// <summary>
        /// Initializes a new instance of part settings and sets all class fields to the provided settings instance, <paramref name="s"/>.
        /// </summary>
        /// <param name="s">The Setting instance to rep.</param>
        public PartSettings(PartSettings s)
        {
            if (s != null)
            {
                collisionSettings = s.collisionSettings;
                assembleType = s.assembleType;
                assemblyTypeJointSettings = s.assemblyTypeJointSettings;
                installedPartToLayer = s.installedPartToLayer;
                notInstalledPartToLayer = s.notInstalledPartToLayer;
                setPositionRotationOnInitialisePart = s.setPositionRotationOnInitialisePart;
                setPhysicsMaterialOnInitialisePart = s.setPhysicsMaterialOnInitialisePart;
                disassembleCollider = s.disassembleCollider;
                assembleCollider = s.assembleCollider;
                installEitherDirection = s.installEitherDirection;
                tightnessThreshold = s.tightnessThreshold;
            }
        }
    }
}
