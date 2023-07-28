using System;
using System.Linq;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
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
        public bool disassembleLogicEnabled { get; internal set; } = true;
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

        /// <summary>
        /// checks all parts that have this trigger to see if <paramref name="col"/> is a <see cref="Part"/> and that it can be installed to this trigger.
        /// </summary>
        /// <param name="col">The collider that was in the trigger.</param>
        /// <param name="part">the <see cref="Part"/> that was in the trigger, null if <paramref name="col"/> wasn't a <see cref="Part"/> or if the part can't be installed to this trigger.</param>
        /// <returns></returns>
        protected virtual bool triggerCheck(Collider col, out Part part) 
        {
            // Written, 21.08.2022

            for (int i = 0; i < trigger.parts?.Count; i++)
            {
                if (trigger.parts[i].gameObject.GetInstanceID() == col.gameObject.GetInstanceID())
                {
                    part = trigger.parts[i];
                    return true;
                }
            }
            part = null;
            return false;
        }

        #endregion

        #region Unity runtime methods

        /// <summary>
        /// awake function of the trigger callback.
        /// </summary>
        protected virtual void Awake()
        {
            callbackCollider = GetComponent<Collider>();
        }
        /// <summary>
        /// the update function of the trigger callback.
        /// </summary>
        protected virtual void Update()
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
                {
                    part.mouseOverGuiDisassembleEnable(false);
                }
            }
        }
        /// <summary>
        /// the disable function of this callback. inactives <see cref="Part.mouseOver"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (part)
            {
                if (part.mouseOver)
                {
                    part.mouseOverGuiDisassembleEnable(false);
                }
            }
        }
        /// <summary>
        /// Occurs when a collider exits this trigger.
        /// </summary>
        /// <param name="collider">the collider that invoked this</param>
        protected virtual void OnTriggerExit(Collider collider)
        {
            if (triggerCheck(collider, out Part part))
            {
                part.onTriggerExit(this);
                onTriggerExit?.Invoke(part, this);
            }
        }
        /// <summary>
        /// Occurs when a collider enters this trigger.
        /// </summary>
        /// <param name="collider">the collider that invoked this</param>
        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (triggerCheck(collider, out Part part))
            {
                part.onTriggerEnter(this);
                onTriggerEnter?.Invoke(part, this);
            }
        }

        #endregion
    }
}
