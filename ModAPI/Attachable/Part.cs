using MSCLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TommoJProductions.ModApi.Attachable.CallBacks;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents a pickable and installable part for the satsuma (or anything).
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
            public bool installed
            {
                get;
                set;
            }
            /// <summary>
            /// The install point index that the part is installed to located in <see cref="Part.triggers" />.
            /// </summary>
            public int installedPointIndex
            {
                get;
                set;
            }
            /// <summary>
            /// Represents the parts world position.
            /// </summary>
            public Vector3 position
            {
                get;
                set;
            }
            /// <summary>
            /// Represents the parts world rotation (euler angles)
            /// </summary>
            public Vector3 rotation
            {
                get;
                set;
            }

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

                installed = inSave.installed;
                installedPointIndex = inSave.installedPointIndex;
                position = inSave.position;
                rotation = inSave.rotation;
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
            public AssemblyTypeJointSettings assemblyTypeJointSettings = new AssemblyTypeJointSettings() { installPointRigidbodies = null };
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
            /// Represents the disassemble collider. collider must not be of IsTrigger. if null, logic uses Parts Gameobject to determine if part is being looked at. otherwise logic uses this collider to determine if part is being looked at.
            /// </summary>
            public Collider disassembleCollider = null;
            /// <summary>
            /// Represents the assemble collider. collider must be of IsTrigger. if null, logic uses <see cref="installPointColliders"/> to determine if part is in trigger. otherwise logic uses this collider to determine if part is in trigger.
            /// </summary>
            public Collider assembleCollider = null;
            /// <summary>
            /// Sets the parts colliders physics material if enabled.
            /// </summary>
            public bool setPhysicsMaterialOnInitialisePart = false;
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
                    assembleType = s.assembleType;
                    assemblyTypeJointSettings = s.assemblyTypeJointSettings;
                    installedPartToLayer = s.installedPartToLayer;
                    notInstalledPartToLayer = s.notInstalledPartToLayer;
                    setPositionRotationOnInitialisePart = s.setPositionRotationOnInitialisePart;
                    collisionSettings.installedCollisionDetectionMode = s.collisionSettings.installedCollisionDetectionMode;
                    collisionSettings.notInstalledCollisionDetectionMode = s.collisionSettings.notInstalledCollisionDetectionMode;
                    setPhysicsMaterialOnInitialisePart = s.setPhysicsMaterialOnInitialisePart;
                    collisionSettings.physicMaterial = s.collisionSettings.physicMaterial;
                    disassembleCollider = s.disassembleCollider;
                    assembleCollider = s.assembleCollider;
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
            /// Provided colliders. used to set physic mat on initailizePart if <see cref="setPhysicsMaterialOnInitialisePart"/> is true.
            /// and <see cref="physicMaterialType"/> is set to <see cref="PhysicMaterialType.setOnProvidedColliders"/>.
            /// </summary>
            public Collider[] providedColliders;
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
            /// Inits new joint settings class and assigns rbs
            /// </summary>
            /// <param name="rigidbodies"></param>
            public AssemblyTypeJointSettings(params Rigidbody[] rigidbodies)
            {
                installPointRigidbodies = rigidbodies;
            }
        }

        #endregion

        #region Fields / Properties

        /// <summary>
        /// Represents the on assemble event.
        /// </summary>
        public event Action onAssemble;
        /// <summary>
        /// Represents the on disassemble event.
        /// </summary>
        public event Action onDisassemble;
        /// <summary>
        /// Represents the on pre assemble event.
        /// </summary>
        public event Action onPreAssemble;
        /// <summary>
        /// Represents the on pre disassemble event.
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
        /// Represents all triggers; install points for this part.
        /// </summary>
        public Trigger[] triggers;
        /// <summary>
        /// Represents if the part is installed or not.
        /// </summary>
        public bool installed
        {
            get => loadedSaveInfo.installed;
            private set => loadedSaveInfo.installed = value;
        }
        /// <summary>
        /// Represents the current install point index that this part is installed to.
        /// </summary>
        public int installPointIndex
        {
            get => loadedSaveInfo.installedPointIndex;
            private set => loadedSaveInfo.installedPointIndex = value;
        }
        /// <summary>
        /// Represents the default save info for this part.
        /// </summary>
        public PartSaveInfo defaultSaveInfo 
        {
            get { return _defaultSaveInfo; }
            set { _defaultSaveInfo = new PartSaveInfo(value); }
        }

        /// <summary>
        /// Represents the parts settings.
        /// </summary>
        public PartSettings partSettings;

        /// <summary>
        /// Represents all bolts for the part.
        /// </summary>
        public Bolt[] bolts;

        /// <summary>
        /// Represents default save info.
        /// </summary>
        private PartSaveInfo _defaultSaveInfo;
        /// <summary>
        /// Represents loaded save info.
        /// </summary>
        private PartSaveInfo loadedSaveInfo;
        /// <summary>
        /// Represents if part in trigger a trigger (install point -any)
        /// </summary>
        private bool inTrigger = false;
        /// <summary>
        /// Represents if mouse over activated.
        /// </summary>
        public bool mouseOver = false;
        /// <summary>
        /// Represents the trigger routine.
        /// </summary>
        private Coroutine triggerRoutine;
        /// <summary>
        /// Represents the install point colliers (just a list of <see cref="Trigger.triggerCollider"/> in order).
        /// </summary>
        private Collider[] installPointColliders;
        /// <summary>
        /// Represents the cached rigidbody.
        /// </summary>
        public Rigidbody cachedRigidBody { get; private set; }
        /// <summary>
        /// Represents the cached mass of the parts rigidbody.mass property.
        /// </summary>
        public float cachedMass;
        /// <summary>
        /// Represents the fixed joint when using <see cref="AssembleType.joint"/>.
        /// </summary>
        public FixedJoint joint;
        /// <summary>
        /// Represents if part has been initialized (<see cref="initPart(PartSaveInfo, PartSettings, Trigger[])"/> invoked
        /// </summary>
        private bool initialized = false;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Represents the awake runtime call
        /// </summary>
        void Awake()
        {
            cachedRigidBody = gameObject.GetComponent<Rigidbody>();
            cachedMass = cachedRigidBody?.mass ?? 1;
        }
        /// <summary>
        /// Represents the joint break runtime call
        /// </summary>
        void OnJointBreak(float breakForce)
        {
            disassemble();
        }

        #endregion

        #region IEnumerators

        /// <summary>
        /// Represents the part in a trigger logic.
        /// </summary>
        /// <param name="other">The collider reference that triggered this call.</param>
        private IEnumerator partInTrigger(Collider other)
        {
            inTrigger = true;
            while (inTrigger)
            {
                ModClient.guiAssemble = true;

                if (Input.GetMouseButtonDown(0))
                {
                    ModClient.guiAssemble = false;
                    assemble(other);
                    break;
                }
                yield return null;
            }
        }

        #endregion

        #region Event Handlers


        /// <summary>
        /// Represents the trigger enter callback
        /// </summary>
        /// <param name="other">the collider that invoked this callback.</param>
        /// <param name="callback_ref">The callback reference that invoked this.</param>
        void callback_onTriggerEnter(Collider other, TriggerCallback callback_ref)
        {
            if (!installed && colliderCheck(callback_ref) && gameObject.isPlayerHolding())
            {
                if (triggerRoutine == null)
                {
                    triggerRoutine = StartCoroutine(partInTrigger(callback_ref.callbackCollider));
                }
            }
        }
        /// <summary>
        /// Represents the trigger exit callback
        /// </summary>
        /// <param name="other">the collider that invoked this callback.</param>
        /// <param name="callback_ref">The callback reference that invoked this.</param>
        void callback_onTriggerExit(Collider other, TriggerCallback callback_ref)
        {
            if (colliderCheck(callback_ref))
            {
                if (triggerRoutine != null)
                {
                    StopCoroutine(triggerRoutine);
                    triggerRoutine = null;
                }
                ModClient.guiAssemble = false;
                inTrigger = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check for deciding if part is in trigger to be installed
        /// </summary>
        /// <param name="callback">the callback that triggered this.</param>
        private bool colliderCheck(TriggerCallback callback) 
        {
            return partSettings.assembleCollider == null ? installPointColliders.Contains(callback.callbackCollider) : callback.callbackCollider == partSettings.assembleCollider;
        }
        /// <summary>
        /// Vaildates the part and reports to mod console. called at: <see cref="initPart(PartSaveInfo, PartSettings, Trigger[])"/>.
        /// </summary>
        private void vaildiatePart()
        {
            if (partSettings.assembleType == AssembleType.joint && (partSettings.assemblyTypeJointSettings?.installPointRigidbodies?.Length ?? -1) <= 0)
                ModConsole.Error("NOTE: assembly type is 'joint' but no install point rigidbodies have been assigned!!! error!!!");
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
                initialized = true;
                partSettings = new PartSettings(partSettingsRef);
                triggers = triggerRefs;

                loadedSaveInfo = new PartSaveInfo(saveInfo ?? defaultSaveInfo);
                installPointColliders = new Collider[triggerRefs.Length];
                makePartPickable(!loadedSaveInfo.installed);

                onPreInitPart?.Invoke();
                vaildiatePart();

                if (triggerRefs.Length > 0)
                {
                    for (int i = 0; i < triggers.Length; i++)
                    {
                        Trigger t = triggers[i];
                        installPointColliders[i] = t.triggerCollider;
                        if (!t.triggerCallback)
                            t.triggerCallback = t.triggerGameObject.AddComponent<TriggerCallback>();
                        t.triggerCallback.onTriggerExit += callback_onTriggerExit;
                        t.triggerCallback.onTriggerEnter += callback_onTriggerEnter;
                    }
                    if (installed)
                    {
                        assemble(installPointColliders[installPointIndex], false);
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
                onPostInitPart?.Invoke();
                ModClient.parts.Add(this);
            }
            else
                throw new Exception($"[ModAPI] error: Part, {name} is already initialized");
        }
        /// <summary>
        /// Attaches this part to the attachment point.
        /// </summary>
        /// <param name="installPoint">The attachment point</param>
        /// <param name="playSound">Play assemble sound?</param>
        public virtual void assemble(Collider installPoint, bool playSound = true)
        {
            onPreAssemble?.Invoke();
            installed = true;
            inTrigger = false;
            installPoint.enabled = false;
            installPointIndex = Array.IndexOf(installPointColliders, installPoint);
            transform.parent = installPoint.transform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            triggers[installPointIndex].triggerCallback.part = this;
            triggers[installPointIndex].triggerCallback.triggerInUse = true;

            if (cachedRigidBody)
            {
                switch (partSettings.assembleType)
                {
                    case AssembleType.static_rigidbodyDelete:
                        cachedMass = cachedRigidBody.mass;
                        Destroy(cachedRigidBody);
                        break;
                    case AssembleType.static_rigidibodySetKinematic:
                        cachedRigidBody.isKinematic = true;
                        break;
                    case AssembleType.joint:
                        joint = gameObject.AddComponent<FixedJoint>();
                        joint.connectedBody = (partSettings.assemblyTypeJointSettings.installPointRigidbodies.Length > 0 ? partSettings.assemblyTypeJointSettings.installPointRigidbodies[installPointIndex] : installPointColliders[installPointIndex].transform.GetComponentInParent<Rigidbody>()) ?? throw new Exception($"[Assemble.{name}] (Joint) Error assigning connected body. could not find a rigidbody in parent. assign a rigidbody manually at 'partSettings.assemblyTypeJointSettings.installPointRigidbodies[installPointIndex]'");
                        joint.breakForce = partSettings.assemblyTypeJointSettings.breakForce;
                        joint.breakTorque = joint.breakForce / 2;
                        break;
                }
            }
            makePartPickable(false);

            if (playSound)
            {
                MasterAudio.PlaySound3DAndForget("CarBuilding", transform, variationName: "assemble");
            }

            onAssemble?.Invoke();
        }
        /// <summary>
        /// Disassemble this part from the installed point
        /// </summary>
        /// <param name="playSound">Play disassemble sound?</param>
        public virtual void disassemble(bool playSound = true)
        {
            onPreDisassemble?.Invoke();
            installed = false;
            if (mouseOver)
                mouseOverGuiDisassembleEnable(false);
            setActiveAttachedToTrigger(true);
            transform.parent = null;
            triggers[installPointIndex].triggerCallback.part = null;
            triggers[installPointIndex].triggerCallback.triggerInUse = false;

            switch (partSettings.assembleType)
            {
                case AssembleType.static_rigidbodyDelete:
                    cachedRigidBody = gameObject.AddComponent<Rigidbody>();
                    cachedRigidBody.mass = cachedMass;
                    break;
                case AssembleType.static_rigidibodySetKinematic:
                    cachedRigidBody.isKinematic = false;
                    break;
                case AssembleType.joint:
                    Destroy(joint);
                    break;
            }
            makePartPickable(true);

            if (playSound)
            {
                MasterAudio.PlaySound3DAndForget("CarBuilding", transform, variationName: "disassemble");
            }

            onDisassemble?.Invoke();
        }
        /// <summary>
        /// Sets all part triggers (install points) to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">activate or not [part]</param>
        /// <param name="disassembleLogic">toggle disassemble logic or not (toggles colliders)</param>
        public void setActiveAllTriggers(bool active, bool disassembleLogic = false)
        {
            if (disassembleLogic)
                triggers.ToList().ForEach(_trigger => _trigger.triggerCallback.disassembleLogicEnabled = active);
            installPointColliders.ToList().ForEach(_trigger => _trigger.enabled = active);
        }
        /// <summary>
        /// Sets a trigger (install point) from <paramref name="index"/> to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        /// <param name="index">The idex ofg the trigger to active or not.</param>
        /// <param name="disassembleLogic">toggle disassemble logic or not (toggles colliders)</param>
        public void setActiveTrigger(bool active, int index, bool disassembleLogic = false)
        {
            if (disassembleLogic)
                triggers.ToList().ForEach(_trigger => _trigger.triggerCallback.disassembleLogicEnabled = active);
            installPointColliders[index].enabled = active;
        }
        /// <summary>
        /// Sets the part trigger (install points) that the part is currently installed to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        /// <param name="disassembleLogic">toggle disassemble logic or not (toggles colliders)</param>
        public void setActiveAttachedToTrigger(bool active, bool disassembleLogic = false)
        {
            if (disassembleLogic)
                triggers.ToList().ForEach(_trigger => _trigger.triggerCallback.disassembleLogicEnabled = active);
            setActiveTrigger(active, installPointIndex);
        }
        /// <summary>
        /// Gets save info for this part. (pos, rot, installed, install index)
        /// </summary>
        public virtual PartSaveInfo getSaveInfo()
        {
            return new PartSaveInfo() { installed = installed, installedPointIndex = installPointIndex, position = transform.position, rotation = transform.eulerAngles };
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
            mouseOver = enable;
            ModClient.guiDisassemble = enable;
        }

        #endregion
    }
}
