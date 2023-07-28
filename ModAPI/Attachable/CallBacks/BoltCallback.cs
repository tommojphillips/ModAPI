using System;
using UnityEngine;
using static UnityEngine.GUILayout;
using static TommoJProductions.ModApi.ModClient;
using MSCLoader;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents a callback for bolts.
    /// </summary>
    public class BoltCallback : MonoBehaviour
    {
        // Written, 02.07.2022

        #region Events

        /// <summary>
        /// Represents the on bolt exit event.
        /// </summary>
        public event Action<BoltCallback> onMouseExit;
        /// <summary>
        /// Represents the on bolt enter event.
        /// </summary>
        public event Action<BoltCallback> onMouseEnter;

        #endregion

        #region Properties

        /// <summary>
        /// Represents the the bolt renderer. cached on awake.
        /// </summary>
        protected MeshRenderer boltRenderer { get; private set; }
        /// <summary>
        /// Represents the the bolt material. cached on awake.
        /// </summary>
        protected Material boltMaterial { get; private set; }
        /// <summary>
        /// Represents the the bolt collider. cached on awake.
        /// </summary>
        protected Collider boltCollider { get; private set; }
        /// <summary>
        /// Represents the bolt that this callback is linked to.
        /// </summary>
        public Bolt bolt { get; internal set; }
        /// <summary>
        /// The bolt size for this callback.
        /// </summary>
        public BoltSize boltSize { get; internal set; }
        /// <summary>
        /// Represents the bolt check. checks if in the correct mode based on <see cref="boltSize"/> (if <see cref="Bolt.BoltSize.hand"/> player would need to be in HandMode otherwise tool mode would be required) and that the player is holding the correct tool for the fastener.
        /// </summary>
        public virtual bool boltCheck => doBoltCheck();

        #endregion

        #region unity runtime

        /// <summary>
        /// awake
        /// </summary>
        protected virtual void Awake()
        {
            // Written, 21.08.2022

            boltCollider = GetComponent<Collider>();
            boltRenderer = GetComponent<MeshRenderer>();
            boltMaterial = boltRenderer?.material;
            vaildate();
        }

        #endregion

        /// <summary>
        /// The bolt on enter logic
        /// </summary>
        protected internal virtual void onBoltEnter()
        {
            // Written, 24.08.2022

            if (boltCheck)
            {
                if (bolt.boltSettings.highlightBoltWhenActive)
                {
                    boltRenderer.material = getActiveBoltMaterial;
                }
                bolt.bcb_mouseEnter(this);
                onMouseEnter?.Invoke(this);
            }
        }
        /// <summary>
        /// The bolt on exit logic
        /// </summary>
        protected internal virtual void onBoltExit()
        {
            // Written, 24.08.2022

            if (bolt.boltSettings.highlightBoltWhenActive)
            {
                boltRenderer.material = boltMaterial;
            }
            bolt.bcb_mouseExit(this);
            onMouseExit?.Invoke(this);
        }

        /// <summary>
        /// executes stock bolt check. 
        /// </summary>
        /// <returns>returns true if player is holding correct tool for the bolt.</returns>
        protected virtual bool doBoltCheck() 
        {
            if (boltSize == BoltSize.hand)
            {
                return isHandEmpty && getToolWrenchSize_float == 0;
            }
            else
            {
                return !isInHandMode && getToolWrenchSize_boltSize == boltSize;
            }
        }
        /// <summary>
        /// vaildates this bolt callback
        /// </summary>
        /// <returns>true if callback is setup correctly. otherwise prints errors to mod console.</returns>
        /// <exception cref="Exception">throw an expection with a message describing the issues with the bolt callback.</exception>
        protected virtual void vaildate()
        {
            // Written, 21.08.2022

            string error = "";
            if (!boltRenderer)
            {
                error += "# could not find a MeshRenderer on the model. <u>required</u>: <i>MeshRenderer:</i>\n";
            }
            if (!boltCollider)
            {
                error += $"# could not find a collider on the bolt model. player will not be able to detect this bolt. <u>required</u>: <i>Collider:isTrigger:<b>false</b></i> | {gameObject.name} ({bolt?.boltID ?? "null"})\n";
            }

            if (!string.IsNullOrEmpty(error))
            {
                ModClient.print("[BoltCallback]\n" + error);
                throw new Exception(error);
            }
        }
    }
}
