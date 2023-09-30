using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents the parts collision settings.
    /// </summary>
    public class CollisionSettings
    {
        // Written, 29.05.2022

        /// <summary>
        /// Represents the default part physic material.
        /// </summary>
        public static readonly PhysicMaterial defaultPhysicMaterial = new PhysicMaterial("ModAPI.Part.defaultPhysicMaterial")
        {
            staticFriction = 0.4f,
            dynamicFriction = 0.6f,
        };
        /// <summary>
        /// Represents all types of applying physic material
        /// </summary>
        public enum PhysicMaterialType
        {
            /// <summary>
            /// set physic material on all found colliders on part.
            /// </summary>
            setOnAllFoundColliders,
            /// <summary>
            /// set physic material on provided collider/s (<see cref="providedColliders"/>).
            /// </summary>
            setOnProvidedColliders,
        }
        /// <summary>
        /// Represents the collision detection mode on installed parts.
        /// </summary>
        public CollisionDetectionMode installedCollisionDetectionMode = CollisionDetectionMode.Discrete;
        /// <summary>
        /// Represents the collision detection mode on not installed parts. (pickable items)
        /// </summary>
        public CollisionDetectionMode notInstalledCollisionDetectionMode = CollisionDetectionMode.Continuous;
        /// <summary>
        /// Represents the physic material.
        /// </summary>
        public PhysicMaterial physicMaterial = defaultPhysicMaterial;
        /// <summary>
        /// Represents the current physic material type setting.
        /// </summary>
        public PhysicMaterialType physicMaterialType = PhysicMaterialType.setOnAllFoundColliders;

        /// <summary>
        /// Provided colliders. used to set physic mat on initailizePart if <see cref="PartSettings.setPhysicsMaterialOnInitialisePart"/> is true.
        /// and <see cref="physicMaterialType"/> is set to <see cref="PhysicMaterialType.setOnProvidedColliders"/>.
        /// </summary>
        public Collider[] providedColliders;
        /// <summary>
        /// Inits new isntance
        /// </summary>
        public CollisionSettings() { }
        /// <summary>
        /// inits new instacne and assigns instance variables.
        /// </summary>
        /// <param name="cs"></param>
        public CollisionSettings(CollisionSettings cs)
        {
            if (cs != null)
            {
                installedCollisionDetectionMode = cs.installedCollisionDetectionMode;
                notInstalledCollisionDetectionMode = cs.notInstalledCollisionDetectionMode;
                physicMaterial = cs.physicMaterial;
                physicMaterialType = cs.physicMaterialType;
                providedColliders = cs.providedColliders;
            }
        }
    }
}
