using UnityEngine;

namespace ModApi.Attachable
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
        public bool isInstalled
        {
            get;
            set;
        }
        /// <summary>
        /// Part using fixed joints?
        /// </summary>
        public bool useFixedJoints
        {
            get;
            set;
        }
        /// <summary>
        /// Parts breakforce?
        /// </summary>
        public float breakForce
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
        /// <param name="inPart"></param>
        public PartSaveInfo(Part inPart)
        {
            // Written, 04.10.2018

            this.isInstalled = inPart.installed;
            this.useFixedJoints = inPart.useFixedJoint;
            this.breakForce = inPart.breakForce;
            this.position = inPart.transform.position;
            this.rotation = inPart.transform.rotation;
        }
        /// <summary>
        /// Initializes a new instance of this.
        /// </summary>
        public PartSaveInfo()
        {
            // Written, 04.10.2018
        }

        #endregion
    }
}
