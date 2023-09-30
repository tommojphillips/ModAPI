using System;
using System.Collections;

using HutongGames.PlayMaker;
using TommoJProductions.ModApi.YieldInstructions;
using UnityEngine;

using static MSCLoader.ModConsole;
using static UnityEngine.Object;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents a bolt for a <see cref="Part"/>.
    /// </summary>
    [Obsolete]
    public class OldBolt
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
        /// Returns true if this bolt is tight (<see cref="BoltSaveInfo.boltTightness"/> == 8)
        /// </summary>
        public bool isTight => loadedSaveInfo.boltTightness >= 8;
        /// <summary>
        /// Returns true if this bolt is loose (<see cref="BoltSaveInfo.boltTightness"/> == 0)
        /// </summary>
        public bool isLoose => loadedSaveInfo.boltTightness <= 0;
        /// <summary>
        /// Represennts the runtime ID for this bolt. Note Runtime.. could change next game load.
        /// </summary>
        public string boltID { get; private set; }

        internal Trigger trigger;

        /// <summary>
        /// Represents settings for this bolt.
        /// </summary>
        public BoltWithNutSettings boltSettings { get; private set; }
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
        /// The add nut model. relevant only when <see cref="Bolt.settings"/>.<see cref="BoltWithNutSettings.addNut"/> is <see langword="true"/>.
        /// </summary>
        public GameObject addNutModel { get; private set; }


        /// <summary>
        /// Represents the part that this bolt is linked to.
        /// </summary>
        public Part part { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialies this bolt. save info will be gathered from the part.
        /// </summary>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public OldBolt(BoltWithNutSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default)
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
        public OldBolt(BoltSaveInfo saveInfo = null, BoltWithNutSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default)
        {
            init(saveInfo, settings, position, eulerAngles);
        }
        /// <summary>
        /// Initialies this bolt with an add nut offset override. save info will be gathered from the part. (only vaild to bolts that have the <see cref="BoltWithNutSettings.addNut"/> setting on)
        /// </summary>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        /// <param name="addNutOffsetOverride">add nut offset override float. ignored if null.</param>
        public OldBolt(BoltWithNutSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default, float? addNutOffsetOverride = null)
        {
            init(null, settings, position, eulerAngles, addNutOffsetOverride);
        }
        /// <summary>
        /// Initialies this bolt with save info with an add nut offset override. (only vaild to bolts that have the <see cref="BoltWithNutSettings.addNut"/> setting on).
        /// </summary>
        /// <param name="saveInfo">The save info for the bolt.</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        /// <param name="addNutOffsetOverride">add nut offset override float. ignored if null.</param>
        public OldBolt(BoltSaveInfo saveInfo = null, BoltWithNutSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default, float? addNutOffsetOverride = null)
        {
            init(saveInfo, settings, position, eulerAngles, addNutOffsetOverride);
        }

        #endregion

        #region IEnumerators

        internal IEnumerator boltFunction(BoltCallback bcb)
        {
            // Written, 03.07.2022

            float scrollInput;
            float scrollOutput;
            float wait;
            FsmBool hasRatchet = ModClient.getPlayerHasRatchet;
            FsmBool ratchetSwitch = ModClient.getRatchetSwitch;

            if (hasRatchet.Value)
            {
                if (!boltSettings.canUseRachet)
                {
                    yield break;
                }
                wait = ModClient.getRachetBoltingSpeed;
            }
            else
            {
                wait = ModClient.getSpannerBoltingSpeed;
            }

            while (bcb.boltCheck)
            {
                scrollInput = Input.mouseScrollDelta.y;

                if (scrollInput != 0)
                {
                    if (hasRatchet.Value)
                    {
                        int direction = ratchetSwitch.Value ? 1 : -1;
                        scrollOutput = Mathf.Abs(scrollInput) * direction;
                    }
                    else
                    {
                        scrollOutput = scrollInput;
                    }

                    boltTightnessTemp = (int)Mathf.Clamp(loadedSaveInfo.boltTightness + (scrollOutput * 1), 0, 8);
                    nutTightnessTemp = (int)Mathf.Clamp(loadedSaveInfo.addNutTightness + (scrollOutput * 1), 0, 8);

                    if (bcb == boltCallback && (/*!boltSettings.addNut || */loadedSaveInfo.addNutTightness == 0))
                    {
                        isNut = false;
                        foundBolt = true;
                        tightnessChanged = boltTightnessTemp != loadedSaveInfo.boltTightness;
                    }
                    else if (/*boltSettings.addNut && */bcb == addNutCallback && loadedSaveInfo.boltTightness == 8)
                    {
                        isNut = true;
                        foundBolt = true;
                        tightnessChanged = nutTightnessTemp != loadedSaveInfo.addNutTightness;
                    }

                    yield return new WaitForSeconds(wait);//Wa new WaitForSecondsRealTime(wait);//bcb.StartCoroutine(CustomYieldInstructions.waitForSecondsRealTime(bcb, wait));

                    if (foundBolt && tightnessChanged)
                    {
                        if (isTight)
                            outTight?.Invoke();
                        else if (isLoose)
                            outLoose?.Invoke();
                        
                        if (isNut)
                            loadedSaveInfo.addNutTightness = nutTightnessTemp;
                        else
                            loadedSaveInfo.boltTightness = boltTightnessTemp;

                        updateModelPosition();

                        if (isTight)
                            onTight?.Invoke();
                        else if (isLoose)
                            onLoose?.Invoke();

                        onScrew?.Invoke();

                        part?.boltOnScrew();

                        if (tightnessChanged)
                        { 
                            ModClient.playSoundAtInterupt(boltModel.transform, "CarBuilding", "bolt_screw");
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
        private protected virtual void init(BoltSaveInfo saveInfo, BoltWithNutSettings settings, Vector3 position, Vector3 eulerAngles, float? addNutOffsetOverride = null)
        {
            loadedSaveInfo = saveInfo;
            boltSettings = new BoltWithNutSettings(settings);
            startPosition = position;
            startEulerAngles = eulerAngles;

            if (addNutOffsetOverride != null)
            {
                settings.offset = (float)addNutOffsetOverride;
            }
        }
        /// <summary>
        /// sets up a nut on the opposite side of the bolt.
        /// </summary>
        private void addNut()
        {
            // Written, 15.07.2022

            if (boltSettings.nutSettings.customPrefab != null)
            {
                addNutModel = Instantiate(boltSettings.nutSettings.customPrefab);
            }
            else
            {
                addNutModel = Instantiate(ModClient.getBoltManager.nutPrefab);
            }
            setParent(addNutModel);
            addNutModel.name = "nut" + boltID.Replace("bolt", "");
            addNutCallback = addNutModel.AddComponent<BoltCallback>();
            //addNutCallback.bolt = this;

            BoltSize size = boltSettings.nutSettings.size;
            if (size == BoltSize.none)
            {
                addNutCallback.boltSize = boltSettings.size;
            }
            else
            {
                addNutCallback.boltSize = size;
            }
        }
        /// <summary>
        /// Initialies this bolt.
        /// </summary>
        internal void initBolt(Trigger trigger = null)
        {
            // Written, 02.07.2022

            if (ModClient.getBoltManager.assetsLoaded)
            {
                if (!boltInitialized)
                {
                    boltID = "bolt" + ModClient.loadedBolts.Count.ToString("000");
                    this.trigger = trigger;

                    setupBoltModel();
                    boltModel.SetActive(false);

                    //ModClient.loadedBolts.Add(this);
                    boltInitialized = true;
                }
            }
        }
        internal void setPart(Part p)
        {
            part = p;
        }

        private void setupBoltModel()
        {
            // Written, 29.10.2022

            if (boltSettings.customPrefab != null)
            {
                boltModel = Instantiate(boltSettings.customPrefab);
            }
            else
            {
                switch (boltSettings.type)
                {
                    case BoltType.nut:
                        boltModel = Instantiate(ModClient.getBoltManager.nutPrefab);
                        break;
                    case BoltType.screw:
                        boltModel = Instantiate(ModClient.getBoltManager.screwPrefab);
                        break;
                    case BoltType.longBolt:
                        boltModel = Instantiate(ModClient.getBoltManager.longBoltPrefab);
                        break;
                    default:
                    case BoltType.shortBolt:
                        boltModel = Instantiate(ModClient.getBoltManager.shortBoltPrefab);
                        break;
                }
            }
            setParent(boltModel);
            boltCallback = boltModel.AddComponent<BoltCallback>();
            //boltCallback.bolt = this;
            boltCallback.boltSize = boltSettings.size;
            /*if (boltSettings.addNut)
            {
                addNut();
            }*/

            boltModel.name = boltSettings.name ?? boltID;
        }

        private void setParent(GameObject child)
        {
            // Written, 29.10.2022

            if (trigger != null)
            {
                child.transform.SetParent(trigger.boltParent.transform);
            }
            else
            {
                child.transform.SetParent(part.boltParent.transform);
            }
        }

        /// <summary>
        /// Updates the models position and rotation based off <see cref="startPosition"/>, <see cref="startEulerAngles"/>,
        /// </summary>
        internal void updateModelPosition()
        {
            if (boltModel)
            {
                float boltTightness = loadedSaveInfo?.boltTightness ?? 0;
                boltModel.transform.localPosition = startPosition + positionDelta * boltTightness;
                boltModel.transform.localEulerAngles = startEulerAngles + rotationDelta * boltTightness;

                if (/*boltSettings.addNut && */addNutModel)
                {
                    Vector3 nutAbsolute = positionDelta * 8;
                    Vector3 nutPos = startPosition + (nutAbsolute * 2) + (boltSettings.offset * boltSettings.posDirection);
                    float nutTightness = loadedSaveInfo?.addNutTightness ?? 0;
                    addNutModel.transform.localPosition = nutPos - positionDelta * nutTightness;
                    addNutModel.transform.localEulerAngles = startEulerAngles - rotationDelta * nutTightness;
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
            /*if (boltSettings.addNut)
                loadedSaveInfo.addNutTightness = 0;*/
        }
        /// <summary>
        /// sets bolt and addnut tightness to 8.
        /// </summary>
        public void setMaxTightness() 
        {
            // Written, 25.08.2022

            loadedSaveInfo.boltTightness = 8;
            /*if (boltSettings.addNut)
                loadedSaveInfo.addNutTightness = 8;*/
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

        /// <summary>
        /// Vaildiates bolts after bolts have been initialized.
        /// </summary>
        /// <returns>returns true if no errors were detected with the bolts.</returns>
        /// <exception cref="Exception">Throws an exception is any issues were detected. to prevent part from initializing.</exception>
        internal static bool postVaildiateBolts(OldBolt[] bolts)
        {
            // Written, 08.10.2022

            string error = String.Empty;

            for (int i = 0; i < bolts.Length; i++)
            {
                if (bolts[i].boltSettings.size == BoltSize.hand)
                {
                    if (!bolts[i].boltModel.isOnLayer(LayerMasksEnum.Parts))
                    {
                        error += $"bolt ref ({i}) has a bolt size of type 'hand' but is not on the layer 'Parts' Player will not be able to interact with this bolt/add nut.\n";
                    }
                }
                else
                {
                    if (!bolts[i].boltModel.isOnLayer(LayerMasksEnum.Bolts))
                    {
                        error += $"bolt ref ({i}) has a bolt size of a type other than 'hand' but is not on the layer 'Bolts' Player will not be able to interact with this bolt/add nut.\n";
                    }
                }
            }

            if (error != String.Empty)
            {
                Error(error);
                throw new Exception(error);
            }
            return true;
        }
        /// <summary>
        /// Gets bolts save info 
        /// </summary>
        /// <returns>All <see cref="BoltSaveInfo"/>s in <paramref name="bolts"/></returns>
        internal static int[] getBoltSaveInfo(OldBolt[] bolts, bool hasBolts)
        {
            // Written, 27.08.2022

            if (bolts == null)
                return null;

            int[] infos = new int[bolts.Length];

            if (hasBolts)
            {
                for (int i = 0; i < bolts.Length; i++)
                {
                    infos[i] = bolts[i].loadedSaveInfo.boltTightness;
                }
            }
            return infos;
        }
        /// <summary>
        /// Creates a Bolt Parent on <paramref name="transform"/>.
        /// </summary>
        /// <returns>The Bolt Parent <see cref="GameObject"/>.</returns>
        internal static GameObject createBoltParentGameObject(Transform transform)
        {
            // Written, 03.07.2022

            GameObject boltParent = new GameObject("Bolts");
            boltParent.transform.SetParent(transform);
            boltParent.transform.localPosition = Vector3.zero;
            boltParent.transform.localEulerAngles = Vector3.zero;

            return boltParent;
        }

    }
}
