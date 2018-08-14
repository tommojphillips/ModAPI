using UnityEngine;

namespace ModAPI.Triggers
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

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance and creates a trigger at the provided position and the provided scale.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="parent">The parent gameobject of the trigger.</param>
        /// <param name="position">The position for the trigger. ('local' related to the parent).</param>
        /// <param name="scale">The scale for the trigger.</param>
        public Trigger(string triggerName, GameObject parent, Vector3 position, Vector3 scale)
        {
            // Written, 10.08.2018

            this.triggerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube); // creating trigger gameobject.
            this.triggerGameObject.transform.SetParent(parent.transform, false); // setting the parent for the trigger.
            this.triggerGameObject.name = triggerName; // setting the triggers name.
            this.triggerGameObject.transform.localPosition = position;// setting the position for the trigger.
            this.triggerGameObject.transform.localScale = scale; // setting the scale for the trigger.
            this.triggerGameObject.GetComponent<Collider>().isTrigger = true; // making the gameobject a trigger.
            this.triggerGameObject.GetComponent<Renderer>().enabled = false; // making the trigger invisible.
        }

        #endregion
    }
}
