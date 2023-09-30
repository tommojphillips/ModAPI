using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents the Has Bolts interface. 
    /// </summary>
    public interface IHasBolts
    {
        /// <summary>
        /// The bolts.
        /// </summary>
        Bolt[] bolts { get; }
        /// <summary>
        /// <see langword="true"/> if this object has bolts.
        /// </summary>
        bool hasBolts { get; }
        /// <summary>
        /// inits bolts.
        /// </summary>
        /// <param name="bolts">the array of bolt references to init.</param>
        void initBolts(Bolt[] bolts);
    }
}
