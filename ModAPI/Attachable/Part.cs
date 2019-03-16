using System;
using ModApi.Attachable.CallBacks;
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
        public abstract PartSaveInfo defaultPartSaveInfo
        {
            get;
        }
        /// <summary>
        /// Represents the parent for the part. The gameobject that this part connects to.
        /// </summary>
        public GameObject parent
        {
            get;
        }
        /// <summary>
        /// Represents the rigid part; the installed/fixed part.
        /// </summary>
        public abstract GameObject rigidPart
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the active part; the pickable part.
        /// </summary>
        public abstract GameObject activePart
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
                if (this.activePart.isOnLayer(LayerMasksEnum.Wheel))
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
        /// Represents the loaded save info. If no loaded info, is <see langword="null"/>.
        /// </summary>
        public PartSaveInfo loadedSaveInfo
        {
            get;
            private set;
        }

        #endregion

        #region Constructors 

        /// <summary>
        /// Initializes a new instance and assigns object properties to parameters.
        /// </summary>
        /// <param name="inPartSaveInfo">The part save info to load.</param>
        /// <param name="inPart">The gameobject to create a part from. note this gameobject expects to have a rigidbody and collider attached.</param>
        /// <param name="inParent">The parent of the gameobject; the gameobject that the part 'installs' to.</param>
        /// <param name="inPartTrigger">The trigger for the part.</param>
        /// <param name="inPartPosition">The position that the part will stay relavited to the parent. (when installed).</param>
        /// <param name="inPartRotation">The rotation that the part will stay relavited to the parent. (when installed).</param>
        public Part(PartSaveInfo inPartSaveInfo, GameObject inPart, GameObject inParent, Trigger inPartTrigger, Vector3 inPartPosition, Quaternion inPartRotation)
        {
            // Written, 16.10.2018

            // Setting parent.
            this.parent = inParent;
            // Getting loaded settings
            this.loadedSaveInfo = inPartSaveInfo;
            // Setting up trigger for the part.
            this.partTrigger = inPartTrigger;
            this.initializeTriggerCallback();
            // Setting up free part.
            this.activePart = GameObject.Find(inPart.name) ?? UnityEngine.Object.Instantiate(inPart); // instaniating new part in scene only if part was not found in game scene.
            this.activePart.AddComponent<Rigidbody>();
            this.makePartPickable(true);
            // Setting up installed part.
            this.rigidPart = UnityEngine.Object.Instantiate(inPart);
            UnityEngine.Object.Destroy(this.rigidPart.GetComponent<Rigidbody>()); // Destorying any rigidbody to stop the gameobject
            this.rigidPart.AddComponent<Rigid>().part = this;            
            this.makePartPickable(false, inPartInstance: PartInstanceTypeEnum.Rigid);// Sets the tag to 'Untagged' as this makes the Gameobject not pick-a-up-able.. :)
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
            ModClient.activeParts.Add(this);
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
        /// <param name="inPickable">Make part pickable?</param>
        /// <param name="inLayer">Make part on different layer</param>
        /// <param name="inPartInstance">Represents whether to modify the active or rigid gameobject.</param>
        public void makePartPickable(bool inPickable, LayerMasksEnum inLayer = LayerMasksEnum.Parts, PartInstanceTypeEnum inPartInstance = PartInstanceTypeEnum.Active)
        {
            // Written, 14.08.2018

            if (inPartInstance == PartInstanceTypeEnum.Active)
            {
                if (inPickable)
                    this.activePart.tag = "PART";
                else
                    this.activePart.tag = "DontPickThis";
                this.activePart.layer = inLayer.layer();
            }
            else
            {
                if (inPickable)
                    this.rigidPart.tag = "PART";
                else
                    this.rigidPart.tag = "DontPickThis";
                this.rigidPart.layer = inLayer.layer();
            }
        }
        /// <summary>
        /// Checks that the parmeter, <paramref name="inCollider"/>'s <see cref="UnityEngine.Object.name"/> property is equal to the Part's
        /// <see cref="UnityEngine.Object.name"/> property.
        /// </summary>
        /// <param name="inCollider">The collider that hit the trigger.</param>
        /// <param name="inPartInstance">Test either rigid or active gameobject's <see cref="UnityEngine.Object.name"/> property againist the colliders parenting gameobject's <see cref="UnityEngine.Object.name"/> property.</param>
        public bool isPartCollider(Collider inCollider, PartInstanceTypeEnum inPartInstance = PartInstanceTypeEnum.Active)
        {
            // Written, 10.08.2018

            string _name = inCollider.gameObject.name;
            if (inPartInstance == PartInstanceTypeEnum.Active)
            {
                if (_name == this.activePart.name)
                    return true;
            }
            else
            {
                if (_name == this.rigidPart.name)
                    return true;
            }
            return false;
        }
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
        /// Disassembles the part.
        /// </summary>
        protected internal virtual void disassemble(bool inStartup = false)
        {
            // Written, 16.10.2018

            this.activePart.SetActive(true);
            this.rigidPart.SetActive(false);
            this.partTrigger.triggerGameObject.SetActive(true);
            this.activePart.transform.position = this.rigidPart.transform.position;
            this.activePart.transform.rotation = this.rigidPart.transform.rotation;
            if (!inStartup)
            {
                ModClient.disassembleAudio.transform.position = this.activePart.transform.position;
                ModClient.disassembleAudio.Play();
            }
            this.installed = false;
        }
        /// <summary>
        /// Assembles the part.
        /// </summary>
        /// <param name="inStartup">Optional; if true, does not trigger the sound. (for when loading data)</param>
        protected internal virtual void assemble(bool inStartup = false)
        {
            // Written, 16.10.2018
                        
            this.activePart.SetActive(false);
            this.rigidPart.SetActive(true);
            this.partTrigger.triggerGameObject.SetActive(false);
            if (!inStartup)
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
        protected internal virtual void onTriggerExit(Collider inCollider)
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
        protected internal virtual void onTriggerStay(Collider inCollider)
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
