using UnityEngine;

namespace ModApi.Attachable
{
    /// <summary>
    /// Represents install parameters for a <see cref="Part"/>
    /// </summary>
    public struct PartInstallParameters
    {
        /// <summary>
        /// Represents the parent for the part. The gameobject that this part attaches to. (installed)
        /// </summary>
        public GameObject parent { get; set; }
        /// <summary>
        /// Represents the installed local position relative to <see cref="parent"/>.
        /// </summary>
        public Vector3 installPosition { get; set; }
        /// <summary>
        /// 
        /// Represents the installed local euler angles relative to <see cref="parent"/>.
        /// </summary>
        public Vector3 installEulerAngles { get; set; }
        /// <summary>
        /// Represents the trigger for the part.
        /// </summary>
        public Trigger trigger { get; set; }
    }
}
