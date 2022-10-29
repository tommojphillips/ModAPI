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
        public GameObject jonnez
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
        public GameObject kekmet
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
        public GameObject hayosiko
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
        public GameObject ruscko
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
        public GameObject ferndale
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
        public GameObject combine
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
        public GameObject gifu
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
    }
    /// <summary>
    /// Represents vehicle data.
    /// </summary>
    public class DatabaseVehicle
    {
        private Drivetrain _drivetrain;
        private CarDynamics _carDynamics;

        /// <summary>
        /// Represents the vehicles root game object.
        /// </summary>
        public readonly GameObject gameObject;
        /// <summary>
        /// Represents the vehicles drivetrain.
        /// </summary>
        public Drivetrain drivetrain 
        {
            get
            {
                if (!_drivetrain)
                {
                    _drivetrain = gameObject.GetComponent<Drivetrain>();
                }
                return _drivetrain;
            }
        }
        /// <summary>
        /// Represents the vehicles car dynamics.
        /// </summary>
        public CarDynamics carDynamics
        {
            get
            {
                if (!_carDynamics)
                {
                    _carDynamics = gameObject.GetComponent<CarDynamics>();
                }
                return _carDynamics;
            }
        }

        /// <summary>
        /// inits new instance of database vehicle.
        /// </summary>
        /// <param name="vehicleReference">the reference to the root of a vehicle.</param>
        public DatabaseVehicle(GameObject vehicleReference)
        {
            this.gameObject = vehicleReference;
        }



        /// <summary>
        /// Gets the gameobject of the vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        public static implicit operator GameObject(DatabaseVehicle vehicle) => vehicle.gameObject;
    }
    /// <summary>
    /// Represents the satsuma.
    /// </summary>
    public class Satsuma : DatabaseVehicle
    {
        private GameObject _engine;
        private GameObject _carSimulation;

        /// <summary>
        /// Represents the satsumas engine gameobject. if the engine is on, this gameobject is active.
        /// </summary>
        public GameObject engine
        {
            get
            {
                if (!_engine)
                {
                    _engine = carSimulation.transform.Find("Engine").gameObject;
                }
                return _engine;
            }
        }
        /// <summary>
        /// Represents the satsuma car simulatin game object.
        /// </summary>
        public GameObject carSimulation
        {
            get
            {
                if (!_carSimulation)
                {
                    _carSimulation = gameObject.transform.Find("CarSimulation").gameObject;
                }
                return _carSimulation;
            }
        }
        /// <summary>
        /// inits new instance of satsuma.
        /// </summary>
        /// <param name="vehicleReference">the reference to the root of a vehicle.</param>
        public Satsuma(GameObject vehicleReference) : base(vehicleReference) { }
    }
}
