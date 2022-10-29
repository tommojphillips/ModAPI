namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents the bolt save info class object.
    /// </summary>
    public class BoltSaveInfo
        {
            /// <summary>
            /// Represents the tightness of this bolt. range: 0 - <see cref="BoltSettings.maxTightness"/>.
            /// </summary>
            public float boltTightness { get; set; } = 0;
            /// <summary>
            /// Represents the tightness of the nut if addnut setting is true.. range: 0 - <see cref="BoltSettings.maxTightness"/>.
            /// </summary>
            public float addNutTightness { get; set; } = 0;
        }
}
