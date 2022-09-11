using HutongGames.PlayMaker;
using UnityEngine;

namespace TommoJProductions.ModApi.Database
{
    public class Status
    {
        private static GameObject _partsStatusGo;

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
}
namespace TommoJProductions.ModApi.Database.PartsStatus
{
    public class Battery
    {
        private readonly FsmFloat _positiveBoltTightness;
        private readonly FsmFloat _negativeBoltTightness;

        public FsmFloat positiveBoltTightness
        {
            get
            {
                if (_positiveBoltTightness == null) 
                    ;
                return _positiveBoltTightness;
            }
        }
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
