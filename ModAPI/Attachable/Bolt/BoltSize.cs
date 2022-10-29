using System.ComponentModel;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents all bolt sizes in game.
    /// </summary>
    [Description("Bolt Size")]
    public enum BoltSize
    {
        /// <summary>
        /// Represents hand
        /// </summary>
        [Description("Hand")]
        hand = 0,
        /// <summary>
        /// Represents sparkplug wrench
        /// </summary>
        [Description("Spark plug socket")]
        sparkplug = 55,
        /// <summary>
        /// Represents flat head screwdriver
        /// </summary>
        [Description("Screwdriver")]
        flathead = 64,
        /// <summary>
        /// Represents 5mm
        /// </summary>
        [Description("5 MM")]
        _5mm = 50,
        /// <summary>
        /// Represents 6mm
        /// </summary>
        [Description("6 MM")]
        _6mm = 60,
        /// <summary>
        /// Represents 7mm
        /// </summary>
        [Description("7 MM")]
        _7mm = 69,
        /// <summary>
        /// Represents 8mm
        /// </summary>
        [Description("8 MM")]
        _8mm = 80,
        /// <summary>
        /// Represents 9mm
        /// </summary>
        [Description("9 MM")]
        _9mm = 89,
        /// <summary>
        /// Represents 10mm
        /// </summary>
        [Description("10 MM")]
        _10mm = 100,
        /// <summary>
        /// Represents 11mm
        /// </summary>
        [Description("11 MM")]
        _11mm = 110,
        /// <summary>
        /// Represents 12mm
        /// </summary>
        [Description("12 MM")]
        _12mm = 120,
        /// <summary>
        /// Represents 13mm
        /// </summary>
        [Description("13 MM")]
        _13mm = 129,
        /// <summary>
        /// Represents 14mm
        /// </summary>
        [Description("14 MM")]
        _14mm = 139,
        /// <summary>
        /// Represents 15mm
        /// </summary>
        [Description("15 MM")]
        _15mm = 150,
    }
}
