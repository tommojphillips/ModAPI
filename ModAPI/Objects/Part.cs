using System;
using UnityEngine;
using Object = UnityEngine.Object;
using ModAPI.Triggers;
using ModAPI.Joint;

namespace ModAPI.Objects
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
        /// Represents the trigger for the part.
        /// </summary>
        public Trigger partTrigger
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the part's position when installed.
        /// </summary>
        public Vector3 partTriggerPosition
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the part's rotation when installed.
        /// </summary>
        public Quaternion partTriggerRotation
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
                if (this.gameObject.layer == LayerMask.NameToLayer("Wheel"))
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

        #region Fields

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance. Note after assigning, <see cref="partTrigger"/>, call <see cref="initializeTriggerCallback"/>.
        /// </summary>
        public Part()
        {
            // Written, 10.08.2018

            this.installed = false;

        }
        /// <summary>
        /// Initializes a new instance and assigns object properties to parameters.
        /// </summary>
        /// <param name="inPart">The part to assign. should have collider</param>
        /// <param name="inPartTrigger">The trigger for the part to assign.</param>
        /// <param name="inPartPosition">The position that the part will stay relavited to the parent.</param>
        /// <param name="inPartRotation">The rotation that the part will stay relavited to the parent.</param>
        public Part(Trigger inPartTrigger, Vector3 inPartPosition, Quaternion inPartRotation)
        {
            // Written, 09.08.2018
            this.partTrigger = inPartTrigger;
            this.initializeTriggerCallback();
            this.partTriggerPosition = inPartPosition;
            this.partTriggerRotation = inPartRotation;
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

            this.gameObject.layer = layer.getLayer();
            if (pickable)
                this.gameObject.tag = "PART";
            else
                this.gameObject.tag = "Untagged";
        }
        /// <summary>
        /// Checks that the parmeter, <paramref name="collider"/>'s <see cref="UnityEngine.Object.name"/> property is equal to the <see cref="part"/>'s
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

            this.makePartPickable(true);
            this.gameObject.transform.SetParent(null);
            FixedJoint fixedJoint = this.parent.GetComponent<FixedJoint>();
            Object.Destroy(fixedJoint);
            JointCallBack jcb = this.parent.GetComponent<JointCallBack>();
            Object.Destroy(jcb);
            this.partTrigger.triggerGameObject.SetActive(true);
            ModAPI.disassembleAudio.transform.position = this.gameObject.transform.position;
            ModAPI.disassembleAudio.Play();
            this.installed = false;
        }
        /// <summary>
        /// Assembles the <see cref="GameObject.gameObject"/>.
        /// </summary>
        public virtual void assemble()
        {
            // Written, 10.08.2018

            this.makePartPickable(false);
            this.gameObject.transform.localPosition = this.partTriggerPosition;
            this.gameObject.transform.localRotation = this.partTriggerRotation;
            FixedJoint fixedJoint = this.parent.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = this.gameObject.GetComponent<Collider>().attachedRigidbody;
            fixedJoint.enableCollision = false;
            fixedJoint.breakForce = this.breakForce;
            this.gameObject.transform.SetParent(this.parent.transform, false);
            JointCallBack jcb = this.parent.AddComponent<JointCallBack>();
            jcb.onJointBreak += new Action<float>(this.onJointBreak);
            this.partTrigger.triggerGameObject.SetActive(false);
            ModAPI.guiAssemble = false;
            ModAPI.assembleAudio.transform.position = this.gameObject.transform.position;
            ModAPI.assembleAudio.Play();
            this.installed = true;
        }
        /// <summary>
        /// Occurs every frame
        /// </summary>
        public virtual void Update()
        {
            // Written, 02.10.2018

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
            /*else
            {
                if (layer == (int)LayerMasksEnum.Wheel)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (this.isPartInTrigger)
                            this.assemble();
                    }
                }
            }*/
        }

        #endregion

        #region Event Handlers

        /*/// <summary>
        /// Occurs when the part has entered the trigger.
        /// </summary>
        /// <param name="inCollider"></param>
        private void onTriggerEnter(Collider inCollider)
        {
            // Written, 10.08.2018
            
            if (this.isPartCollider(inCollider) && isPlayerHoldingPart)
            {
                ModAPI.guiAssemble = true;
                this.isPartInTrigger = true;
            }
        }*/
        /// <summary>
        /// Occurs when the part has exited the trigger.
        /// </summary>
        /// <param name="inCollider">The collider that exited the trigger.</param>
        private void onTriggerExit(Collider inCollider)
        {
            // Written, 10.08.2018
            
            if (this.isPartCollider(inCollider))
            {
                ModAPI.guiAssemble = false;
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
                    ModAPI.guiAssemble = true;
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
