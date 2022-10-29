using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// settings for addNut
    /// </summary>
    public class AddNutSettings
        {
            /// <summary>
            /// Represents the tool size required to un/tighten this nut. if null uses the parent bolts size.
            /// </summary>
            public BoltSize? nutSize = null;
            /// <summary>
            /// Represents the custom nut prefab to use. if null uses modapi nut prefab.
            /// </summary>
            public GameObject customNutPrefab = null;
            /// <summary>
            /// Represents the add nut offset. an offset applied to the model. on <see cref="BoltSettings.posDirection"/>
            /// </summary>
            public float nutOffset = 0;
            /// <summary>
            /// init with default settings
            /// </summary>
            public AddNutSettings() { }
            /// <summary>
            /// inits this and copies s to instance.
            /// </summary>
            /// <param name="s">the instance to copy.</param>
            public AddNutSettings(AddNutSettings s)
            {
                if (s != null)
                {
                    nutSize = s.nutSize;
                    customNutPrefab = s.customNutPrefab;
                    nutOffset = s.nutOffset;
                }
            }
        }
}
