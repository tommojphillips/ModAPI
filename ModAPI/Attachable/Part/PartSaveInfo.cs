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
        /// Represents whether or not the part is installed.
        /// </summary>
        public bool installed { get; set; } = false;
        /// <summary>
        /// Represents whether or not the part is bolted.
        /// </summary>
        public bool bolted { get; set; } = false;
        /// <summary>
        /// The install point index that the part is installed to located in <see cref="Part.triggers" />.
        /// </summary>
        public int installedPointIndex { get; set; } = 0;
        /// <summary>
        /// Represents the parts world position.
        /// </summary>
        public Vector3Info position { get; set; } = Vector3.zero;
        /// <summary>
        /// Represents the parts world rotation (euler angles)
        /// </summary>
        public Vector3Info rotation { get; set; } = Vector3.zero;
        /// <summary>
        /// Represents all bolt save infos. <see langword="null"/> if part has no bolts.
        /// </summary>
        public BoltSaveInfo[] boltSaveInfos { get; set; } = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new part save info with default values..
        /// </summary>
        public PartSaveInfo() { }
        /// <summary>
        /// Initializes a new part save info and assigns fields.
        /// </summary>
        /// <param name="inSave">The save info to replicate</param>
        public PartSaveInfo(PartSaveInfo inSave)
        {
            // Written, 01.05.2022

            if (inSave != null)
            {
                installed = inSave.installed;
                bolted = inSave.bolted;
                installedPointIndex = inSave.installedPointIndex;
                position = inSave.position;
                rotation = inSave.rotation;
                boltSaveInfos = inSave.boltSaveInfos;
            }
        }

        #endregion
    }
}
