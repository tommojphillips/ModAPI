using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database
{
    /// <summary>
    /// Represents the game database.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Represents the motor database.
        /// </summary>
        private DatabaseMotor motor;
        /// <summary>
        /// Represents the mechanics database.
        /// </summary>
        private DatabaseMechanics mechanics;
        /// <summary>
        /// Represents the part orders database.
        /// </summary>
        private DatabaseOrders orders;
        private Status status;
        private DatabaseVehicles vehicles;

        private static Database _instance;

        private static Database instance => _instance;

        internal static void refreshCache() 
        {
            _instance = new Database();
        }

        /// <summary>
        /// Represents all stock engine parts.
        /// </summary>
        public static DatabaseMotor databaseMotor
        {
            get
            {
                if (_instance.motor == null)
                    _instance.motor = new DatabaseMotor();
                return _instance.motor;
            }
        }
        /// <summary>
        /// Represents all database mechanics
        /// </summary>
        public static DatabaseMechanics databaseMechanics
        {
            get
            {
                if (_instance.mechanics == null)
                    _instance.mechanics = new DatabaseMechanics();
                return _instance.mechanics;
            }
        }
        /// <summary>
        /// Represents all purchase-able parts.
        /// </summary>
        public static DatabaseOrders databaseOrders
        {
            get
            {
                if (_instance.orders == null)
                    _instance.orders = new DatabaseOrders();
                return _instance.orders;
            }
        }
        /// <summary>
        /// Represents part stats for parts you have to buy. eg => battery, oil filters, etc.
        /// </summary>
        public static Status partsStatus
        {
            get
            {
                if (_instance.status == null)
                    _instance.status = new Status();
                return _instance.status;
            }
        }
        /// <summary>
        /// Represents the vehicle database. all vehicle references in the game.
        /// </summary>
        public static DatabaseVehicles databaseVehicles
        {
            get
            {
                if (_instance.vehicles == null)
                    _instance.vehicles = new DatabaseVehicles();
                return _instance.vehicles;
            }
        }

    }
}
