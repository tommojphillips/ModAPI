using System;
using UnityEngine;

namespace TommoJProductions.ModApi.v0_1_3_0_alpha.Attachable.CallBacks
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
        public event Action<Collider> onTriggerExit;
        /// <summary>
        /// Represents the on trigger stay event.
        /// </summary>
        public event Action<Collider> onTriggerStay;
        /// <summary>
        /// Represents the on trigger enter event.
        /// </summary>
        public event Action<Collider> onTriggerEnter;

        #endregion

        #region Methods

        private void OnTriggerExit(Collider collider)
        {
            onTriggerExit?.Invoke(collider);
        }
        private void OnTriggerStay(Collider collider)
        {
            onTriggerStay?.Invoke(collider);
        }
        private void OnTriggerEnter(Collider collider)
        {
            onTriggerStay?.Invoke(collider);
        }

        #endregion
    }
}
