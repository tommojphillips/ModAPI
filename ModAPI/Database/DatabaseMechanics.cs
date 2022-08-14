using UnityEngine;

namespace TommoJProductions.ModApi.Database
{
    public class DatabaseMechanics
    {
        private static GameObject _databaseMechanicsGo;

        internal static GameObject getDatabaseMechanicsGameobject
        {
            get
            {
                if (!_databaseMechanicsGo)
                {
                    _databaseMechanicsGo = GameObject.Find("Database/DatabaseMechanics");
                }
                return _databaseMechanicsGo;
            }
        }
    }
}
