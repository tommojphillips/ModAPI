using System;
using UnityEngine;
using ModApi.Attachable.CallBacks;

namespace ModApi.Attachable
{
    /// <summary>
    /// Represents an attachable gameobject for the satsuma.
    /// </summary>
    public abstract class Part : MonoBehaviour
    {
        // Written, 09.08.2018

        #region Properties

        /// <summary>
        /// Represents the default save info for when there is no save data passed.
        /// </summary>
        public abstract PartSaveInfo defaultPartSaveInfo
        {
            get;
        }
        /// <summary>
        /// Represents the parent for the part. The gameobject that this part connects to.
        /// </summary>
        public abstract GameObject parent
        {
            get;
        }
        /// <summary>
        /// Represents the loaded save info. If no loaded info, is <see langword="null"/>.
        /// </summary>
        public PartSaveInfo loadedSaveInfo
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the trigger for the part.
        /// </summary>
        public Trigger partTrigger
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the breakforce for the part.
        /// </summary>
        public float breakForce
        {
            get;
            set;
        }
        /// <summary>
        /// Represents if the part is installed to the trigger or not.
        /// </summary>
        public bool installed
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents whether the player is holding the part.
        /// </summary>
        public bool isPlayerHoldingPart
        {
            get
            {
                if (this.gameObject.isOnLayer(LayerMasksEnum.Wheel))
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Represents whether the part is within the trigger.
        /// </summary>
        public bool isPartInTrigger
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents whether to use fixed joint or parenting to attach the object.
        /// </summary>
        public bool useFixedJoint
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance. Note after assigning, <see cref="partTrigger"/>, call <see cref="initializeTriggerCallback"/>.
        /// </summary>
        public Part(PartSaveInfo inPartSaveInfo)
        {
            // Written, 10.08.2018

            this.loadedSaveInfo = inPartSaveInfo;

        }
        /// <summary>
        /// Initializes a new instance and assigns object properties to parameters.
        /// </summary>
        /// <param name="inPartSaveInfo">The part save info to load.</param>
        /// <param name="inPartTrigger">The trigger for the part to assign.</param>
        /// <param name="inPartPosition">The position that the part will stay relavited to the parent.</param>
        /// <param name="inPartRotation">The rotation that the part will stay relavited to the parent.</param>
        public Part(PartSaveInfo inPartSaveInfo, Trigger inPartTrigger, Vector3 inPartPosition, Quaternion inPartRotation)
        {
            // Written, 09.08.2018

            this.loadedSaveInfo = inPartSaveInfo;
            this.partTrigger = inPartTrigger;
            this.partTrigger.triggerPosition = inPartPosition;
            this.partTrigger.triggerRotation = inPartRotation;
            this.initializeTriggerCallback();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets all part data to save info.
        /// </summary>
        public PartSaveInfo getSaveInfo()
        {
            // Written, 04.10.2018

            return new PartSaveInfo(this);
        }
        /// <summary>
        /// Makes the part a pickable item depending on the provided values.
        /// </summary>
        /// <param name="pickable">Make part pickable?</param>
        /// <param name="layer">Make part on different layer</param>
        public void makePartPickable(bool pickable, LayerMasksEnum layer = LayerMasksEnum.Parts)
        {
            // Written, 14.08.2018
            
            if (pickable)
                this.gameObject.tag = "PART";
            else
                this.gameObject.tag = "Untagged";
            this.gameObject.layer = layer.layer();
        }
        /// <summary>
        /// Checks that the parmeter, <paramref name="collider"/>'s <see cref="UnityEngine.Object.name"/> property is equal to the Part's
        /// <see cref="UnityEngine.Object.name"/> property.
        /// </summary>
        /// <param name="collider">The Collider.</param>
        public bool isPartCollider(Collider collider)
        {
            // Written, 10.08.2018

            string _name = collider.gameObject.name;
            if (_name == this.gameObject.name)
                return true;
            return false;
        }
        /// <summary>
        /// Initializes the trigger call backs.
        /// </summary>
        public void initializeTriggerCallback()
        {
            // Written, 10.08.2018

            this.partTrigger.triggerGameObject.AddComponent<TriggerCallback>().onTriggerStay += new Action<Collider>(this.onTriggerStay);
            //this.partTrigger.triggerGameObject.GetComponent<TriggerCallback>().onTriggerEnter += new Action<Collider>(this.onTriggerEnter);
            this.partTrigger.triggerGameObject.GetComponent<TriggerCallback>().onTriggerExit += new Action<Collider>(this.onTriggerExit);
            }
        /// <summary>
        /// Disassembles the <see cref="GameObject.gameObject"/>.
        /// </summary>
        public virtual void disassemble()
        {
            // Written, 09.08.2018
            
            this.gameObject.transform.SetParent(null);
            FixedJoint fixedJoint = this.parent.GetComponent<FixedJoint>();
            Destroy(fixedJoint);
            JointCallBack jcb = this.parent.GetComponent<JointCallBack>();
            Destroy(jcb);
            this.partTrigger.triggerGameObject.SetActive(true);
            ModClient.disassembleAudio.transform.position = this.gameObject.transform.position;
            ModClient.disassembleAudio.Play();
            this.installed = false;
        }
        /// <summary>
        /// Assembles the <see cref="GameObject.gameObject"/>.
        /// </summary>
        /// <param name="startUp">Optional; if true, does not trigger the sound. (for when loading data)</param>
        public virtual void assemble(bool startUp = false)
        {
            // Written, 10.08.2018

            this.makePartPickable(false);
            this.transform.SetParent(this.parent.transform, false);
            if (this.useFixedJoint)
            {
                FixedJoint fixedJoint = this.parent.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = this.gameObject.GetComponent<Collider>().attachedRigidbody;
                fixedJoint.enableCollision = false;
                fixedJoint.breakForce = this.breakForce;
                JointCallBack jcb = this.parent.AddComponent<JointCallBack>();
                jcb.onJointBreak += new Action<float>(this.onJointBreak);
            }
            else
            {
                Destroy(this.gameObject.GetComponent<Rigidbody>());
                MSCLoader.ModConsole.Print("[MODAPI.Part.assemble(bool)] -  set part, " + this.gameObject.name + "'s parent to: " + this.parent.name);
            }
            this.gameObject.transform.localPosition = this.partTrigger.triggerPosition;
            this.gameObject.transform.localRotation = this.partTrigger.triggerRotation;
            if (!startUp)
            {
                ModClient.guiAssemble = false;
                ModClient.assembleAudio.transform.position = this.gameObject.transform.position;
                ModClient.assembleAudio.Play();
            }
            this.installed = true;
            this.partTrigger.triggerGameObject.SetActive(false);
        }
        /// <summary>
        /// Occurs every frame.
        /// </summary>
        public virtual void Update()
        {
            // Written, 02.10.2018

            try
            {
                if (!this.partTrigger.triggerGameObject.activeInHierarchy)
                {
                    if (this.installed)
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse1))
                        {
                            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 1, this.gameObject.layer) && this.isPartCollider(hitInfo.collider))
                            {
                                this.disassemble();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MSCLoader.ModConsole.Error("[ModAPI.Part.Start] - " + ex.ToString());
            }
        }
        /// <summary>
        /// Occurs on game start.
        /// Currently expects <see cref="defaultPartSaveInfo"/> override.
        /// Attempts to load <see cref="loadedSaveInfo"/> data, if fails- uses <see cref="defaultPartSaveInfo"/> data.
        /// </summary>
        public virtual void Start()
        {
            // Written, 02.10.2018

            try
            {
                PartSaveInfo psi;
                if (this.loadedSaveInfo == null)
                    psi = this.defaultPartSaveInfo;
                else
                    psi = this.loadedSaveInfo;

                this.breakForce = psi.breakForce;
                this.useFixedJoint = psi.useFixedJoints;
                if (psi.isInstalled)
                {
                    this.assemble(true);
                }
                else
                {
                    this.makePartPickable(true);
                    this.gameObject.sendChildrenToLayer(LayerMasksEnum.Parts);
                    this.gameObject.transform.position = psi.position;
                    this.gameObject.transform.rotation = psi.rotation;
                    this.installed = false;
                }
            }
            catch (Exception ex)
            {
                MSCLoader.ModConsole.Error("[ModAPI.Part.Start] - " + ex.ToString());
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs when the part has entered the trigger. [NOT IN USE] using <see cref="onTriggerStay(Collider)"/> instead.
        /// </summary>
        /// <param name="inCollider">Not Vaild.</param>
        [Obsolete("Currently as of version: v0.1.6852.22751-alpha 'onTriggerEnter(Collider)' is not used by the api.")]
        private void onTriggerEnter(Collider inCollider)
        {
            // Written, 10.08.2018
            
            if (this.isPartCollider(inCollider) && isPlayerHoldingPart)
            {
                ModClient.guiAssemble = true;
                this.isPartInTrigger = true;
            }
        }
        /// <summary>
        /// Occurs when the part has exited the trigger.
        /// </summary>
        /// <param name="inCollider">The collider that exited the trigger.</param>
        private void onTriggerExit(Collider inCollider)
        {
            // Written, 10.08.2018
            
            if (this.isPartCollider(inCollider))
            {
                ModClient.guiAssemble = false;
                this.isPartInTrigger = false;
            }
        }
        /// <summary>
        /// Occurs when the part is in the trigger.
        /// </summary>
        /// <param name="inCollider">The collider that's in the trigger.</param>
        private void onTriggerStay(Collider inCollider)
        {
            // Written, 02.10.2018

            if (this.isPlayerHoldingPart)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (this.isPartInTrigger)
                        this.assemble();
                }

                if (this.isPartCollider(inCollider))
                {
                    ModClient.guiAssemble = true;
                    this.isPartInTrigger = true;
                }
            }      
        }
        /// <summary>
        /// Occurs when the joint is broken.
        /// </summary>
        /// <param name="force"></param>
        private void onJointBreak(float force)
        {
            // Written, 10.08.2018

            this.disassemble();
        }

        #endregion
    }
}
