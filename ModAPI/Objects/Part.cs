using System;
using UnityEngine;
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance. Note after assigning, <see cref="partTrigger"/>, call <see cref="initializeTriggerCallback"/>.
        /// </summary>
        public Part()
        {
            // Written, 10.08.2018

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
        /// Checks that the parmeter, <paramref name="collider"/>'s <see cref="UnityEngine.Object.name"/> property is equal to the <see cref="part"/>'s
        /// <see cref="UnityEngine.Object.name"/> property.
        /// </summary>
        /// <param name="collider">The Collider.</param>
        public bool isPartCollider(Collider collider)
        {
            // Written, 10.08.2018

            string _name = collider.gameObject.name;
            if (_name == this.part.name || _name == String.Format("{0}(Clone)", this.part.name))
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
            this.partTrigger.triggerGameObject.GetComponent<TriggerCallback>().onTriggerStay += new Action<Collider>(this.onTriggerStay);
        }
        /// <summary>
        /// Disassembles the <see cref="part"/>.
        /// </summary>
        public virtual void disassemble()
        {
            // Written, 09.08.2018
        }
        /// <summary>
        /// Assembles the <see cref="part"/>.
        /// </summary>
        public virtual void assemble()
        {
            // Written, 10.08.2018

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

            ModConsole.Print("[Assemble] - parent = " + this.part.transform.parent);
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

            if (this.isPartCollider(collider))
                ModAPI.guiAssemble = true;
        }
        /// <summary>
        /// Occurs when the part has exited the trigger.
        /// </summary>
        /// <param name="collider"></param>
        private void onTriggerExit(Collider collider)
        {
            // Written, 10.08.2018

            if (this.isPartCollider(collider))
                ModAPI.guiAssemble = false;
        }
        /// <summary>
        /// Occurs when the part is within the trigger.
        /// </summary>
        /// <param name="collider"></param>
        private void onTriggerStay(Collider collider)
        {
            // Written, 10.08.2018

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (this.isPartCollider(collider))
                {
                    ModConsole.Print("collider's game object equals this part, " + this.part.name);
                    this.assemble();
                }
                else
                {
                    ModConsole.Print("collider's game object nonequals this part, " + this.part.name);
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

            this.partTrigger.triggerGameObject.SetActive(true);
            ModAPI.disassembleAudio.transform.position = this.part.transform.position;
            ModAPI.disassembleAudio.Play();
            ModConsole.Print(this.part.name + " Joint Broken, Float: " + force);
        }

        #endregion
    }
}
