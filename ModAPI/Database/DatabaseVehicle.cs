using UnityEngine;

namespace TommoJProductions.ModApi.Database
{
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
}
