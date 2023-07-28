using UnityEngine;

namespace TommoJProductions.ModApi.Database
{
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
