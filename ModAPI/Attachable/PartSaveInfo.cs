using UnityEngine;

namespace TommoJProductions.ModApi.v0_1_3_0_alpha.Attachable
{
    /// <summary>
    /// Represents save info about a particluar part.
    /// </summary>
    public class PartSaveInfo
    {
        // Written, 04.10.2018

        #region Properties

        /// <summary>
        /// Represents whether or not the part is installed.
        /// </summary>
        public bool installed
        {
            get;
            set;
        }        
        /// <summary>
        /// The position of the gameobject. (not installed; free part)
        /// </summary>
        public Vector3 position
        {
            get;
            set;
        }
        /// <summary>
        /// The rotation of the gameobject. (not installed; free part)
        /// </summary>
        public Quaternion rotation
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of this.
        /// </summary>
        /// <param name="inPart">The part to save</param>
        public PartSaveInfo(Part inPart)
        {
            // Written, 04.10.2018

            installed = inPart.installed;
            position = inPart.transform.position;
            rotation = inPart.transform.rotation;
        }

        #endregion
    }
}
