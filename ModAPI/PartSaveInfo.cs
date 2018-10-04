using ModAPI.Objects;

namespace ModAPI
{
    /// <summary>
    /// Represents save info about a particluar part.
    /// </summary>
    public class PartSaveInfo
    {
        // Written, 04.10.2018

        #region Properties

        /// <summary>
        /// Represnets whether or not the part is installed.
        /// </summary>
        public bool isInstalled
        {
            get;
            private set;
        }
        /// <summary>
        /// Part using fixed joints?
        /// </summary>
        public bool useFixedJoints
        {
            get;
            private set;
        }
        /// <summary>
        /// Parts breakforce?
        /// </summary>
        public float breakForce
        {
            get;
            private set;
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

        }

        #endregion
    }
}
