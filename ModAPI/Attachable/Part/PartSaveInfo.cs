using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>        
    /// Represents save info about a particluar part.        
    /// </summary>
    public class PartSaveInfo
    {

        // Written, 04.10.2018

        #region Properties

        /// <summary>
        /// Represents the parts world position.
        /// </summary>
        public Vector3 position { get; set; }
        /// <summary>
        /// Represents the parts world rotation (euler angles)
        /// </summary>
        public Vector3 rotation { get; set; }
        /// <summary>
        /// Represents whether or not the part is installed.
        /// </summary>
        public bool installed { get; set; } = false;
        /// <summary>
        /// Represents the install point id 
        /// </summary>
        public string installPointId { get; set; } = null;
        /// <summary>
        /// Represents all bolt save infos. <see langword="null"/> if part has no bolts.
        /// </summary>
        public int[] boltTightness { get; set; } = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new part save info with default values..
        /// </summary>
        public PartSaveInfo() { }

        #endregion

        /// <summary>
        /// Copies fields of <see cref="PartSaveInfo"/> to a new instance and returns.
        /// </summary>
        public PartSaveInfo copy()
        {
            // Written, 01.05.2022

            return copy(this);
        }
        /// <summary>
        /// copies part save info and assigns fields.
        /// </summary>
        /// <param name="save">The save info to replicate</param>
        public static PartSaveInfo copy(PartSaveInfo save)
        {
            // Written, 01.05.2022

            PartSaveInfo info = new PartSaveInfo();
            if (save != null)
            {
                info.position = save.position;
                info.rotation = save.rotation;
                info.installed = save.installed;
                info.installPointId = save.installPointId;
                info.boltTightness = save.boltTightness;
            }
            return info;
        }
    }
}
