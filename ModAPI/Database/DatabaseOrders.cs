using UnityEngine;

namespace TommoJProductions.ModApi.Database
{
    public class DatabaseOrders
    {
        private static GameObject _databaseOrdersGo;

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
