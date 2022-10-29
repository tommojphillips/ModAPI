using System;
using System.Collections;
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
        
        internal Coroutine boltRoutine;

        private int boltTightnessTemp;
        private int nutTightnessTemp;
        private int tempTightness;
        private bool isNut = false;
        private bool foundBolt = false;
        private bool tightnessChanged = false;

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if bolt has been initialized.
        /// </summary>
        public bool boltInitialized { get; internal set; }
        /// <summary>
        /// Returns true if this bolt is tight (<see cref="BoltSaveInfo.boltTightness"/> == <see cref="BoltSettings.maxTightness"/>)
        /// </summary>
        public bool isTight => loadedSaveInfo.boltTightness >= boltSettings.maxTightness;
        /// <summary>
        /// Returns true if this bolt is loose (<see cref="BoltSaveInfo.boltTightness"/> == 0)
        /// </summary>
        public bool isLoose => loadedSaveInfo.boltTightness <= 0;
        /// <summary>
        /// Represennts the runtime ID for this bolt. Note Runtime.. could change next game load.
        /// </summary>
        public string boltID { get; private set; }

        /// <summary>
        /// Represents settings for this bolt.
        /// </summary>
        public BoltSettings boltSettings { get; private set; }
        /// <summary>
        /// Represents current loaded save info.
        /// </summary>
        public BoltSaveInfo loadedSaveInfo { get; internal set; }

        /// <summary>
        /// Represents the start position of the bolt.
        /// </summary>
        public Vector3 startPosition { get; internal set; }
        /// <summary>
        /// Represents the start rotation of the bolt.
        /// </summary>
        public Vector3 startEulerAngles { get; internal set; }
        /// <summary>
        /// Represents the position delta.
        /// </summary>
        public Vector3 positionDelta => boltSettings.posDirection * boltSettings.posStep;
        /// <summary>
        /// Represents the rotation delta.
        /// </summary>
        public Vector3 rotationDelta => boltSettings.rotDirection * boltSettings.rotStep;

        /// <summary>
        /// Represents the bolt callback.
        /// </summary>
        public BoltCallback boltCallback { get; private set; }
        /// <summary>
        /// Represents the add nut bolt callback.
        /// </summary>
        public BoltCallback addNutCallback { get; private set; }

        /// <summary>
        /// Represents the bolt gameobject.
        /// </summary>
        public GameObject boltModel { get; private set; }
        /// <summary>
        /// The add nut model. relevant only when <see cref="Bolt.boltSettings"/>.<see cref="BoltSettings.addNut"/> is <see langword="true"/>.
        /// </summary>
        public GameObject addNutModel { get; private set; }


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
        public Bolt(BoltSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default)
        {
            init(null, settings, position, eulerAngles);
        }
        /// <summary>
        /// Initialies this bolt with save info.
        /// </summary>
        /// <param name="saveInfo">The save info for the bolt.</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public Bolt(BoltSaveInfo saveInfo = null, BoltSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default)
        {
            init(saveInfo, settings, position, eulerAngles);
        }
        /// <summary>
        /// Initialies this bolt with an add nut offset override. save info will be gathered from the part. (only vaild to bolts that have the <see cref="BoltSettings.addNut"/> setting on)
        /// </summary>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        /// <param name="addNutOffsetOverride">add nut offset override float. ignored if null.</param>
        public Bolt(BoltSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default, float? addNutOffsetOverride = null)
        {
            init(null, settings, position, eulerAngles, addNutOffsetOverride);
        }
        /// <summary>
        /// Initialies this bolt with save info with an add nut offset override. (only vaild to bolts that have the <see cref="BoltSettings.addNut"/> setting on).
        /// </summary>
        /// <param name="saveInfo">The save info for the bolt.</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        /// <param name="addNutOffsetOverride">add nut offset override float. ignored if null.</param>
        public Bolt(BoltSaveInfo saveInfo = null, BoltSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default, float? addNutOffsetOverride = null)
        {
            init(saveInfo, settings, position, eulerAngles, addNutOffsetOverride);
        }
        /// <summary>
        /// deconstructs this bolt. removes this bolt from the loaded bolts list. <see cref="ModClient.loadedBolts"/>
        /// </summary>
        ~Bolt() 
        {
            ModClient.loadedBolts.Remove(this);
        }

        #endregion

        #region IEnumerators

        internal IEnumerator boltFunction(BoltCallback bcb)
        {
            // Written, 03.07.2022

            while (bcb.boltCheck)
            {
                float scrollInput = Input.mouseScrollDelta.y;

                if (scrollInput != 0)
                {
                    if (ModClient.playerHasRatchet)
                    {
                        scrollInput = Mathf.Abs(scrollInput) * (ModClient.ratchetSwitch ? 1 : -1);
                    }

                    boltTightnessTemp = (int)Mathf.Clamp(loadedSaveInfo.boltTightness + (scrollInput * boltSettings.tightnessStep), 0, boltSettings.maxTightness);
                    nutTightnessTemp = (int)Mathf.Clamp(loadedSaveInfo.addNutTightness + (scrollInput * boltSettings.tightnessStep), 0, boltSettings.maxTightness);
                    tempTightness = 0;
                    if (bcb == boltCallback && (!boltSettings.addNut || loadedSaveInfo.addNutTightness == 0))
                    {
                        tempTightness = boltTightnessTemp;
                        isNut = false;
                        foundBolt = true;
                        tightnessChanged = boltTightnessTemp != loadedSaveInfo.boltTightness;
                    }
                    else if (boltSettings.addNut && bcb == addNutCallback && loadedSaveInfo.boltTightness == boltSettings.maxTightness)
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

                        updateModelPosition();

                        if (isTight)
                            onTight?.Invoke();
                        else if (isLoose)
                            onLoose?.Invoke();

                        onScrew?.Invoke();

                        part.boltOnScrew();

                        if (tightnessChanged)
                        { 
                            ModClient.playSoundAtInterupt(boltModel.transform, "CarBuilding", "bolt_screw");
                            yield return new WaitForSeconds(ModClient.playerHasRatchet ? ModClient.getRachetBoltingSpeed : ModClient.getSpannerBoltingSpeed);
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
        /// <param name="addNutOffsetOverride">add nut offset override float. ignored if null.</param>
        private void init(BoltSaveInfo saveInfo, BoltSettings settings, Vector3 position, Vector3 eulerAngles, float? addNutOffsetOverride = null)
        {
            loadedSaveInfo = saveInfo;
            boltSettings = new BoltSettings(settings);
            startPosition = position;
            startEulerAngles = eulerAngles;

            if (addNutOffsetOverride != null)
            {
                boltSettings.addNutSettings.nutOffset = (float)addNutOffsetOverride;
            }
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
                addNutModel = Instantiate(ModClient.nutPrefab);
            }
            setParent(addNutModel);
            addNutModel.name = "nut" + boltID.Replace("bolt", "");
            addNutCallback = addNutModel.AddComponent<BoltCallback>();
            addNutCallback.bolt = this;
            addNutCallback.boltSize = boltSettings.addNutSettings.nutSize ?? boltSettings.boltSize;
        }
        /// <summary>
        /// Initialies this bolt on <paramref name="p"/>.
        /// </summary>
        internal void initBoltOnPart(Part p, BoltSaveInfo saveInfo = default)
        {
            // Written, 02.07.2022

            if (ModClient.boltAssetsLoaded)
            {
                if (!boltInitialized)
                {
                    boltID = "bolt" + ModClient.loadedBolts.Count.ToString("000");
                    part = p;

                    if (loadedSaveInfo == null)
                        loadedSaveInfo = saveInfo ?? new BoltSaveInfo();
                    setupBoltModel();
                    updateModelPosition();

                    ModClient.loadedBolts.Add(this);
                    boltInitialized = true;
                }
                else
                    Print($"Error initialializing Bolt on part, {p.name}. bolt already initialized.");
            }
            else
                Print($"Error initialializing Bolt on part, {p.name}. bolt assets arent loaded.");
        }

        private void setupBoltModel()
        {
            // Written, 29.10.2022

            switch (boltSettings.boltType)
            {
                case BoltType.custom:
                    boltModel = Instantiate(boltSettings.customBoltPrefab);
                    break;
                case BoltType.nut:
                    boltModel = Instantiate(ModClient.nutPrefab);
                    break;
                case BoltType.screw:
                    boltModel = Instantiate(ModClient.screwPrefab);
                    break;
                case BoltType.longBolt:
                    boltModel = Instantiate(ModClient.longBoltPrefab);
                    break;
                default:
                case BoltType.shortBolt:
                    boltModel = Instantiate(ModClient.shortBoltPrefab);
                    break;
            }
            setParent(boltModel);
            boltCallback = boltModel.AddComponent<BoltCallback>();
            boltCallback.bolt = this;
            boltCallback.boltSize = boltSettings.boltSize;
            if (boltSettings.addNut)
            {
                addNut();
            }

            boltModel.name = boltSettings.name ?? boltID;
        }

        private void setParent(GameObject child)
        {
            // Written, 29.10.2022

            if (boltSettings.customParent)
            {
                child.transform.SetParent(boltSettings.customParent);
            }
            else
            {
                if (boltSettings.parentBoltToTrigger)
                {
                    child.transform.SetParent(part.triggers[boltSettings.parentBoltToTriggerIndex].boltParent.transform);
                }
                else
                {
                    child.transform.SetParent(part.boltParent.transform);
                }
            }
        }

        /// <summary>
        /// Updates the models position and rotation based off <see cref="startPosition"/>, <see cref="startEulerAngles"/>,
        /// </summary>
        internal void updateModelPosition()
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
        /// Resets bolt tightness. if add nut is <see langword="true"/>. resets nut tightness too.
        /// </summary>
        public void resetTightness() 
        {
            // Written, 16.07.2022

            loadedSaveInfo.boltTightness = 0;
            if (boltSettings.addNut)
                loadedSaveInfo.addNutTightness = 0;
        }
        /// <summary>
        /// sets bolt and addnut tightness to the max tightness setting. <see cref="BoltSettings.maxTightness"/>
        /// </summary>
        public void setMaxTightness() 
        {
            // Written, 25.08.2022

            loadedSaveInfo.boltTightness = boltSettings.maxTightness;
            if (boltSettings.addNut)
                loadedSaveInfo.addNutTightness = boltSettings.maxTightness;
        }
        
        internal void bcb_mouseEnter(BoltCallback bcb)
        {
            if (boltRoutine == null)
            {
                boltRoutine = boltCallback.StartCoroutine(boltFunction(bcb));
            }
        }
        internal void bcb_mouseExit(BoltCallback bcb)
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
