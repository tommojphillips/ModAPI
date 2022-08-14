using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database
{
    public static class Database
    {

        private static DatabaseMotor _databaseMotor;
        private static DatabaseMechanics _databaseMechanics;
        private static DatabaseOrders _databaseOrders;
        private static Status _partsStatus;

        /// <summary>
        /// Represents all stock engine parts.
        /// </summary>
        public static DatabaseMotor databaseMotor
        {
            get
            {
                if (_databaseMotor == null)
                    _databaseMotor = new DatabaseMotor();
                return _databaseMotor;
            }
        }
        public static DatabaseMechanics databaseMechanics
        {
            get
            {
                if (_databaseMechanics == null)
                    _databaseMechanics = new DatabaseMechanics();
                return _databaseMechanics;
            }
        }
        /// <summary>
        /// Represents all purchase-able parts.
        /// </summary>
        public static DatabaseOrders databaseOrders
        {
            get
            {
                if (_databaseOrders == null)
                    _databaseOrders = new DatabaseOrders();
                return _databaseOrders;
            }
        }
        /// <summary>
        /// Represents part stats for parts you have to buy. eg => battery, oil filters, etc.
        /// </summary>
        public static Status partsStatus
        {
            get
            {
                if (_partsStatus == null)
                    _partsStatus = new Status();
                return _partsStatus;
            }
        }

    } 
}
