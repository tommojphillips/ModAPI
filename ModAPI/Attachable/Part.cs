using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TommoJProductions.ModApi.Attachable.CallBacks;
using static MSCLoader.ModConsole;
using static UnityEngine.GUILayout;
using static TommoJProductions.ModApi.ModClient;
using static TommoJProductions.ModApi.ModApiLoader;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents an installable part.
    /// </summary>
    public class Part : MonoBehaviour
    {
        // Written, 27.10.2018 | Updated, 09.2021

        #region Part Classes / Enums

        /// <summary>        
        /// Represents save info about a particluar part.        
        /// </summary>
        public class PartSaveInfo
        {
            // Written, 04.10.2018

            #region Properties

            /// <summary>
            /// Represents whether or not the part is installed.
            /// </summary>
            public bool installed { get; set; } = false;
            /// <summary>
            /// Represents whether or not the part is bolted.
            /// </summary>
            public bool bolted { get; set; } = false;
            /// <summary>
            /// The install point index that the part is installed to located in <see cref="Part.triggers" />.
            /// </summary>
            public int installedPointIndex { get; set; } = 0;
            /// <summary>
            /// Represents the parts world position.
            /// </summary>
            public Vector3Info position { get; set; } = Vector3.zero;
            /// <summary>
            /// Represents the parts world rotation (euler angles)
            /// </summary>
            public Vector3Info rotation { get; set; } = Vector3.zero;
            /// <summary>
            /// Represents all bolt save infos. <see langword="null"/> if part has no bolts.
            /// </summary>
            public Bolt.BoltSaveInfo[] boltSaveInfos { get; set; } = null;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of this.
            /// </summary>
            public PartSaveInfo() { }
            /// <summary>
            /// Initializes a new instance of this and assigns the parts fields to this.
            /// </summary>
            /// <param name="inPart">The part to save</param>
            public PartSaveInfo(Part inPart)
            {
                // Written, 04.10.2018

                installed = inPart.installed;
                installedPointIndex = inPart.installPointIndex;
                position = inPart.transform.position;
                rotation = inPart.transform.eulerAngles;
            }/// <summary>
             /// Initializes a new instance of this and assigns the part save info fields to this.
             /// </summary>
             /// <param name="inSave">The save info to replicate</param>
            public PartSaveInfo(PartSaveInfo inSave)
            {
                // Written, 01.05.2022

                if (inSave != null)
                {
                    installed = inSave.installed;
                    installedPointIndex = inSave.installedPointIndex;
                    position = inSave.position;
                    rotation = inSave.rotation;
                }
            }

            #endregion
        }
        /// <summary>
        /// Represents settings for the part class.
        /// </summary>
        public class PartSettings
        {
            /// <summary>
            /// Represents the physic Material Settings.
            /// </summary>
            public CollisionSettings collisionSettings = new CollisionSettings();
            /// <summary>
            /// Represents the assemble type of the part.
            /// </summary>
            public AssembleType assembleType = AssembleType.static_rigidbodyDelete;
            /// <summary>
            /// Represents '<see cref="AssembleType.joint"/>' settings.
            /// </summary>
            public AssemblyTypeJointSettings assemblyTypeJointSettings = new AssemblyTypeJointSettings() { installPointRigidbodies = null, breakForce = float.PositiveInfinity };
            /// <summary>
            /// Represents the layer to send a part that is installed
            /// </summary>
            public LayerMasksEnum installedPartToLayer = LayerMasksEnum.Parts;
            /// <summary>
            /// Represents the layer to send a part that is not installed
            /// </summary>
            public LayerMasksEnum notInstalledPartToLayer = LayerMasksEnum.Parts;
            /// <summary>
            /// Represents if <see cref="initPart(PartSaveInfo, PartSettings, Trigger[])"/> will set pos and rot of part if NOT installed.
            /// </summary>
            public bool setPositionRotationOnInitialisePart = true;
            /// <summary>
            /// Sets the parts colliders physics material if enabled.
            /// </summary>
            public bool setPhysicsMaterialOnInitialisePart = false;
            /// <summary>
            /// Represents the disassemble collider. collider must not be of IsTrigger. if null, logic uses Parts Gameobject to determine if part is being looked at. otherwise logic uses this collider to determine if part is being looked at.
            /// </summary>
            public Collider disassembleCollider = null;
            /// <summary>
            /// Represents the assemble collider. collider must be of IsTrigger. if null, logic uses <see cref="installPointColliders"/> to determine if part is in trigger. otherwise logic uses this collider to determine if part is in trigger.
            /// </summary>
            public Collider assembleCollider = null;
            /// <summary>
            /// Represents whether this part can be installed by: 1.) (false) holding this part in one of its triggers. OR 2.) (true) if the trigger is a child of another part. you can hold the root part to install aswell.
            /// </summary>
            public bool installEitherDirection = false;
            /// <summary>
            /// Represents the tightness threshold. 0.25f - 1. at what percent of all bolt tightness does the part trigger disable. 0 = triggers will disable at 25% of total tightness. 1 = triggers will disable at 100% of total tightness (tightness == maxTightness)
            /// </summary>
            public float tightnessThreshold = 0.3f;


            /// <summary>
            /// Initializes a new instance of part settings.
            /// </summary>
            public PartSettings() { }
            /// <summary>
            /// Initializes a new instance of part settings and sets all class fields to the provided settings instance, <paramref name="s"/>.
            /// </summary>
            /// <param name="s">The Setting instance to rep.</param>
            public PartSettings(PartSettings s)
            {
                if (s != null)
                {
                    collisionSettings.installedCollisionDetectionMode = s.collisionSettings.installedCollisionDetectionMode;
                    collisionSettings.notInstalledCollisionDetectionMode = s.collisionSettings.notInstalledCollisionDetectionMode;
                    assembleType = s.assembleType;
                    assemblyTypeJointSettings = s.assemblyTypeJointSettings;
                    installedPartToLayer = s.installedPartToLayer;
                    notInstalledPartToLayer = s.notInstalledPartToLayer;
                    setPositionRotationOnInitialisePart = s.setPositionRotationOnInitialisePart;
                    setPhysicsMaterialOnInitialisePart = s.setPhysicsMaterialOnInitialisePart;
                    collisionSettings.physicMaterial = s.collisionSettings.physicMaterial;
                    disassembleCollider = s.disassembleCollider;
                    assembleCollider = s.assembleCollider;
                    installEitherDirection = s.installEitherDirection;
                    tightnessThreshold = s.tightnessThreshold;
                }
            }
        }
        /// <summary>
        /// Represents the parts collision settings.
        /// </summary>
        public class CollisionSettings
        {
            // Written, 29.05.2022

            /// <summary>
            /// Represents all types of applying physic material
            /// </summary>
            public enum PhysicMaterialType
            {
                /// <summary>
                /// set physic material on all found colliders on part.
                /// </summary>
                setOnAllFoundColliders,
                /// <summary>
                /// set physic material on provided collider/s (<see cref="providedColliders"/>).
                /// </summary>
                setOnProvidedColliders,
            }
            /// <summary>
            /// Represents the default part physic material.
            /// </summary>
            public static readonly PhysicMaterial defaultPhysicMaterial = new PhysicMaterial("ModAPI.Part.defaultPhysicMaterial")
            {
                staticFriction = 0.4f,
                dynamicFriction = 0.6f,
            };
            /// <summary>
            /// Represents the collision detection mode on installed parts.
            /// </summary>
            public CollisionDetectionMode installedCollisionDetectionMode = CollisionDetectionMode.Discrete;
            /// <summary>
            /// Represents the collision detection mode on not installed parts. (pickable items)
            /// </summary>
            public CollisionDetectionMode notInstalledCollisionDetectionMode = CollisionDetectionMode.Continuous;
            /// <summary>
            /// Represents the physic material.
            /// </summary>
            public PhysicMaterial physicMaterial = defaultPhysicMaterial;
            /// <summary>
            /// Represents the current physic material type setting.
            /// </summary>
            public PhysicMaterialType physicMaterialType = PhysicMaterialType.setOnAllFoundColliders;

            /// <summary>
            /// Provided colliders. used to set physic mat on initailizePart if <see cref="PartSettings.setPhysicsMaterialOnInitialisePart"/> is true.
            /// and <see cref="physicMaterialType"/> is set to <see cref="PhysicMaterialType.setOnProvidedColliders"/>.
            /// </summary>
            public Collider[] providedColliders;
            /// <summary>
            /// Inits new isntance
            /// </summary>
            public CollisionSettings() { }
            /// <summary>
            /// inits new instacne and assigns instance variables.
            /// </summary>
            /// <param name="cs"></param>
            public CollisionSettings(CollisionSettings cs)
            {
                if (cs != null)
                {
                    installedCollisionDetectionMode = cs.installedCollisionDetectionMode;
                    notInstalledCollisionDetectionMode = cs.notInstalledCollisionDetectionMode;
                    physicMaterial = cs.physicMaterial;
                    physicMaterialType = cs.physicMaterialType;
                    providedColliders = cs.providedColliders;
                }
            }
        }
        /// <summary>
        /// Represents supported assemble types.
        /// </summary>
        public enum AssembleType
        {
            /// <summary>
            /// Represents a static assembly via, deleting parts rigidbody.
            /// </summary>
            static_rigidbodyDelete,
            /// <summary>
            /// Represents a static assembly via, setting the parts rigidbody to kinematic.
            /// </summary>
            static_rigidibodySetKinematic,
            /// <summary>
            /// Represents a fixed joint assembly via, adding a fixed joint to connect two rigidbodies together.
            /// </summary>
            joint
        }
        /// <summary>
        /// Represents settings for assembly type, joint
        /// </summary>
        public class AssemblyTypeJointSettings
        {
            /// <summary>
            /// Represents a list of install point rigidbodies for when using assemblyType:Joint
            /// </summary>
            public Rigidbody[] installPointRigidbodies;
            /// <summary>
            /// Represents the break force to break this joint. NOTE: unbreakable joints are <see cref="float.PositiveInfinity"/>.
            /// </summary>
            public float breakForce = float.PositiveInfinity;
            /// <summary>
            /// Represents the breakforce min limit. Relevant only to boltable parts that have a joint to install. and only when 
            /// </summary>
            public float breakForceMin = 0;
            /// <summary>
            /// represents if the bolts tightness effects the joints breakforce.
            /// </summary>
            public bool boltTightnessEffectsBreakforce = false;
            /// <summary>
            /// Inits new joint settings class with default values
            /// </summary>
            public AssemblyTypeJointSettings() { }
            /// <summary>
            /// Inits new joint settings class and assigns rigidbodies.
            /// </summary>
            public AssemblyTypeJointSettings(params Rigidbody[] rigidbodies)
            {
                installPointRigidbodies = rigidbodies;
            }
            /// <summary>
            /// Inits new joint settings class and assigns class variables.
            /// </summary>
            public AssemblyTypeJointSettings(float _breakForce, params Rigidbody[] rigidbodies)
            {
                breakForce = _breakForce;
                installPointRigidbodies = rigidbodies;
            }
            /// <summary>
            /// Inits new joint settings class and assigns all class variables.
            /// </summary>
            public AssemblyTypeJointSettings(float _breakForce, float _breakForceMin, bool _boltTightnessEffectsBreakForce, params Rigidbody[] rigidbodies)
            {
                breakForce = _breakForce;
                installPointRigidbodies = rigidbodies;
                breakForceMin = _breakForceMin;
                boltTightnessEffectsBreakforce = _boltTightnessEffectsBreakForce;
            }
            /// <summary>
            /// Inits new joint settings class and assigns instance variables.
            /// </summary>
            /// <param name="atjs"></param>
            public AssemblyTypeJointSettings(AssemblyTypeJointSettings atjs)
            {
                if (atjs != null)
                {
                    installPointRigidbodies = atjs.installPointRigidbodies;
                    breakForce = atjs.breakForce;
                    breakForceMin = atjs.breakForceMin;
                    boltTightnessEffectsBreakforce = atjs.boltTightnessEffectsBreakforce;
                }
            }
        }

        #endregion

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

        /// <summary>
        /// Represents the default save info for this part.
        /// </summary>
        public PartSaveInfo defaultSaveInfo
        {
            get { return _defaultSaveInfo; }
            set { _defaultSaveInfo = new PartSaveInfo(value); }
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
        /// Represents the  tightness step. calculated in <see cref="setupBoltTightnessVariables"/>
        /// </summary>
        public float tightnessStep { get; private set; }
        /// <summary>
        /// Represents the bolt parent. all bolts will be a child of this game object. NOTE null if no bolts are on this part.
        /// </summary>
        public GameObject boltParent { get; private set; } = null;
        /// <summary>
        /// Represents the cached rigidbody.
        /// </summary>
        public Rigidbody cachedRigidBody { get; private set; }
        /// <summary>
        /// Represents the fixed joint when using <see cref="AssembleType.joint"/> and installed.
        /// </summary>
        public FixedJoint joint;
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
        /// Represents max tightness of all bolts.(<see cref="Bolt.BoltSettings.maxTightness"/> * <see cref="bolts"/>.Length). if any present on part. (<see cref="hasBolts"/>)
        /// </summary>
        public float maxTightnessTotal { get; private set; } = 0;
        /// <summary>
        /// Represents tightness of all bolts (<see cref="Bolt.BoltSaveInfo.boltTightness"/> * <see cref="bolts"/>.Length). if any present on part. (<see cref="hasBolts"/>)
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
        private bool initialized = false;
        /// <summary>
        /// Represents default save info.
        /// </summary>
        private PartSaveInfo _defaultSaveInfo;
        private bool _holdingCheck;
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
        void OnJointBreak(float breakForce)
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
                guiAssemble = true;
                if (Input.GetMouseButtonDown(0))
                {
                    assemble(callback_ref.callbackCollider);
                    break;
                }
            }
            guiAssemble = false;
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

            yield return null;
            onPreAssemble?.Invoke();
            installPointIndex = Array.IndexOf(installPointColliders, installPoint);
            triggers[installPointIndex].triggerCallback.part = this;
            triggers[installPointIndex].invokePreAssembledEvent();
            installed = true;
            installPoint.enabled = false;
            transform.parent = triggers[installPointIndex].partPivot.transform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            setActiveAttachedToTrigger(false, false);
            if (hasBolts)
            {
                boltParent.SetActive(true);
                updateTotalBoltTightness();
            }
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
                    if (joint)
                    {
                        Destroy(joint);
                        yield return null;
                    }
                    joint = gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = (partSettings.assemblyTypeJointSettings.installPointRigidbodies.Length > 0 ? partSettings.assemblyTypeJointSettings.installPointRigidbodies[installPointIndex] : installPointColliders[installPointIndex].transform.GetComponentInParent<Rigidbody>()) ?? throw new Exception($"[Assemble.{name}] (Joint) Error assigning connected body. could not find a rigidbody in parent. assign a rigidbody manually at 'partSettings.assemblyTypeJointSettings.installPointRigidbodies[installPointIndex]'");
                    updateJointBreakForce();
                    break;
            }
            makePartPickable(false);

            if (playSound)
            {
                playSoundAt(transform, "CarBuilding", "assemble");
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
                mouseOverGuiDisassembleEnable(false);
            setActiveAttachedToTrigger(true, true);
            transform.SetParent(null);

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

            playSoundAt(transform, "CarBuilding", "disassemble");

            if (hasBolts)
            {
                bolts.forEach(delegate(Bolt b) 
                {
                    b.resetTightness();
                    b.updateNutPosRot();
                });
                boltParent.SetActive(false);
            }
            triggers[installPointIndex].invokeDisassembledEvent();
            onDisassemble?.Invoke();
        }

        #endregion

        #region Constructors

        ~Part()
        {
            loadedParts.Remove(this);
        }

        #endregion

        #region Event Handlers

        private void callback_onTriggerEnter(Part p, TriggerCallback callback_ref)
        {
            // Written, 04.10.2018 | Modified, 02.07.2022

            if (holdingCheck(callback_ref) && colliderCheck(callback_ref) && p == this)
            {
                if (triggerRoutine == null)
                {
                    inTrigger = true;
                    triggerRoutine = StartCoroutine(partInTrigger(callback_ref));
                }
            }
        }
        
        private void callback_onTriggerExit(Part p, TriggerCallback callback_ref)
        {
            // Written, 14.08.2018 | updated 18.09.2021

            if (p == this && colliderCheck(callback_ref))
            {
                if (triggerRoutine != null)
                {
                    stopTriggerRoutine();
                }
                guiAssemble = false;
            }
        }
        
        private void boltOnScrew()
        {
            // Written, 08.07.2022

            updateTotalBoltTightness();
            if (installed && partSettings.assembleType == AssembleType.joint && !float.IsPositiveInfinity(partSettings.assemblyTypeJointSettings.breakForce))
            {
                updateJointBreakForce();
            }
            boltTightnessCheck();
            onBoltScrew?.Invoke();
        }

        #endregion

        #region Methods

        private void modApiSetupCheck() 
        {
            // Written, 03.07.2022

            if (!modApiSetUp)
            {
                if (!modapiGo)
                    setUpLoader();
                loadModApi();
            }
        }

        /// <summary>
        /// Initializes this part.
        /// </summary>
        /// <param name="saveInfo">The save info to load this part with.</param>
        /// <param name="partSettingsRef">The part settings to load this part with.</param>
        /// <param name="triggerRefs">The Install points for this part. (in the form of a trigger)</param>
        public virtual void initPart(PartSaveInfo saveInfo, PartSettings partSettingsRef = default(PartSettings), params Trigger[] triggerRefs)
        {
            // Written, 08.09.2021
            if (!initialized)
            {
                modApiSetupCheck();
                PartID = "part" + loadedParts.Count.ToString("000");
                partSettings = new PartSettings(partSettingsRef);
                triggers = triggerRefs;
                addInstanceToTriggers();
                cachedRigidBody = GetComponent<Rigidbody>();
                cachedMass = cachedRigidBody?.mass ?? 1;
                loadedSaveInfo = new PartSaveInfo(saveInfo ?? defaultSaveInfo);
                makePartPickable(!installed);
                onPreInitPart?.Invoke();
                vaildiatePart();
                if (triggerRefs != null && triggerRefs.Length > 0)
                {
                    installPointColliders = new Collider[triggerRefs.Length];
                
                    for (int i = 0; i < triggers.Length; i++)
                    {
                        installPointColliders[i] = triggers[i].triggerCollider;
                        triggers[i].triggerCallback.onTriggerExit += callback_onTriggerExit;
                        triggers[i].triggerCallback.onTriggerEnter += callback_onTriggerEnter;
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
                    IEnumerable<Collider> colliders;
                    switch (partSettings.collisionSettings.physicMaterialType)
                    {
                        case CollisionSettings.PhysicMaterialType.setOnAllFoundColliders:
                            colliders = GetComponents<Collider>().Where(_col => !_col.isTrigger);
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
                loadedParts.Add(this);
                initialized = true;
                onPostInitPart?.Invoke();
            }
            else
                Error($"[ModAPI] error: Part, {name} is already initialized");
        }
        /// <summary>
        /// Initializes this part with bolts
        /// </summary>
        /// <param name="saveInfo">The save info to load this part and bolts with.</param>
        /// <param name="partSettingsRef">The part settings to load this part with.</param>
        /// <param name="boltRefs">The bolts for this part</param>
        /// <param name="triggerRefs">The Install points for this part. (in the form of a trigger)</param>
        public virtual void initPart(PartSaveInfo saveInfo, PartSettings partSettingsRef = default(PartSettings), Bolt[] boltRefs = null, params Trigger[] triggerRefs)
        {
            // Written, 03.07.2022

            if (!initialized)
            {
                modApiSetupCheck();
                initBolts(boltRefs, saveInfo?.boltSaveInfos);
                initPart(saveInfo, partSettingsRef, triggerRefs);
                if (!installed)
                {
                    boltParent.SetActive(false);
                    setupBoltTightnessVariables();
                }
                else
                {
                    boltTightnessCheck();
                }
            }
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
                waitCoroutine(assembleFunction(installPoint, playSound));
            else
                StartCoroutine(assembleFunction(installPoint, playSound));
        }
        /// <summary>
        /// Starts the disassemble coroutine. (<see cref="disassembleFunction(bool)"/>). async or sync
        /// </summary>
        /// <param name="runSynchronously">Runs the disassemble function synchronously.</param>
        /// <param name="playSound">Play disassemble sound?</param>
        public virtual void disassemble(bool runSynchronously = false)
        {
            // Written, 11.07.2022

            if (runSynchronously)
                waitCoroutine(disassembleFunction());
            else
                StartCoroutine(disassembleFunction());
        }
        /// <summary>
        /// Gets save info for this part and its bolts. (pos, rot, installed, install index, tightness)
        /// </summary>
        public virtual PartSaveInfo getSaveInfo()
        {
            // Written, 18.10.2018 | Modified, 11.07.2022

            PartSaveInfo info = new PartSaveInfo(loadedSaveInfo);
            info.position = transform.position;
            info.rotation = transform.eulerAngles;
            if (hasBolts)
            {
                info.boltSaveInfos = new Bolt.BoltSaveInfo[bolts.Length];
                for (int i = 0; i < bolts.Length; i++)
                {
                    info.boltSaveInfos[i] = bolts[i].loadedSaveInfo;
                }
            }
            return info;
        }
        
        /// <summary>
        /// Sets all part triggers (install points) to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">activate or not [part]</param>
        /// <param name="disassembleLogic">toggle disassemble logic or not (toggles colliders)</param>
        public void setActiveAllTriggers(bool active, bool disassembleLogic = false)
        {
            // Written, 18.09.2021 | Modified, 23.10.2021

            if (disassembleLogic)
                triggers.forEach(_trigger => _trigger.triggerCallback.disassembleLogicEnabled = active);
            installPointColliders.forEach(_trigger => _trigger.enabled = active);
        }
        /// <summary>
        /// Sets a trigger (install point) from <paramref name="index"/> to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        /// <param name="index">The idex ofg the trigger to active or not.</param>
        /// <param name="disassembleLogic">toggle disassemble logic or not (toggles colliders)</param>
        public void setActiveTrigger(bool active, int index, bool disassembleLogic = false)
        {
            // Written, 18.09.2021 | Modified, 23.10.2021

            if (disassembleLogic)
                triggers[index].triggerCallback.disassembleLogicEnabled = active;
            installPointColliders[index].enabled = active;
        }
        /// <summary>
        /// Sets the part trigger (install points) that the part is currently installed to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        /// <param name="disassembleLogic">toggle disassemble logic or not (toggles colliders)</param>
        public void setActiveAttachedToTrigger(bool active, bool disassembleLogic = false)
        {
            // Written, 18.09.2021 | Modified, 23.10.2021

            if (disassembleLogic)
                triggers[installPointIndex].triggerCallback.disassembleLogicEnabled = active;
            setActiveTrigger(active, installPointIndex);
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
            guiDisassemble = enable;
        }         
        
        /// <summary>
        /// Initializes all part bolts.
        /// </summary>
        /// <param name="boltRefs">The bolt refs to init</param>
        /// <param name="infos">The save infos for the bolts.</param>
        private void initBolts(Bolt[] boltRefs, Bolt.BoltSaveInfo[] infos)
        {
            // Written, 03.07.2022

            bool error = false;
            if (boltRefs != null)
            {
                if (boltRefs.Length > 0)
                {
                    hasBolts = true;
                    createBoltParentGameObject();
                    bolts = boltRefs;
                    for (int i = 0; i < bolts.Length; i++)
                    {
                        bolts[i].onScrew += boltOnScrew;
                        if (infos != null)
                            bolts[i].initBoltOnPart(this, infos[i]);
                        else
                            bolts[i].initBoltOnPart(this);
                    }
                }
                else
                    error = true;
            }
            else
                error = true;

            if (error) 
                Print($"Error initialializing Part, {name}. initBolts was called but no bolts were found (either length was zero or boltRef array was null).");
        }
        /// <summary>
        /// Adds this part instance to each trigger in <see cref="triggers"/>.
        /// </summary>
        private void addInstanceToTriggers() 
        {
            // Written, 02.07.2022

            foreach (Trigger trigger in triggers) 
            {
                trigger.parts.Add(this);
            }
        }
        /// <summary>
        /// Updates joint breakforce. (bolts)
        /// </summary>
        private void updateJointBreakForce() 
        {
            // Written, 08.07.2022

            if (hasBolts && partSettings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce)
            {
                joint.breakForce = (tightnessStep * tightnessTotal) + partSettings.assemblyTypeJointSettings.breakForceMin;
                joint.breakTorque = joint.breakForce / 2;
            }
            else
            {
                joint.breakForce = partSettings.assemblyTypeJointSettings.breakForce;
                joint.breakTorque = joint.breakForce / 2;
            }
        }
        /// <summary>
        /// sets up <see cref="maxTightnessTotal"/> and <see cref="tightnessStep"/>
        /// </summary>
        private void setupBoltTightnessVariables() 
        {
            // Written, 22.07.2022

            maxTightnessTotal = 0;
            tightnessStep = 0;
            foreach (Bolt b in bolts)
            {
                maxTightnessTotal += b.boltSettings.maxTightness;
                if (b.boltSettings.addNut)
                {
                    maxTightnessTotal += b.boltSettings.maxTightness;
                }
            }
            if (partSettings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce)
            {
                tightnessStep = (partSettings.assemblyTypeJointSettings.breakForce - partSettings.assemblyTypeJointSettings.breakForceMin) / maxTightnessTotal;
            }
            updateTotalBoltTightness();
        }
        /// <summary>
        /// Updates <see cref="tightnessTotal"/> from current state of <see cref="bolts"/>. see: <see cref="Bolt.BoltSaveInfo.boltTightness"/> and <see cref="Bolt.BoltSaveInfo.addNutTightness"/>
        /// </summary>
        private void updateTotalBoltTightness() 
        {
            // Written, 10.07.2022

            tightnessTotal = 0;
            foreach (Bolt b in bolts)
            {
                tightnessTotal += b.loadedSaveInfo.boltTightness;
                if (b.boltSettings.addNut)
                {
                    tightnessTotal += b.loadedSaveInfo.addNutTightness;
                }
            }
        }
        /// <summary>
        /// Creates and assigns <see cref="boltParent"/> Bolt parent.
        /// </summary>
        private void createBoltParentGameObject()
        {
            // Written, 03.07.2022

            boltParent = new GameObject("Bolts");
            boltParent.transform.SetParent(transform);
            boltParent.transform.localPosition = Vector3.zero;
            boltParent.transform.localEulerAngles = Vector3.zero;
            boltParent.transform.localScale = Vector3.one;
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
        /// <summary>
        /// Check for deciding if part is in trigger to be installed
        /// </summary>
        /// <param name="callback">the callback that triggered this.</param>
        private bool colliderCheck(TriggerCallback callback)
        {
            // Written, 09.06.2022

            if (!partSettings.assembleCollider)
                return true;
            return callback.callbackCollider.gameObject == partSettings.assembleCollider.gameObject;
        }
        /// <summary>
        /// Vaildates the part and reports to mod console. called at: <see cref="initPart(PartSaveInfo, PartSettings, Trigger[])"/>.
        /// </summary>
        private bool vaildiatePart()
        {
            // Written, 25.09.2021

            string e = null;
            if (partSettings.assembleType == AssembleType.joint && (partSettings.assemblyTypeJointSettings?.installPointRigidbodies?.Length ?? -1) <= 0)
            {
                e += "ModApi.Part.vaildiatePart: invaild part assembly setup. assembly type 'joint' must have install point rigidbodies assigned.\n";
               
            }
            if (e != null)
            {
                Error(e);
                throw new Exception(e);
            }
            return true;
        }
        /// <summary>
        /// checks all bolts for their <see cref="Bolt.boltSettings"/>.<see cref="Bolt.BoltSaveInfo.boltTightness"/>.
        /// invokes, <see cref="setActiveAttachedToTrigger(bool, bool)"/>, 
        /// sets <see cref="bolted"/> state. and
        /// invokes <see cref="onPartBolted"/>, <see cref="onPartUnBolted"/> events.
        /// </summary>
        private void boltTightnessCheck() 
        {
            // Written, 08.07.2022

            if (tightnessTotal >= maxTightnessTotal * Mathf.Clamp(partSettings.tightnessThreshold, 0.25f, 1))
            {
                setActiveAttachedToTrigger(false, true);
                guiDisassemble = false;
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
                setActiveAttachedToTrigger(true, true);
                onPartUnBolted?.Invoke();
            }
        }
        /// <summary>
        /// Represents the holding check for a <see cref="Part"/>. Used for starting and checking trigger routine. <see cref="partInTrigger(TriggerCallback)"/>
        /// </summary>
        /// <param name="callback_ref">The trigger callback</param>
        public bool holdingCheck(TriggerCallback callback_ref) 
        {
            // Written, 02.07.2022

            _holdingCheck = gameObject.isPlayerHolding();
            if (partSettings.installEitherDirection)
                _holdingCheck |= callback_ref.callbackCollider.gameObject.getBehaviourInParent<Part>(part => part && !part.installed)?.gameObject.isPlayerHolding() ?? false;
            return _holdingCheck;
        }

        internal void invokePickedUpEvent()
        {
            // Written, 11.07.2022

            onPartPickUp?.Invoke();
        }
        internal void invokeThrownEvent()
        {
            // Written, 11.07.2022

            onPartThrow?.Invoke();
        }
        internal void invokeDroppedEvent()
        {
            // Written, 11.07.2022

            onPartDrop?.Invoke();
        }

        #endregion
    }
}
