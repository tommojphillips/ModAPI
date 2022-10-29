namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents supported assemble types.
    /// </summary>
    public enum AssembleType
    {
        /// <summary>
        /// Represents a static assembly via, deleting parts rigidbody.
        /// </summary>
        static_rigidbodyDelete,
        /// <summary>
        /// Represents a static assembly via, setting the parts rigidbody to kinematic.
        /// </summary>
        static_rigidibodySetKinematic,
        /// <summary>
        /// Represents a fixed joint assembly via, adding a fixed joint to connect two rigidbodies together.
        /// </summary>
        joint
    }
}
