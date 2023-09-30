using HutongGames.PlayMaker;
using UnityEngine;

namespace TommoJProductions.ModApi.Database
{
    /// <summary>
    /// Represents the status database.
    /// </summary>
    public class Status
    {
        private static GameObject _partsStatusGo;
        /// <summary>
        /// Represents the Part Status database.
        /// </summary>
        internal static GameObject getDatabasePartsStatusGameobject
        {
            get
            {
                if (!_partsStatusGo)
                {
                    _partsStatusGo = GameObject.Find("Database/PartsStatus");
                }
                return _partsStatusGo;
            }
        }
    }
    /// <summary>
    /// Represents the battery status.
    /// </summary>
    public class Battery
    {
        private readonly FsmFloat _positiveBoltTightness;
        private readonly FsmFloat _negativeBoltTightness;

        /// <summary>
        /// The positive terminal bolt tightness.
        /// </summary>
        public FsmFloat positiveBoltTightness
        {
            get
            {
                if (_positiveBoltTightness == null) 
                    ;
                return _positiveBoltTightness;
            }
        }
        /// <summary>
        /// The negative terminal bolt tightness 
        /// </summary>
        public FsmFloat negativeBoltTightness
        {
            get
            {
                if (_negativeBoltTightness == null)
                    ;
                return _negativeBoltTightness;
            }
        }
    }
}
