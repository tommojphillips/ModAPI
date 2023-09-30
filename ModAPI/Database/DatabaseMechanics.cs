using UnityEngine;

namespace TommoJProductions.ModApi.Database
{
    /// <summary>
    /// Represents the Database Mechanics
    /// </summary>
    public class DatabaseMechanics
    {
        /// <summary>
        /// Represents the database mechanics gameobject.
        /// </summary>
        private static GameObject _databaseMechanicsGo;

        /// <summary>
        /// [CACHE] The database mechanics gameobject.
        /// </summary>
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
