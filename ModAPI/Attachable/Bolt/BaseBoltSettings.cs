using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents base bolt settings eg => the size of the bolt.
    /// </summary>
    public class BaseBoltSettings
    {
        /// <summary>
        /// Represents the tool size required to un/tighten this fastener.
        /// </summary>
        public BoltSize size = BoltSize.none;
        /// <summary>
        /// Represents the custom prefab to use. set this to the prefab you want the fastener to be. leave null if you want modapi to handle model creation.
        /// </summary>
        public GameObject customPrefab = null;

        /// <summary>
        /// default values
        /// </summary>
        public BaseBoltSettings() { }
        /// <summary>
        /// inits new instance of bolt settings and copies instance values.
        /// </summary>
        /// <param name="s">the instance of bolt settings to copy.</param>
        public BaseBoltSettings(BaseBoltSettings s)
        {
            if (s != null)
            {
                size = s.size;
                customPrefab = s.customPrefab;
            }
        }

        /// <summary>
        /// Copies field values to a new instance and returns.
        /// </summary>
        /// <returns>A new instance with the same values</returns>
        public BaseBoltSettings copy()
        {
            return new BaseBoltSettings(this);
        }
    }
}
