using System;
using UnityEngine;
using Object = UnityEngine.Object;
using ModAPI.Triggers;
using ModAPI.Joint;
using MSCLoader;

namespace ModAPI.Objects
{
    /// <summary>
    /// Represents an attachable gameobject for the satsuma.
    /// </summary>
    public abstract class Part
    {
        // Written, 09.08.2018

        #region Properties

        /// <summary>
        /// Represents the part as a gameobject.
        /// </summary>
        public GameObject part
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the parent for the part.
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
                if (this.part.layer == LayerMask.NameToLayer("Wheel"))
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


        #endregion

        #region Fields

        /// <summary>
        /// Holds the rigidbody for the part when the part is assembled.
        /// </summary>
        private Rigidbody tempRigidbody;

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
        public Part(GameObject inPart, Trigger inPartTrigger, Vector3 inPartPosition, Quaternion inPartRotation)
        {
            // Written, 09.08.2018

            this.part = inPart;            
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
        /// <param name="layer">Make part on different layer. if this is <see langword="null"/>, it will give the part an "Untagged" tag.</param>
        public void makePartPickable(bool pickable, string layer = null)
        {
            // Written, 14.08.2018

            this.part.layer = LayerMask.NameToLayer(layer ?? "Parts");
            if (pickable)
                this.part.tag = "PART";
            else
                this.part.tag = "Untagged";
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
            if (_name == this.part.name)
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
            Transform transform = GameObject.Find(this.part.name).transform;
            transform.SetParent(null);
            FixedJoint fixedJoint = this.parent.GetComponent<FixedJoint>();
            Object.Destroy(fixedJoint);
            JointCallBack jcb = this.parent.GetComponent<JointCallBack>();
            Object.Destroy(jcb);
            Rigidbody rigidbody = this.part.AddComponent<Rigidbody>();
            rigidbody = this.tempRigidbody;
            this.partTrigger.triggerGameObject.SetActive(true);
            ModAPI.disassembleAudio.transform.position = this.part.transform.position;
            ModAPI.disassembleAudio.Play();
            this.installed = false;

            //ModConsole.Print("[Disassemble] - parent = " + this.part.transform.parent);
        }
        /// <summary>
        /// Assembles the <see cref="part"/>.
        /// </summary>
        public virtual void assemble()
        {
            // Written, 10.08.2018

            this.makePartPickable(false, this.part.name);
            Rigidbody rigidbody = this.part.GetComponent<Rigidbody>();
            this.tempRigidbody = rigidbody;
            Object.Destroy(rigidbody);
            Transform transform = GameObject.Find(this.part.name).transform;
            transform.SetParent(this.parent.transform, false);
            transform.localPosition = this.partTriggerPosition;
            transform.localRotation = this.partTriggerRotation;
            FixedJoint fixedJoint = this.parent.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = this.part.GetComponent<Collider>().attachedRigidbody;
            fixedJoint.enableCollision = false;
            fixedJoint.breakForce = this.breakForce;
            JointCallBack jcb = this.parent.AddComponent<JointCallBack>();
            jcb.onJointBreak += new Action<float>(this.onJointBreak);
            this.partTrigger.triggerGameObject.SetActive(false);
            ModAPI.guiAssemble = false;
            ModAPI.assembleAudio.transform.position = this.part.transform.position;
            ModAPI.assembleAudio.Play();
            this.installed = true;
        }

        public void update() // Testing
        {
            // Testing

            if (this.installed)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 1, LayerMask.NameToLayer(this.part.name)/*~(1 << LayerMask.NameToLayer(this.part.name))*/) && this.isPartCollider(hitInfo.collider))
                    {
                        this.disassemble();
                    }
                    ModConsole.Print(hitInfo.collider.gameObject.name);
                    ModConsole.Print(hitInfo.collider.transform.gameObject.name);
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
