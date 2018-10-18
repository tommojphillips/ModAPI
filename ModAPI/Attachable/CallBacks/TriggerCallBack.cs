using System;
using UnityEngine;

namespace ModApi.Attachable.CallBacks
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
        public Action<Collider> onTriggerExit;
        /// <summary>
        /// Represents the on trigger stay event.
        /// </summary>
        public Action<Collider> onTriggerStay;

        #endregion

        #region Methods

        private void OnTriggerExit(Collider collider)
        {
            this.onTriggerExit(collider);
        }

        private void OnTriggerStay(Collider collider)
        {
            this.onTriggerStay(collider);
        }

        #endregion
    }
}
