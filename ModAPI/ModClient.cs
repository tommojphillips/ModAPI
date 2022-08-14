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
    /// Represents inject types for <see cref="injectAction"/>
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
        /// Represents the wheel layer. (All UnityCar.Wheels use this. eg FL wheel on satsuma will be on this layer.
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

    public class saveDataNotFoundException : Exception 
    {
        // Written, 15.07.2022

        public saveDataNotFoundException(string message) : base(message) { }
    }

    #endregion
      

    /// <summary>
    /// Represents useful properties for interacting with My Summer Car and PlayMaker.
    /// </summary>
    public static class ModClient
    {
        // Written, 10.08.2018 | Modified 25.09.2021

        #region fields

        /// <summary>
        /// Represents the complied runtime version of the api.
        /// </summary>
        public const string version = VersionInfo.version;
        
        public static DevMode devModeBehaviour { get; internal set; }
#if DEBUG
        internal static bool devMode = true;
#else
        internal static bool devMode = false;
#endif

        internal static Part _inspectingPart = null;
        internal static BoltCallback _inspectingBolt = null;

        private static PlayMakerFSM _pickUp;
        private static FsmGameObject _pickedUpGameObject;
        private static FsmGameObject _raycastHitGameObject;
        private static AudioSource _assembleAudio;
        private static AudioSource _disassembleAudio;
        private static AudioSource _screwAudio;
        private static FsmBool _guiDisassemble;
        private static FsmBool _guiAssemble;
        private static FsmBool _guiUse;
        private static FsmBool _guiDrive;
        private static FsmBool _handEmpty;
        private static FsmBool _playerInMenu;
        private static FsmString _guiInteraction;
        private static FsmString _playerCurrentVehicle;
        private static FieldInfo _modsFolderFieldInfo;
        private static FsmFloat _toolWrenchSize;
        private static FsmFloat _boltingSpeed;
        private static string[] _maskNames;
        private static string _propertyString = "";
        private static Material _activeBoltMaterial;
        private static GameObject _masterAudioGameObject;
        private static Dictionary<string, AudioSource> masterAudioDictionary = new Dictionary<string, AudioSource>();
        private static Camera _playerCamera;
        private static FsmGameObject _POV;

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
        /// Represents the assemble assemble audio source.
        /// </summary>
        public static AudioSource assembleAudio
        {
            get
            {
                if (_assembleAudio == null)
                    _assembleAudio = GameObject.Find("MasterAudio/CarBuilding/assemble").GetComponent<AudioSource>();
                return _assembleAudio;
            }
        }
        /// <summary>
        /// Represents the disassemble audio source.
        /// </summary>
        public static AudioSource disassembleAudio
        {
            get
            {
                if (_disassembleAudio == null)
                    _disassembleAudio = GameObject.Find("MasterAudio/CarBuilding/disassemble").GetComponent<AudioSource>();
                return _disassembleAudio;
            }
        }
        /// <summary>
        /// Represents the bolt screw audio source.
        /// </summary>
        public static AudioSource screwAudio
        {
            get
            {
                if (_screwAudio == null)
                    _screwAudio = GameObject.Find("MasterAudio/CarBuilding/bolt_screw").GetComponent<AudioSource>();
                return _screwAudio;
            }
        }
        /// <summary>
        /// Represents whether the gui disassemble icon is shown or not. (Circle with line through it.)
        /// </summary>
        public static bool guiDisassemble
        {
            get
            {
                if (_guiDisassemble == null)
                    _guiDisassemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdisassemble");
                return _guiDisassemble.Value;
            }
            set
            {
                if (_guiDisassemble == null)
                    _guiDisassemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdisassemble");
                _guiDisassemble.Value = value;
            }
        }
        /// <summary>
        /// Represents whether the gui assemble icon is shown or not. (Tick Symbol)
        /// </summary>
        public static bool guiAssemble
        {
            get
            {
                if (_guiAssemble == null)
                    _guiAssemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble");
                return _guiAssemble.Value;
            }
            set
            {
                if (_guiAssemble == null)
                    _guiAssemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble");
                _guiAssemble.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not the gui use icon is shown. (Hand Symbol)
        /// </summary>
        public static bool guiUse
        {
            get
            {
                if (_guiUse == null)
                    _guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
                return _guiUse.Value;
            }
            set
            {
                if (_guiUse == null)
                    _guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
                _guiUse.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not to display gui interaction text.
        /// </summary>
        public static string guiInteraction
        {
            get
            {
                if (_guiInteraction == null)
                    _guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
                return _guiInteraction.Value;
            }
            set
            {
                if (_guiInteraction == null)
                    _guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
                _guiInteraction.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not to display gui drive icon.
        /// </summary>
        public static bool guiDrive
        {
            get
            {
                if (_guiDrive == null)
                    _guiDrive = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdrive");
                return _guiDrive.Value;
            }
            set
            {
                if (_guiDrive == null)
                    _guiDrive = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdrive");
                _guiDrive.Value = value;
            }
        }
        /// <summary>
        /// Represents whether or not to display gui drive icon.
        /// </summary>
        public static bool playerInMenu
        {
            get
            {
                if (_playerInMenu == null)
                    _playerInMenu = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerInMenu");
                return _playerInMenu.Value;
            }
            set
            {
                if (_playerInMenu == null)
                    _playerInMenu = PlayMakerGlobals.Instance.Variables.FindFsmBool("PlayerInMenu");
                _playerInMenu.Value = value;
            }
        }
        /// <summary>
        /// Represents the player current vehicle state.
        /// </summary>
        public static string playerCurrentVehicle
        {
            get
            {
                if (_playerCurrentVehicle == null)
                    _playerCurrentVehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle").Value;
                return _playerCurrentVehicle.Value;
            }
            set
            {
                if (_playerCurrentVehicle == null)
                    _playerCurrentVehicle = PlayMakerGlobals.Instance.Variables.FindFsmString("PlayerCurrentVehicle").Value;
                _playerCurrentVehicle.Value = value;
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
                if (_pickUp == null)
                    _pickUp = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand")?.GetPlayMaker("PickUp");
                return _pickUp;
            }
        }
        /// <summary>
        /// Gets the gameobject that the player is holding.
        /// </summary>
        public static FsmGameObject getPickedUpGameObject
        {
            get
            {
                if (_pickedUpGameObject == null)
                    _pickedUpGameObject = getHandPickUpFsm?.FsmVariables.GetFsmGameObject("PickedObject");
                return _pickedUpGameObject;
            }
        }
        /// <summary>
        /// Gets the gameobject that the player is looking at.
        /// </summary>
        public static FsmGameObject getRaycastHitGameObject
        {
            get
            {
                if (_raycastHitGameObject == null)
                    _raycastHitGameObject = getHandPickUpFsm?.FsmVariables.GetFsmGameObject("RaycastHitObject");
                return _raycastHitGameObject;
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
                if (_handEmpty == null)
                    _handEmpty = getHandPickUpFsm?.FsmVariables.FindFsmBool("HandEmpty");
                return _handEmpty.Value;
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
                if (_modsFolderFieldInfo == null)
                    _modsFolderFieldInfo = typeof(ModLoader).GetField("ModsFolder", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
                return _modsFolderFieldInfo;
            }
        }
        /// <summary>
        /// Gets the currently used mods folder path.
        /// </summary>
        public static string getModsFolder => (string)getModsFolderFi.GetValue(null);
        /// <summary>
        /// gets the currently used wrench size casted to a <see cref="Bolt.BoltSize"/>
        /// </summary>
        public static Bolt.BoltSize getToolWrenchSize_boltSize
        {
            get
            {
                return (Bolt.BoltSize)(getToolWrenchSize_float * 100f);
            }
        }
        /// <summary>
        /// gets the currently used wrench size
        /// </summary>
        public static float getToolWrenchSize_float
        {
            get
            {
                if (_toolWrenchSize == null)
                    _toolWrenchSize = PlayMakerGlobals.Instance.Variables.FindFsmFloat("ToolWrenchSize");
                return _toolWrenchSize.Value;
            }
        }
        /// <summary>
        /// Represents if mod api is set up.
        /// </summary>
        public static bool modApiSetUp => ModApiLoader.modapiGo;
        /// <summary>
        /// Gets the bolting speed wait.
        /// </summary>
        public static float getBoltingSpeed
        {
            get
            {
                if (_boltingSpeed == null)
                    _boltingSpeed = PlayMakerGlobals.Instance.Variables.FindFsmFloat("BoltingSpeed");
                return _boltingSpeed.Value;
            }
        }
        /// <summary>
        /// Gets the active bolt material. (green bolt texture)
        /// </summary>
        public static Material getActiveBoltMaterial
        {
            get
            {
                if (_activeBoltMaterial == null)
                {
                    Material[] gameMaterials = Resources.FindObjectsOfTypeAll<Material>();
                    _activeBoltMaterial = gameMaterials.First(m => m.name == "activebolt");
                }
                return _activeBoltMaterial;
            }
        }

        public static GameObject getMasterAudioGameObject
        {
            get
            {
                if (!_masterAudioGameObject)
                    _masterAudioGameObject = GameObject.Find("MasterAudio");
                return _masterAudioGameObject;
            }
        }
        
        /// <summary>
        /// cache. gets and stores a reference to the player camera gameobject.
        /// </summary>
        public static GameObject getPOV 
        {
            get 
            {
                if (_POV == null)
                {
                    _POV = PlayMakerGlobals.Instance.Variables.FindFsmGameObject("POV");
                }
                return _POV.Value;
            }
        }
        /// <summary>
        /// cache. gets and stores a reference to the player camera.
        /// </summary>
        public static Camera getPlayerCamera 
        {
            get 
            {
                if (_playerCamera == null)
                {
                    _playerCamera = getPOV.GetComponent<Camera>();
                }
                return _playerCamera;
            }
        }

        #endregion

        #region IEnumerator

        /// <summary>
        /// Runs a coroutine synchronously. Waits for the coroutine to finish.
        /// </summary>
        /// <param name="func">The corountine to run synchronously.</param>
        public static void waitCoroutine(IEnumerator func)
        {
            while (func.MoveNext())
            {
                if (func.Current != null)
                {
                    IEnumerator num;
                    try
                    {
                        num = (IEnumerator)func.Current;
                    }
                    catch (InvalidCastException)
                    {
                        if (func.Current.GetType() == typeof(WaitForSeconds))
                            Debug.LogWarning("[wait for coroutine] Skipped call to WaitForSeconds.");
                        return;
                    }
                    waitCoroutine(num);
                }
            }
        }


        #endregion

        #region Methods


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
            if (!masterAudioDictionary.TryGetValue(search, out source))
            {
                source = getMasterAudioGameObject.transform.Find(search)?.gameObject.GetComponent<AudioSource>();
                if (source)
                    masterAudioDictionary.Add(search, source);
                else
                    print($"Error: Could not find audio path: {search}.");
            }
            return source;
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
        /// [GUI] draws a property that can be edited
        /// </summary>
        /// <param name="propertyName">The property float name</param>
        /// <param name="property">the reference float to draw/edit</param>
        /// <param name="maxLength">max length of textfield</param>
        public static void drawPropertyEdit(string propertyName, ref float property, int maxLength = 10)
        {
            using (new HorizontalScope())
            {
                drawProperty(propertyName);
                _propertyString = TextField(property.ToString(), maxLength);
            }
            float.TryParse(_propertyString, out property);
        }
        /// <summary>
        /// [GUI] draws a property that can be edited
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
        /// <param name="inValue">The value..</param>
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
        /// Gets the layermask. Used to make raycast masks.
        /// </summary>
        /// <param name="masks">layermasks</param>
        public static int getMask(params LayerMasksEnum[] masks)
        {
            // Written, 03.07.2022

            _maskNames = new string[masks.Length];
            for (int i = 0; i < masks.Length; i++)
            {
                _maskNames[i] = masks[i].ToString();
            }
            return LayerMask.GetMask(_maskNames);
        }
        /// <summary>
        /// Raycasts for a type of behaviour
        /// </summary>
        /// <typeparam name="T">the type of behaviour to raycast for.</typeparam>
        /// <param name="centerOfScreen">if true. uses the center of the screen regardless where the mouse is.</param>
        /// <returns></returns>
        public static T raycastForBehaviour<T>(bool centerOfScreen = false) where T : MonoBehaviour
        {
            RaycastHit hitInfo;
            bool hasHit = Physics.Raycast(centerOfScreen ? getPlayerCamera.ViewportPointToRay(Vector3.one * 0.5f) : getPlayerCamera.ScreenPointToRay(Input.mousePosition), out hitInfo);
            T t = null;
            if (hasHit)
            {
                t = hitInfo.collider.GetComponent<T>();
            }
            return t;
        }

        // Expose event invoke.
        internal static void invokePickUpEvent(GameObject gameObject)
        {
            onGameObjectPickUp?.Invoke(gameObject);
        }
        internal static void invokeDropEvent(GameObject gameObject)
        {
            onGameObjectDrop?.Invoke(gameObject);
        }
        internal static void invokeThrowEvent(GameObject gameObject)
        {
            onGameObjectThrow?.Invoke(gameObject);
        }

        #endregion

        #region ExMethods

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
        /// <summary>
        /// Teleports a part to a world position.
        /// </summary>
        /// <param name="part">The part to teleport.</param>
        /// <param name="force">force a uninstall if required?</param>
        /// <param name="position">The position to teleport the part to.</param>
        public static void teleport(this Part part, bool force, Vector3 position)
        {
            // Written, 09.07.2022

            if (part.installed && force)
                part.disassemble(true);
            part.transform.teleport(position);
        }
        /// <summary>
        /// Teleports a gameobject to a world position.
        /// </summary>
        /// <param name="gameobject">The gameobject to teleport</param>
        /// <param name="position">The position to teleport the go to.</param>
        public static void teleport(this GameObject gameobject, Vector3 position)
        {
            // Written, 09.07.2022

            gameobject.transform.teleport(position);
        }
        /// <summary>
        /// Teleports a transform to a world position.
        /// </summary>
        /// <param name="transform">The transform to teleport</param>
        /// <param name="position">The position to teleport the go to.</param>
        public static void teleport(this Transform transform, Vector3 position)
        {
            // Written, 09.07.2022

            Rigidbody rb = transform.GetComponent<Rigidbody>();
            if (rb)
                if (!rb.isKinematic)
                    rb = null;
                else
                    rb.isKinematic = true;
            transform.root.position = position;
            if (rb)
                rb.isKinematic = false;
        }
        
        /// <summary>
        /// Gets the description of the enum member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string getDescription<T>(this T e) where T : Enum 
        {
            return e.GetType().GetField(e.ToString()).getDescription();
        }
        /// <summary>
        /// Gets <see cref="DescriptionAttribute.Description"/> on provided object type. if attribute doesn't exist, returns <see cref="MemberInfo.Name"/>
        /// </summary>
        /// <param name="mi">the member info to get info from.</param>
        public static string getDescription(this MemberInfo mi)
        {
            // Written, 07.07.2022

            object o = mi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            DescriptionAttribute[] d = o as DescriptionAttribute[];
            if (d != null && d.Length > 0)
            {
                return d[0].Description;
            }
            return mi.Name;
        }
        
        /// <summary>
        /// [GUI] draws a property. eg => <see cref="Part.partSettings"/>.drawProperty("assembleType") would draw a property as such: "assembleType: joint". if member has <see cref="DescriptionAttribute"/> will use the description as the property title. Works with fields, properties and enums
        /// </summary>
        /// <param name="t">The class instance get value from.</param>
        /// <param name="memberName">The class member name to get a field/property instance from.</param>
        public static void drawProperty<T>(this T t, string memberName) where T : class
        {
            // Written, 08.07.2022

            Type _t = t.GetType();
            FieldInfo fi = _t.GetField(memberName);
            PropertyInfo pi = null;
            if (fi == null)
                pi = _t.GetProperty(memberName);
            string title = "null";
            object value = "null";
            if (_t.IsEnum)
            {
                title = _t.getDescription();
                value = _t.GetField(memberName).getDescription();
            }
            else if (fi != null)
            {
                title = fi.getDescription();
                value = fi.GetValue(t);
            }
            else if (pi != null)
            {
                title = pi.getDescription();
                value = pi.GetValue(t, null);
            }
            Label($"{title}: {value}");
        }
        /// <summary>
        /// [GUI] draws an enum as a property.
        /// </summary>
        /// <param name="t">The enum instance get value from.</param>
        public static void drawProperty<T>(this T t) where T : Enum
        {
            // Written, 08.07.2022

            Type _t = t.GetType();
            string title = _t.getDescription();
            string valueName = t.ToString();
            string value = _t.GetField(valueName)?.getDescription();
            Label($"{title}: {value ?? valueName}");
        }
        
        /// <summary>
        /// gets the actual size of the mesh. based off mesh bounds and transform scale
        /// </summary>
        /// <param name="filter">the mesh filter to get mesh and scale.</param>
        /// <returns>filter.mesh size * transform scale.</returns>
        public static Vector3 getMeshSize(this MeshFilter filter)
        {
            // Written, 15.07.2022

            Vector3 size = filter.mesh.bounds.size;
            Vector3 scale = filter.transform.localScale;
            return new Vector3(size.x * scale.x, size.y * scale.y, size.z * scale.z);
        }
        /// <summary>
        /// gets the actual size of the mesh. based off mesh bounds and transform scale
        /// </summary>
        /// <param name="transform">the transform that this mesh is on.</param>
        /// <param name="mesh">the mesh to get size of</param>
        /// <returns>mesh size * transform scale.</returns>
        public static Vector3 getMeshSize(this Transform transform, Mesh mesh)
        {
            // Written, 15.07.2022

            Vector3 size = mesh.bounds.size;
            Vector3 scale = transform.localScale;
            return new Vector3(size.x * scale.x, size.y * scale.y, size.z * scale.z);
        }
        
        /// <summary>
        /// loads data from a file or throws an error on if save data doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of Object to load</typeparam>
        /// <param name="data">The ref to load data to</param>
        /// <param name="mod">The mod to save data on</param>
        /// <param name="saveFileName">The save file name with or without file extention. </param>
        /// <exception cref="saveDataNotFoundException"/>
        public static void loadDataOrThrow<T>(this Mod mod, out T data, string saveFileName) where T : class, new()
        {
            // Written, 15.07.2022

            if (File.Exists(Path.Combine(ModLoader.GetModSettingsFolder(mod), saveFileName)))
            {
                data = SaveLoad.DeserializeSaveFile<T>(mod, saveFileName);
                ModConsole.Print($"{mod.ID}: loaded save data (Exists).");
            }
            else
            {
                string error = $"{mod.ID}: save file didn't exist.. throwing saveDataNotFoundException.";
                ModConsole.Print(error);
                throw new saveDataNotFoundException(error);
            }
        }
        /// <summary>
        /// saves data to a file.
        /// </summary>
        /// <typeparam name="T">The type of Object to save</typeparam>
        /// <param name="data">the data to save.</param>
        /// <param name="mod">The mods config folder to save data in</param>
        /// <param name="saveFileName">The save file name with or without file extention. </param>
        /// <returns>Returns <see langword="true"/> if save file exists.</returns>
        public static void saveData<T>(this Mod mod, T data, string saveFileName) where T : class, new()
        {
            // Written, 12.06.2022

            SaveLoad.SerializeSaveFile(mod, data, saveFileName);
            ModConsole.Print($"{mod.ID}: saved data.");
        }
        /// <summary>
        /// Loads data from a file. if data doesn't exist, creates new data.
        /// </summary>
        /// <typeparam name="T">The type of Object to load</typeparam>
        /// <param name="data">The ref to load data to</param>
        /// <param name="mod">The mod to save data on</param>
        /// <param name="saveFileName">The save file name with or without file extention. </param>
        /// <returns>Returns <see langword="true"/> if save file exists.</returns>
        public static bool loadOrCreateData<T>(this Mod mod , out T data, string saveFileName) where T : class, new()
        {
            // Written, 12.06.2022

            if (File.Exists(Path.Combine(ModLoader.GetModSettingsFolder(mod), saveFileName)))
            {
                data = SaveLoad.DeserializeSaveFile<T>(mod, saveFileName);
                ModConsole.Print($"{mod.ID}: loaded save data (Exists).");
                return true;
            }
            else
            {
                ModConsole.Print($"{mod.ID}: save file didn't exist.. creating a save file.");
                data = new T();
                return false;
            }
        }
        
        
        /// <summary>
        /// executes an action on all objects of an array.
        /// </summary>
        /// <typeparam name="T">The type of array</typeparam>
        /// <param name="objects">the instance of the array</param>
        /// <param name="func">the function to perform on all elements of the array.</param>
        public static void forEach<T>(this T[] objects, Action<T> func) where T : class
        {
            // Written, 11.07.2022

            for (int i = 0; i < objects.Count(); i++)
            {
                func.Invoke(objects[i]);
            }
        }
        
        /// <summary>
        /// Gets the first found behaviour in the parent. where <paramref name="func"/> action returns true.
        /// </summary>
        /// <typeparam name="T">The type of behaviour to get</typeparam>
        /// <param name="gameObject">The gameobject to get the behaviour on.</param>
        /// <param name="func">the function to check with.</param>
        /// <returns></returns>
        public static T getBehaviourInParent<T>(this GameObject gameObject, Func<T, bool> func) where T : MonoBehaviour
        {
            // Written, 02.07.2022

            Transform parent = gameObject.transform.parent;
            if (parent)
            {
                T t = parent.GetComponent<T>();
                if (t && func.Invoke(t))
                {
                    return t;
                }
                return parent.gameObject.getBehaviourInParent(func);
            }
            return null;
        }
        /// <summary>
        /// Gets the first found behaviour in the parent.
        /// </summary>
        /// <typeparam name="T">The type of behaviour to get</typeparam>
        /// <param name="gameObject">The gameobject to get the behaviour on.</param>
        /// <returns></returns>
        public static T getBehaviourInParent<T>(this GameObject gameObject) where T : MonoBehaviour
        {
            // Written, 02.07.2022

            return getBehaviourInParent<T>(gameObject, func => true);
        }
        /// <summary>
        /// Gets all behaviours found in all parents. where <paramref name="func"/> action returns true.
        /// </summary>
        /// <typeparam name="T">The type of behaviour to get</typeparam>
        /// <param name="gameObject">The gameobject to get the behaviour on.</param>
        /// <param name="func">the function to check with.</param>
        /// <returns></returns>
        public static T[] getBehavioursInParent<T>(this GameObject gameObject, Func<T, bool> func) where T : MonoBehaviour
        {
            // Written, 02.07.2022

            List<T> values = new List<T>();
            Transform parent = gameObject.transform.parent;
            if (parent)
            {
                T t = parent.GetComponent<T>();
                if (t && func.Invoke(t))
                {
                    values.Add(t);
                }
                T[] v = parent.gameObject.getBehavioursInParent(func);
                if (v != null && v.Length > 0)
                    values.AddRange(v);
            }
            if (values.Count == 0)
                return null;
            return values.ToArray();
        }
        /// <summary>
        /// Gets all behaviours found in all parents.
        /// </summary>
        /// <typeparam name="T">The type of behaviour to get</typeparam>
        /// <param name="gameObject">The gameobject to get the behaviour on.</param>
        /// <returns></returns>
        public static T[] getBehavioursInParent<T>(this GameObject gameObject) where T : MonoBehaviour
        {
            // Written, 02.07.2022

            return getBehavioursInParent<T>(gameObject, func => true);
        }
        /// <summary>
        /// Gets the first found behaviour in the children. where <paramref name="func"/> action returns true.
        /// </summary>
        /// <typeparam name="T">The type of behaviour to get</typeparam>
        /// <param name="gameObject">The gameobject to get the behaviour on.</param>
        /// <param name="func">the function to check with.</param>
        /// <returns></returns>
        public static T getBehaviourInChildren<T>(this GameObject gameObject, Func<T, bool> func) where T : MonoBehaviour
        {
            // Written, 11.07.2022

            if (gameObject.transform.childCount > 0)
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    Transform child = gameObject.transform.GetChild(i);
                    T t = child.GetComponent<T>();
                    if (t && func.Invoke(t))
                    {
                        return t;
                    }
                    return child.gameObject.getBehaviourInChildren<T>();
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the first found behaviour in the children.
        /// </summary>
        /// <typeparam name="T">The type of behaviour to get</typeparam>
        /// <param name="gameObject">The gameobject to get the behaviour on.</param>
        public static T getBehaviourInChildren<T>(this GameObject gameObject) where T : MonoBehaviour
        {
            // Written, 11.07.2022

            return getBehaviourInChildren<T>(gameObject, func => true);
        }
        /// <summary>
        /// Gets all behaviours found in all children. where <paramref name="func"/> action returns true.
        /// </summary>
        /// <typeparam name="T">The type of behaviour to get</typeparam>
        /// <param name="gameObject">The gameobject to get the behaviour on.</param>
        /// <param name="func">the function to check with.</param>
        /// <returns></returns>
        public static T[] getBehavioursInChildren<T>(this GameObject gameObject, Func<T, bool> func) where T : MonoBehaviour
        {
            // Written, 02.07.2022

            if (gameObject.transform.childCount > 0)
            {
                List<T> values = new List<T>();
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    Transform child = gameObject.transform.GetChild(i);
                    T t = child.GetComponent<T>();
                    if (t && func.Invoke(t))
                    {
                        values.Add(t);
                    }
                    T[] v = child.gameObject.getBehavioursInChildren(func);
                    if (v != null && v.Length > 0)
                        values.AddRange(v);
                }
                if (values.Count == 0)
                    return null;
                return values.ToArray();
            }
            return null;
        }
        /// <summary>
        /// Gets all behaviours found in all children.
        /// </summary>
        /// <typeparam name="T">The type of behaviour to get</typeparam>
        /// <param name="gameObject">The gameobject to get the behaviour on.</param>
        public static T[] getBehavioursInChildren<T>(this GameObject gameObject) where T : MonoBehaviour
        {
            // Written, 02.07.2022

            return getBehavioursInChildren<T>(gameObject, func => true);
        }
       
        /// <summary>
        /// returns whether the player is currently looking at an gameobject. By raycast.
        /// </summary>
        /// <param name="gameObject">The gameobject to detect againist.</param>
        /// <param name="withinDistance">raycast within distance from main camera.</param>
        public static bool isPlayerLookingAt(this GameObject gameObject, float withinDistance = 1)
        {
            if (Physics.Raycast(getPlayerCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, withinDistance, 1 << gameObject.layer))
            {
                if (hit.collider?.gameObject == gameObject)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Checks if player is holding the GameObject, <paramref name="inGameObject"/>. by checking if <paramref name="inGameObject"/>'s <see cref="GameObject.layer"/> equals <see cref="LayerMasksEnum.Wheel"/>
        /// </summary>
        /// <param name="inGameObject">The gameObject to check on.</param>
        public static bool isPlayerHolding(this GameObject inGameObject)
        {
            // Written, 02.10.2018

            if (inGameObject.isOnLayer(LayerMasksEnum.Wheel))
                return true;
            return false;
        }
        /// <summary>
        /// checks if the player is holding this gameobject. returns false if player is holding nothing. by checking Players pickedupgameobject FSM
        /// </summary>
        public static bool isPlayerHoldingByPickup(this GameObject go)
        {
            // Written, 08.05.2022

            return getPickedUpGameObject?.Value == go;
        }
        /// <summary>
        /// Checks if the player is looking at this gameobject. returns false if player is looking at nothing. by checking Players raycasthitgameobject FSM
        /// </summary>
        public static bool isPlayerLookingAtByPickUp(this GameObject go)
        {
            // Written, 08.05.2022

            return getRaycastHitGameObject?.Value == go;
        }
        /// <summary>
        /// Checks if the gameobject is on the layer, <paramref name="inLayer"/>.
        /// </summary>
        /// <param name="inGameObject">The gameobject to check layer.</param>
        /// <param name="inLayer">The layer to check for.</param>
        public static bool isOnLayer(this GameObject inGameObject, LayerMasksEnum inLayer)
        {
            // Written, 02.10.2018

            if (inGameObject.layer == inLayer.layer())
                return true;
            return false;
        }
        
        /// <summary>
        /// Sends all children of game object to layer.
        /// </summary>
        /// <param name="inGameObject">The gameobject to get children from.</param>
        /// <param name="inLayer">The layer to send gameobject children to.</param>
        public static void sendChildrenToLayer(this GameObject inGameObject, LayerMasksEnum inLayer)
        {
            // Written, 27.09.2018

            Transform transform = inGameObject.transform;

            if (transform != null)
                for (int i = 0; i < inGameObject.transform.childCount; i++)
                {
                    inGameObject.transform.GetChild(i).gameObject.sendToLayer(inLayer);
                }
        }
        /// <summary>
        /// Sends a gameobject to the desired layer.
        /// </summary>
        /// <param name="inGameObject">The gameObject.</param>
        /// <param name="inLayer">The Layer.</param>
        /// <param name="inSendAllChildren">[Optional] sends all children to the layer.</param>
        public static void sendToLayer(this GameObject inGameObject, LayerMasksEnum inLayer, bool inSendAllChildren = false)
        {
            // Written, 02.10.2018

            inGameObject.layer = layer(inLayer);
            if (inSendAllChildren)
                inGameObject.sendChildrenToLayer(inLayer);
        }
        /// <summary>
        /// Returns the layer index number.
        /// </summary>
        /// <param name="inLayer">The layer to get index.</param>
        public static int layer(this LayerMasksEnum inLayer)
        {
            // Written, 02.10.2018

            return (int)inLayer;
        }
        /// <summary>
        /// Returns the name of the layer.
        /// </summary>
        /// <param name="inLayer">The layer to get.</param>
        public static string name(this LayerMasksEnum inLayer)
        {
            // Written, 02.10.2018

            string layerName;
            switch (inLayer)
            {
                case LayerMasksEnum.Bolts:
                    layerName = "Bolts";
                    break;
                case LayerMasksEnum.Cloud:
                    layerName = "Cloud";
                    break;
                case LayerMasksEnum.Collider:
                    layerName = "Collider";
                    break;
                case LayerMasksEnum.Collider2:
                    layerName = "Collider2";
                    break;
                case LayerMasksEnum.Dashboard:
                    layerName = "Dashboard";
                    break;
                case LayerMasksEnum.Datsun:
                    layerName = "Datsun";
                    break;
                case LayerMasksEnum.Default:
                    layerName = "Default";
                    break;
                case LayerMasksEnum.DontCollide:
                    layerName = "DontCollide";
                    break;
                case LayerMasksEnum.Forest:
                    layerName = "Forest";
                    break;
                case LayerMasksEnum.Glass:
                    layerName = "Glass";
                    break;
                case LayerMasksEnum.GUI:
                    layerName = "GUI";
                    break;
                case LayerMasksEnum.HingedObjects:
                    layerName = "HingedObjects";
                    break;
                case LayerMasksEnum.IgnoreRaycast:
                    layerName = "IgnoreRaycast";
                    break;
                case LayerMasksEnum.Lifter:
                    layerName = "Lifter";
                    break;
                case LayerMasksEnum.NoRain:
                    layerName = "NoRain";
                    break;
                case LayerMasksEnum.Parts:
                    layerName = "Parts";
                    break;
                case LayerMasksEnum.Player:
                    layerName = "Player";
                    break;
                case LayerMasksEnum.PlayerOnlyColl:
                    layerName = "PlayerOnlyColl";
                    break;
                case LayerMasksEnum.Road:
                    layerName = "Road";
                    break;
                case LayerMasksEnum.Terrain:
                    layerName = "Terrain";
                    break;
                case LayerMasksEnum.Tools:
                    layerName = "Tools";
                    break;
                case LayerMasksEnum.TransparentFX:
                    layerName = "TransparentFX";
                    break;
                case LayerMasksEnum.TriggerOnly:
                    layerName = "TriggerOnly";
                    break;
                case LayerMasksEnum.UI:
                    layerName = "UI";
                    break;
                case LayerMasksEnum.Water:
                    layerName = "Water";
                    break;
                case LayerMasksEnum.Wheel:
                    layerName = "Wheel";
                    break;
                default:
                    layerName = "Blank";
                    break;
            }
            return layerName;
        }
        
        /// <summary>
        /// Creates and returns a new rigidbody on the gameobject and assigns parameters listed.
        /// </summary>
        /// <param name="gameObject">The gameobject to create a rigidbody on.</param>
        /// <param name="rigidbodyConstraints">Create rigidbody with these constraints.</param>
        /// <param name="mass">The mass of the rb.</param>
        /// <param name="drag">The drag of the rb</param>
        /// <param name="angularDrag">The angular drag of the rb.</param>
        /// <param name="isKinematic">is this rigidbody kinmatic?</param>
        /// <param name="useGravity">is this rigidbody affected by gravity?</param>
        public static Rigidbody createRigidbody(this GameObject gameObject, RigidbodyConstraints rigidbodyConstraints = RigidbodyConstraints.None, float mass = 1,
            float drag = 0, float angularDrag = 0.05f, bool isKinematic = false, bool useGravity = true)
        {
            // Written, 10.09.2021

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = rigidbodyConstraints;
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.isKinematic = isKinematic;
            rb.useGravity = useGravity;
            return rb;
        }
        /// <summary>
        /// Fixes a transform to another transform.
        /// </summary>
        /// <param name="transform">The transform to fix</param>
        /// <param name="parent">The parent transform to fix <paramref name="transform"/></param>
        /// <param name="onFixedToParent">action to call when fixed to parent.</param>
        public static IEnumerator fixToParent(this Transform transform, Transform parent)
        {
            while (transform.parent != parent)
            {
                transform.parent = parent;
                yield return null;
            }
        }
        /// <summary>
        /// Sets its position and rotation.
        /// </summary>
        /// <param name="transform">The transform to fix</param>
        /// <param name="pos">The pos to set</param>
        /// <param name="eulerAngles">the rot to set</param>
        /// <param name="onFixedTransform">action to call when fixed transform.</param>
        public static IEnumerator fixTransform(this Transform transform, Vector3 pos, Vector3 eulerAngles)
        {
            while (transform.localPosition != pos && transform.localEulerAngles != eulerAngles)
            {
                transform.localPosition = pos;
                transform.localEulerAngles = eulerAngles;
                yield return null;
            }
        }
        
        private static FsmStateActionCallback determineAndCreateCallbackType(CallbackTypeEnum type, Action action, bool everyFrame = false)
        {
            // Written, 13.06.2022

            switch (type)
            {
                case CallbackTypeEnum.onFixedUpdate:
                    return new OnFixedUpdateCallback(action, everyFrame);
                case CallbackTypeEnum.onUpdate:
                    return new OnUpdateCallback(action, everyFrame);
                case CallbackTypeEnum.onGui:
                    return new OnGuiCallback(action, everyFrame);
                default:
                case CallbackTypeEnum.onEnter:
                    return new OnEnterCallback(action, everyFrame);
            }
        }
        private static FsmStateActionCallback determineAndCreateActionType(this FsmState state, InjectEnum injectType, Action action, CallbackTypeEnum callbackType, bool everyFrame, int index = 0)
        {
            // Written, 18.06.2022

            FsmStateActionCallback cb;
            switch (injectType)
            {
                case InjectEnum.prepend:
                    cb = state.prependNewAction(action, callbackType, everyFrame);
                    break;
                case InjectEnum.insert:
                    cb = state.insertNewAction(action, index, callbackType, everyFrame);
                    break;
                case InjectEnum.replace:
                    cb = state.replaceAction(action, index, callbackType, everyFrame);
                    break;
                default:
                case InjectEnum.append:
                    cb = state.appendNewAction(action, callbackType, everyFrame);
                    break;
            }
            return cb;
        }
        /// <summary>
        /// adds a new global transition.
        /// </summary>
        /// <param name="fsm"></param>
        /// <param name="_event"></param>
        /// <param name="toStateName"></param>
        public static FsmTransition addNewGlobalTransition(this PlayMakerFSM fsm, FsmEvent _event, string toStateName)
        {
            FsmTransition[] fsmGlobalTransitions = fsm.FsmGlobalTransitions;
            List<FsmTransition> temp = new List<FsmTransition>();
            foreach (FsmTransition t in fsmGlobalTransitions)
            {
                temp.Add(t);
            }
            FsmTransition ft = new FsmTransition
            {
                FsmEvent = _event,
                ToState = toStateName
            };
            temp.Add(ft);
            fsm.Fsm.GlobalTransitions = temp.ToArray();
            print($"Adding new global transition, to fsm, {fsm.gameObject.name}.{fsm.name}. {_event.Name} transitions this fsm state to {toStateName}");
            return ft;
        }
        /// <summary>
        /// Adds a new local transition.
        /// </summary>
        /// <param name="state">The state to add a transition.</param>
        /// <param name="eventName">The event that triggers this transition.</param>
        /// <param name="toStateName">the state it should change to when transition is triggered.</param>
        public static FsmTransition addNewTransitionToState(this FsmState state, string eventName, string toStateName)
        {
            List<FsmTransition> temp = new List<FsmTransition>();
            foreach (FsmTransition t in state.Transitions)
            {
                temp.Add(t);
            }
            FsmTransition ft = new FsmTransition
            {
                FsmEvent = state.Fsm.GetEvent(eventName),
                ToState = toStateName
            };
            temp.Add(ft);
            state.Transitions = temp.ToArray();
            print($"Adding new local transition, to state, {state.Fsm.GameObject.name}.{state.Fsm.Name}.{state.Name}. {eventName} transitions this fsm from {state.Name} to {toStateName}");
            return ft;
        }
        /// <summary>
        /// Appends a new action in a state.
        /// </summary>
        /// <param name="state">the sate to modify.</param>
        /// <param name="action">the action to append.</param>
        /// <param name="callbackType">Represents the callback type.</param>
        /// <param name="everyFrame">Represents if it should run everyframe..</param>
        public static FsmStateActionCallback appendNewAction(this FsmState state, Action action, CallbackTypeEnum callbackType = 0, bool everyFrame = false)
        {
            List<FsmStateAction> temp = new List<FsmStateAction>();
            foreach (FsmStateAction v in state.Actions)
            {
                temp.Add(v);
            }
            FsmStateActionCallback cb = determineAndCreateCallbackType(callbackType, action, everyFrame);
            temp.Add(cb);
            state.Actions = temp.ToArray();
            print($"Appending new action {action.Method.Name} to {state.Fsm.GameObject.name}.{state.Fsm.Name}.{state.Name}. Called {callbackType} {(everyFrame ? "every frame" : "once")}.");
            return cb;
        }
        /// <summary>
        /// prepends a new action in a state.
        /// </summary>
        /// <param name="state">the sate to modify.</param>
        /// <param name="action">the action to prepend.</param>
        /// <param name="callbackType">Represents the callback type.</param>
        /// <param name="everyFrame">Represents if it should run everyframe..</param>
        public static FsmStateActionCallback prependNewAction(this FsmState state, Action action, CallbackTypeEnum callbackType = 0, bool everyFrame = false)
        {
            List<FsmStateAction> temp = new List<FsmStateAction>();
            FsmStateActionCallback cb = determineAndCreateCallbackType(callbackType, action, everyFrame);
            temp.Add(cb);
            foreach (FsmStateAction v in state.Actions)
            {
                temp.Add(v);
            }
            state.Actions = temp.ToArray();
            print($"Prepending new {action.Method.Name} to {state.Fsm.GameObject.name}.{state.Fsm.Name}.{state.Name}. Called {(everyFrame ? "every frame" : "once")}.");
            return cb;
        }
        /// <summary>
        /// Inserts a new action in a state.
        /// </summary>
        /// <param name="state">the sate to modify.</param>
        /// <param name="action">the action to insert.</param>
        /// <param name="index">the index to insert action</param>
        /// <param name="callbackType">Represents the callback type.</param>
        /// <param name="everyFrame">Represents if it should run everyframe..</param>
        public static FsmStateActionCallback insertNewAction(this FsmState state, Action action, int index, CallbackTypeEnum callbackType = 0, bool everyFrame = false)
        {
            List<FsmStateAction> temp = new List<FsmStateAction>();
            foreach (FsmStateAction v in state.Actions)
            {
                temp.Add(v);
            }
            FsmStateActionCallback cb = determineAndCreateCallbackType(callbackType, action, everyFrame);
            temp.Insert(index, cb);
            state.Actions = temp.ToArray();
            print($"Inserting new {action.Method.Name} into {state.Fsm.GameObject.name}.{state.Fsm.Name}.{state.Name} at index:{index}. Called {(everyFrame ? "every frame" : "once")}.");
            return cb;
        }
        /// <summary>
        /// Replaces an action in a state.
        /// </summary>
        /// <param name="state">the sate to modify.</param>
        /// <param name="action">the action to replace.</param>
        /// <param name="index">the index to replace action</param>
        /// <param name="callbackType">Represents the callback type.</param>
        /// <param name="everyFrame">Represents if it should run everyframe..</param>
        public static FsmStateActionCallback replaceAction(this FsmState state, Action action, int index, CallbackTypeEnum callbackType = 0, bool everyFrame = false)
        {
            // Written, 13.04.2022

            List<FsmStateAction> temp = new List<FsmStateAction>();
            FsmStateActionCallback cb = determineAndCreateCallbackType(callbackType, action, everyFrame);
            for (int i = 0; i < state.Actions.Length; i++)
            {
                if (i == index)
                    temp.Add(cb);
                else
                    temp.Add(state.Actions[i]);
            }
            state.Actions = temp.ToArray();
            print($"Replacing {action.Method.Name} with {state.Fsm.GameObject.name}.{state.Fsm.Name}.{state.Name} at index:{index}. Called {(everyFrame ? "every frame" : "once")}.");
            return cb;
        }
        /// <summary>
        /// Represents inject action logic.
        /// </summary>
        /// <param name="playMakerFSM">The playermaker to search on.</param>
        /// <param name="stateName">the state name to inject on.</param>
        /// <param name="injectType">inject type.</param>
        /// <param name="callback">inject callback</param>
        /// <param name="index">index to inject at. NOTE: only applies to <see cref="InjectEnum.insert"/> and <see cref="InjectEnum.replace"/></param>
        /// <param name="callbackType">Represents the callback type.</param>
        /// <param name="everyFrame">Represents if it should run everyframe..</param>
        public static FsmStateActionCallback injectAction(this PlayMakerFSM playMakerFSM, string stateName, InjectEnum injectType, Action callback, int index = 0, CallbackTypeEnum callbackType = 0, bool everyFrame = false)
        {
            return injectAction(playMakerFSM.gameObject, playMakerFSM.FsmName, stateName, injectType, callback, index, callbackType, everyFrame);
        }
        /// <summary>
        /// Represents inject action logic.
        /// </summary>
        /// <param name="go">The gamobject to search on.</param>
        /// <param name="fsmName">the playmaker fsm name to search on</param>
        /// <param name="stateName">the state name to inject on.</param>
        /// <param name="injectType">inject type.</param>
        /// <param name="callback">inject callback</param>
        /// <param name="index">index to inject at. NOTE: only applies to <see cref="InjectEnum.insert"/> and <see cref="InjectEnum.replace"/></param>
        /// <param name="callbackType">Represents the callback type.</param>
        /// <param name="everyFrame">Represents if it should run everyframe..</param>
        public static FsmStateActionCallback injectAction(this GameObject go, string fsmName, string stateName, InjectEnum injectType, Action callback, int index = 0, CallbackTypeEnum callbackType = 0, bool everyFrame = false)
        {
            PlayMakerFSM fsm = go.GetPlayMaker(fsmName);
            FsmState state = go.GetPlayMakerState(stateName);
            return state.determineAndCreateActionType(injectType, callback, callbackType, everyFrame, index);
        }
       
        /// <summary>
        /// Rounds to <paramref name="decimalPlace"/>
        /// </summary>
        /// <param name="fsmFloat">The fsmFloat to round</param>
        /// <param name="decimalPlace">how many decimal places. eg. 2 = 0.01</param>
        public static FsmFloat round(this FsmFloat fsmFloat, int decimalPlace = 0)
        {
            return fsmFloat.Value.round(decimalPlace);
        }
        /// <summary>
        /// Rounds to <paramref name="decimalPlace"/>
        /// </summary>
        /// <param name="_float">The float to round</param>
        /// <param name="decimalPlace">how many decimal places. eg. 2 = 0.01</param>
        public static float round(this float _float, int decimalPlace = 0)
        {
            // Written, 15.01.2022

            return (float)Math.Round(_float, decimalPlace);
        }
        
        /// <summary>
        /// Maps a value and its range to another. eg. (v=1 min=0, max=2). could map to (v=2 min=1 max=3).
        /// </summary>
        /// <param name="mainValue">the value to map</param>
        /// <param name="inValueMin">value min</param>
        /// <param name="inValueMax">value max</param>
        /// <param name="outValueMin">result min</param>
        /// <param name="outValueMax">result max</param>
        public static float mapValue(this float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
        {
            return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
        }
        /// <summary>
        /// Converts a <see cref="Vector3Info"/> to a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="i">the vector3 info to convert</param>
        /// <returns>the vector3 info as a vector3.</returns>
        public static Vector3 toVector3(this Vector3Info i)
        {
            return new Vector3(i.x, i.y, i.z);
        }
        /// <summary>
        /// converts a vector3.
        /// </summary>
        /// <param name="v">the vector3 to convert.</param>
        /// <returns>new instance Vector3Info</returns>
        public static Vector3Info toInfo(this Vector3 v)
        {
            return new Vector3Info(v);
        }
        /// <summary>
        /// Creates a new vector3 from an old one.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 clone(this Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        #endregion
    }
}
