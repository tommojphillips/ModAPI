using MSCLoader;
using UnityEngine;
using TommoJProductions.ModApi.Database.GameParts;
using System.Reflection;
using System.Linq;

namespace TommoJProductions.ModApi.Database
{
    /// <summary>
    /// Represents the Database Motor.
    /// </summary>
    public class DatabaseMotor
    {
        private GameObject _databaseMotorGo;
        private PropertyInfo[] _infos = null;

        internal GameObject getDatabaseMotor
        {
            get
            {
                if (!_databaseMotorGo)
                {
                    _databaseMotorGo = GameObject.Find("Database/DatabaseMotor");
                }
                return _databaseMotorGo;
            }
        }

        internal PlayMakerFSM getData(string childName)
        {
            return ModClient.getData(getDatabaseMotor, childName);
        }

        internal PropertyInfo[] getProperties() 
        {
            // Written, 20.07.2022

            if (_infos == null)
            {
                _infos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).ToArray();
            }
            return _infos;
        }

        // playmaker references
        private PlayMakerFSM _blockData;
        private PlayMakerFSM _oilPanData;
        private PlayMakerFSM _headgasketData;
        private PlayMakerFSM _piston1Data;
        private PlayMakerFSM _piston2Data;
        private PlayMakerFSM _piston3Data;
        private PlayMakerFSM _piston4Data;
        private PlayMakerFSM _crankshaftData;
        private PlayMakerFSM _rockershaftData;
        private PlayMakerFSM _alternatorData;
        private PlayMakerFSM _gearboxData;
        private PlayMakerFSM _waterpumpData;
        private PlayMakerFSM _clutchPressurePlateData;
        private PlayMakerFSM _clutchCoverPlateData;
        private PlayMakerFSM _clutchPlateData;
        private PlayMakerFSM _crankBearing1Data;
        private PlayMakerFSM _crankBearing2Data;
        private PlayMakerFSM _crankBearing3Data;
        private PlayMakerFSM _carburatorData;
        private PlayMakerFSM _airFilterData;
        private PlayMakerFSM _driveGearData;
        private PlayMakerFSM _cylinderHeadData;
        private PlayMakerFSM _distributorData;
        private PlayMakerFSM _enginePlateData;
        private PlayMakerFSM _fuelPumpData;
        private PlayMakerFSM _fuelLineData;
        private PlayMakerFSM _flywheelData;
        private PlayMakerFSM _timingChainData;
        private PlayMakerFSM _camshaftData;
        private PlayMakerFSM _camshaftGearData;
        private PlayMakerFSM _valveCoverData;
        private PlayMakerFSM _starterData;
        private PlayMakerFSM _timingCoverData;
        private PlayMakerFSM _crankwheelData;
        private PlayMakerFSM _waterPumpPulleyData;
        private PlayMakerFSM _headersData;
        private PlayMakerFSM _inspectionCoverData;

        // object references
        private Block _block;
        private OilPan _oilPan;
        private GamePartTime _headgasket;
        private GamePartWear _piston1;
        private GamePartWear _piston2;
        private GamePartWear _piston3;
        private GamePartWear _piston4;
        private GamePartTime _crankshaft;
        private RockerShaft _rockershaft;
        private GamePartTime _alternator;
        private GamePart _gearbox;
        private GamePart _waterpump;
        private GamePartTime _clutchPressurePlate;
        private GamePartTime _clutchCoverPlate;
        private GamePart _clutchPlate;
        private GamePartTime _crankBearing1;
        private GamePartTime _crankBearing2;
        private GamePartTime _crankBearing3;
        private Carburator _carburator;
        private GamePartTime _airFilter;
        private GamePartTime _driveGear;
        private GamePartTime _cylinderHead;
        private Distributor _distributor;
        private GamePartTime _enginePlate;
        private GamePartTime _fuelPump;
        private GamePart _fuelLine;
        private GamePartTime _flywheel;
        private GamePartTime _timingChain;
        private GamePartTime _camshaft;
        private CamshaftGear _camshaftGear;
        private GamePartTime _valveCover;
        private GamePartTime _starter;
        private GamePartTime _timingCover;
        private GamePartTime _crankwheel;
        private GamePartTime _waterPumpPulley;
        private GamePartTime _headers;
        private GamePartTime _inspectionCover;

