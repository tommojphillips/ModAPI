using System;
using System.Collections;
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
        #region Part Classes

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
            /// The install point index that the part is installed to located in <see cref="partInstallParameters" href=".installPoints" />.
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
            }

            #endregion
        }
        /// <summary>
        /// Represents settings for the part class.
        /// </summary>
        public class PartSettings 
        {
            /// <summary>
            /// Represents the assemble type of the part.
            /// </summary>
            public AssembleType assembleType = AssembleType.static_rigidbodyDelete;
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
            static_rigidibodySetKinematic
        }

        #endregion
        
        #region Public Fields / Properties

        /// <summary>
        /// Represents the on assemble event.
        /// </summary>
        public event Action onAssemble;
        /// <summary>
        /// Represents the on disassemble event.
        /// </summary>
        public event Action onDisassemble;
        /// <summary>
        /// Represents the default save info for this part.
        /// </summary>
        public PartSaveInfo defaultSaveInfo;
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
        /// Represents the parts settings.
        /// </summary>
        public PartSettings partSettings;
        
        #endregion

        #region Private Fields

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
        private bool mouseOver = false;
        /// <summary>
        /// Represents the trigger routine.
        /// </summary>
        private Coroutine triggerRoutine;
        /// <summary>
        /// Represents the installed Routine.
        /// </summary>
        private Coroutine installedRoutine;
        /// <summary>
        /// Represents the install point colliers (just a list of <see cref="Trigger.triggerCollider"/> in order).
        /// </summary>
        private Collider[] installPointColliders;
        /// <summary>
        /// Represents the cached rigidbody.
        /// </summary>
        private Rigidbody cachedRigidBody;
        /// <summary>
        /// Represents the cached mass of the parts rigidbody.mass property.
        /// </summary>
        private float cachedMass;

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
        /// Represents the enabled runtime call
        /// </summary>
        void OnEnable()
        {
            if (installed)
                StartCoroutine(partInstalled());
        }

        #endregion

        #region IEnumerators

        /// <summary>
        /// Represents the installed part logic (eg. mousebutton1=>disassemble).
        /// </summary>
        private IEnumerator partInstalled()
        {
            while (installed)
            {
                if (gameObject.isPlayerLookingAt())
                {
                    mouseOver = true;
                    ModClient.guiDisassemble = true;

                    if (Input.GetMouseButtonDown(1))
                    {
                        disassemble();
                        mouseOverReset();
                    }
                }
                else if (mouseOver)
                    mouseOverReset();
                yield return null;
            }
        }
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
            if (!installed && installPointColliders.Contains(callback_ref.callbackCollider) && gameObject.isPlayerHolding())
            {
                if (triggerRoutine != null)
                    StopCoroutine(triggerRoutine);
                triggerRoutine = StartCoroutine(partInTrigger(callback_ref.callbackCollider));
            }
        }
        /// <summary>
        /// Represents the trigger exit callback
        /// </summary>
        /// <param name="other">the collider that invoked this callback.</param>
        /// <param name="callback_ref">The callback reference that invoked this.</param>
        void callback_onTriggerExit(Collider other, TriggerCallback callback_ref)
        {
            if (installPointColliders.Contains(callback_ref.callbackCollider))
            {
                if (triggerRoutine != null)
                    StopCoroutine(triggerRoutine);
                inTrigger = false;
                ModClient.guiAssemble = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this part.
        /// </summary>
        /// <param name="saveInfo">The save info to load this part with.</param>
        /// <param name="partSettings">The part settings to load this part with.</param>
        /// <param name="triggerRefs">The Install points for this part. (in the form of a trigger)</param>
        public void initPart(PartSaveInfo saveInfo, PartSettings partSettings = default(PartSettings), params Trigger[] triggerRefs)
        {
            // Written, 08.09.2021

            this.partSettings = partSettings;

            triggers = triggerRefs;

            loadedSaveInfo = saveInfo ?? defaultSaveInfo ?? new PartSaveInfo();
            installPointColliders = new Collider[triggerRefs.Length];
            if (triggerRefs.Length > 0)
            {
                TriggerCallback callback;
                for (int i = 0; i < triggers.Length; i++)
                {
                    installPointColliders[i] = triggers[i].triggerCollider;
                    callback = triggers[i].triggerGameObject.GetComponent<TriggerCallback>();
                    if (!callback)
                        callback = triggers[i].triggerGameObject.AddComponent<TriggerCallback>();
                    callback.onTriggerExit += callback_onTriggerExit;
                    callback.onTriggerEnter += callback_onTriggerEnter;
                }
                if (installed)
                    assemble(installPointColliders[installPointIndex], false);
            }
            if (!installed)
            {
                transform.position = loadedSaveInfo.position;
                transform.eulerAngles = loadedSaveInfo.rotation;
            }
            ModClient.parts.Add(this);
        }
        /// <summary>
        /// Attaches this part to the attachment point.
        /// </summary>
        /// <param name="installPoint">The attachment point</param>
        /// <param name="playSound">Play assemble sound?</param>
        public void assemble(Collider installPoint, bool playSound = true)
        {
            installed = true;
            inTrigger = false;
            installPoint.enabled = false;
            installPointIndex = Array.IndexOf(installPointColliders, installPoint);
            makePartPickable(false);
            transform.parent = installPoint.transform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;

            if (cachedRigidBody)
            {
                switch (partSettings.assembleType)
                {
                    case AssembleType.static_rigidbodyDelete:
                        cachedMass = cachedRigidBody.mass;
                        Destroy(cachedRigidBody);
                        break;
                    case AssembleType.static_rigidibodySetKinematic:
                        cachedRigidBody.isKinematic = false;
                        break;
                }
            }

            if (playSound)
            {
                MasterAudio.PlaySound3DAndForget("CarBuilding", transform, variationName: "assemble");
            }

            onAssemble?.Invoke();

            if (installedRoutine != null)
                StopCoroutine(installedRoutine);
            installedRoutine = StartCoroutine(partInstalled());
        }
        /// <summary>
        /// Disassemble this part from the installed point
        /// </summary>
        /// <param name="playSound">Play disassemble sound?</param>
        public void disassemble(bool playSound = true)
        {
            installed = false;
            if (mouseOver)
                mouseOverReset();
            makePartPickable(true);
            transform.parent = null;            
            setActiveAttachedToTrigger(true);

            switch (partSettings.assembleType)
            {
                case AssembleType.static_rigidbodyDelete:
                    cachedRigidBody = gameObject.AddComponent<Rigidbody>();
                    cachedRigidBody.mass = cachedMass;
                    break;
                case AssembleType.static_rigidibodySetKinematic:
                    cachedRigidBody.isKinematic = true;
                    break;
            }

            if (playSound)
            {
                MasterAudio.PlaySound3DAndForget("CarBuilding", transform, variationName: "disassemble");
            }

            onDisassemble?.Invoke();
        }
        /// <summary>
        /// Sets all part triggers (install points) to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        public void setActiveAllTriggers(bool active)
        {
            installPointColliders.ToList().ForEach(_trigger => _trigger.enabled = active);
        }
        /// <summary>
        /// Sets a trigger (install point) from <paramref name="index"/> to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        /// <param name="index">The idex ofg the trigger to active or not.</param>
        public void setActiveTrigger(bool active, int index)
        {
            installPointColliders[index].enabled = active;
        }
        /// <summary>
        /// Sets the part trigger (install points) that the part is currently installed to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        public void setActiveAttachedToTrigger(bool active)
        {
            setActiveTrigger(active, installPointIndex);
        }
        /// <summary>
        /// Gets save info for this part. (pos, rot, installed, install index)
        /// </summary>
        public PartSaveInfo getSaveInfo()
        {
            return new PartSaveInfo() { installed = installed, installedPointIndex = installPointIndex, position = transform.position, rotation = transform.eulerAngles };
        }
        /// <summary>
         /// Makes the part a pickable item depending on the provided values.
         /// </summary>
         /// <param name="inPickable">Make part pickable?</param>
         /// <param name="inLayer">Make part on different layer</param>
        public void makePartPickable(bool inPickable, LayerMasksEnum inLayer = LayerMasksEnum.Parts)
        {
            // Written, 14.08.2018 | Modified, 10.09.2021

            if (inPickable)
                gameObject.tag = "PART";
            else
                gameObject.tag = "DontPickThis";
            gameObject.layer = inLayer.layer();
        }
        /// <summary>
        /// ends (resets) a gui interaction.
        /// </summary>
        private void mouseOverReset()
        {
            mouseOver = false;
            ModClient.guiDisassemble = false;
        }

        #endregion
    }
}
