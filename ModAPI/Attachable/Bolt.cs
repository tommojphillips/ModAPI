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
            /// <summary>
            /// for initializing a new instance of bolt with custom name.
            /// </summary>
            public string name = null;

            /// <summary>
            /// Represents the bolts type.
            /// </summary>
            public BoltType boltType = BoltType.shortBolt;
            /// <summary>
            /// Represents the tool size required to un/tighten this bolt
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
            /// <summary>
            /// Represents the custom bolt prefab to use. NOTE: Set <see cref="boltType"/> to <see cref="BoltType.custom"/>.
            /// </summary>
            public GameObject customBoltPrefab = null;
            /// <summary>
            /// if true, <see cref="boltFunction"/> does not check if tightness has changed (tightness is either completely tight or loose). therefore invokes bolt event/s anyway. eg => if bolt is already tight and user tries to tighten more. it will call <see cref="onTight"/> anyway.
            /// </summary>
            public bool ignoreTightnessChangedCheck = false;
            /// <summary>
            /// if true adds a nut to the other side of the bolt. note both bolt and nut will need to be tightened.
            /// </summary>
            public bool addNut = false;
            /// <summary>
            /// Settings for <see cref="addNut"/> setting. 
            /// </summary>
            public AddNutSettings addNutSettings = default(AddNutSettings);
            /// <summary>
            /// Initializes new instance of b settings with default values.
            /// </summary>
            public BoltSettings() { }
            /// <summary>
            /// inits new instance of bolt settings and copies instance values.
            /// </summary>
            /// <param name="s">the instance of bolt settings to copy.</param>
            public BoltSettings(BoltSettings s)
            {
                if (s != null)
                {
                    boltType = s.boltType;
                    customBoltPrefab = s.customBoltPrefab;
                    boltSize = s.boltSize;
                    posStep = s.posStep;
                    rotStep = s.rotStep;
                    tightnessStep = s.tightnessStep;
                    maxTightness = s.maxTightness;
                    posDirection = s.posDirection;
                    rotDirection = s.rotDirection;
                    ignoreTightnessChangedCheck = s.ignoreTightnessChangedCheck;
                    addNut = s.addNut;
                    addNutSettings = s.addNutSettings;
                }
            }
        }
        /// <summary>
        /// settings for addNut
        /// </summary>
        public class AddNutSettings
        {
            /// <summary>
            /// Represents the tool size required to un/tighten this bolt. if null uses the parent bolts size.
            /// </summary>
            public BoltSize? nutSize = null;
            /// <summary>
            /// Represents the custom nut prefab to use. if null uses modapi nut prefab.
            /// </summary>
            public GameObject customNutPrefab = null;
            /// <summary>
            /// Represents the add nut offset. an offset applied to the model. on <see cref="BoltSettings.posDirection"/>
            /// </summary>
            public float nutOffset = 0;
            /// <summary>
            /// init with default settings
            /// </summary>
            public AddNutSettings() { }
            /// <summary>
            /// inits this and copies s to instance.
            /// </summary>
            /// <param name="s">the instance to copy.</param>
            public AddNutSettings(AddNutSettings s)
            {
                if (s != null)
                {
                    nutSize = s.nutSize;
                    customNutPrefab = s.customNutPrefab;
                    nutOffset = s.nutOffset;
                }
            }
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
            flathead = 64,
            /// <summary>
            /// Represents 5mm
            /// </summary>
            [Description("5 MM")]
            _5mm = 50,
            /// <summary>
            /// Represents 6mm
            /// </summary>
            [Description("6 MM")]
            _6mm = 60,
            /// <summary>
            /// Represents 7mm
            /// </summary>
            [Description("7 MM")]
            _7mm = 69,
            /// <summary>
            /// Represents 8mm
            /// </summary>
            [Description("8 MM")]
            _8mm = 80,
            /// <summary>
            /// Represents 9mm
            /// </summary>
            [Description("9 MM")]
            _9mm = 89,
            /// <summary>
            /// Represents 10mm
            /// </summary>
            [Description("10 MM")]
            _10mm = 100,
            /// <summary>
            /// Represents 11mm
            /// </summary>
            [Description("11 MM")]
            _11mm = 110,
            /// <summary>
            /// Represents 12mm
            /// </summary>
            [Description("12 MM")]
            _12mm = 120,
            /// <summary>
            /// Represents 13mm
            /// </summary>
            [Description("13 MM")]
            _13mm = 129,
            /// <summary>
            /// Represents 14mm
            /// </summary>
            [Description("14 MM")]
            _14mm = 139,
            /// <summary>
            /// Represents 15mm
            /// </summary>
            [Description("15 MM")]
            _15mm = 150,
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

        /// <summary>
        /// Represents the bolt save info class object.
        /// </summary>
        public class BoltSaveInfo
        {
            /// <summary>
            /// Represents the tightness of this bolt. range: 0 - <see cref="BoltSettings.maxTightness"/>.
            /// </summary>
            public float boltTightness { get; set; } = 0;
            /// <summary>
            /// Represents the tightness of the nut if addnut setting is true.. range: 0 - <see cref="BoltSettings.maxTightness"/>.
            /// </summary>
            public float addNutTightness { get; set; } = 0;
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
        /// Represennts the runtime ID for this bolt. Note Runtime.. could change next game load.
        /// </summary>
        public string boltID;      
        /// <summary>
        /// Represents settings for this bolt.
        /// </summary>
        public BoltSettings boltSettings;
        /// <summary>
        /// Represents the bolt gameobject.
        /// </summary>
        public GameObject boltModel;
        /// <summary>
        /// The add nut model. relevant only when <see cref="Bolt.boltSettings"/>.<see cref="BoltSettings.addNut"/> is <see langword="true"/>.
        /// </summary>
        GameObject addNutModel;

        internal Vector3 startPosition;
        internal Vector3 startEulerAngles;
        internal Coroutine boltRoutine;

        internal static bool boltAssetsLoaded = false;
        internal static GameObject shortBoltPrefab;
        internal static GameObject longBoltPrefab;
        internal static GameObject screwPrefab;
        internal static GameObject nutPrefab;

        /// <summary>
        /// Represents the bolt callback.
        /// </summary>
        public BoltCallback boltCallback;
        /// <summary>
        /// Represents the add nut bolt callback.
        /// </summary>
        public BoltCallback addNutCallback;

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
        /// Returns true if this bolt is tight (<see cref="BoltSaveInfo.boltTightness"/> == <see cref="BoltSettings.maxTightness"/>)
        /// </summary>
        public bool isTight => loadedSaveInfo.boltTightness >= boltSettings.maxTightness;
        /// <summary>
        /// Returns true if this bolt is loose (<see cref="BoltSaveInfo.boltTightness"/> == 0)
        /// </summary>
        public bool isLoose => loadedSaveInfo.boltTightness <= 0;
        /// <summary>
        /// Represents the position delta.
        /// </summary>
        public Vector3 positionDelta => boltSettings.posDirection * boltSettings.posStep;
        /// <summary>
        /// Represents the rotation delta.
        /// </summary>
        public Vector3 rotationDelta => boltSettings.rotDirection * boltSettings.rotStep;
        /// <summary>
        /// Represents the part that this bolt is linked to.
        /// </summary>
        public Part part { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialies this bolt. save info will be gathered from the part.
        /// </summary>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public Bolt(BoltSettings settings = default(BoltSettings), Vector3 position = default(Vector3), Vector3 eulerAngles = default(Vector3), float? addNutOffsetOverride = null)
        {
            init(null, settings, position, eulerAngles, addNutOffsetOverride);
        }
        /// <summary>
        /// Initialies this bolt with save info.
        /// </summary>
        /// <param name="saveInfo">The save info for the bolt.</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public Bolt(BoltSaveInfo saveInfo = null, BoltSettings settings = default(BoltSettings), Vector3 position = default(Vector3), Vector3 eulerAngles = default(Vector3), float? addNutOffsetOverride = null)
        {
            init(saveInfo, settings, position, eulerAngles, addNutOffsetOverride);
        }
        /// <summary>
        /// deconstructs this bolt. removes this boat from the loaded bolts list. <see cref="ModClient.loadedBolts"/>
        /// </summary>
        ~Bolt() 
        {
            ModClient.loadedBolts.Remove(this);
        }

        #endregion

        #region IEnumerators

        internal IEnumerator boltFunction(BoltCallback bcb, PointerEventData data)
        {
            // Written, 03.07.2022

            int boltTightnessTemp;
            int nutTightnessTemp;
            int tempTightness;
            bool isNut = false;
            bool foundBolt = false;
            bool tightnessChanged = false;
            while (bcb.boltCheck)
            {
                float scrollInput = data.scrollDelta.y;

                if (data.IsScrolling())
                {
                    boltTightnessTemp = (int)Mathf.Clamp(loadedSaveInfo.boltTightness + (scrollInput * boltSettings.tightnessStep), 0, boltSettings.maxTightness);
                    nutTightnessTemp = (int)Mathf.Clamp(loadedSaveInfo.addNutTightness + (scrollInput * boltSettings.tightnessStep), 0, boltSettings.maxTightness);
                    tempTightness = 0;
                    if (bcb == boltCallback && data.pointerEnter == boltModel && (boltSettings.addNut ? loadedSaveInfo.addNutTightness == 0 : true))
                    {
                        tempTightness = boltTightnessTemp;
                        isNut = false;
                        foundBolt = true;
                        tightnessChanged = boltTightnessTemp != loadedSaveInfo.boltTightness;
                    }
                    else if (boltSettings.addNut && bcb == addNutCallback && data.pointerEnter == addNutModel && loadedSaveInfo.boltTightness == boltSettings.maxTightness)
                    {
                        tempTightness = nutTightnessTemp;
                        isNut = true;
                        foundBolt = true;
                        tightnessChanged = nutTightnessTemp != loadedSaveInfo.addNutTightness;
                    }
                    if (foundBolt && (boltSettings.ignoreTightnessChangedCheck || tightnessChanged))
                    {
                        if (isTight)
                            outTight?.Invoke();
                        else if (isLoose)
                            outLoose?.Invoke();
                        
                        if (isNut)
                            loadedSaveInfo.addNutTightness = tempTightness;
                        else
                            loadedSaveInfo.boltTightness = tempTightness;

                        updateNutPosRot();

                        if (isTight)
                            onTight?.Invoke();
                        else if (isLoose)
                            onLoose?.Invoke();

                        onScrew?.Invoke();

                        if (tightnessChanged)
                        {
                            ModClient.playSoundAtInterupt(boltModel.transform, "CarBuilding", "bolt_screw");
                            yield return new WaitForSeconds(ModClient.getBoltingSpeed);
                            continue;
                        }
                    }
                }
                yield return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// sets up this bolt. -standard.
        /// </summary>
        /// <param name="saveInfo">the save info to set.</param>
        /// <param name="settings">the bolt settings to set.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rot of the bolt.</param>
        private void init(BoltSaveInfo saveInfo, BoltSettings settings, Vector3 position, Vector3 eulerAngles, float? addNutOffsetOverride = null)
        {
            loadedSaveInfo = saveInfo;
            boltSettings = new BoltSettings(settings);
            startPosition = position;
            startEulerAngles = eulerAngles;

            if (addNutOffsetOverride != null)
                boltSettings.addNutSettings.nutOffset = (float)addNutOffsetOverride;
        }
        /// <summary>
        /// sets up a nut on the opposite side of the bolt.
        /// </summary>
        private void addNut() 
        {
            // Written, 15.07.2022

            if (boltSettings.addNutSettings.customNutPrefab != null)
            {
                addNutModel = Instantiate(boltSettings.addNutSettings.customNutPrefab);
            }
            else
            {
                addNutModel = Instantiate(nutPrefab);
            }
            addNutModel.transform.parent = part.boltParent.transform;
            addNutModel.name = boltModel.name + "_nut";
            addNutCallback = addNutModel.AddComponent<BoltCallback>();
            addNutCallback.bolt = this;
            addNutCallback.boltSize = boltSettings.addNutSettings.nutSize ?? boltSettings.boltSize;
            addNutCallback.onMouseEnter += bcb_mouseEnter;
            addNutCallback.onMouseExit += bcb_mouseExit;
        }
        /// <summary>
        /// Initialies this bolt on <paramref name="p"/>.
        /// </summary>
        internal void initBoltOnPart(Part p, BoltSaveInfo saveInfo = default(BoltSaveInfo))
        {
            // Written, 02.07.2022

            if (boltAssetsLoaded)
            {
                if (!boltInitialized)
                {
                    boltID = "bolt" + ModClient.loadedBolts.Count.ToString("000");
                    part = p;

                    if (loadedSaveInfo == null)
                        loadedSaveInfo = saveInfo ?? new BoltSaveInfo();
                    switch (boltSettings.boltType)
                    {
                        case BoltType.custom:
                            boltModel = Instantiate(boltSettings.customBoltPrefab);
                            break;
                        case BoltType.nut:
                            boltModel = Instantiate(nutPrefab);
                            break;
                        case BoltType.screw:
                            boltModel = Instantiate(screwPrefab);
                            break;
                        case BoltType.longBolt:
                            boltModel = Instantiate(longBoltPrefab);
                            break;
                        default:
                        case BoltType.shortBolt:
                            boltModel = Instantiate(shortBoltPrefab);
                            break;
                    }
                    boltModel.transform.parent = part.boltParent.transform;
                    boltModel.name = boltSettings.name == null ? boltID : boltSettings.name;
                    boltCallback = boltModel.AddComponent<BoltCallback>();
                    boltCallback.bolt = this;
                    boltCallback.boltSize = boltSettings.boltSize;
                    boltCallback.onMouseEnter += bcb_mouseEnter;
                    boltCallback.onMouseExit += bcb_mouseExit;
                    if (boltSettings.addNut)
                    {
                        addNut();
                    }
                    updateNutPosRot();

                    ModClient.loadedBolts.Add(this);
                    boltInitialized = true;
                }
                else
                    Print($"Error initialializing Bolt on part, {p.name}. bolt already initialized.");
            }
            else
                Print($"Error initialializing Bolt on part, {p.name}. bolt assets arent loaded.");
        }
        /// <summary>
        /// Updates the models position and rotation based off <see cref="startPosition"/>, <see cref="startEulerAngles"/>,
        /// </summary>
        internal void updateNutPosRot()
        {
            if (boltModel)
            {
                boltModel.transform.localPosition = startPosition + positionDelta * loadedSaveInfo.boltTightness;
                boltModel.transform.localEulerAngles = startEulerAngles + rotationDelta * loadedSaveInfo.boltTightness;

                if (boltSettings.addNut && addNutModel)
                {
                    Vector3 nutAbsolute = positionDelta * boltSettings.maxTightness;
                    Vector3 nutPos = startPosition + (nutAbsolute * 2) + (boltSettings.addNutSettings.nutOffset * boltSettings.posDirection);
                    addNutModel.transform.localPosition = nutPos - positionDelta * loadedSaveInfo.addNutTightness;
                    addNutModel.transform.localEulerAngles = startEulerAngles - rotationDelta * loadedSaveInfo.addNutTightness;
                }
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
                    Print("[ModApi.Bolt] bolt assets loaded");
                    boltAssetsLoaded = true;
                }
                else
                {
                    Print("Please install Mod Reference, 'ModApi' correctly. modapi.unity3d could not be found. Error.");
                }
            }
            else
                Print("[ModApi.Bolt] bolt assets already loaded. :)");
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
        /// <summary>
        /// Resets bolt tightness. if add nut is <see langword="true"/>. resets nut tightness too.
        /// </summary>
        public void resetTightness() 
        {
            // Written, 16.07.2022

            loadedSaveInfo.boltTightness = 0;
            if (boltSettings.addNut)
                loadedSaveInfo.addNutTightness = 0;
        }
        
        #endregion

        #region Event Handlers

        internal void bcb_mouseEnter(BoltCallback bcb, PointerEventData data)
        {
            if (boltRoutine == null)
            {
                boltRoutine = boltCallback.StartCoroutine(boltFunction(bcb, data));
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