        // Game Part Getters
        /// <summary>
        /// The block gamepart.
        /// </summary>
        public Block block
        {
            get
            {
                if (_blockData == null)
                {
                    _blockData = getData("Block");
                }
                if (_block == null)
                {
                    _block = new Block(_blockData);
                }
                return _block;
            }
        }
        /// <summary>
        /// The oilpan gamepart.
        /// </summary>
        public OilPan oilPan
        {
            get
            {
                if (_oilPanData == null)
                {
                    _oilPanData = getData("Oilpan");
                }
                if (_oilPan == null)
                {
                    _oilPan = new OilPan(_oilPanData);
                }
                return _oilPan;
            }
        }
        /// <summary>
        /// The head gasket gamepart.
        /// </summary>
        public GamePartTime headgasket
        {
            get
            {
                if (_headgasketData == null)
                {
                    _headgasketData = getData("Headgasket");
                }
                if (_headgasket == null)
                {
                    _headgasket = new GamePartTime(_headgasketData);
                }
                return _headgasket;
            }
        }
        /// <summary>
        /// The piston 1 gamepart.
        /// </summary>
        public GamePartWear piston1
        {
            get
            {
                if (_piston1Data == null)
                {
                    _piston1Data = getData("Piston1");
                }
                if (_piston1 == null)
                {
                    _piston1 = new GamePartWear(_piston1Data);
                }
                return _piston1;
            }
        }
        /// <summary>
        /// The piston 2 gamepart.
        /// </summary>
        public GamePartWear piston2
        {
            get
            {
                if (_piston2Data == null)
                {
                    _piston2Data = getData("Piston2");
                }
                if (_piston2 == null)
                {
                    _piston2 = new GamePartWear(_piston2Data);
                }
                return _piston2;
            }
        }
        /// <summary>
        /// The piston 3 gamepart.
        /// </summary>
        public GamePartWear piston3
        {
            get
            {
                if (_piston3Data == null)
                {
                    _piston3Data = getData("Piston3");
                }
                if (_piston3 == null)
                {
                    _piston3 = new GamePartWear(_piston3Data);
                }
                return _piston3;
            }
        }
        /// <summary>
        /// The piston 4 gamepart.
        /// </summary>
        public GamePartWear piston4
        {
            get
            {
                if (_piston4Data == null)
                {
                    _piston4Data = getData("Piston4");
                }
                if (_piston4 == null)
                {
                    _piston4 = new GamePartWear(_piston4Data);
                }
                return _piston4;
            }
        }
        /// <summary>
        /// The crankshaft gamepart.
        /// </summary>
        public GamePartTime crankshaft
        {
            get
            {
                if (_crankshaftData == null)
                {
                    _crankshaftData = getData("Crankshaft");
                }
                if (_crankshaft == null)
                {
                    _crankshaft = new GamePartTime(_crankshaftData);
                }
                return _crankshaft;
            }
        }
        /// <summary>
        /// The rockershaft gamepart.
        /// </summary>
        public RockerShaft rockershaft
        {
            get
            {
                if (_rockershaftData == null)
                {
                    _rockershaftData = getData("RockerShaft");
                }
                if (_rockershaft == null)
                {
                    _rockershaft = new RockerShaft(_rockershaftData);
                }
                return _rockershaft;
            }
        }
        /// <summary>
        /// The alternator gamepart.
        /// </summary>
        public GamePartTime alternator
        {
            get
            {
                if (_alternatorData == null)
                {
                    _alternatorData = getData("Alternator");
                }
                if (_alternator == null)
                {
                    _alternator = new GamePartTime(_alternatorData);
                }
                return _alternator;
            }
        }
        /// <summary>
        /// The gearbox gamepart.
        /// </summary>
        public GamePart gearbox
        {
            get
            {
                if (_gearboxData == null)
                {
                    _gearboxData = getData("Gearbox");
                }
                if (_gearbox == null)
                {
                    _gearbox = new GamePart(_gearboxData);
                }
                return _gearbox;
            }
        }
        /// <summary>
        /// The water pump gamepart.
        /// </summary>
        public GamePart waterpump
        {
            get
            {
                if (_waterpumpData == null)
                {
                    _waterpumpData = getData("Waterpump");
                }
                if (_waterpump == null)
                {
                    _waterpump = new GamePart(_waterpumpData);
                }
                return _waterpump;
            }
        }
        /// <summary>
        /// The clutch pressure plate gamepart.
        /// </summary>
        public GamePartTime clutchPressurePlate
        {
            get
            {
                if (_clutchPressurePlateData == null)
                {
                    _clutchPressurePlateData = getData("ClutchPressureplate");
                }
                if (_clutchPressurePlate == null)
                {
                    _clutchPressurePlate = new GamePartTime(_clutchPressurePlateData);
                }
                return _clutchPressurePlate;
            }
        }
        /// <summary>
        /// The clutch cover plate gamepart.
        /// </summary>
        public GamePartTime clutchCoverPlate
        {
            get
            {
                if (_clutchCoverPlateData == null)
                {
                    _clutchCoverPlateData = getData("ClutchCoverplate");
                }
                if (_clutchCoverPlate == null)
                {
                    _clutchCoverPlate = new GamePartTime(_clutchCoverPlateData);
                }
                return _clutchCoverPlate;
            }
        }
        /// <summary>
        /// The clutch friction plate gamepart.
        /// </summary>
        public GamePart clutchPlate
        {
            get
            {
                if (_clutchPlateData == null)
                {
                    _clutchPlateData = getData("ClutchPlate");
                }
                if (_clutchPlate == null)
                {
                    _clutchPlate = new GamePart(_clutchPlateData);
                }
                return _clutchPlate;
            }
        }
        /// <summary>
        /// The crank bearing 1 gamepart.
        /// </summary>
        public GamePartTime crankBearing1
        {
            get
            {
                if (_crankBearing1Data == null)
                {
                    _crankBearing1Data = getData("CrankBearing1");
                }
                if (_crankBearing1 == null)
                {
                    _crankBearing1 = new GamePartTime(_crankBearing1Data);
                }
                return _crankBearing1;
            }
        }
        /// <summary>
        /// The crank bearing 2 gamepart.
        /// </summary>
        public GamePartTime crankBearing2
        {
            get
            {
                if (_crankBearing2Data == null)
                {
                    _crankBearing2Data = getData("CrankBearing2");
                }
                if (_crankBearing2 == null)
                {
                    _crankBearing2 = new GamePartTime(_crankBearing2Data);
                }
                return _crankBearing2;
            }
        }
        /// <summary>
        /// The crank bearing 3 gamepart.
        /// </summary>
        public GamePartTime crankBearing3
        {
            get
            {
                if (_crankBearing3Data == null)
                {
                    _crankBearing3Data = getData("CrankBearing3");
                }
                if (_crankBearing3 == null)
                {
                    _crankBearing3 = new GamePartTime(_crankBearing3Data);
                }
                return _crankBearing3;
            }
        }
        /// <summary>
        /// The stock carburator gamepart.
        /// </summary>
        public Carburator carburator
        {
            get
            {
                if (_carburatorData == null)
                {
                    _carburatorData = getData("Carburator");
                }
                if (_carburator == null)
                {
                    _carburator = new Carburator(_carburatorData);
                }
                return _carburator;
            }
        }
        /// <summary>
        /// The stock air filter gamepart.
        /// </summary>
        public GamePartTime airFilter
        {
            get
            {
                if (_airFilterData == null)
                {
                    _airFilterData = getData("Airfilter");
                }
                if (_airFilter == null)
                {
                    _airFilter = new GamePartTime(_airFilterData);
                }
                return _airFilter;
            }
        }
        /// <summary>
        /// The drive gear gamepart.
        /// </summary>
        public GamePartTime driveGear
        {
            get
            {
                if (_driveGearData == null)
                {
                    _driveGearData = getData("Drivegear");
                }
                if (_driveGear == null)
                {
                    _driveGear = new GamePartTime(_driveGearData);
                }
                return _driveGear;
            }
        }
        /// <summary>
        /// The cylinder head gamepart.
        /// </summary>
        public GamePartTime cylinderHead
        {
            get
            {
                if (_cylinderHeadData == null)
                {
                    _cylinderHeadData = getData("Cylinderhead");
                }
                if (_cylinderHead == null)
                {
                    _cylinderHead = new GamePartTime(_cylinderHeadData);
                }
                return _cylinderHead;
            }
        }
        /// <summary>
        /// The distributor gamepart.
        /// </summary>
        public Distributor distributor
        {
            get
            {
                if (_distributorData == null)
                {
                    _distributorData = getData("Distributor");
                }
                if (_distributor == null)
                {
                    _distributor = new Distributor(_distributorData);
                }
                return _distributor;
            }
        }
        /// <summary>
        /// The engine plate gamepart.
        /// </summary>
        public GamePartTime enginePlate
        {
            get
            {
                if (_enginePlateData == null)
                {
                    _enginePlateData = getData("Engineplate");
                }
                if (_enginePlate == null)
                {
                    _enginePlate = new GamePartTime(_enginePlateData);
                }
                return _enginePlate;
            }
        }
        /// <summary>
        /// The fuel pump gamepart.
        /// </summary>
        public GamePartTime fuelPump
        {
            get
            {
                if (_fuelPumpData == null)
                {
                    _fuelPumpData = getData("Fuelpump");
                }
                if (_fuelPump == null)
                {
                    _fuelPump = new GamePartTime(_fuelPumpData);
                }
                return _fuelPump;
            }
        }
        /// <summary>
        /// The fuel line gamepart.
        /// </summary>
        public GamePart fuelLine
        {
            get
            {
                if (_fuelLineData == null)
                {
                    _fuelLineData = getData("FuelLine");
                }
                if (_fuelLine == null)
                {
                    _fuelLine = new GamePart(_fuelLineData);
                }
                return _fuelLine;
            }
        }
        /// <summary>
        /// The flywheel gamepart.
        /// </summary>
        public GamePartTime flywheel
        {
            get
            {
                if (_flywheelData == null)
                {
                    _flywheelData = getData("Flywheel");
                }
                if (_flywheel == null)
                {
                    _flywheel = new GamePartTime(_flywheelData);
                }
                return _flywheel;
            }
        }
        /// <summary>
        /// The timing chain gamepart.
        /// </summary>
        public GamePartTime timingChain
        {
            get
            {
                if (_timingChainData == null)
                {
                    _timingChainData = getData("Timingchain");
                }
                if (_timingChain == null)
                {
                    _timingChain = new GamePartTime(_timingChainData);
                }
                return _timingChain;
            }
        }
        /// <summary>
        /// The camshaft gamepart.
        /// </summary>
        public GamePartTime camshaft
        {
            get
            {
                if
                    (_camshaftData == null)
                {
                    _camshaftData = getData("Camshaft");
                }
                if (_camshaft == null)
                {
                    _camshaft = new GamePartTime(_camshaftData);
                }
                return _camshaft;
            }
        }
        /// <summary>
        /// The camshaft gear gamepart.
        /// </summary>
        public CamshaftGear camshaftGear
        {
            get
            {
                if
                    (_camshaftGearData == null)
                {
                    _camshaftGearData = getData("CamshaftGear");
                }
                if (_camshaftGear == null)
                {
                    _camshaftGear = new CamshaftGear(_camshaftGearData);
                }
                return _camshaftGear;
            }
        }
        /// <summary>
        /// The vavle cover gamepart.
        /// </summary>
        public GamePartTime valveCover
        {
            get
            {
                if (_valveCoverData == null)
                {
                    _valveCoverData = getData("Valvecover");
                }
                if (_valveCover == null)
                {
                    _valveCover = new GamePartTime(_valveCoverData);
                }
                return _valveCover;
            }
        }
        /// <summary>
        /// The starter gamepart.
        /// </summary>
        public GamePartTime starter
        {
            get
            {
                if (_starterData == null)
                {
                    _starterData = getData("Starter");
                }
                if (_starter == null)
                {
                    _starter = new GamePartTime(_starterData);
                }
                return _starter;
            }
        }
        /// <summary>
        /// The timing cover gamepart.
        /// </summary>
        public GamePartTime timingCover
        {
            get
            {
                if (_timingCoverData == null)
                {
                    _timingCoverData = getData("Timingcover");
                }
                if (_timingCover == null)
                {
                    _timingCover = new GamePartTime(_timingCoverData);
                }
                return _timingCover;
            }
        }
        /// <summary>
        /// The crank pulley gamepart.
        /// </summary>
        public GamePartTime crankwheel
        {
            get
            {
                if (_crankwheelData == null)
                {
                    _crankwheelData = getData("Crankwheel");
                }
                if (_crankwheel == null)
                {
                    _crankwheel = new GamePartTime(_crankwheelData);
                }
                return _crankwheel;
            }
        }
        /// <summary>
        /// The water pump pulley gamepart.
        /// </summary>
        public GamePartTime waterPumpPulley
        {
            get
            {
                if (_waterPumpPulleyData == null)
                {
                    _waterPumpPulleyData = getData("WaterpumpPulley");
                }
                if (_waterPumpPulley == null)
                {
                    _waterPumpPulley = new GamePartTime(_waterPumpPulleyData);
                }
                return _waterPumpPulley;
            }
        }
        /// <summary>
        /// The stock exhaust headers gamepart.
        /// </summary>
        public GamePartTime headers
        {
            get
            {
                if (_headersData == null)
                {
                    _headersData = getData("Headers");
                }
                if (_headers == null)
                {
                    _headers = new GamePartTime(_headersData);
                }
                return _headers;
            }
        }
        /// <summary>
        /// The clutch inspection cover gamepart.
        /// </summary>
        public GamePartTime inspectionCover
        {
            get
            {
                if (_inspectionCoverData == null)
                {
                    _inspectionCoverData = getData("InspectionCover");
                }
                if (_inspectionCover == null)
                {
                    _inspectionCover = new GamePartTime(_inspectionCoverData);
                }
                return _inspectionCover;
            }
        }
    }
}
