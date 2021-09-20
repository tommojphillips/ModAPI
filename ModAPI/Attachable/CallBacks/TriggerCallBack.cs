using System;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable.CallBacks
{
    /// <summary>
    /// Represents call back for triggers.
    /// </summary>
    public class TriggerCallback : MonoBehaviour
    {
        // Written, 10.08.2018

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
        #endregion

        #region Unity runtime methods

        private void Awake()
        {
            callbackCollider = GetComponent<Collider>();
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
