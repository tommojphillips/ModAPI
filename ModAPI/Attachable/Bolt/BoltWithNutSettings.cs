using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    // Written, 11.05.2022

    /// <summary>
    /// Represents all settings for a bolt.
    /// </summary>
    public class BoltWithNutSettings : BoltSettings
    {
        /// <summary>
        /// Represents the add nut offset. an offset applied to the model.
        /// </summary>
        public float offset = 0;
        /// <summary>
        /// Settings for the nut. 
        /// </summary>
        public BaseBoltSettings nutSettings = default;

        /// <summary>
        /// Initializes new instance of b settings with default values.
        /// </summary>
        public BoltWithNutSettings() { }
        /// <summary>
        /// inits new instance of bolt settings and copies instance values.
        /// </summary>
        /// <param name="s">the instance of bolt settings to copy.</param>
        public BoltWithNutSettings(BoltWithNutSettings s) : base (s)
        {
            if (s != null)
            {
                offset = s.offset;
                nutSettings = s.nutSettings.copy();
            }
        }
        /// <summary>
        /// copies values to new instance and returns.
        /// </summary>
        public new BoltWithNutSettings copy()
        {
            return new BoltWithNutSettings(this);
        }
    }
}
