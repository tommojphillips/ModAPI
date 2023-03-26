using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using TommoJProductions.ModApi.Attachable;
using TommoJProductions.ModApi.Database;
using UnityEngine;
using MSCLoader;
using TommoJProductions.ModApi.PlaymakerExtentions.Callbacks;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using static UnityEngine.GUILayout;
using TommoJProductions.ModApi.Attachable.CallBacks;
using System.IO;
using TommoJProductions.ModApi.Database.GameParts;

namespace TommoJProductions.ModApi
{
    #region enums

    /// <summary>
    /// Represents inject types.
    /// </summary>
    public enum InjectEnum
    {
        /// <summary>
        /// Rpresents inject type, append. appends an action a fsmState.
        /// </summary>
        append,
        /// <summary>
        /// Rpresents inject type, prepend. prepends an action a fsmState
        /// </summary>
        prepend,
        /// <summary>
        /// Rpresents inject type, insert. insert an action in a fsmState at an index
        /// </summary>
        insert,
        /// <summary>
        /// Rpresents inject type, replace. replace an action in a fsmState at an index.
        /// </summary>
        replace
    }
    /// <summary>
    /// Represents callback types for playmaker action injection any of the action extention methods: append, prepend, replace, insert. see <see cref="InjectEnum"/>.
    /// </summary>
    public enum CallbackTypeEnum
    {
        /// <summary>
        /// Represents the on enter callback.
        /// </summary>
        onEnter,
        /// <summary>
        /// Represents the on  fixed update callback. 
        /// </summary>
        onFixedUpdate,
        /// <summary>
        /// Represents the on update callback. 
        /// </summary>
        onUpdate,
        /// <summary>
        /// Represents the on gui callback.
        /// </summary>
        onGui
    }
    /// <summary>
    /// Represents all databases in game.
    /// </summary>
    public enum Databases
    {
        // Written, 04.07.2022

        /// <summary>
        /// Represents the motor database.
        /// </summary>
        [Description("DatabaseMotor")]
        motor,
        /// <summary>
        /// Represents the mechanics database.
        /// </summary>
        [Description("DatabaseMechanics")]
        mechanics,
        /// <summary>
        /// Represents the orders database.
        /// </summary>
        [Description("DatabaseOrders")]
        orders,
        /// <summary>
        /// Represents the body database.
        /// </summary>
        [Description("DatabaseBody")]
        body
    }
    /// <summary>
    /// Represents gui interact symbols.
    /// </summary>
    public enum GuiInteractSymbolEnum
    {
        // Written, 16.03.2019

        /// <summary>
        /// Represents no symbol.
        /// </summary>
        None,
        /// <summary>
        /// Represents the hand symbol.
        /// </summary>
        Hand,
        /// <summary>
        /// Represents the disassemble symbol.
        /// </summary>
        Disassemble,
        /// <summary>
        /// Represents the assemble symbol.
        /// </summary>
        Assemble,
    }
    /// <summary>
    /// Represents a player mode.
    /// </summary>
    public enum PlayerModeEnum
    {
        // Written, 09.08.2018

        /// <summary>
        /// Represents that the player is on foot.
        /// </summary>
        OnFoot,
        /// <summary>
        /// Represents that the player is driving the satsuma.
        /// </summary>
        InSatsuma,
        /// <summary>
        /// Represents the the player is driving other.
        /// </summary>
        InOther,
    }
    /// <summary>
    /// Represents all layers of my summer car.
    /// </summary>
    public enum LayerMasksEnum
    {
        /// <summary>
        /// Represents the default layer.
        /// </summary>
        Default,
        /// <summary>
        /// Represents the transparent fx layer.
        /// </summary>
        TransparentFX,
        /// <summary>
        /// Represents the ignore raycast layer.
        /// </summary>
        IgnoreRaycast,
        /// <summary>
        /// Represents an empty layer.
        /// </summary>
        Layer3,
        /// <summary>
        /// Represents the water layer.
        /// </summary>
        Water,
        /// <summary>
        /// Represents the user interaction layer.
        /// </summary>
        UI,
        /// <summary>
        /// Represents an empty layer.
        /// </summary>
        Layer6,
        /// <summary>
        /// Represents an empty layer.
        /// </summary>
        Layer7,
        /// <summary>
        /// Represents the road layer.
        /// </summary>
        Road,
        /// <summary>
        /// Represents the hinged objects layer.
        /// </summary>
        HingedObjects,
        /// <summary>
        /// Represents the terrian layer.
        /// </summary>
        Terrain,
        /// <summary>
        /// Represents the dont collide layer.
        /// </summary>
        DontCollide,
        /// <summary>
        /// Represents the bolts layer.
        /// </summary>
        Bolts,
        /// <summary>
        /// Represents the dashboard layer.
        /// </summary>
        Dashboard,
        /// <summary>
        /// Represents the graphical user interface layer.
        /// </summary>
        GUI,
        /// <summary>
        /// Represents the tool layer.
        /// </summary>
        Tools,
        /// <summary>
        /// Represents the wheel layer.
        /// </summary>
        Wheel,
        /// <summary>
        /// Represents the collider layer.
        /// </summary>
        Collider,
        /// <summary>
        /// Represents the datsun layer. (all cars use this layer)
        /// </summary>
        Datsun,
        /// <summary>
        /// Represents the parts layer.
        /// </summary>
        Parts,
        /// <summary>
        /// Represents the player layer.
        /// </summary>
        Player,
        /// <summary>
        /// Represents the lifter layer.
        /// </summary>
        Lifter,
        /// <summary>
        /// Represents the collider2 layer.
        /// </summary>
        Collider2,
        /// <summary>
        /// Represents the player only collider layer.
        /// </summary>
        PlayerOnlyColl,
        /// <summary>
        /// Represents the cloud layer.
        /// </summary>
        Cloud,
        /// <summary>
        /// Represents the grass layer.
        /// </summary>
        Glass,
        /// <summary>
        /// Represents the forest layer.
        /// </summary>
        Forest,
        /// <summary>
        /// Represents the no rain layer.
        /// </summary>
        NoRain,
        /// <summary>
        /// Represents the trigger-only layer.
        /// </summary>
        TriggerOnly,
        /// <summary>
        /// Represents an empty layer.
        /// </summary>
        Layer29,
        /// <summary>
        /// Represents an empty layer.
        /// </summary>
        Layer30,
    }

