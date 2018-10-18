using ModApi.Attachable.CallBacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModApi.Attachable
{
    /// <summary>
    /// Represents a pickable and installable part for the satsuma (or anything).
    /// </summary>
    public abstract class Part
    {
        #region Properties

        /// <summary>
        /// Represents the default save info for when there is no save data passed.
        /// </summary>
        protected internal abstract PartSaveInfo defaultPartSaveInfo
        {
            get;
        }
        /// <summary>
        /// Represents the parent for the part. The gameobject that this part connects to.
        /// </summary>
        protected internal GameObject parent
        {
            get;
        }
        /// <summary>
        /// Represents the rigid part; the installed/fixed part.
        /// </summary>
        protected internal abstract GameObject rigidPart
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the active part; the pickable part.
        /// </summary>
        protected internal abstract GameObject activePart
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the trigger for the part.
        /// </summary>
        protected internal Trigger partTrigger
        {
            get;
            set;
        }
        /// <summary>
        /// Represents if the part is installed to the trigger or not.
        /// </summary>
        protected internal bool installed
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents whether the player is holding the part.
        /// </summary>
        protected internal bool isPlayerHoldingPart
        {
            get
            {
                if (this.activePart.isOnLayer(LayerMasksEnum.Wheel))
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Represents whether the part is within the trigger.
        /// </summary>
        protected internal bool isPartInTrigger
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the loaded save info. If no loaded info, is <see langword="null"/>.
        /// </summary>
        protected internal PartSaveInfo loadedSaveInfo
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance and assigns object properties to parameters.
        /// </summary>
        /// <param name="inPartSaveInfo">The part save info to load.</param>
        /// <param name="part">The gameobject to create a part from. note this gameobject expects to have a rigidbody and collider attached.</param>
        /// <param name="parent">The parent of the gameobject; the gameobject that the part 'installs' to.</param>
        /// <param name="inPartTrigger">The trigger for the part.</param>
        /// <param name="inPartPosition">The position that the part will stay relavited to the parent. (when installed).</param>
        /// <param name="inPartRotation">The rotation that the part will stay relavited to the parent. (when installed).</param>
        public Part(PartSaveInfo inPartSaveInfo, GameObject part, GameObject parent, Trigger inPartTrigger, Vector3 inPartPosition, Quaternion inPartRotation)
        {
            // Written, 16.10.2018

            // Setting parent.
            this.parent = parent;
            // Getting loaded settings
            this.loadedSaveInfo = inPartSaveInfo;
            // Setting up trigger for the part.
            this.partTrigger = inPartTrigger;
            this.initializeTriggerCallback();
            // Setting up free part.
            this.activePart = UnityEngine.Object.Instantiate(part);
            this.activePart.AddComponent<Rigidbody>();
            this.activePart.sendChildrenToLayer(LayerMasksEnum.Parts);
            this.makePartPickable(true);
            // Setting up installed part.
            this.rigidPart = UnityEngine.Object.Instantiate(part);
            UnityEngine.Object.Destroy(this.rigidPart.GetComponent<Rigidbody>());
            this.rigidPart.AddComponent<Rigid>().part = this;
            this.rigidPart.sendToLayer(LayerMasksEnum.Parts, true);
            this.rigidPart.transform.SetParent(this.parent.transform, false);
            this.rigidPart.transform.localPosition = inPartPosition;
            this.rigidPart.transform.localRotation = inPartRotation;
            // Setting up part state.
            try
            {
                if (this.loadedSaveInfo == null)
                    this.loadedSaveInfo = this.defaultPartSaveInfo;
                if (this.loadedSaveInfo.installed)
                {
                    this.assemble(true);
                }
                else
                {
                    this.disassemble(true);
                    this.activePart.transform.position = this.loadedSaveInfo.position;
                    this.activePart.transform.rotation = this.loadedSaveInfo.rotation;
                }
            }
            catch (Exception ex)
            {
                MSCLoader.ModConsole.Error("[ModAPI.Part] - " + ex.ToString());
            }
        }

        #region Methods

        /// <summary>
        /// Initializes the trigger call backs. (TriggerStay + TriggerExit)
        /// </summary>
        protected internal void initializeTriggerCallback()
        {
            // Written, 10.08.2018

            this.partTrigger.triggerGameObject.AddComponent<TriggerCallback>().onTriggerStay += new Action<Collider>(this.onTriggerStay);
            this.partTrigger.triggerGameObject.GetComponent<TriggerCallback>().onTriggerExit += new Action<Collider>(this.onTriggerExit);
        }
        /// <summary>
        /// Gets all part data to save info.
        /// </summary>
        protected internal PartSaveInfo getSaveInfo()
        {
            // Written, 04.10.2018

            return new PartSaveInfo(this);
        }
        /// <summary>
        /// Makes the part a pickable item depending on the provided values.
        /// </summary>
        /// <param name="pickable">Make part pickable?</param>
        /// <param name="layer">Make part on different layer</param>
        protected internal void makePartPickable(bool pickable, LayerMasksEnum layer = LayerMasksEnum.Parts)
        {
            // Written, 14.08.2018

            if (pickable)
                this.activePart.tag = "PART";
            else
                this.activePart.tag = "Untagged";
            this.activePart.layer = layer.layer();
        }
        /// <summary>
        /// Checks that the parmeter, <paramref name="collider"/>'s <see cref="UnityEngine.Object.name"/> property is equal to the Part's
        /// <see cref="UnityEngine.Object.name"/> property.
        /// </summary>
        /// <param name="collider">The collider that hit the trigger.</param>
        protected internal bool isPartCollider(Collider collider)
        {
            // Written, 10.08.2018

            string _name = collider.gameObject.name;
            if (_name == this.activePart.name)
                return true;
            return false;
        }
        /// <summary>
        /// Disassembles the part.
        /// </summary>
        protected internal virtual void disassemble(bool startup = false)
        {
            // Written, 16.10.2018

            this.activePart.SetActive(true);
            this.rigidPart.SetActive(false);
            this.partTrigger.triggerGameObject.SetActive(true);
            this.activePart.transform.position = this.rigidPart.transform.position;
            if (!startup)
            {
                ModClient.disassembleAudio.transform.position = this.activePart.transform.position;
                ModClient.disassembleAudio.Play();
            }
            this.installed = false;
        }
        /// <summary>
        /// Assembles the part.
        /// </summary>
        /// <param name="startUp">Optional; if true, does not trigger the sound. (for when loading data)</param>
        protected internal virtual void assemble(bool startUp = false)
        {
            // Written, 16.10.2018
                        
            this.activePart.SetActive(false);
            this.rigidPart.SetActive(true);
            this.partTrigger.triggerGameObject.SetActive(false);
            if (!startUp)
            {
                ModClient.guiAssemble = false;
                ModClient.assembleAudio.transform.position = this.rigidPart.transform.position;
                ModClient.assembleAudio.Play();
            }
            this.installed = true;
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
                    {
                        this.assemble();
                        return;
                    }
                }

                if (this.isPartCollider(inCollider))
                {
                    ModClient.guiAssemble = true;
                    this.isPartInTrigger = true;
                }
            }
        }

        #endregion
    }
}
