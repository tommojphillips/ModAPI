using UnityEngine;

namespace TommoJProductions.ModApi.v0_1_3_0_alpha.Attachable
{
    /// <summary>
    /// Represents a trigger for a part.
    /// </summary>
    public class Trigger
    {
        // Written, 10.08.2018

        #region Properties

        /// <summary>
        /// Represents the trigger game object.
        /// </summary>
        public GameObject triggerGameObject
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the triggers collider.
        /// </summary>
        public Collider triggerCollider
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the triggers renderer.
        /// </summary>
        public Renderer triggerRenderer
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the default trigger collider scale.
        /// </summary>
        private Vector3 defaultScale = new Vector3(0.05f, 0.05f, 0.05f);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of trigger. Note, Make sure to set up the trigger! <see cref="setUpTrigger(string, GameObject, Vector3, Quaternion, Vector3, bool)"/>
        /// </summary>
        public Trigger()
        {
            // Written, 04.10.2018
        }

        /// <summary>
        /// Initializes a new instance and creates a trigger at the provided position, rotation and scale.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="parent">The parent gameobject of the trigger.</param>
        /// <param name="position">The position for the trigger. ('local' related to the parent).</param>
        /// <param name="eulerAngles">The rotation for the trigger. ('local') related to the parent.</param>
        /// <param name="scale">The scale for the trigger. NOTE: if null sets scale to 0.05 on all XYZ axes.</param>
        /// <param name="visible">Sets whether the trigger should be visualized or not.</param>
        public Trigger(string triggerName, GameObject parent, Vector3? position = null, Vector3? eulerAngles = null, Vector3? scale = null, bool visible = false)
        {
            // Written, 10.08.2018

            setUpTrigger(triggerName, parent, position, eulerAngles, scale, visible);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets up the trigger.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="parent">The parent gameobject of the trigger.</param>
        /// <param name="position">The position for the trigger. ('local' related to the parent).</param>
        /// <param name="eulerAngles">The rotation for the trigger. ('local') related to the parent.</param>
        /// <param name="scale">The scale for the trigger.</param>
        /// <param name="visible">Sets whether the trigger should be visualized or not.</param>
        public void setUpTrigger(string triggerName, GameObject parent, Vector3? position = null, Vector3? eulerAngles = null, Vector3? scale = null, bool visible = false)
        {
            // Written, 04.10.2018
            triggerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            triggerGameObject.transform.SetParent(parent.transform, false);
            triggerGameObject.name = triggerName;
            triggerGameObject.transform.localPosition = position ?? Vector3.zero;
            triggerGameObject.transform.localRotation = Quaternion.Euler(eulerAngles ?? Vector3.zero);
            triggerGameObject.transform.localScale = scale ?? defaultScale;
            triggerCollider = triggerGameObject.GetComponent<Collider>();
            triggerCollider.isTrigger = true;
            triggerRenderer = triggerGameObject.GetComponent<Renderer>();
            triggerRenderer.enabled = visible;
        }

        #endregion
    }
}
