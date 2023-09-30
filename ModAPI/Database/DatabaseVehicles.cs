using System.Linq;
using TommoJProductions.ModApi.Attachable;

using UnityEngine;

namespace TommoJProductions.ModApi.Database
{
    /// <summary>
    /// Represents all vehicles in the game.
    /// </summary>
    public class DatabaseVehicles
    {
        private Satsuma _satsuma;
        private DatabaseVehicle _jonnez;
        private DatabaseVehicle _kekmet;
        private DatabaseVehicle _hayosiko;
        private DatabaseVehicle _ruscko;
        private DatabaseVehicle _ferndale;
        private DatabaseVehicle _combine;
        private DatabaseVehicle _gifu;
        private DatabaseVehicle[] _databaseVehicles;

        /// <summary>
        /// Represents the satsuma.
        /// </summary>
        public Satsuma satsuma
        {
            get
            {
                if (_satsuma == null)
                {
                    _satsuma = new Satsuma(Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.name == "SATSUMA(557kg, 248)")?.ToArray()[0]);
                }
                return _satsuma;
            }
        }
        /// <summary>
        /// Represents the jonnez
        /// </summary>
        public DatabaseVehicle jonnez
        {
            get
            {
                if (_jonnez == null)
                {
                    _jonnez = new DatabaseVehicle(Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.name == "JONNEZ ES(Clone)")?.ToArray()[0]);
                }
                return _jonnez;
            }
        }
        /// <summary>
        /// Represents the kekment.
        /// </summary>
        public DatabaseVehicle kekmet
        {
            get
            {
                if (_kekmet == null)
                {
                    _kekmet = new DatabaseVehicle(Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.name == "KEKMET(350-400psi)")?.ToArray()[0]);
                }
                return _kekmet;
            }
        }
        /// <summary>
        /// Represents the hayosiko.
        /// </summary>
        public DatabaseVehicle hayosiko
        {
            get
            {
                if (_hayosiko == null)
                {
                    _hayosiko = new DatabaseVehicle(Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.name == "HAYOSIKO(1500kg, 250)")?.ToArray()[0]);
                }
                return _hayosiko;
            }
        }
        /// <summary>
        /// Represents the Ruscko
        /// </summary>
        public DatabaseVehicle ruscko
        {
            get
            {
                if (_ruscko == null)
                {
                    _ruscko = new DatabaseVehicle(Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.name == "RCO_RUSCKO12(270)")?.ToArray()[0]);
                }
                return _ruscko;
            }
        }
        /// <summary>
        /// Represents the Ferndale.
        /// </summary>
        public DatabaseVehicle ferndale
        {
            get
            {
                if (_ferndale == null)
                {
                    _ferndale = new DatabaseVehicle(Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.name == "FERNDALE(1630kg)")?.ToArray()[0]);
                }
                return _ferndale;
            }
        }
        /// <summary>
        /// Represents the Combine Harvester.
        /// </summary>
        public DatabaseVehicle combine
        {
            get
            {
                if (_combine == null)
                {
                    _combine = new DatabaseVehicle(Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.name == "COMBINE(350-400psi)")?.ToArray()[0]);
                }
                return _combine;
            }
        }
        /// <summary>
        /// Represents the GIFU poop truck.
        /// </summary>
        public DatabaseVehicle gifu
        {
            get
            {
                if (_gifu == null)
                {
                    _gifu = new DatabaseVehicle(Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.name == "GIFU(750/450psi)")?.ToArray()[0]);
                }
                return _gifu;
            }
        }
        /// <summary>
        /// Represents the all vehicles in game as an array. (includes mod vehicles)
        /// </summary>
        public DatabaseVehicle[] databaseVehicles 
        {
            get
            {
                if (_databaseVehicles == null)
                {
                    GameObject[] gos = Resources.FindObjectsOfTypeAll<Drivetrain>().Select(dt => dt.gameObject).ToArray();
                    _databaseVehicles = new DatabaseVehicle[gos.Length];
                    for (int i = 0; i < gos.Length; i++)
                    {
                        _databaseVehicles[i] = new DatabaseVehicle(gos[i]);
                    }
                }
                return _databaseVehicles;
            }
        }
        /// <summary>
        /// Refreshes data base vehicles. next call to <see cref="databaseVehicles"/> will refresh array.
        /// </summary>
        public void refreshVehicles()
        {
            // Written, 20.08.2023

            _databaseVehicles = null;
        }
    }
}
