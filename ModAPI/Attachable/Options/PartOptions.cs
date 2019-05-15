namespace TommoJProductions.ModApi.Attachable.Options
{
    /// <summary>
    /// Represents options for a part.
    /// </summary>
    public class PartOptions
    {
        // Written, 05.05.2019

        /// <summary>
        /// Represents the part initializesation prcedure.
        /// </summary>
        public PartInitializationProcedureEnum partInitializationProcedure { get; set; } = PartInitializationProcedureEnum.FindThenInstantiate;
        /// <summary>
        /// Represents if active part has a rigidbodt attached when <see cref="Part.setUpPart(PartSaveInfo, PartOptions, UnityEngine.GameObject, UnityEngine.GameObject, Trigger, UnityEngine.Vector3, UnityEngine.Quaternion)"/> is called.
        /// </summary>
        public bool activePartAddRigidbodyComponent{ get; set; } = true;
        /// <summary>
        /// Represents if active part has it's position and rotation set when <see cref="Part.setUpPart(PartSaveInfo, PartOptions, UnityEngine.GameObject, UnityEngine.GameObject, Trigger, UnityEngine.Vector3, UnityEngine.Quaternion)"/> is called.
        /// </summary>
        public bool activePartSetPositionRotation { get; set; } = true;
    }
}
