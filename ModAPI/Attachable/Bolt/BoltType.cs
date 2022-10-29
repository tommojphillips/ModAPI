using System.ComponentModel;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents different types of bolts.
    /// </summary>
    [Description("Bolt Type")]
        public enum BoltType
        {
            /// <summary>
            /// Represents a nut
            /// </summary>
            [Description("Nut")]
            nut,
            /// <summary>
            /// Represents a short bolt.
            /// </summary>
            [Description("Short bolt")]
            shortBolt,
            /// <summary>
            /// Represents a long bolt.
            /// </summary>
            [Description("Long bolt")]
            longBolt,
            /// <summary>
            /// Represents a screw.
            /// </summary>
            [Description("Screw")]
            screw,
            /// <summary>
            /// Represents the usage of a custom bolt.
            /// </summary>
            [Description("Custom bolt")]
            custom,
        }
}
