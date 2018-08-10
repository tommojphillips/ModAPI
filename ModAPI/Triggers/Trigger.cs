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
        /// Initializes a new instance.
        /// </summary>
        /// <param name="parent">The parent gameobject of the trigger.</param>
        /// <param name="position"></param>
        public Trigger(string triggerName, GameObject parent, Vector3 position, Vector3 scale)
        {
            // Written, 10.08.2018

            this.triggerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            this.triggerGameObject.transform.SetParent(parent.transform, false);
            this.triggerGameObject.name = triggerName;
            this.triggerGameObject.transform.localPosition = position;
            this.triggerGameObject.transform.localScale = scale;
            this.triggerGameObject.GetComponent<Collider>().isTrigger = true;
            this.triggerGameObject.GetComponent<Renderer>().enabled = false; // making trigger invisible
        }

        #endregion
    }
}
