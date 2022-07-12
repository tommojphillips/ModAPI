using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using TommoJProductions.ModApi.Attachable.CallBacks;
using UnityEngine;
using UnityEngine.EventSystems;
using static MSCLoader.ModConsole;
using static UnityEngine.Object;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents a bolt for a <see cref="Part"/>.
    /// </summary>
    public class Bolt
    {
        // Written, 11.05.2022

        #region enums/classes

        /// <summary>
        /// Represents all settings for a bolt.
        /// </summary>
        public class BoltSettings
        {
            internal string name = null;
            /// <summary>
            /// Represents the bolts type.
            /// </summary>
            public BoltType boltType = BoltType.shortBolt;
            /// <summary>
            /// Represents the custom bolt prefab to use. NOTE: Set <see cref="boltType"/> to <see cref="BoltType.custom"/>.
            /// </summary>
            public GameObject customBoltPrefab = null;
            /// <summary>
            /// Represents the spanner size 
            /// </summary>
            public BoltSize boltSize = BoltSize._7mm;
            /// <summary>
            /// Represents the position step. (how quick the position of the bolt chances when un/tightening)
            /// </summary>
            public float posStep = 0.0005f;
            /// <summary>
            /// Represents the rot step. (how quick the bolt rotates when un/tightening.
            /// </summary>
            public float rotStep = 30;
            /// <summary>
            /// Represents the tightness step.
            /// </summary>
            public float tightnessStep = 1;
            /// <summary>
            /// Represents the max tightness.
            /// </summary>
            public float maxTightness = 8;
            /// <summary>
            /// Represents positional direction of bolt.
            /// </summary>
            public Vector3 posDirection = Vector3.forward;
            /// <summary>
            /// Represents the rotational direction of bolt.
            /// </summary>
            public Vector3 rotDirection = Vector3.forward;
        }
        /// <summary>
        /// Represents all bolt sizes in game.
        /// </summary>
        [Description("Bolt Size")]
        public enum BoltSize
        {
            /// <summary>
            /// Represents sparkplug wrench
            /// </summary>
            [Description("Spark plug socket")]
            sparkplug = 55,
            /// <summary>
            /// Represents flat head screwdriver
            /// </summary>
            [Description("Screwdriver")]
            flathead = 65,
            /// <summary>
            /// Represents 5mm
            /// </summary>
            [Description("5 MM")]
            _5mm = 50,
            /// <summary>
            /// Represents 6mm
            /// </summary>
            [Description("6 MM")]
            _6_mm = 60,
            /// <summary>
            /// Represents 7mm
            /// </summary>
            [Description("7 MM")]
            _7mm = 70,
            /// <summary>
            /// Represents 8mm
            /// </summary>
            [Description("8 MM")]
            _8_mm = 80,
            /// <summary>
            /// Represents 9mm
            /// </summary>
            [Description("9 MM")]
            _9mm = 90,
            /// <summary>
            /// Represents 10mm
            /// </summary>
            [Description("10 MM")]
            _10_mm = 100,
            /// <summary>
            /// Represents 11mm
            /// </summary>
            [Description("11 MM")]
            _11mm = 110,
            /// <summary>
            /// Represents 12mm
            /// </summary>
            [Description("12 MM")]
            _12_mm = 120,
            /// <summary>
            /// Represents 13mm
            /// </summary>
            [Description("13 MM")]
            _13mm = 130,
            /// <summary>
            /// Represents 14mm
            /// </summary>
            [Description("14 MM")]
            _14mm = 140,
            /// <summary>
            /// Represents 15mm
            /// </summary>
            [Description("15 MM")]
            _15_mm = 150,
        }
        /// <summary>
        /// Represents different types of bolts.
        /// </summary>
        [Description("Bolt Type")]
        public enum BoltType
        {
            /// <summary>
            /// Represents a nut
            /// </summary>
            [Description("Nut")]
            nut,
            /// <summary>
            /// Represents a short bolt.
            /// </summary>
            [Description("Short bolt")]
            shortBolt,
            /// <summary>
            /// Represents a long bolt.
            /// </summary>
            [Description("long bolt")]
            longBolt,
            /// <summary>
            /// Represents a screw.
            /// </summary>
            [Description("screw")]
            screw,
            /// <summary>
            /// Represents the usage of a custom bolt.
            /// </summary>
            [Description("Custom bolt")]
            custom,
        }

        public class BoltSaveInfo
        {
        /// <summary>
        /// Represents the tightness of this bolt. range: 0 - <see cref="BoltSettings.maxTightness"/>.
        /// </summary>
            public float tightness { get; set; } = 0;
        }

        #endregion

        #region Events

        /// <summary>
        /// Represents the post onscrew event. occurs when this bolt is screwed in or out. called after all other bolt events are called. <see cref="onLoose"/>, <see cref="onTight"/>.
        /// </summary>
        public event Action onScrew;
        /// <summary>
        /// Represents the on loose event. occurs when this bolt becomes loose (screwed to loose state)
        /// </summary>
        public event Action onLoose;
        /// <summary>
        /// Represents the on tight event. occurs when this bolt becomes tight (screwed to tight state)
        /// </summary>
        public event Action onTight;
        /// <summary>
        /// Represents the out loose event. occurs when this bolt was loose but was screw in (screwed in from loose state)
        /// </summary>
        public event Action outLoose;
        /// <summary>
        /// Represents the out tight event. occurs when this bolt was tight but was screw out. (screwed in from tight state)
        /// </summary>
        public event Action outTight;

        #endregion 

        #region Fields
        
        /// <summary>
        /// Represennts the runtime ID for this bolt. Note Rutime.. could change next game load.
        /// </summary>
        public string ID;      
        /// <summary>
        /// Represents settings for this bolt.
        /// </summary>
        public BoltSettings boltSettings;
        /// <summary>
        /// Represents the bolt gameobject.
        /// </summary>
        public GameObject model;
        /// <summary>
        /// Represents the wait time between bolt screw. in seconds
        /// </summary>
        public static float wait = 0.33f;

        internal Vector3 startPosition;
        internal Vector3 startEulerAngles;
        internal Coroutine boltRoutine;

        internal static bool boltAssetsLoaded = false;
        internal static GameObject shortBoltPrefab;
        internal static GameObject longBoltPrefab;
        internal static GameObject screwPrefab;
        internal static GameObject nutPrefab;
        
        #endregion

        #region Properties

        /// <summary>
        /// Returns true if bolt has been initialized.
        /// </summary>
        public bool boltInitialized { get; internal set; }
        /// <summary>
        /// Represents current loaded save info.
        /// </summary>
        public BoltSaveInfo loadedSaveInfo { get; internal set; }
        /// <summary>
        /// Returns true if this bolt is tight (<see cref="tightness"/> == <see cref="BoltSettings.maxTightness"/>)
        /// </summary>
        public bool isTight => loadedSaveInfo.tightness >= boltSettings.maxTightness;
        /// <summary>
        /// Returns true if this bolt is loose (<see cref="tightness"/> == 0)
        /// </summary>
        public bool isLoose => loadedSaveInfo.tightness <= 0;
        /// <summary>
        /// Represents the bolt callback.
        /// </summary>
        public BoltCallback boltCallback;
        /// <summary>
        /// Represents the position delta.
        /// </summary>
        public Vector3 positionDelta => boltSettings.posDirection * boltSettings.posStep;
        /// <summary>
        /// Represents the rotation delta.
        /// </summary>
        public Vector3 rotationDelta => boltSettings.rotDirection * boltSettings.rotStep;
        /// <summary>
        /// Represents the bolt check. checks if in tool mode and that the player is holding the correct tool for the fastener.
        /// </summary>
        public bool boltCheck => !ModClient.isInHandMode && ModClient.getTool == boltSettings.boltSize;
        /// <summary>
        /// Represents the part that this bolt is linked to.
        /// </summary>
        public Part part { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialies this bolt with save info
        /// </summary>
        /// <param name="saveInfo">The save info for the bolt.</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public Bolt(BoltSaveInfo saveInfo = null, BoltSettings settings = default(BoltSettings), Vector3 position = default(Vector3), Vector3 eulerAngles = default(Vector3))
        {
            initOption1(saveInfo, settings, position, eulerAngles);
        }
        /// <summary>
        /// Initialies this bolt with a custom name and save info
        /// </summary>
        /// <param name="name">the name of the bolt</param>
        /// <param name="saveInfo">The save info for the bolt.</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public Bolt(string name, BoltSaveInfo saveInfo = null, BoltSettings settings = default(BoltSettings), Vector3 position = default(Vector3), Vector3 eulerAngles = default(Vector3))
        {
            initOption1(saveInfo, settings, position, eulerAngles);
            initOption2(name);
        }
        /// <summary>
        /// Initialies this bolt
        /// </summary>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public Bolt(BoltSettings settings = default(BoltSettings), Vector3 position = default(Vector3), Vector3 eulerAngles = default(Vector3))
        {
            initOption1(null, settings, position, eulerAngles);
        }
        /// <summary>
        /// Initialies this bolt with a custom name
        /// </summary>
        /// <param name="name">the name of the bolt</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public Bolt(string name, BoltSettings settings = default(BoltSettings), Vector3 position = default(Vector3), Vector3 eulerAngles = default(Vector3))
        {
            initOption1(null, settings, position, eulerAngles);
            initOption2(name);
        }

        #endregion

        #region IEnumerators

        internal IEnumerator boltFunction()
        {
            // Written, 03.07.2022
#if DEBUG
            Print("bolt function: started");
#endif
            while (boltCheck)
            {
                float scrollInput = Input.mouseScrollDelta.y;

                if (Mathf.Abs(scrollInput) > 0)
                {
                    int tempTightness = (int)Mathf.Clamp(loadedSaveInfo.tightness + (scrollInput * boltSettings.tightnessStep), 0, boltSettings.maxTightness);
                    if (tempTightness != loadedSaveInfo.tightness)
                    {
                        if (isTight)
                            outTight?.Invoke();
                        else if (isLoose)
                            outLoose?.Invoke();
                        loadedSaveInfo.tightness = tempTightness;
                        updateNutPosRot();
                        if (isTight)
                            onTight?.Invoke();
                        else if (isLoose)
                            onLoose?.Invoke();
                        onScrew?.Invoke();
                        MasterAudio.PlaySound3DAndForget("CarBuilding", model.transform, variationName: "bolt_screw");
                        yield return new WaitForSeconds(wait);
                    }
                }
                yield return null;
            }
#if DEBUG
            Print("bolt function: ended");
#endif
        }

        #endregion

        #region Methods

        void initOption1(BoltSaveInfo saveInfo, BoltSettings settings, Vector3 position, Vector3 eulerAngles)
        {
            loadedSaveInfo = saveInfo;
            boltSettings = settings;
            startPosition = position;
            startEulerAngles = eulerAngles;
        }
        void initOption2(string name)
        {
            boltSettings.name = name;
        }

        /// <summary>
        /// Initialies this bolt.
        /// </summary>
        internal void initBolt(Part p, BoltSaveInfo saveInfo = default(BoltSaveInfo))
        {
            // Written, 02.07.2022

            if (!boltInitialized && boltAssetsLoaded)
            {
                if (loadedSaveInfo == null)
                    loadedSaveInfo = saveInfo ?? new BoltSaveInfo();
                switch (boltSettings.boltType)
                {
                    case BoltType.custom:
                        model = Instantiate(boltSettings.customBoltPrefab);
                        break;
                    case BoltType.nut:
                        model = Instantiate(nutPrefab);
                        break;
                    case BoltType.screw:
                        model = Instantiate(screwPrefab);
                        break;
                    case BoltType.longBolt:
                        model = Instantiate(longBoltPrefab);
                        break;
                    default:
                    case BoltType.shortBolt:
                        model = Instantiate(shortBoltPrefab);
                        break;
                }
                boltCallback = model.AddComponent<BoltCallback>();
                boltCallback.bolt = this;
                boltCallback.onMouseEnter += bcb_mouseEnter;
                boltCallback.onMouseExit += bcb_mouseExit;
                updateNutPosRot();
                ModClient.bolts.Add(this);
                ID = "bolt" + ModClient.bolts.Count.ToString("000");
                part = p;
                model.name = ID;
                model.transform.SetParent(part.boltParent.transform);
                boltInitialized = true;
            }
        }
        /// <summary>
        /// Updates the models position and rotation based off <see cref="startPosition"/>, <see cref="startEulerAngles"/>, <see cref="BoltSaveInfo.tightness"/>
        /// </summary>
        internal void updateNutPosRot()
        {
            if (model)
            {
                model.transform.localPosition = startPosition + positionDelta * loadedSaveInfo.tightness;
                model.transform.localEulerAngles = startEulerAngles + rotationDelta * loadedSaveInfo.tightness;
            }
        }
        /// <summary>
        /// trys to load bolt assets.
        /// </summary>
        /// <returns>returns whether or not the bolt assets were loaded.</returns>
        internal static bool tryLoadBoltAssets()
        {
            // Written, 02.07.2022

            if (!boltAssetsLoaded)
            {
                string assetPath = Path.Combine(ModClient.getModsFolder, "Assets/ModApi/modapi.unity3d");

                if (File.Exists(assetPath))
                {
                    AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(assetPath));
                    nutPrefab = ab.LoadAsset("nut.prefab") as GameObject;
                    screwPrefab = ab.LoadAsset("screw.prefab") as GameObject;
                    shortBoltPrefab = ab.LoadAsset("short bolt.prefab") as GameObject;
                    longBoltPrefab = ab.LoadAsset("long bolt.prefab") as GameObject;
                    ab.Unload(false);
                    Print("[ModApi.Part] bolt assets loaded");
                    boltAssetsLoaded = true;
                }
                else
                {
                    Print("Please install Mod Reference, 'ModApi' correctly. modapi.unity3d could not be found. Error.");
                }
            }
            else
                Print("[ModApi.Part] bolt assets already loaded. :)");
            return boltAssetsLoaded;
        }
        /// <summary>
        /// gets bolt save info.
        /// </summary>
        internal BoltSaveInfo getSaveInfo() 
        {
            // Written, 11.07.2022

            return loadedSaveInfo;
        }

        #endregion

        #region Event Handlers

        internal void bcb_mouseEnter(BoltCallback bcb, PointerEventData data)
        {
            if (boltRoutine == null)
            {
                boltRoutine = boltCallback.StartCoroutine(boltFunction());
            }
        }
        internal void bcb_mouseExit(BoltCallback bcb, PointerEventData data)
        {
            if (boltRoutine != null)
            {
                boltCallback.StopCoroutine(boltRoutine);
                boltRoutine = null;
            }
        }

        #endregion

    }
}
