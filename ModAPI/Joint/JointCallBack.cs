using System;
using UnityEngine;

namespace ModAPI.Joint
{
    /// <summary>
    /// Represents a joint call back.
    /// </summary>
    public class JointCallBack : MonoBehaviour
    {
        // Written, 10.08.2018

        #region Fields

        /// <summary>
        /// Represents the on joint break event.
        /// </summary>
        public Action<float> onJointBreak;

        #endregion

        #region Methods

        private void OnJointBreak(float breakForce)
        {
            this.onJointBreak(breakForce);
        }

        #endregion
    }
}
