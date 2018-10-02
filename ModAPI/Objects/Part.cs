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
        /// Represents the parent for the part. The gameobject that this part connects to.
        /// </summary>
        public GameObject parent
        {

            get;
            set;
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
        /// Makes the part a pickable item depending on the provided values.
        /// </summary>
        /// <param name="pickable">Make part pickable?</param>
        /// <param name="layer">Make part on different layer. if this is <see langword="null"/>, it will give the part the "Parts" layer.</param>
        public void makePartPickable(bool pickable, string layer = null)
        {
            // Written, 14.08.2018

            this.gameObject.layer = LayerMask.NameToLayer(layer ?? "Parts");
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

            this.partTrigger.triggerGameObject.AddComponent<TriggerCallback>();
            this.partTrigger.triggerGameObject.GetComponent<TriggerCallback>().onTriggerEnter += new Action<Collider>(this.onTriggerEnter);
            this.partTrigger.triggerGameObject.GetComponent<TriggerCallback>().onTriggerExit += new Action<Collider>(this.onTriggerExit);
            }
        /// <summary>
        /// Disassembles the <see cref="part"/>.
        /// </summary>
        public virtual void disassemble()
        {
            // Written, 09.08.2018

            this.makePartPickable(true);
            //this.gameObject.transform.SetParent(null);
            if (this.useFixedJoint)
            {

                FixedJoint fixedJoint = this.parent.GetComponent<FixedJoint>();
                Object.Destroy(fixedJoint);
                JointCallBack jcb = this.parent.GetComponent<JointCallBack>();
                Object.Destroy(jcb);
            }
            this.partTrigger.triggerGameObject.SetActive(true);
            ModAPI.disassembleAudio.transform.position = this.gameObject.transform.position;
            ModAPI.disassembleAudio.Play();
            this.installed = false;
        }
        /// <summary>
        /// Assembles the <see cref="part"/>.
        /// </summary>
        public virtual void assemble()
        {
            // Written, 10.08.2018

            this.makePartPickable(false);            
            this.gameObject.transform.SetParent(this.parent.transform, false);
            this.gameObject.transform.localPosition = this.partTriggerPosition;
            this.gameObject.transform.localRotation = this.partTriggerRotation;
            if (this.useFixedJoint)
            {
                FixedJoint fixedJoint = this.parent.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = this.gameObject.GetComponent<Collider>().attachedRigidbody;
                fixedJoint.enableCollision = false;
                fixedJoint.breakForce = this.breakForce;
                JointCallBack jcb = this.parent.AddComponent<JointCallBack>();
                jcb.onJointBreak += new Action<float>(this.onJointBreak);
            }
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
            // Testing

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
            else
            {
                if (this.isPartInTrigger && isPlayerHoldingPart)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        this.assemble();
                    }
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs when the part has entered the trigger.
        /// </summary>
        /// <param name="collider"></param>
        private void onTriggerEnter(Collider collider)
        {
            // Written, 10.08.2018
            
            if (this.isPartCollider(collider) && isPlayerHoldingPart)
            {
                ModAPI.guiAssemble = true;
                this.isPartInTrigger = true;
            }
        }
        /// <summary>
        /// Occurs when the part has exited the trigger.
        /// </summary>
        /// <param name="collider"></param>
        private void onTriggerExit(Collider collider)
        {
            // Written, 10.08.2018
            
            if (this.isPartCollider(collider))
            {
                ModAPI.guiAssemble = false;
                this.isPartInTrigger = false;
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
