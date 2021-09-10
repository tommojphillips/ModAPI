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
        /// Represents the triggers local position.
        /// </summary>
        public Vector3 triggerPosition
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the triggers local rotation.
        /// </summary>
        public Quaternion triggerRotation
        {
            get;
            set;
        }

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
        /// <param name="rotation">The rotation for the trigger. ('local') related to the parent.</param>
        /// <param name="scale">The scale for the trigger.</param>
        /// <param name="visible">Sets whether the trigger should be visualized or not.</param>
        public Trigger(string triggerName, GameObject parent, Vector3 position, Quaternion rotation, Vector3 scale, bool visible = false)
        {
            // Written, 10.08.2018

            this.setUpTrigger(triggerName, parent, position, rotation, scale, visible);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets up the trigger.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="parent">The parent gameobject of the trigger.</param>
        /// <param name="position">The position for the trigger. ('local' related to the parent).</param>
        /// <param name="rotation">The rotation for the trigger. ('local') related to the parent.</param>
        /// <param name="scale">The scale for the trigger.</param>
        /// <param name="visible">Sets whether the trigger should be visualized or not.</param>
        public void setUpTrigger(string triggerName, GameObject parent, Vector3 position, Quaternion rotation, Vector3 scale, bool visible = false)
        {
            // Written, 04.10.2018

            this.triggerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); // creating trigger gameobject.
            this.triggerGameObject.transform.SetParent(parent.transform, false); // setting the parent for the trigger.
            this.triggerGameObject.name = triggerName; // setting the triggers name.
            this.triggerGameObject.transform.localPosition = position; // setting the position for the trigger.
            this.triggerGameObject.transform.localRotation = rotation; // setting the rotation for the trigger.
            this.triggerGameObject.transform.localScale = scale; // setting the scale for the trigger.
            this.triggerGameObject.GetComponent<Collider>().isTrigger = true; // making the gameobject a trigger.
            this.triggerGameObject.GetComponent<Renderer>().enabled = visible; // making the gameobject in/visible
        }

        #endregion
    }
}
