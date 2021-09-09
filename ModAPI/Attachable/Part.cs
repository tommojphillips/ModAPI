using System;
using ModApi.Attachable.CallBacks;
using UnityEngine;

namespace ModApi.Attachable
{
    /// <summary>
    /// Represents a pickable and installable part for the satsuma (or anything).
    /// </summary>
    public abstract class Part : MonoBehaviour
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
        /// Represents if the part is installed to the trigger or not.
        /// </summary>
        public bool installed
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the parts install parameters.
        /// </summary>
        public PartInstallParameters partInstallParameters 
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
                if (gameObject.isOnLayer(LayerMasksEnum.Wheel))
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
        /// <param name="inPartInstallParameters">The Part install parameters</param>
        public Part(PartSaveInfo inPartSaveInfo, PartInstallParameters inPartInstallParameters)
        {
            // Written, 16.10.2018 | Modified, 09.10.2021

            partInstallParameters = inPartInstallParameters;
            loadedSaveInfo = inPartSaveInfo;
            initializeTriggerCallback(); 
            try
            {
                if (loadedSaveInfo == null)
                    loadedSaveInfo = defaultPartSaveInfo;
                if (loadedSaveInfo.installed)
                {
                    assemble(true);
                }
                else
                {
                    disassemble(true);
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
        /// Checks that the parmeter, <paramref name="inCollider"/>'s <see cref="UnityEngine.Object.name"/> property is equal to the Part's
        /// <see cref="UnityEngine.Object.name"/> property.
        /// </summary>
        /// <param name="inCollider">The collider that hit the trigger.</param>
        /// <param name="inPartInstance">Test either rigid or active gameobject's <see cref="UnityEngine.Object.name"/> property againist the colliders parenting gameobject's <see cref="UnityEngine.Object.name"/> property.</param>
        public bool isPartCollider(Collider inCollider)
        {
            // Written, 10.08.2018 | Modified, 10.09.2021

            if (inCollider.gameObject.name == gameObject.name)
                return true;
            return false;
        }
        /// <summary>
        /// Initializes the trigger call backs. (TriggerStay + TriggerExit)
        /// </summary>
        protected internal void initializeTriggerCallback()
        {
            // Written, 10.08.2018

            partInstallParameters.trigger.triggerGameObject.AddComponent<TriggerCallback>().onTriggerStay += new Action<Collider>(this.onTriggerStay);
            partInstallParameters.trigger.triggerGameObject.GetComponent<TriggerCallback>().onTriggerExit += new Action<Collider>(this.onTriggerExit);
        }
        /// <summary>
        /// Disassembles the part.
        /// </summary>
        protected internal virtual void disassemble(bool inStartup = false)
        {
            // Written, 16.10.2018 | Modified, 10.09.2021

            /*this.activePart.SetActive(true);
            this.rigidPart.SetActive(false);
            this.partTrigger.triggerGameObject.SetActive(true);
            this.activePart.transform.position = this.rigidPart.transform.position;
            this.activePart.transform.rotation = this.rigidPart.transform.rotation;
            if (!inStartup)
            {
                ModClient.disassembleAudio.transform.position = this.activePart.transform.position;
                ModClient.disassembleAudio.Play();
            }
            this.installed = false;*/
        }
        /// <summary>
        /// Assembles the part.
        /// </summary>
        /// <param name="inStartup">Optional; if true, does not trigger the sound. (for when loading data)</param>
        protected internal virtual void assemble(bool inStartup = false)
        {
            // Written, 16.10.2018
                        
            /*this.activePart.SetActive(false);
            this.rigidPart.SetActive(true);
            this.partTrigger.triggerGameObject.SetActive(false);
            if (!inStartup)
            {
                ModClient.guiAssemble = false;
                ModClient.assembleAudio.transform.position = this.rigidPart.transform.position;
                ModClient.assembleAudio.Play();
            }
            this.installed = true;*/
        }
        /// <summary>
        /// Occurs when the part has exited the trigger.
        /// </summary>
        /// <param name="inCollider">The collider that exited the trigger.</param>
        protected internal virtual void onTriggerExit(Collider inCollider)
        {
            // Written, 10.08.2018

            if (isPartCollider(inCollider))
            {
                ModClient.guiAssemble = false;
                isPartInTrigger = false;
            }
        }
        /// <summary>
        /// Occurs when the part is in the trigger.
        /// </summary>
        /// <param name="inCollider">The collider that's in the trigger.</param>
        protected internal virtual void onTriggerStay(Collider inCollider)
        {
            // Written, 02.10.2018

            if (isPlayerHoldingPart)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (isPartInTrigger)
                    {
                        assemble();
                        return;
                    }
                }

                if (isPartCollider(inCollider))
                {
                    ModClient.guiAssemble = true;
                    isPartInTrigger = true;
                }
            }
        }

        #endregion
    }
}
