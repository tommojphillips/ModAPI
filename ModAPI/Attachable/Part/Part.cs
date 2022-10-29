using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TommoJProductions.ModApi.Attachable.CallBacks;
using static MSCLoader.ModConsole;
using static UnityEngine.GUILayout;
using MSCLoader;
using TanjentOGG;
using HutongGames.PlayMaker.Actions;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents an installable and boltable part.
    /// </summary>
    public class Part : MonoBehaviour
    {
        // Written, 27.10.2018 | Updated, 09.2021

        #region Events

        /// <summary>
        /// Represents the on assemble event.invoked after assembly logic executes
        /// </summary>
        public event Action onAssemble;
        /// <summary>
        /// Represents the on disassemble event. invoked after disassembly logic executes
        /// </summary>
        public event Action onDisassemble;
        /// <summary>
        /// Represents the on pre assemble event. invoked before assembly logic executes
        /// </summary>
        public event Action onPreAssemble;
        /// <summary>
        /// Represents the on pre disassemble event. invoked before disassembly logic executes
        /// </summary>
        public event Action onPreDisassemble;
        /// <summary>
        /// Represents the on pre init part event. invoked after fields are assigned but before init part logic has run.
        /// </summary>
        public event Action onPreInitPart;
        /// <summary>
        /// Represents the on post init part event. invoked after init part logic has run.
        /// </summary>
        public event Action onPostInitPart;
        /// <summary>
        /// Represents the on part bolted. invoked when the part has been completely bolted..
        /// </summary>
        public event Action onPartBolted;
        /// <summary>
        /// Represents the on part unbolted. invoked when the part has been completely unbolted.
        /// </summary>
        public event Action onPartUnBolted;
        /// <summary>
        /// Represents the on bolt screwed. occurs when any of the bolts have been screwed (up or down).
        /// </summary>
        public event Action onBoltScrew;
        /// <summary>
        /// Represents the on part picked up event. occurs when this part has been picked up.
        /// </summary>
        public event Action onPartPickUp;
        /// <summary>
        /// Represents the on part thrown event. occurs when this part has been thrown.
        /// </summary>
        public event Action onPartThrow;
        /// <summary>
        /// Represents the on part dropped event. occurs when this part has been dropped.
        /// </summary>
        public event Action onPartDrop;
        /// <summary>
        /// Represents the on joint break event. occurs when this parts Joint breaks. invoked after disassembly logic executes.
        /// </summary>
        public event Action onJointBreak;

        #endregion

        #region Fields / Properties

        private PartSaveInfo _defaultSaveInfo = default;

        /// <summary>
        /// Represents the default save info for this part.
        /// </summary>
        public PartSaveInfo defaultSaveInfo
        {
            get => _defaultSaveInfo;
            set => _defaultSaveInfo = new PartSaveInfo(value);
        }
        /// <summary>
        /// Represents the runtime id of this part.
        /// </summary>
        public string PartID { get; private set; }
        /// <summary>
        /// Represents if the part is installed or not.
        /// </summary>
        public bool installed
        {
            get => loadedSaveInfo.installed;
            private set => loadedSaveInfo.installed = value;
        }
        /// <summary>
        /// Represents if the part is completely bolted or not.
        /// </summary>
        public bool bolted
        {
            get => loadedSaveInfo.bolted;
            private set => loadedSaveInfo.bolted = value;
        }
        /// <summary>
        /// Represents if mouse over activated.
        /// </summary>
        public bool mouseOver { get; private set; } = false;
        /// <summary>
        /// Represents if this part has any bolts!
        /// </summary>
        public bool hasBolts { get; private set; } = false;
        /// <summary>
        /// Represents the current install point index that this part is installed to.
        /// </summary>
        public int installPointIndex
        {
            get => loadedSaveInfo.installedPointIndex;
            private set => loadedSaveInfo.installedPointIndex = value;
        }
        /// <summary>
        /// Represents the cached mass of the parts rigidbody.mass property.
        /// </summary>
        public float cachedMass { get; private set; }
        /// <summary>
        /// Represents the  tightness step. calculated in <see cref="setupBoltTightnessVariables"/>. how much the breakforce of the install point joint increases per bolt screw.
        /// </summary>
        public float tightnessStep { get; private set; }
        /// <summary>
        /// Represents the bolt parent. all bolts will be a child of this game object. NOTE null if no bolts are on this part or the part has bolts but none of them are relevant to the part alone. bolts could be assigned to a trigger.
        /// </summary>
        public GameObject boltParent { get; private set; } = null;
        /// <summary>
        /// Represents the cached rigidbody.
        /// </summary>
        public Rigidbody cachedRigidBody { get; private set; }
        /// <summary>
        /// Represents the fixed joint when using <see cref="AssembleType.joint"/> and installed.
        /// </summary>
        public FixedJoint joint { get; private set; }
        /// <summary>
        /// Represents all triggers; install points for this part.
        /// </summary>
        public Trigger[] triggers { get; private set; }
        /// <summary>
        /// Represents all bolts for the part.
        /// </summary>
        public Bolt[] bolts { get; private set; }
        /// <summary>
        /// Represents the parts settings.
        /// </summary>
        public PartSettings partSettings { get; private set; }
        /// <summary>
        /// Represents loaded save info.
        /// </summary>
        public PartSaveInfo loadedSaveInfo { get; private set; }
        /// <summary>
        /// Represents max tightness of all bolts.(<see cref="BoltSettings.maxTightness"/> * <see cref="bolts"/>.Length). if any present on part. (<see cref="hasBolts"/>)
        /// </summary>
        public float maxTightnessTotal { get; private set; } = 0;
        /// <summary>
        /// Represents tightness of all bolts (<see cref="BoltSaveInfo.boltTightness"/> * <see cref="bolts"/>.Length). if any present on part. (<see cref="hasBolts"/>)
        /// </summary>
        public float tightnessTotal { get; private set; } = 0;
        /// <summary>
        /// Represents if the part is in a trigger.
        /// </summary>
        public bool inTrigger { get; private set; } = false;

        /// <summary>
        /// Represents the trigger routine.
        /// </summary>
        internal Coroutine triggerRoutine;
        /// <summary>
        /// Represents the install point colliers (just a list of <see cref="Trigger.triggerCollider"/> in order).
        /// </summary>
        internal Collider[] installPointColliders;

        /// <summary>
        /// Represents if part has been initialized (<see cref="initPart(PartSaveInfo, PartSettings, Trigger[])"/> invoked
        /// </summary>
        public bool initialized { get; private set; } = false;
        /// <summary>
        /// Represents if bolts have been initialized.
        /// </summary>
        public bool boltsInitialized { get; private set; } = false;

        /// <summary>
        /// Represents if this part is inherently picked up. (is a child of a part that is currently picked up)
        /// </summary>
        public bool inherentlyPickedUp { get; internal set; }
        /// <summary>
        /// Represents if this part is currently picked up.
        /// </summary>
        public bool pickedUp { get; internal set; }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Represents the joint break runtime call
        /// </summary>
        protected virtual void OnJointBreak(float breakForce)
        {
            disassemble();
            onJointBreak?.Invoke();
        }

        #endregion

        #region IEnumerators

        /// <summary>
        /// Represents the part in a trigger logic.
        /// </summary>
        /// <param name="callback_ref">The trigger callback that invoked this call.</param>
        public virtual IEnumerator partInTrigger(TriggerCallback callback_ref)
        {
            // Written, undocumented | Modified, 02.07.2022

            while (inTrigger && holdingCheck(callback_ref))
            {                
                yield return null;
                ModClient.guiAssemble = true;
                if (Input.GetMouseButtonDown(0))
                {
                    assemble(callback_ref.callbackCollider);
                    break;
                }
            }
            ModClient.guiAssemble = false;
            stopTriggerRoutine();
        }
        /// <summary>
        /// installs this part to the install point.
        /// </summary>
        /// <param name="installPoint">The install point</param>
        /// <param name="playSound">Play assemble sound?</param>
        public virtual IEnumerator assembleFunction(Collider installPoint, bool playSound = true)
        {
            // Written, 10.08.2018 | Modified, 25.09.2021 | 09.06.2022

            onPreAssemble?.Invoke();
            installPointIndex = Array.IndexOf(installPointColliders, installPoint);
            triggers[installPointIndex].triggerCallback.part = this;
            triggers[installPointIndex].invokePreAssembledEvent();
            installed = true;
            installPoint.enabled = false;
            transform.parent = triggers[installPointIndex].partPivot.transform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            if (boltsInitialized && hasBolts)
            {
                activateBolts(bolt => bolt.boltSettings.parentBoltToTrigger ? bolt.boltSettings.parentBoltToTriggerIndex == installPointIndex : true);
                updateTotalBoltTightness();
            }
            enableColliderAttachedToTrigger(false);
            switch (partSettings.assembleType)
            {
                case AssembleType.static_rigidbodyDelete:
                    if (cachedRigidBody)
                    {
                        cachedMass = cachedRigidBody.mass;
                        Destroy(cachedRigidBody);
                        yield return null;
                    }
                    break;
                case AssembleType.static_rigidibodySetKinematic:
                    cachedRigidBody.isKinematic = true;
                    break;
                case AssembleType.joint:
                    joint = gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = (partSettings.assemblyTypeJointSettings.installPointRigidbodies.Length > 0 ? partSettings.assemblyTypeJointSettings.installPointRigidbodies[installPointIndex] : installPointColliders[installPointIndex].transform.GetComponentInParent<Rigidbody>()) ?? throw new Exception($"[Assemble.{name}] (Joint) Error assigning connected body. could not find a rigidbody in parent. assign a rigidbody manually at 'partSettings.assemblyTypeJointSettings.installPointRigidbodies[installPointIndex]'");
                    updateJointBreakForce();
                    break;
            }
            makePartPickable(false);

            if (playSound)
            {
                ModClient.playSoundAt(transform, "CarBuilding", "assemble");
            }
            triggers[installPointIndex].invokeAssembledEvent();
            onAssemble?.Invoke();
        }
        /// <summary>
        /// Disassemble this part from the installed point
        /// </summary>
        public virtual IEnumerator disassembleFunction()
        {
            // Written, 10.08.2018 | Modified, 25.09.2021 | 09.06.2022

            onPreDisassemble?.Invoke();
            triggers[installPointIndex].invokePreDisassembledEvent();
            triggers[installPointIndex].triggerCallback.part = null;
            installed = false;
            if (mouseOver)
            {
                mouseOverGuiDisassembleEnable(false);
            }

            transform.parent = null;

            if (hasBolts)
            {
                resetBoltTightness();
                activateBolts(false);
            }
            enableColliderAttachedToTrigger(true);

            switch (partSettings.assembleType)
            {
                case AssembleType.static_rigidbodyDelete:
                    if (!cachedRigidBody)
                    {
                        cachedRigidBody = gameObject.AddComponent<Rigidbody>();
                    }
                    cachedRigidBody.mass = cachedMass;
                    break;
                case AssembleType.static_rigidibodySetKinematic:
                    cachedRigidBody.isKinematic = false;
                    break;
                case AssembleType.joint:
                    Destroy(joint);
                    yield return null;
                    break;
            }
            makePartPickable(true);

            ModClient.playSoundAt(transform, "CarBuilding", "disassemble");
            
            triggers[installPointIndex].invokeDisassembledEvent();
            onDisassemble?.Invoke();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// removes this instance from <see cref="ModClient.loadedParts"/>.
        /// </summary>
        ~Part()
        {
            ModClient.loadedParts.Remove(this);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// invoked when this part enters one of its triggers.
        /// </summary>
        /// <param name="callback_ref">the trigger callback that invoked this.</param>
        protected internal virtual void onTriggerEnter(TriggerCallback callback_ref)
        {
            // Written, 04.10.2018 | Modified, 02.07.2022

            if (colliderCheck(callback_ref) && holdingCheck(callback_ref))
            {
                if (triggerRoutine == null)
                {
                    inTrigger = true;
                    triggerRoutine = StartCoroutine(partInTrigger(callback_ref));
                }
            }
        }
        /// <summary>
        /// invoked when this part exits one of its triggers.
        /// </summary>
        /// <param name="callback_ref">the trigger callback that invoked this.</param>
        protected internal virtual void onTriggerExit(TriggerCallback callback_ref)
        {
            // Written, 14.08.2018 | updated 18.09.2021

            if (colliderCheck(callback_ref))
            {
                if (triggerRoutine != null)
                {
                    stopTriggerRoutine();
                }
                ModClient.guiAssemble = false;
            }
        }
        /// <summary>
        /// invoked when any bolt on this part is screwed in or out.
        /// </summary>
        protected internal virtual void boltOnScrew()
        {
            // Written, 08.07.2022

            updateTotalBoltTightness();
            if (!float.IsPositiveInfinity(partSettings.assemblyTypeJointSettings.breakForce))
            {
                updateJointBreakForce();
            }
            boltTightnessCheck();
            onBoltScrew?.Invoke();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if modapi is set up. if not set up, invokes <see cref="ModApiLoader.loadModApi()"/>
        /// </summary>
        public static void modApiSetupCheck() 
        {
            // Written, 03.07.2022

            if (!ModClient.modApiSetUp)
            {
                ModApiLoader.loadModApi();
            }
        }

        /// <summary>
        /// Initializes this part.
        /// </summary>
        /// <param name="saveInfo">The save info to load this part with.</param>
        /// <param name="partSettingsRef">The part settings to load this part with.</param>
        /// <param name="triggerRefs">The Install points for this part. (in the form of a trigger)</param>
        public virtual void initPart(PartSaveInfo saveInfo, PartSettings partSettingsRef = default, params Trigger[] triggerRefs)
        {
            // Written, 08.09.2021

            modApiSetupCheck();
            vaildiatePart(partSettingsRef);

            PartID = "part" + ModClient.loadedParts.Count.ToString("000");
            partSettings = new PartSettings(partSettingsRef);
            triggers = triggerRefs;
            addInstanceToTriggers();
            cachedRigidBody = GetComponent<Rigidbody>();
            cachedMass = cachedRigidBody?.mass ?? 1;
            loadedSaveInfo = new PartSaveInfo(saveInfo ?? defaultSaveInfo);
            makePartPickable(!installed);

            onPreInitPart?.Invoke();

            if (triggers != null && triggers.Length > 0)
            {
                installPointColliders = new Collider[triggers.Length];

                for (int i = 0; i < triggers.Length; i++)
                {
                    installPointColliders[i] = triggers[i].triggerCollider;
                }
                if (installed)
                {
                    assemble(installPointColliders[installPointIndex], false, true);
                }
            }
            if (!installed && partSettings.setPositionRotationOnInitialisePart)
            {
                transform.position = loadedSaveInfo.position;
                transform.eulerAngles = loadedSaveInfo.rotation;
            }
            if (partSettings.setPhysicsMaterialOnInitialisePart)
            {
                Collider[] colliders;
                switch (partSettings.collisionSettings.physicMaterialType)
                {
                    case CollisionSettings.PhysicMaterialType.setOnAllFoundColliders:
                        colliders = GetComponents<Collider>().Where(col => !col.isTrigger).ToArray();
                        break;
                    default:
                    case CollisionSettings.PhysicMaterialType.setOnProvidedColliders:
                        colliders = partSettings.collisionSettings.providedColliders;
                        break;
                }
                foreach (Collider collider in colliders)
                {
                    collider.material = partSettings.collisionSettings.physicMaterial;
                }
            }
            ModClient.loadedParts.Add(this);
            initialized = true;
            onPostInitPart?.Invoke();
        }
        /// <summary>
        /// Initializes this part with bolts
        /// </summary>
        /// <param name="saveInfo">The save info to load this part and bolts with.</param>
        /// <param name="partSettingsRef">The part settings to load this part with.</param>
        /// <param name="boltRefs">The bolts for this part</param>
        /// <param name="triggerRefs">The Install points for this part. (in the form of a trigger)</param>
        public virtual void initPart(PartSaveInfo saveInfo, PartSettings partSettingsRef = default, Bolt[] boltRefs = null, params Trigger[] triggerRefs)
        {
            // Written, 03.07.2022

            initPart(saveInfo, partSettingsRef, triggerRefs);
            initBolts(boltRefs, saveInfo?.boltSaveInfos);
        }
        /// <summary>
        /// Initializes all part bolts.
        /// </summary>
        /// <param name="boltRefs">The bolt refs to init</param>
        /// <param name="infos">The save infos for the bolts.</param>
        protected virtual void initBolts(Bolt[] boltRefs, BoltSaveInfo[] infos)
        {
            // Written, 03.07.2022

            if (!vaildiateBolts(boltRefs))
                return;

            hasBolts = true;
            bolts = boltRefs;
            createBoltParentGameObject();

            for (int i = 0; i < bolts.Length; i++)
            {
                if (infos != null)
                    bolts[i].initBoltOnPart(this, infos[i]);
                else
                    bolts[i].initBoltOnPart(this);
            }
            postVaildiateBolts();
            updateTotalBoltTightness();
            if (!installed)
            {
                activateBolts(bolt => bolt.boltSettings.activeWhenUninstalled);
            }
            else
            {
                boltTightnessCheck();
                updateJointBreakForce();
            }
            boltsInitialized = true;
        }

        /// <summary>
        /// Starts the assemble coroutine. (<see cref="assembleFunction(Collider, bool)"/>). async or sync
        /// </summary>
        /// <param name="runSynchronously">Runs the assemble function synchronously.</param>
        /// <param name="installPoint">the install point that this part is assemble-ing to. Note: <paramref name="installPoint"/> must be in <see cref="installPointColliders"/></param>
        /// <param name="playSound">Play disassemble sound?</param>
        public virtual void assemble(Collider installPoint, bool playSound = true, bool runSynchronously = false)
        {
            // Written, 11.07.2022

            if (runSynchronously)
                assembleFunction(installPoint, playSound).waitCoroutine();
            else
                StartCoroutine(assembleFunction(installPoint, playSound));
        }
        /// <summary>
        /// Starts the disassemble coroutine. (<see cref="disassembleFunction"/>). async or sync
        /// </summary>
        /// <param name="runSynchronously">Runs the disassemble function synchronously.</param>
        public virtual void disassemble(bool runSynchronously = false)
        {
            // Written, 11.07.2022

            if (runSynchronously)
                disassembleFunction().waitCoroutine();
            else
                StartCoroutine(disassembleFunction());
        }

        /// <summary>
        /// Gets save info for this part and its bolts (if the part has any). (pos, rot, installed, install index, tightness)
        /// </summary>
        public virtual PartSaveInfo getSaveInfo()
        {
            // Written, 18.10.2018 | Modified, 11.07.2022

            PartSaveInfo info = new PartSaveInfo(loadedSaveInfo);
            if (partSettings.setPositionRotationOnInitialisePart)
            {
                info.position = transform.position;
                info.rotation = transform.eulerAngles;
            }
            else
            {
                info.position = Vector3.zero;
                info.rotation = Vector3.zero;
            }
            info.boltSaveInfos = getBoltSaveInfo();
            return info;
        }
        /// <summary>
        /// Gets bolts save info 
        /// </summary>
        /// <returns></returns>
        public virtual BoltSaveInfo[] getBoltSaveInfo() 
        {
            // Written, 27.08.2022

            BoltSaveInfo[] boltSaveInfos = null;

            if (hasBolts)
            {
                boltSaveInfos = new BoltSaveInfo[bolts.Length];
                for (int i = 0; i < bolts.Length; i++)
                {
                    boltSaveInfos[i] = bolts[i].loadedSaveInfo;
                }
            }
            return boltSaveInfos;
        }
  
        /// <summary>
        /// Sets the triggers collider enable (install point) from <paramref name="index"/> to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        /// <param name="index">The idex ofg the trigger to active or not.</param>
        public virtual void enableColliderTrigger(bool active, int index)
        {
            // Written, 18.09.2021 | Modified, 23.10.2021

            installPointColliders[index].enabled = active;
        }
        /// <summary>
        /// Sets the triggers collider enable  (install points) that the part is currently installed to <paramref name="active"/>. (<see cref="installPointIndex"/>)
        /// </summary>
        /// <param name="active">active or not [part]</param>
        public virtual void enableColliderAttachedToTrigger(bool active)
        {
            // Written, 18.09.2021 | Modified, 23.10.2021

            enableColliderTrigger(active, installPointIndex);
        }

        /// <summary>
        /// Sets the triggers disasemble logic enable (install point) from <paramref name="index"/> to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        /// <param name="index">The idex ofg the trigger to active or not.</param>
        public virtual void enableDissembleLogicActiveTrigger(bool active, int index)
        {
            // Written, 18.09.2021 | Modified, 23.10.2021

            triggers[index].triggerCallback.disassembleLogicEnabled = active;
        }
        /// <summary>
        /// Sets the triggers disasemble logic enable (install points) that the part is currently installed to <paramref name="active"/>. (<see cref="installPointIndex"/>)
        /// </summary>
        /// <param name="active">active or not [part]</param>
        public virtual void enableDisassembleLogicAttachedToTrigger(bool active)
        {
            // Written, 18.09.2021 | Modified, 23.10.2021

            enableDissembleLogicActiveTrigger(active, installPointIndex);
        }

        /// <summary>
        /// Makes the part a pickable item depending on the provided values.
        /// </summary>
        /// <param name="inPickable">Make part pickable?</param>
        public void makePartPickable(bool inPickable)
        {
            // Written, 14.08.2018 | Modified, 30.09.2021 | Modified, 04.06.2022

            if (inPickable)
            {
                gameObject.tag = "PART";
                gameObject.layer = partSettings.notInstalledPartToLayer.layer();

                if (cachedRigidBody)
                    cachedRigidBody.collisionDetectionMode = partSettings.collisionSettings.notInstalledCollisionDetectionMode;
            }
            else
            {
                gameObject.tag = "DontPickThis";
                gameObject.layer = partSettings.installedPartToLayer.layer();

                if (cachedRigidBody)
                    cachedRigidBody.collisionDetectionMode = partSettings.collisionSettings.installedCollisionDetectionMode;
            }
        }

        /// <summary>
        /// ends or starts a disassembly gui interaction.
        /// </summary>
        public void mouseOverGuiDisassembleEnable(bool enable)
        {
            // Written, 04.10.2018 | Modified, 18.09.2021

            mouseOver = enable;
            ModClient.guiDisassemble = enable;
        }         
                       
        /// <summary>
        /// Vaildates the part and reports to mod console. called at: <see cref="initPart(PartSaveInfo, PartSettings, Trigger[])"/>.
        /// </summary>
        protected virtual void vaildiatePart(PartSettings partSettings)
        {
            // Written, 25.09.2021

            string e = String.Empty;

            if (initialized)
            {
                e += $"error: Part, {name} is already initialized\n";
            }
            if (!ModClient.boltAssetsLoaded)
            {
                e += $"- Bolt assets aren't loaded. refused to initialize part, {name}\n";
            }
            if (!ModClient.modApiSetUp)
            {
                e += $"- mod api isnt setup. refused to initialize part, {name}\n";
            }
            if (partSettings.assembleType == AssembleType.joint && (partSettings.assemblyTypeJointSettings?.installPointRigidbodies?.Length ?? -1) <= 0)
            {
                e += "- Invaild part assembly setup. assembly type 'joint' but no install point rigidbodies assigned.\n";               
            }
            if (e != String.Empty)
            {
                Error(e);
                throw new Exception(e);
            }
        }
        /// <summary>
        /// Vaildiates bolt refs. called prior to initialization of bolts on the part. invoked in <see cref="initBolts(Bolt[], BoltSaveInfo[])"/>
        /// </summary>
        /// <param name="boltRefs">the bolt refs to vaildiate.</param>
        /// <returns>True if no issues were detected with the bolt references.</returns>
        protected virtual bool vaildiateBolts(Bolt[] boltRefs)
        {
            // Written, 08.10.2022

            string error = String.Empty;

            if (boltRefs != null)
            {
                if (boltRefs.Length == 0)
                {
                    error += "initBolts was called but no bolts were found (array length was zero)\n";
                }
            }
            else
            {
                error += "initBolts was called but no bolts were found (boltRef array was null)\n";
            }

            if (error != String.Empty)
            {
                Error($"Error initialializing Part, {name}. {error}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Vaildiates bolts after bolts have been initialized.
        /// </summary>
        /// <returns>returns true if no errors were detected with the bolts.</returns>
        /// <exception cref="Exception">Throws an exception is any issues were detected. to prevent part from initializing.</exception>
        protected virtual bool postVaildiateBolts()
        {
            // Written, 08.10.2022

            string error = String.Empty;

            for (int i = 0; i < bolts.Length; i++)
            {
                if (bolts[i].boltSettings.boltSize == BoltSize.hand)
                {
                    if (!bolts[i].boltModel.isOnLayer(LayerMasksEnum.Parts) || (!bolts[i].addNutModel?.isOnLayer(LayerMasksEnum.Parts) ?? false))
                    {
                        error += $"bolt ref ({i}) has a bolt size of type 'hand' but is not on the layer 'Parts' Player will not be able to interact with this bolt/add nut.\n";
                    }
                }
                else
                {
                    if (!bolts[i].boltModel.isOnLayer(LayerMasksEnum.Bolts) || (!bolts[i].addNutModel?.isOnLayer(LayerMasksEnum.Bolts) ?? false))
                    {
                        error += $"bolt ref ({i}) has a bolt size of a type other than 'hand' but is not on the layer 'Bolts' Player will not be able to interact with this bolt/add nut.\n";
                    }
                }
            }

            if (error != String.Empty)
            {
                error = $"Error initialializing Part, {name}. {error}";
                Error(error);
                throw new Exception(error);
            }
            return true;
        }

        /// <summary>
        /// Check for deciding if part is in trigger to be installed
        /// </summary>
        /// <param name="callback">the callback that triggered this.</param>
        protected virtual bool colliderCheck(TriggerCallback callback)
        {
            // Written, 09.06.2022

            if (!partSettings.assembleCollider)
                return true;
            return callback.callbackCollider.gameObject == partSettings.assembleCollider.gameObject;
        }
        /// <summary>
        /// Represents the holding check for a <see cref="Part"/>. Used for starting and checking trigger routine. <see cref="partInTrigger(TriggerCallback)"/>
        /// </summary>
        /// <param name="callback_ref">The trigger callback</param>
        protected virtual bool holdingCheck(TriggerCallback callback_ref) 
        {
            // Written, 02.07.2022

            return pickedUp || (partSettings.installEitherDirection && (callback_ref.callbackCollider.gameObject.getBehaviourInParent<Part>(part => !part.installed)?.pickedUp ?? false));
        }

        /// <summary>
        /// Updates <see cref="tightnessTotal"/> from current state of <see cref="bolts"/>. see: <see cref="BoltSaveInfo.boltTightness"/> and <see cref="BoltSaveInfo.addNutTightness"/>
        /// </summary>
        protected virtual void updateTotalBoltTightness() 
        {
            // Written, 10.07.2022

            tightnessTotal = 0;
            maxTightnessTotal = 0;

            foreach (Bolt b in bolts)
            {
                if (b.boltSettings.parentBoltToTrigger && b.part.installPointIndex != b.boltSettings.parentBoltToTriggerIndex)
                {
                    continue;
                }

                tightnessTotal += b.loadedSaveInfo.boltTightness;
                if (b.boltSettings.addNut)
                {
                    tightnessTotal += b.loadedSaveInfo.addNutTightness;
                }

                maxTightnessTotal += b.boltSettings.maxTightness;
                if (b.boltSettings.addNut)
                {
                    maxTightnessTotal += b.boltSettings.maxTightness;
                }
            }
        }
        /// <summary>
        /// checks all bolts for their <see cref="Bolt.boltSettings"/>.<see cref="BoltSaveInfo.boltTightness"/>.
        /// invokes, <see cref="enableDisassembleLogicAttachedToTrigger(bool)"/>, 
        /// sets <see cref="bolted"/> state. and
        /// invokes <see cref="onPartBolted"/>, <see cref="onPartUnBolted"/> events.
        /// </summary>
        protected virtual void boltTightnessCheck() 
        {
            // Written, 08.07.2022

            if (tightnessTotal >= maxTightnessTotal * Mathf.Clamp(partSettings.tightnessThreshold, 0.25f, 1))
            {
                enableDisassembleLogicAttachedToTrigger(false);
                mouseOverGuiDisassembleEnable(false);
            }
            if (tightnessTotal == maxTightnessTotal)
            {
                bolted = true;
                onPartBolted?.Invoke();
            }
            else if (bolted && tightnessTotal < maxTightnessTotal)
            {
                bolted = false;
            }
            else if (tightnessTotal == 0)
            {
                enableDisassembleLogicAttachedToTrigger(true);
                onPartUnBolted?.Invoke();
            }
        }
        /// <summary>
        /// resets all bolts tightness on this part
        /// </summary>
        public void resetBoltTightness() 
        {
            // Written, 25.08.2022

            setBoltTightness(bolt => bolt.resetTightness());
            updateJointBreakForce();
        }
        /// <summary>
        /// sets all bolt tightness to the max tightness.
        /// </summary>
        public void setMaxBoltTightness()
        {
            // Written, 25.08.2022

            setBoltTightness(bolt => bolt.setMaxTightness());
            updateJointBreakForce();
        }
        /// <summary>
        /// Calls the <paramref name="func"/> on each bolt and updates bolt model, part total tightness and executes <see cref="boltTightnessCheck"/>
        /// </summary>
        /// <param name="func">the action to execute on each bolt</param>
        private void setBoltTightness(Action<Bolt> func)
        {
            // Written, 25.08.2022

            for (int i = 0; i < bolts.Length; i++)
            {
                func(bolts[i]);
                bolts[i].updateModelPosition();
            }
            updateTotalBoltTightness();
            boltTightnessCheck();
        }

        /// <summary>
        /// Updates joint breakforce. (bolts)
        /// </summary>
        internal void updateJointBreakForce()
        {
            // Written, 08.07.2022

            if (joint)
            {
                if (hasBolts && partSettings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce)
                {
                    tightnessStep = (partSettings.assemblyTypeJointSettings.breakForce - partSettings.assemblyTypeJointSettings.breakForceMin) / maxTightnessTotal;
                    joint.breakForce = tightnessStep * tightnessTotal + partSettings.assemblyTypeJointSettings.breakForceMin;
                }
                else
                {
                    joint.breakForce = partSettings.assemblyTypeJointSettings.breakForce;
                }
                joint.breakTorque = joint.breakForce / 2;
            }
        }
        
        /// <summary>
        /// invokes the picked up event
        /// </summary>
        internal void invokePickedUpEvent()
        {
            // Written, 11.07.2022

            onPartPickUp?.Invoke();
        }
        /// <summary>
        /// invokes the thrown event.
        /// </summary>
        internal void invokeThrownEvent()
        {
            // Written, 11.07.2022

            onPartThrow?.Invoke();
        }
        /// <summary>
        /// invokes the dropped event.
        /// </summary>
        internal void invokeDroppedEvent()
        {
            // Written, 11.07.2022

            onPartDrop?.Invoke();
        }

        /// <summary>
        /// Adds this part instance to each trigger in <see cref="triggers"/>.
        /// </summary>
        private void addInstanceToTriggers() 
        {
            // Written, 02.07.2022

            foreach (Trigger trigger in triggers)
            {
                if (!trigger.parts.Contains(this))
                {
                    trigger.parts.Add(this);
                }
            }
        }
        /// <summary>
        /// Creates and assigns <see cref="boltParent"/> Bolt parent.
        /// </summary>
        private void createBoltParentGameObject()
        {
            // Written, 03.07.2022 | Modified, 29.10.2022

            if (bolts.Any(bolt => !bolt.boltSettings.parentBoltToTrigger))
            {
                boltParent = new GameObject("Bolts");
                boltParent.transform.SetParent(transform);
                boltParent.transform.localPosition = Vector3.zero;
                boltParent.transform.localEulerAngles = Vector3.zero;
                //boltParent.transform.localScale = Vector3.one;
            }
            if (bolts.Any(bolt => bolt.boltSettings.parentBoltToTrigger))
            {
                Trigger trigger;
                for (int i = 0; i < bolts.Length; i++)
                {
                    trigger = triggers[bolts[i].boltSettings.parentBoltToTriggerIndex];
                    if (!trigger.boltParent)
                    {
                        trigger.boltParent = new GameObject("Bolts");
                        trigger.boltParent.transform.SetParent(trigger.triggerGameObject.transform);
                        trigger.boltParent.transform.localPosition = Vector3.zero;
                        trigger.boltParent.transform.localEulerAngles = Vector3.zero;
                        //trigger.boltParent.transform.localScale = Vector3.one;
                    }
                }
            }
        }
        /// <summary>
        /// Stops the trigger coroutine and sets <see cref="inTrigger"/> to false. brute force
        /// </summary>
        private void stopTriggerRoutine()
        {
            // Written, 10.07.2022

            inTrigger = false;
            StopCoroutine(triggerRoutine);
            triggerRoutine = null;
        }

        private void activateBolts(bool active)
        {
            // Written, 28.10.2022

            activateBolts(bolt => active);
        }
        private void activateBolts(Func<Bolt, bool> func)
        {
            // Written, 28.10.2022

            bool active;

            for (int i = 0; i < bolts.Length; i++)
            {
                active = func.Invoke(bolts[i]);
                if (!bolts[i].boltSettings.activeWhenUninstalled || active)
                {
                    bolts[i].boltModel.SetActive(active);
                    if (bolts[i].boltSettings.addNut)
                    {
                        bolts[i].addNutModel.SetActive(active);
                    }
                }
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets the gameobject of the part.
        /// </summary>
        /// <param name="p"></param>
        public static implicit operator GameObject(Part p) => p.gameObject;

        #endregion
    }
}
