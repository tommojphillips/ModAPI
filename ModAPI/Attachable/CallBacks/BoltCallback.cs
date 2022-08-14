using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GUILayout;
using static TommoJProductions.ModApi.ModClient;

namespace TommoJProductions.ModApi.Attachable.CallBacks
{
    /// <summary>
    /// Represents a callback for bolts.
    /// </summary>
    public class BoltCallback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Written, 02.07.2022

        #region Events

        /// <summary>
        /// Represents the on bolt exit event.
        /// </summary>
        public event Action<BoltCallback, PointerEventData> onMouseExit;
        /// <summary>
        /// Represents the on bolt enter event.
        /// </summary>
        public event Action<BoltCallback, PointerEventData> onMouseEnter;

        #endregion

        #region Fields

        private MeshRenderer boltRenderer;
        private Material boltMaterial;

        /// <summary>
        /// Represents the bolt that this callback is linked to.
        /// </summary>
        public Bolt bolt;

        /// <summary>
        /// The bolt size for this callback.
        /// </summary>
        public Bolt.BoltSize boltSize { get; internal set; }
        /// <summary>
        /// Represents the bolt check. checks if in tool mode and that the player is holding the correct tool for the fastener.
        /// </summary>
        public bool boltCheck => !isInHandMode && getToolWrenchSize_boltSize == boltSize;

        #endregion

        #region unity runtime

        public void Awake()
        {
            boltRenderer = GetComponent<MeshRenderer>();
            boltMaterial = boltRenderer.material;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (boltCheck)
            {
                if (boltRenderer)
                    boltRenderer.material = getActiveBoltMaterial;
                onMouseEnter?.Invoke(this, eventData);
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (boltRenderer)
                boltRenderer.material = boltMaterial;
            onMouseExit?.Invoke(this, eventData);
        }
        
        #endregion  
    }
}
