using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents settings for Triggers.
    /// </summary>
    public class TriggerSettings
    {
        /// <summary>
        /// The name of the Trigger.
        /// </summary>
        public string triggerID = "Trigger";
        /// <summary>
        /// The Data type for this trigger. Parts with the same Data type will be installable.
        /// </summary>
        public TriggerData triggerData = null;
        /// <summary>
        /// The Triggers Position. (local space to parent)
        /// </summary>
        public Vector3 triggerPosition = default;
        /// <summary>
        /// The Triggers Rotation. (local space to parent)
        /// </summary>
        public Vector3 triggerEuler = default;
        /// <summary>
        /// The Triggers Size. (Trigger Collider)
        /// </summary>
        public float triggerRadius = 0.05f;
        /// <summary>
        /// The Install point Position. (local space to parent)
        /// </summary>
        public Vector3 pivotPosition = default;
        /// <summary>
        /// The Install point Rotation. (local space to parent)
        /// </summary>
        public Vector3 pivotEuler = default;
        /// <summary>
        /// If <see langword="true"/>, <see cref="triggerPosition"/> and <see cref="triggerEuler"/> will be used in place of <see cref="pivotPosition"/> and <see cref="pivotEuler"/>. 
        /// To position where the part will be when installed.
        /// </summary>
        public bool useTriggerTransformData = true;

        /// <summary>
        /// Initializes a new instance of trigger settings with default values.
        /// </summary>
        public TriggerSettings() { }
        /// <summary>
        /// Initializes a new instance of trigger settings and sets all class fields to the provided settings instance, <paramref name="s"/>.
        /// </summary>
        /// <param name="s">The Setting instance to replicate.</param>
        public TriggerSettings(TriggerSettings s)
        {
            if (s != null)
            {
                triggerID = s.triggerID;
                triggerData = s.triggerData;
                triggerPosition = s.triggerPosition;
                triggerEuler = s.triggerEuler;
                triggerRadius = s.triggerRadius;
                pivotPosition = s.pivotPosition;
                pivotEuler = s.pivotEuler;
                useTriggerTransformData = s.useTriggerTransformData;
            }
        }
    }
}