    #endregion

    #region Excections

    /// <summary>
    /// reps a save data not found error
    /// </summary>
    public class saveDataNotFoundException : Exception 
    {
        // Written, 15.07.2022

        /// <summary>
        /// reps a save data not found error with a message.
        /// </summary>
        /// <param name="message"></param>
        public saveDataNotFoundException(string message) : base(message) { }
    }

    #endregion      

    /// <summary>
    /// Represents useful properties for interacting with My Summer Car and PlayMaker.
    /// </summary>
    public class ModClient
    {
        // Written, 10.08.2018 | Modified 25.09.2021

        #region fields

        /// <summary>
        /// Represents the complied runtime version of the api.
        /// </summary>
        public const string VERSION = VersionInfo.version;
        
#if DEBUG
        internal static bool devMode = true;
#else
        internal static bool devMode = false;
#endif

        private static ModClient instance;

        private GameObject _masterAudioGameObject;
        private GameObject _player;
        private GameObject _fps;

        private AudioSource _assembleAudio;
        private AudioSource _disassembleAudio;
        private AudioSource _screwAudio;

        private Material _activeBoltMaterial;

        private Camera _playerCamera;

        private PlayMakerFSM _pickUp;

        private FsmGameObject _pickedUpGameObject;
        private FsmGameObject _raycastHitGameObject;
        private FsmGameObject _POV;

        private FsmBool _guiDisassemble;
        private FsmBool _guiAssemble;
        private FsmBool _guiUse;
        private FsmBool _guiDrive;
        private FsmBool _handEmpty;
        private FsmBool _playerInMenu;
        private FsmBool _ratchetSwitch;
        private FsmBool _playerHasRatchet;

        private FsmString _guiInteraction;
        private FsmString _playerCurrentVehicle;

        private FsmFloat _toolWrenchSize;

        private FieldInfo _modsFolderFieldInfo;

        private string[] _maskNames;
        private string _propertyString = "";
        private string _gameDirectory;

        private int _propertyInt = 0;
        private float _propertyFloat = 0;
        private double _propertyDouble = 0;

        private readonly Dictionary<string, AudioSource> masterAudioDictionary = new Dictionary<string, AudioSource>();
        internal readonly Dictionary<string, string> descriptionCache = new Dictionary<string, string>();

        internal static bool boltAssetsLoaded { get; set; } = false;

