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
        public event Action<Part, Trigger> onTriggerExit;
        /// <summary>
        /// Represents the on trigger enter event.
        /// </summary>
        public event Action<Part, Trigger> onTriggerEnter;

        #endregion

        #region Fields

        /// <summary>
        /// Represents The Trigger Data. any part can install onto a trigger with the same trigger data.
        /// </summary>
        public TriggerData triggerData;

        private Part _part;
        private Trigger _trigger;

        #endregion

        #region Properties

        /// <summary>
        /// Represents the gui check.
        /// </summary>
        public static bool guiCheck => !ModClient.guiUse && !ModClient.guiAssemble && !ModClient.playerInMenu; 
        /// <summary>
        /// Represents the part that is currently installed to this trigger. note null if no installed part.
        /// </summary>
        public Part part => _part;
        /// <summary>
        /// Represents the trigger that is instance is linked to.
        /// </summary>
        public Trigger trigger
        {
            get => _trigger;
            internal set => _trigger = value;
        }

        #endregion

        #region Methods

        internal void setPart(Part part)
        {
            // Written, 11.09.2023

            _part = part;
        }

        /// <summary>
        /// checks all parts that have this trigger to see if <paramref name="col"/> is a <see cref="Part"/> and that it can be installed to this trigger.
        /// </summary>
        /// <param name="col">The collider that was in the trigger.</param>
        /// <param name="part">the <see cref="Part"/> that was in the trigger, null if <paramref name="col"/> wasn't a <see cref="Part"/> or if the part can't be installed to this trigger.</param>
        /// <returns></returns>
        protected virtual bool triggerCheck(Collider col, out Part part) 
        {
            // Written, 24.08.2023

            part = col.GetComponent<Part>();

            if (part && triggerData == part.triggerData)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Unity runtime methods


        /// <summary>
        /// the update function of the trigger callback.
        /// </summary>
        protected virtual void Update()
        {
            if (_part)
            {
                if (colliderCheck() && guiCheck && ModClient.isHandEmpty)
                {
                    if (ModClient.guiDrive)
                    {
                        ModClient.guiDrive = false;
                    }
                    _part.mouseOverGuiDisassembleEnable(true);
                    if (Input.GetMouseButtonDown(1))
                    {
                        _part.disassemble();
                    }
                }
                else if (_part.mouseOver)
                {
                    _part.mouseOverGuiDisassembleEnable(false);
                }
            }
        }

        private bool colliderCheck()
        {
            // Written, 11.09.2023

            if (_part.partSettings.disassembleCollider)
            {
                return _part.partSettings.disassembleCollider.gameObject.isPlayerLookingAt();
            }
            return _part.gameObject.isPlayerLookingAt();
        }

        /// <summary>
        /// the disable function of this callback. inactives <see cref="Part.mouseOver"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (!_part)
                return;

            if (_part.mouseOver)
            {
                _part.mouseOverGuiDisassembleEnable(false);
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
                part.onTriggerExit(trigger);
                onTriggerExit?.Invoke(part, trigger);
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
                part.onTriggerEnter(trigger);
                onTriggerEnter?.Invoke(part, trigger);
            }
        }

        #endregion
    }
}
