using UnityEngine;

namespace TommoJProductions.ModApi.Database
{
    /// <summary>
    /// Represents the database orders.
    /// </summary>
    public class DatabaseOrders
    {
        private static GameObject _databaseOrdersGo;
        /// <summary>
        /// [CACHE] the database orders gameobject.
        /// </summary>
        internal static GameObject getDatabaseOrdersGameobject
        {
            get
            {
                if (!_databaseOrdersGo)
                {
                    _databaseOrdersGo = GameObject.Find("Database/DatabaseOrders");
                }
                return _databaseOrdersGo;
            }
        }
    }
}