        internal static GameObject shortBoltPrefab;
        internal static GameObject longBoltPrefab;
        internal static GameObject screwPrefab;
        internal static GameObject nutPrefab;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the player picks up a gameobject.
        /// </summary>
        ///
        public static event Action<GameObject> onGameObjectPickUp;
        /// <summary>
        /// Occurs when the player drops a gameobject.
        /// </summary>
        public static event Action<GameObject> onGameObjectDrop;
        /// <summary>
        /// Occurs when the player throws a gameobject.
        /// </summary>
        public static event Action<GameObject> onGameObjectThrow;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current <see cref="ModClient"/> instance.
        /// </summary>
        public static ModClient getInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ModClient();
                }
                return instance;
            }
        }

        /// <summary>
        /// Represents the assemble assemble audio source.
        /// </summary>
        public static AudioSource assembleAudio
        {
            get
            {
                if (getInstance._assembleAudio == null)
                    getInstance._assembleAudio = GameObject.Find("MasterAudio/CarBuilding/assemble").GetComponent<AudioSource>();
                return getInstance._assembleAudio;
            }
        }
        /// <summary>
        /// Represents the disassemble audio source.
        /// </summary>
        public static AudioSource disassembleAudio
        {
            get
            {
                if (getInstance._disassembleAudio == null)
                    getInstance._disassembleAudio = GameObject.Find("MasterAudio/CarBuilding/disassemble").GetComponent<AudioSource>();
                return getInstance._disassembleAudio;
            }
        }
        /// <summary>
        /// Represents the bolt screw audio source.
        /// </summary>
        public static AudioSource screwAudio
        {
            get
            {
                if (getInstance._screwAudio == null)
                    getInstance._screwAudio = GameObject.Find("MasterAudio/CarBuilding/bolt_screw").GetComponent<AudioSource>();
                return getInstance._screwAudio;
            }
        }
        /// <summary>
        /// Represents whether the gui disassemble icon is shown or not. (Circle with line through it.)
        /// </summary>
        public static bool guiDisassemble
        {
            get
            {
                if (getInstance._guiDisassemble == null)
                    getInstance._guiDisassemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdisassemble");
                return getInstance._guiDisassemble.Value;
            }
            set
            {
                if (getInstance._guiDisassemble == null)
                    getInstance._guiDisassemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdisassemble");
                getInstance._guiDisassemble.Value = value;
            }
        }
        /// <summary>
        /// Represents whether the gui assemble icon is shown or not. (Tick Symbol)
        /// </summary>
        public static bool guiAssemble
        {
            get
            {
                if (getInstance._guiAssemble == null)
                    getInstance._guiAssemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble");
                return getInstance._guiAssemble.Value;
            }
            set
            {
                if (getInstance._guiAssemble == null)
                    getInstance._guiAssemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble");
                getInstance._guiAssemble.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not the gui use icon is shown. (Hand Symbol)
        /// </summary>
        public static bool guiUse
        {
            get
            {
                if (getInstance._guiUse == null)
                    getInstance._guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
                return getInstance._guiUse.Value;
            }
            set
            {
                if (getInstance._guiUse == null)
                    getInstance._guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
                getInstance._guiUse.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not to display gui interaction text.
        /// </summary>
        public static string guiInteraction
        {
            get
            {
                if (getInstance._guiInteraction == null)
                    getInstance._guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
                return getInstance._guiInteraction.Value;
            }
            set
            {
                if (getInstance._guiInteraction == null)
                    getInstance._guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
                getInstance._guiInteraction.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not to display gui drive icon.
        /// </summary>
        public static bool guiDrive
        {
            get
            {
                if (getInstance._guiDrive == null)
                    getInstance._guiDrive = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdrive");
                return getInstance._guiDrive.Value;
            }
            set
            {
                if (getInstance._guiDrive == null)
                    getInstance._guiDrive = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdrive");
                getInstance._guiDrive.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not to display gui drive icon.
        /// </summary>
        public static bool playerInMenu
        {
            get
            {
                if (getInstance._playerInMenu == null)
                    getInstance._playerInMenu = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerInMenu");
                return getInstance._playerInMenu.Value;
            }
            set
            {
                if (getInstance._playerInMenu == null)
                    getInstance._playerInMenu = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerInMenu");
                getInstance._playerInMenu.Value = value;
            }
        }
        /// <summary>
        /// Represents the player current vehicle state.
        /// </summary>
        public static string playerCurrentVehicle
        {
            get
            {
                if (getInstance._playerCurrentVehicle == null)
                    getInstance._playerCurrentVehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle").Value;
                return getInstance._playerCurrentVehicle.Value;
            }
            set
            {
                if (getInstance._playerCurrentVehicle == null)
                    getInstance._playerCurrentVehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle").Value;
                getInstance._playerCurrentVehicle.Value = value;
            }
        }
        /// <summary>
        /// Gets the current player mode, 
        /// </summary>
        public static PlayerModeEnum getMode
        {
            get
            {
                PlayerModeEnum pme;
                switch (playerCurrentVehicle)
                {
                    case "":
                        pme = PlayerModeEnum.OnFoot;
                        break;
                    case "Satsuma":
                        pme = PlayerModeEnum.InSatsuma;
                        break;
                    default:
                        pme = PlayerModeEnum.InOther;
                        break;
                }
                return pme;
            }
        }
        /// <summary>
        /// Represents all parts in the current instance of my summer car. (msc mods that use MODAPI.Attachable.Part
        /// will be listed here)
        /// </summary>
        public static List<Part> loadedParts { get; } = new List<Part>();
        /// <summary>
        /// Represents all bolts in the current instance of my summer car. (Parts that use bolts will be listed here)
        /// </summary>
        public static List<Bolt> loadedBolts { get; } = new List<Bolt>();

        /// <summary>
        /// Gets the pickup playmakerfsm from the hand gameobject.
        /// </summary>
        public static PlayMakerFSM getHandPickUpFsm
        {
            get
            {
                if (getInstance._pickUp == null)
                    getInstance._pickUp = getFPS.transform.Find("1Hand_Assemble/Hand").GetPlayMaker("PickUp");
                return getInstance._pickUp;
            }
        }
        /// <summary>
        /// Gets the gameobject that the player is holding.
        /// </summary>
        public static FsmGameObject getPickedUpGameObject
        {
            get
            {
                if (getInstance._pickedUpGameObject == null)
                    getInstance._pickedUpGameObject = getHandPickUpFsm?.FsmVariables.GetFsmGameObject("PickedObject");
                return getInstance._pickedUpGameObject;
            }
        }
        /// <summary>
        /// Gets the gameobject that the player is looking at.
        /// </summary>
        public static FsmGameObject getRaycastHitGameObject
        {
            get
            {
                if (getInstance._raycastHitGameObject == null)
                    getInstance._raycastHitGameObject = getHandPickUpFsm?.FsmVariables.GetFsmGameObject("RaycastHitObject");
                return getInstance._raycastHitGameObject;
            }
        }
        /// <summary>
        /// Returns true if in hand mode. determines state by <see cref="getHandPickUpFsm"/>.Active.
        /// </summary>
        public static bool isInHandMode => getHandPickUpFsm.Active;
        /// <summary>
        /// Returns true if player is not holding anything
        /// </summary>
        public static bool isHandEmpty
        {
            get
            {
                if (getInstance._handEmpty == null)
                    getInstance._handEmpty = getHandPickUpFsm?.FsmVariables.FindFsmBool("HandEmpty");
                return getInstance._handEmpty.Value;
            }
        }
        /// <summary>
        /// Gets modloaer mods folder field info.
        /// </summary>
        public static FieldInfo getModsFolderFi
        {
            // Written, 02.07.2022

            get
            {
                if (getInstance._modsFolderFieldInfo == null)
                    getInstance._modsFolderFieldInfo = typeof(ModLoader).GetField("ModsFolder", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
                return getInstance._modsFolderFieldInfo;
            }
        }
        /// <summary>
        /// Gets the currently used mods folder path.
        /// </summary>
        public static string getModsFolder => (string)getModsFolderFi.GetValue(null);
        /// <summary>
        /// gets the currently used wrench size casted to a <see cref="Bolt.BoltSize"/>
        /// </summary>
        public static BoltSize getToolWrenchSize_boltSize
        {
            get => (BoltSize)(getToolWrenchSize_float * 100f);
        }
        /// <summary>
        /// gets the currently used wrench size
        /// </summary>
        public static float getToolWrenchSize_float
        {
            get
            {
                if (getInstance._toolWrenchSize == null)
                    getInstance._toolWrenchSize = PlayMakerGlobals.Instance.Variables.FindFsmFloat("ToolWrenchSize");
                return getInstance._toolWrenchSize.Value;
            }
        }
        /// <summary>
        /// Represents if mod api is set up.
        /// </summary>
        public static bool modApiSetUp => ModApiLoader.modapiGo;
        /// <summary>
        /// Gets the spanner bolting speed wait time.
        /// </summary>
        public static float getSpannerBoltingSpeed => 0.28f;
        /// <summary>
        /// Gets the rachet bolting speed wait time.
        /// </summary>
        public static float getRachetBoltingSpeed => 0.08f;
        /// <summary>
        /// Gets the active bolt material. (green bolt texture)
        /// </summary>
        public static Material getActiveBoltMaterial
        {
            get
            {
                if (getInstance._activeBoltMaterial == null)
                {
                    Material[] gameMaterials = Resources.FindObjectsOfTypeAll<Material>();
                    getInstance._activeBoltMaterial = gameMaterials.First(m => m.name == "activebolt");
                }
                return getInstance._activeBoltMaterial;
            }
        }

        /// <summary>
        /// gets the master audio gameobject and caches a reference.
        /// </summary>
        public static GameObject getMasterAudioGameObject
        {
            get
            {
                if (!getInstance._masterAudioGameObject)
                    getInstance._masterAudioGameObject = GameObject.Find("MasterAudio");
                return getInstance._masterAudioGameObject;
            }
        }

        /// <summary>
        /// cache. gets and stores a reference to the player camera gameobject (parent of player camera). PATH: "PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera"
        /// </summary>
        public static GameObject getPOV
        {
            get
            {
                if (getInstance._POV == null)
                {
                    getInstance._POV = PlayMakerGlobals.Instance.Variables.FindFsmGameObject("POV");
                }
                return getInstance._POV.Value;
            }
        }
        /// <summary>
        /// cache. gets and stores a reference to the player gameobject. PATH: "PLAYER"
        /// </summary>
        public static GameObject getPlayer
        {
            get
            {
                if (getInstance._player == null)
                {
                    getInstance._player = getPOV.transform.root.gameObject;
                }
                return getInstance._player;
            }
        }
        /// <summary>
        /// cache. gets and stores a reference to the player fps gameobject. PATH: "PLAYER/Pivot/AnimPivot/Camera/FPSCamera"
        /// </summary>
        public static GameObject getFPS
        {
            get
            {
                if (getInstance._fps == null)
                {
                    getInstance._fps = getPOV.transform.parent.gameObject;
                }
                return getInstance._fps;
            }
        }
        /// <summary>
        /// cache. gets and stores a reference to the player camera. 
        /// </summary>
        public static Camera getPlayerCamera 
        {
            get 
            {
                if (getInstance._playerCamera == null)
                {
                    getInstance._playerCamera = getPOV.GetComponent<Camera>();
                }
                return getInstance._playerCamera;
            }
        }
        /// <summary>
        /// Gets the full game directory path on the user system.
        /// </summary>
        public static string getGameFolder 
        {
            get
            {
                if (getInstance._gameDirectory == null)
                {
                    getInstance._gameDirectory = Path.GetFullPath(".");
                }
                return getInstance._gameDirectory;
            }
        }
        /// <summary>
        /// Represents if the player is using the ratchet.
        /// </summary>
        public static bool playerHasRatchet
        {
            get
            {
                if (getInstance._playerHasRatchet == null)
                {
                    getInstance._playerHasRatchet = PlayMakerGlobals.Instance.Variables.GetFsmBool("PlayerHasRatchet");
                }
                return getInstance._playerHasRatchet.Value;
            }
        }
        /// <summary>
        /// Represents if the ratchet is switched. (direction)
        /// </summary>
        public static bool ratchetSwitch
        {
            get
            {
                if (getInstance._ratchetSwitch == null)
                {
                    getInstance._ratchetSwitch = getFPS.transform.FindChild("2Spanner/Pivot/Ratchet").gameObject.GetPlayMaker("Switch").FsmVariables.GetFsmBool("Switch");
                }
                return getInstance._ratchetSwitch.Value;
            }
        }
        /// <summary>
        /// Represents the dev mode behaviour instance. null if <see cref="devMode"/> is <see langword="false"/>.
        /// </summary>
        public static DevMode devModeBehaviour { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// sets <see cref="instance"/> to null. thus next call to <see cref="getInstance"/> will create a new instance.
        /// </summary>
        internal static void deleteCache() 
        {
            instance = null;
            Database.Database.deleteCache();
        }
        /// <summary>
        /// finds child object of name <paramref name="childName"/> and gets playmaker called, "Data".
        /// </summary>
        /// <param name="go"></param>
        /// <param name="childName"></param>
        /// <returns>"Data" playmakerFsm on child of <paramref name="go"/>.</returns>
        internal static PlayMakerFSM getData(GameObject go, string childName)
        {
            return go.transform.FindChild(childName).GetPlayMaker("Data");
        }

        // [Texture]
        /// <summary>
        /// creates a texure with the color provided.
        /// </summary>
        /// <param name="width">width of texture</param>
        /// <param name="height">height of texture</param>
        /// <param name="col">the color of the texture</param>
        /// <returns>a new texture instance of color param: <paramref name="col"/></returns>
        public static Texture2D createTextureFromColor(int width, int height, Color col)
        {
            // Written, 21.08.2022

            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        // [Sound]
        /// <summary>
        /// Plays a sound at <paramref name="transform"/> world position. if sound is already playing, does nothing.
        /// </summary>
        /// <param name="transform">the world position to play sound at.</param>
        /// <param name="soundType">The sound type group. eg => CarBuilding</param>
        /// <param name="variantName">The sound variant in the soundType group. eg => Assemble</param>
        public static void playSoundAt(Transform transform, string soundType, string variantName)
        {
            // Written, 16.07.2022

            AudioSource source = getSourceFromMasterAudio(soundType, variantName);
            if (source)
            {
                source.transform.position = transform.position;
                source.Play();
            }
        }
        /// <summary>
        /// Plays a sound at <paramref name="transform"/> world position. if sound is already playing, stops and restarts.
        /// </summary>
        /// <param name="transform">the world position to play sound at.</param>
        /// <param name="soundType">The sound type group. eg => CarBuilding</param>
        /// <param name="variantName">The sound variant in the soundType group. eg => Assemble</param>
        public static void playSoundAtInterupt(Transform transform, string soundType, string variantName)
        {
            // Written, 16.07.2022

            AudioSource source = getSourceFromMasterAudio(soundType, variantName);
            if (source)
            {
                source.transform.position = transform.position;
                if (source.isPlaying)
                    source.Stop();
                source.Play();
            }
        }
        /// <summary>
        /// Gets the audio source from the master audio gameobject (MasterAudio). path => <paramref name="soundType"/>/<paramref name="variantName"/> | eg => CarBuilding/assemble. adds it to a dictionary for for a less performat hit next time. asking for the same path.
        /// </summary>
        /// <param name="soundType">The sound type group. eg => CarBuilding</param>
        /// <param name="variantName">The sound variant in the soundType group. eg => assemble</param>
        public static AudioSource getSourceFromMasterAudio(string soundType, string variantName)
        {
            // Written, 16.07.2022

            string search = $"{soundType}/{variantName}";
            AudioSource source;
            if (!getInstance.masterAudioDictionary.TryGetValue(search, out source))
            {
                source = getMasterAudioGameObject.transform.Find(search)?.gameObject.GetComponent<AudioSource>();
                if (source)
                    getInstance.masterAudioDictionary.Add(search, source);
                else
                    print($"Error: Could not find audio path: MasterAudio/{search}.");
            }
            return source;
        }

        // [Lerp]
        /// <summary>
        /// Same logic as <see cref="Mathf.Lerp(float, float, float)"/>, but  <paramref name="t"/> is not clamped. to 0 - 1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float unclampedLerp(float from, float to, float t)
        {
            // Written, 13.11.2022

            return from + (to - from) * t;
        }
        /// <summary>
        /// Same logic as <see cref="Mathf.Lerp(float, float, float)"/> but defines min and max clamp params for <paramref name="t"/>.
        /// </summary>
        /// <param name="from">the start or from value.</param>
        /// <param name="to">the end or to value.</param>
        /// <param name="t">the time value.</param>
        /// <param name="min">The min value for <paramref name="t"/>.</param>
        /// <param name="max">The max value for <paramref name="t"/>.</param>
        /// <returns></returns>
        public static float clampedLerp(float from, float to, float t, float min = 0, float max = 1)
        {
            // Written, 13.11.2022

            return from + (to - from) * Mathf.Clamp(t, min, max);
        }

        /// <summary>
        /// Same logic as <see cref="Mathf.Approximately(float, float)"/> but with threshold parameter.
        /// </summary>
        /// <param name="a">the a param to compare</param>
        /// <param name="b">the b param to compare.</param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool approximately(float a, float b, float threshold)
        {
            return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
        }

        // [GUI]
        /// <summary>
        /// draws a gui of info about a game part. (use within an <see cref="AreaScope"/>).
        /// </summary>
        /// <param name="gp"></param>
        public static void drawGamePartInfo(GamePart gp)
        {
            // Written, 19.07.2022

            using (new VerticalScope())
            {
                drawProperty(gp.thisPart.Value.name);
                drawPropertyVector3("Position", gp.thisPart.Value.transform.position);
                drawProperty("Damaged", gp.damaged);
                drawProperty("Bolted", gp.bolted);
                drawProperty("Installed", gp.installed);
                                
                if (gp is GamePartTime)
                    drawProperty("Time", (gp as GamePartTime).time);
                if (gp is GamePartWear)
                    drawProperty("Wear", (gp as GamePartWear).wear);
                if (gp is Block)
                    drawProperty("In hoist", (gp as Block).inHoist);
                if (gp is OilPan)
                {
                    OilPan o = gp as OilPan;
                    drawProperty("Oil level", o.oilLevel);
                    drawProperty("Oil contamination", o.oilContamination);
                    drawProperty("Oil grade", o.oilGrade);
                }
                if (gp is RockerShaft)
                {
                    RockerShaft r = gp as RockerShaft;
                    drawProperty("Cyl 1 Exh", r.cyl1Ex);
                    drawProperty("Cyl 1 In", r.cyl1In);

                    drawProperty("Cyl 2 Exh", r.cyl2Ex);
                    drawProperty("Cyl 2 In", r.cyl2In);

                    drawProperty("Cyl 3 Exh", r.cyl3Ex);
                    drawProperty("Cyl 3 In", r.cyl3In);

                    drawProperty("Cyl 4 Exh", r.cyl4Ex);
                    drawProperty("Cyl 4 In", r.cyl4In);

                }
                if (gp is Carburator)
                {
                    Carburator c = gp as Carburator;

                    drawProperty("Dirt", c.dirt);
                    drawProperty("idle adjust", c.idleAdjust);

                }
                if (gp is Distributor)
                    drawProperty("Spark angle", (gp as Distributor).sparkAngle);
                if (gp is CamshaftGear)
                    drawProperty("Angle", (gp as CamshaftGear).angle);

                if (Button("Teleport to player"))
                {
                    gp.thisPart.Value.transform.teleport(ModClient.getPOV.transform.position);
                }
                if (Button("Teleport to part"))
                {
                    ModClient.getPlayer.teleport(gp.thisPart.Value.transform.position);
                }
            }
        }
        /// <summary>
        /// [GUI] draws a vector3 that can be edited.
        /// </summary>
        /// <param name="propertyName">The property vector3 name</param>
        /// <param name="vector3">the reference vector3 to draw/edit</param>
        public static void drawPropertyVector3(string propertyName, ref Vector3 vector3)
        {
            // Written, 03.07.2022

            using (new VerticalScope())
            {
                drawProperty(propertyName);
                using (new HorizontalScope())
                {
                    drawPropertyEdit("x", ref vector3.x);
                    drawPropertyEdit("y", ref vector3.y);
                    drawPropertyEdit("z", ref vector3.z);
                }
            }
        }
        /// <summary>
        /// [GUI] draws a vector3 that can be edited.
        /// </summary>
        /// <param name="propertyName">The property vector3 name</param>
        /// <param name="vector3">the reference vector3 to draw/edit</param>
        public static Vector3 drawPropertyVector3(string propertyName, Vector3 vector3)
        {
            // Written, 04.07.2022

            drawPropertyVector3(propertyName, ref vector3);
            return vector3;
        }
        /// <summary>
        /// [GUI] draws an enum that can be edited as a list of toggles.
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <param name="e">Reference enum (selected)</param>
        public static void drawPropertyEnum<T>(ref T e) where T : Enum
        {
            // Written, 25.05.2022

            Type t = e.GetType();
            Array a = Enum.GetNames(t);
            string n = t.getDescription();
            using (new VerticalScope())
            {
                drawProperty(n + ":");
                using (new HorizontalScope())
                {
                    foreach (string i in a)
                    {
                        n = t.GetField(i).getDescription();
                        if (Toggle(e.ToString() == i, n))
                            e = (T)Enum.Parse(t, i);
                    }
                }
            }
        }
        /// <summary>
        /// [GUI] draws an enum that can be edited as a list of toggles.
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <param name="e">Reference enum (selected)</param>
        public static T drawPropertyEnum<T>(T e) where T : Enum
        {
            // Written, 04.07.2022

            drawPropertyEnum(ref e);
            return e;
        }
        /// <summary>
        /// [GUI] draws a property of type <see cref="string"/> that can be edited
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="property">the reference string to draw/edit</param>
        /// <param name="maxLength">max length of textfield (number of letters.)</param>
        /// <returns><see langword="true"/> if <paramref name="property"/> has changed.</returns>
        public static bool drawPropertyEdit(string propertyName, ref string property, int maxLength = 10)
        {
            // Written 19.08.2022

            using (new HorizontalScope())
            {
                Label(propertyName);
                getInstance._propertyString = TextField(property, maxLength);
            }

            if (getInstance._propertyString != property)
            {
                property = getInstance._propertyString;
                return true;
            }
            return false;
        }
        /// <summary>
        /// [GUI] draws a property of type <see cref="string"/> that can be edited
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="property">the reference string to draw/edit</param>
        /// <param name="maxLength">max length of textfield (number of letters.)</param>
        /// <returns><see langword="true"/> returns the property</returns>
        public static string drawPropertyEdit(string propertyName, string property, int maxLength = 10)
        {
            // Written 19.08.2022

            using (new HorizontalScope())
            {
                Label(propertyName);
                return TextField(property, maxLength);
            }
        }
        /// <summary>
        /// [GUI] draws a property of type <see cref="int"/> that can be edited
        /// </summary>
        /// <param name="propertyName">The property int name</param>
        /// <param name="property">the reference int to draw/edit</param>
        /// <param name="maxLength">max length of textfield (number of letters.)</param>
        /// <returns><see langword="true"/> if <paramref name="property"/> has changed.</returns>
        public static bool drawPropertyEdit(string propertyName, ref int property, int maxLength = 10)
        {
            // Written 13.11.2022

            getInstance._propertyString = drawPropertyEdit(propertyName, property.ToString());

            int.TryParse(getInstance._propertyString, out getInstance._propertyInt);

            if (getInstance._propertyInt != property)
            {
                property = getInstance._propertyInt;
                return true;
            }
            return false;
        }
        /// <summary>
        /// [GUI] draws a int property that can be edited
        /// </summary>
        /// <param name="propertyName">The property int name</param>
        /// <param name="property">the reference int to draw/edit</param>
        /// <param name="maxLength">max length of textfield</param>
        public static float drawPropertyEdit(string propertyName, int property, int maxLength = 10)
        {
            // Written, 13.11.2022

            drawPropertyEdit(propertyName, ref property, maxLength);
            return property;
        }

        /// <summary>
        /// [GUI] draws a property of type <see cref="float"/> that can be edited
        /// </summary>
        /// <param name="propertyName">The property float name</param>
        /// <param name="property">the reference float to draw/edit</param>
        /// <param name="maxLength">max length of textfield (number of letters.)</param>
        /// <returns><see langword="true"/> if <paramref name="property"/> has changed.</returns>
        public static bool drawPropertyEdit(string propertyName, ref float property, int maxLength = 10)
        {
            // Written 28.08.2022

            getInstance._propertyString = drawPropertyEdit(propertyName, property.ToString());

            float.TryParse(getInstance._propertyString, out getInstance._propertyFloat);

            if (getInstance._propertyFloat != property)
            {
                property = getInstance._propertyFloat;
                return true;
            }
            return false;
        }
        /// <summary>
        /// [GUI] draws a float property that can be edited
        /// </summary>
        /// <param name="propertyName">The property float name</param>
        /// <param name="property">the reference float to draw/edit</param>
        /// <param name="maxLength">max length of textfield</param>
        public static float drawPropertyEdit(string propertyName, float property, int maxLength = 10)
        {
            // Written, 04.07.2022

            drawPropertyEdit(propertyName, ref property, maxLength);
            return property;
        }
        
        /// <summary>
        /// [GUI] draws a property of type <see cref="double"/> that can be edited
        /// </summary>
        /// <param name="propertyName">The property double name</param>
        /// <param name="property">the reference double to draw/edit</param>
        /// <param name="maxLength">max length of textfield (number of letters.)</param>
        /// <returns><see langword="true"/> if <paramref name="property"/> has changed.</returns>
        public static bool drawPropertyEdit(string propertyName, ref double property, int maxLength = 10)
        {
            // Written 03.10.2022

            getInstance._propertyString = drawPropertyEdit(propertyName, property.ToString());

            double.TryParse(getInstance._propertyString, out getInstance._propertyDouble);

            if (getInstance._propertyDouble != property)
            {
                property = getInstance._propertyDouble;
                return true;
            }
            return false;
        }
        /// <summary>
        /// [GUI] draws a double property that can be edited
        /// </summary>
        /// <param name="propertyName">The property double name</param>
        /// <param name="property">the reference double to draw/edit</param>
        /// <param name="maxLength">max length of textfield</param>
        public static double drawPropertyEdit(string propertyName, double property, int maxLength = 10)
        {
            // Written, 04.07.2022

            drawPropertyEdit(propertyName, ref property, maxLength);
            return property;
        }

        /// <summary>
        /// [GUI] draws a bool that can be edited
        /// </summary>
        /// <param name="propertyName">the property bool name</param>
        /// <param name="property">the reference bool to draw/edit</param>
        public static void drawPropertyBool(string propertyName, ref bool property)
        {
            property = Toggle(property, " " + propertyName);
        }
        /// <summary>
        /// [GUI] draws a bool that can be edited
        /// </summary>
        /// <param name="propertyName">the property bool name</param>
        /// <param name="property">the reference bool to draw/edit</param>
        public static bool drawPropertyBool(string propertyName, bool property)
        {
            // Written, 04.07.2022

            drawPropertyBool(propertyName, ref property);
            return property;
        }

        /// <summary>
        /// [GUI] draws a property.
        /// </summary>
        /// <param name="property">the reference of the property to draw.</param>
        public static void drawProperty(object property)
        {
            Label(property.ToString());
        }
        /// <summary>
        /// [GUI] draws a property.
        /// </summary>
        /// <param name="property">the reference of the property to draw.</param>
        public static void drawProperty(string property)
        {
            Label(property);
        }
        /// <summary>
        /// [GUI] draws a property.
        /// </summary>
        /// <param name="propertyName">The property object name</param>
        /// <param name="property">the reference of the property to draw.</param>
        public static void drawProperty(object propertyName, object property)
        {
            Label($"{propertyName}: {property}");
        }

        /// <summary>
        /// Displays an interaction text and optional hand symbol. If no parameters are passed into the method (default parameters), turns off the interaction.
        /// </summary>
        /// <param name="inText">The text to display.</param>
        /// <param name="inGuiInteractSymbol">Whether or not to display a symbol.</param>
        public static void guiInteract(string inText = "", GuiInteractSymbolEnum inGuiInteractSymbol = GuiInteractSymbolEnum.Hand)
        {
            // Written, 30.09.2018 | Modified, 16.03.2019

            guiInteraction = inText;
            if (inText == "")
                return;
            changeInteractSymbol(inGuiInteractSymbol);
        }
        /// <summary>
        /// displays or removes a particular symbol from the interaction. if <see cref="GuiInteractSymbolEnum.None"/> is passed,
        /// sets all interactions to the passed boolean value.
        /// </summary>
        /// <param name="inGuiInteractSymbol">The gui symbol to change.</param>
        private static void changeInteractSymbol(GuiInteractSymbolEnum inGuiInteractSymbol)
        {
            // Written, 16.03.2019

            switch (inGuiInteractSymbol)
            {
                case GuiInteractSymbolEnum.Hand:
                    guiUse = true;
                    guiAssemble = false;
                    guiDisassemble = false;
                    break;
                case GuiInteractSymbolEnum.Disassemble:
                    guiUse = false;
                    guiAssemble = false;
                    guiDisassemble = true;
                    break;
                case GuiInteractSymbolEnum.Assemble:
                    guiUse = false;
                    guiAssemble = true;
                    guiDisassemble = false;
                    break;
                case GuiInteractSymbolEnum.None:
                    guiUse = false;
                    guiDisassemble = false;
                    guiAssemble = false;
                    break;
            }
        }
        /// <summary>
        /// Reps modapi print-to-console function
        /// </summary>
        public static void print(string format, params object[] args)
        {
            // Written, 09.10.2021

            ModConsole.Log(string.Format("<color=grey>[ModAPI] " + format + "</color>", args));
        }

        // [Raycast]
        /// <summary>
        /// Gets the layermask. Used to make raycast masks. if no masks are passed. returns all layers (everything) '~0'
        /// </summary>
        /// <param name="masks">layermasks</param>
        public static int getMask(params LayerMasksEnum[] masks)
        {
            // Written, 03.07.2022

            if (masks != null && masks.Length > 0)
            {
                getInstance._maskNames = new string[masks.Length];
                for (int i = 0; i < masks.Length; i++)
                {
                    getInstance._maskNames[i] = masks[i].ToString();
                }
                return LayerMask.GetMask(getInstance._maskNames);
            }
            return ~0;
        }
        /// <summary>
        /// Raycasts for a type of behaviour with a max distance of '1' from the camera at the cursor position.
        /// </summary>
        /// <typeparam name="T">the type of behaviour to raycast for.</typeparam>
        /// <param name="layers">the layers to raycast on. Note: if nothing is passed, raycasts on all layers.</param>
        /// <returns>the instance of <typeparamref name="T"/> if found. if not returns null.</returns>
        public static T raycastForBehaviour<T>(params LayerMasksEnum[] layers) where T : MonoBehaviour
        {
            return raycastForBehaviour<T>(false, 1, layers);
        }
        /// <summary>
        /// Raycasts for a type of behaviour with a max distance of '1' from the camera.
        /// </summary>
        /// <typeparam name="T">the type of behaviour to raycast for.</typeparam>
        /// <param name="centerOfScreen">if true. uses the center of the screen regardless where the mouse is.</param>
        /// <param name="layers">the layers to raycast on. Note: if nothing is passed, raycasts on all layers.</param>
        /// <returns>the instance of <typeparamref name="T"/> if found. if not returns null.</returns>
        public static T raycastForBehaviour<T>(bool centerOfScreen, params LayerMasksEnum[] layers) where T : MonoBehaviour
        {
            return raycastForBehaviour<T>(centerOfScreen, 1, layers);
        }
        /// <summary>
        /// Raycasts for a type of behaviour from the camera at the cursor position.
        /// </summary>
        /// <typeparam name="T">the type of behaviour to raycast for.</typeparam>
        /// <param name="maxDistance">the max distance of the ray.</param>
        /// <param name="layers">the layers to raycast on. Note: if nothing is passed, raycasts on all layers.</param>
        /// <returns>the instance of <typeparamref name="T"/> if found. if not returns null.</returns>
        public static T raycastForBehaviour<T>(float maxDistance, params LayerMasksEnum[] layers) where T : MonoBehaviour
        {
            return raycastForBehaviour<T>(false, maxDistance, layers);
        }
        /// <summary>
        /// Raycasts for a type of behaviour
        /// </summary>
        /// <typeparam name="T">the type of behaviour to raycast for.</typeparam>
        /// <param name="maxDistance">the max distance of the ray.</param>
        /// <param name="centerOfScreen">if true. uses the center of the screen regardless where the mouse is.</param>
        /// <param name="layers">the layers to raycast on. Note: if nothing is passed, raycasts on all layers.</param>
        /// <returns>the instance of <typeparamref name="T"/> if found. if not returns null.</returns>
        public static T raycastForBehaviour<T>(bool centerOfScreen, float maxDistance, params LayerMasksEnum[] layers) where T : MonoBehaviour
        {
            if (raycast(out RaycastHit hitInfo, maxDistance, centerOfScreen, layers))
            {
                return hitInfo.collider.GetComponent<T>();
            }
            return null;
        }
        /// <summary>
        /// raycasts. uses cached player camera (see: <see cref="getPlayerCamera"/>) for improved performace. mod developers using ModAPI are recommended to use this instead of their own <see cref="Physics.Raycast(Ray)"/> calls.
        /// </summary>
        /// <param name="raycastHit">the raycast hit info.</param>
        /// <param name="maxDistance">the max distance of the ray.</param>
        /// <param name="centerOfScreen">if true. uses the center of the screen regardless where the mouse is.</param>
        /// <param name="layers">the layers to raycast on. Note: if nothing is passed, raycasts on all layers.</param>
        /// <returns><see langword="true"/> if the raycast hit anything.</returns>
        public static bool raycast(out RaycastHit raycastHit, float maxDistance = 1, bool centerOfScreen = false, params LayerMasksEnum[] layers)
        {
            return Physics.Raycast(centerOfScreen ? getPlayerCamera.ViewportPointToRay(Vector3.one * 0.5f) : getPlayerCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, maxDistance, getMask(layers));
        }

        // Expose event invoke.
        /// <summary>
        /// invokes the object pick up event and passes the param <paramref name="gameObject"/>.
        /// </summary>
        /// <param name="gameObject">the gameobject that was picked up</param>
        internal static void invokePickUpEvent(GameObject gameObject)
        {
            onGameObjectPickUp?.Invoke(gameObject);
        }
        /// <summary>
        /// invokes the object drop event and passes the param <paramref name="gameObject"/>.
        /// </summary>
        /// <param name="gameObject">the gameobject that was dropped</param>
        internal static void invokeDropEvent(GameObject gameObject)
        {
            onGameObjectDrop?.Invoke(gameObject);
        }
        /// <summary>
        /// invokes the object throw event and passes the param <paramref name="gameObject"/>.
        /// </summary>
        /// <param name="gameObject">the gameobject that was thrown</param>
        internal static void invokeThrowEvent(GameObject gameObject)
        {
            onGameObjectThrow?.Invoke(gameObject);
        }

        #endregion       
    }
}
