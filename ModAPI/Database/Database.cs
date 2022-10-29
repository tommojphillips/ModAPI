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
        private DatabaseMotor motor;
        private DatabaseMechanics mechanics;
        private DatabaseOrders orders;
        private Status status;
        private DatabaseVehicles vehicles;

        private static Database instance;

        private static Database getInstance 
        {
            get 
            {
                if (instance == null)
                {
                    instance = new Database();
                }
                return instance;
            }
        }

        internal static void deleteCache() 
        {
            instance = null;
        }

        /// <summary>
        /// Represents all stock engine parts.
        /// </summary>
        public static DatabaseMotor databaseMotor
        {
            get
            {
                if (getInstance.motor == null)
                    getInstance.motor = new DatabaseMotor();
                return getInstance.motor;
            }
        }
        /// <summary>
        /// Represents all database mechanics
        /// </summary>
        public static DatabaseMechanics databaseMechanics
        {
            get
            {
                if (getInstance.mechanics == null)
                    getInstance.mechanics = new DatabaseMechanics();
                return getInstance.mechanics;
            }
        }
        /// <summary>
        /// Represents all purchase-able parts.
        /// </summary>
        public static DatabaseOrders databaseOrders
        {
            get
            {
                if (getInstance.orders == null)
                    getInstance.orders = new DatabaseOrders();
                return getInstance.orders;
            }
        }
        /// <summary>
        /// Represents part stats for parts you have to buy. eg => battery, oil filters, etc.
        /// </summary>
        public static Status partsStatus
        {
            get
            {
                if (getInstance.status == null)
                    getInstance.status = new Status();
                return getInstance.status;
            }
        }
        /// <summary>
        /// Represents the vehicle database. all vehicle references in the game.
        /// </summary>
        public static DatabaseVehicles databaseVehicles
        {
            get
            {
                if (getInstance.vehicles == null)
                    getInstance.vehicles = new DatabaseVehicles();
                return getInstance.vehicles;
            }
        }

    }
}
