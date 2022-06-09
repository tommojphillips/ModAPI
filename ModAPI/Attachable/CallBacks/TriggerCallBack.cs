using System;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable.CallBacks
{
    /// <summary>
    /// Represents call back for triggers.
    /// </summary>
    public class TriggerCallback : MonoBehaviour
    {
        // Written, 10.08.2018 | Updated, 10.2021

        #region Fields

        /// <summary>
        /// Represents the on trigger exit event.
        /// </summary>
        public event Action<Collider, TriggerCallback> onTriggerExit;
        /// <summary>
        /// Represents the on trigger stay event.
        /// </summary>
        public event Action<Collider, TriggerCallback> onTriggerStay;
        /// <summary>
        /// Represents the on trigger enter event.
        /// </summary>
        public event Action<Collider, TriggerCallback> onTriggerEnter;
        /// <summary>
        /// Represents the collider that invoked this callback.
        /// </summary>
        public Collider callbackCollider { get; private set; }
        /// <summary>
        /// Represents if disassemble logic is enabled or not.
        /// </summary>
        public bool disassembleLogicEnabled = true;
        /// <summary>
        /// Represents that the trigger is in use (part installed on this trigger).
        /// </summary>
        internal bool triggerInUse = false;
        /// <summary>
        /// Represents the part that is installed to this trigger. note null if no installed part.
        /// </summary>
        internal Part part;

        #endregion

        #region Unity runtime methods

        private void Awake()
        {
            callbackCollider = GetComponent<Collider>();
        }
        private void Update()
        {
            if (part && triggerInUse && disassembleLogicEnabled)
            {
                bool t = (part.partSettings.disassembleCollider == null ? part.gameObject : part.partSettings.disassembleCollider.gameObject).isPlayerLookingAt();
                if (t)
                {
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
            if (triggerInUse)
            {
                if (part.mouseOver)
                    part.mouseOverGuiDisassembleEnable(false);
            }
        }
        private void OnTriggerExit(Collider collider)
        {
            onTriggerExit?.Invoke(collider, this);
        }
        private void OnTriggerStay(Collider collider)
        {
            onTriggerStay?.Invoke(collider, this);
        }
        private void OnTriggerEnter(Collider collider)
        {
            onTriggerEnter?.Invoke(collider, this);
        }

        #endregion
    }
}
