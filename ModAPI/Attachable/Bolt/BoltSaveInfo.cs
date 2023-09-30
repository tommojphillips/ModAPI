namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents the bolt save info class object.
    /// </summary>
    public class BoltSaveInfo
        {
            /// <summary>
            /// Represents the tightness of this bolt. range: 0 - 8.
            /// </summary>
            public int boltTightness { get; set; } = 0;
            /// <summary>
            /// Represents the tightness of the nut if <see cref="BoltWithNutSettings.addNut"/> is true. range: 0 - 8
            /// </summary>
            public int addNutTightness { get; set; } = 0;
        }
}
