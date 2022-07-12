using System;
using System.Linq;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable.CallBacks
{
    /// <summary>
    /// Represents call back for triggers.
    /// </summary>
    public class TriggerCallback : MonoBehaviour
    {
        // Written, 10.08.2018 | Updated, 10.2021

        #region Events

        /// <summary>
        /// Represents the on trigger exit event.
        /// </summary>
        public event Action<Part, TriggerCallback> onTriggerExit;
        /// <summary>
        /// Represents the on trigger stay event.
        /// </summary>
        public event Action<Part, TriggerCallback> onTriggerStay;
        /// <summary>
        /// Represents the on trigger enter event.
        /// </summary>
        public event Action<Part, TriggerCallback> onTriggerEnter;

        #endregion

        #region Fields

        /// <summary>
        /// Represents the collider that invoked this callback.
        /// </summary>
        public Collider callbackCollider { get; private set; }
        /// <summary>
        /// Represents if disassemble logic is enabled or not.
        /// </summary>
        public bool disassembleLogicEnabled = true;
        /// <summary>
        /// Represents the part that is currently installed to this trigger. note null if no installed part.
        /// </summary>
        public Part part { get; internal set; }
        /// <summary>
        /// Represents the trigger that is instance is linked to.
        /// </summary>
        public Trigger trigger { get; internal set; }
        /// <summary>
        /// Represents the gui check.
        /// </summary>
        public static bool guiCheck => !ModClient.guiUse && !ModClient.guiAssemble && !ModClient.playerInMenu;

        private bool colliderCheck;

        #endregion

        #region Methods

        private bool triggerCheck(Collider col) 
        {
            return trigger.parts.Any(p => p.gameObject == col.gameObject);
        }

        #endregion

        #region Unity runtime methods

        private void Awake()
        {
            callbackCollider = GetComponent<Collider>();
        }
        private void Update()
        {
            if (part && disassembleLogicEnabled)
            {
                colliderCheck = (part.partSettings.disassembleCollider == null ? part.gameObject : part.partSettings.disassembleCollider.gameObject).isPlayerLookingAt();
                if (colliderCheck && guiCheck && ModClient.isHandEmpty)
                {
                    if (ModClient.guiDrive)
                        ModClient.guiDrive = false;
                    part.mouseOverGuiDisassembleEnable(true);
                    if (Input.GetMouseButtonDown(1))
                    {
                        part.disassemble();
                    }
                }
                else if (part.mouseOver)
                    part.mouseOverGuiDisassembleEnable(false);
            }
        }
        private void OnDisable()
        {
            if (part)
            {
                if (part.mouseOver)
                    part.mouseOverGuiDisassembleEnable(false);
            }
        }
        private void OnTriggerExit(Collider collider)
        {
            if (triggerCheck(collider))
                onTriggerExit?.Invoke(collider.GetComponent<Part>(), this);
        }
        private void OnTriggerStay(Collider collider)
        {
            if (triggerCheck(collider))
                onTriggerStay?.Invoke(collider.GetComponent<Part>(), this);
        }
        private void OnTriggerEnter(Collider collider)
        {
            if (triggerCheck(collider))
               onTriggerEnter?.Invoke(collider.GetComponent<Part>(), this);
        }

        #endregion
    }
}
