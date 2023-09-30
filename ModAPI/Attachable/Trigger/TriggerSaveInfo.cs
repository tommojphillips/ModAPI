using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents trigger save info.
    /// </summary>
    public class TriggerSaveInfo
    {
        // Written, 11.09.2023

        #region Properties

        /// <summary>
        /// Represents the bolt save infos.
        /// </summary>
        public int[] boltTightness { get; set; } = null;

        #endregion

        /// <summary>
        /// copies trigger save info and assigns fields.
        /// </summary>
        /// <param name="save">The save info to replicate</param>
        public static TriggerSaveInfo copy(TriggerSaveInfo save)
        {
            // Written, 11.09.2023

            TriggerSaveInfo info = new TriggerSaveInfo();
            if (save != null)
            {
                info.boltTightness = save.boltTightness;
            }
            return info;
        }
    }
}
