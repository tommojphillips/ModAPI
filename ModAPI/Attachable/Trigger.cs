using System;
using System.Collections.Generic;
using TommoJProductions.ModApi.Attachable.CallBacks;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents a trigger for a part.
    /// </summary>
    public class Trigger
    {
        // Written, 10.08.2018 | Updated, 11.2021

        #region Events

        /// <summary>
        /// Represents the on part assembled event. occurs when a part is assembled to this trigger (instance). occurs after assembly logic executes
        /// </summary>
        public event Action<Trigger> onPartAssembledToTrigger;
        /// <summary>
        /// Represents the on part disassembled event. occurs when a part is disassembled from this trigger (instance). occurs after disassembly logic executes
        /// </summary>
        public event Action<Trigger> onPartDisassembledFromTrigger;
        /// <summary>
        /// Represents the on part pre assembled event. occurs when a part is assembled to this trigger (instance). occurs after index of trigger is evaluated but before any assembly logic executes.
        /// </summary>
        public event Action<Trigger> onPartPreAssembledToTrigger;
        /// <summary>
        /// Represents the on part pre disassembled event. occurs when a part is disassembled from this trigger (instance). occurs before any disassembly logic executes.
        /// </summary>
        public event Action<Trigger> onPartPreDisassembledFromTrigger;

        #endregion

        #region Fields / Properties

        /// <summary>
        /// Represents the parent for the trigger.
        /// </summary>
        public Transform parent { get; private set; }
        /// <summary>
        /// Represents the triggers position.
        /// </summary>
        public Vector3 triggerPosition { get; private set; } = Vector3.zero;
        /// <summary>
        /// Represents the triggers rotation.
        /// </summary>
        public Vector3 triggerEuler { get; private set; } = Vector3.zero;
        /// <summary>
        /// Represents the pivot position.
        /// </summary>
        public Vector3 pivotPosition { get; private set; } = Vector3.zero;
        /// <summary>
        /// Represents the pivot rotation.
        /// </summary>
        public Vector3 pivotEuler { get; private set; } = Vector3.zero;
        /// <summary>
        /// Represents the trigger game object.
        /// </summary>
        public GameObject triggerGameObject { get; private set; }
        /// <summary>
        /// Represents the trigger callback reference on the triggers gameobject.
        /// </summary>
        public TriggerCallback triggerCallback { get; private set; }
        /// <summary>
        /// Represents the triggers collider.
        /// </summary>
        public BoxCollider triggerCollider { get; private set; }
        /// <summary>
        /// Represents the triggers renderer.
        /// </summary>
        public Renderer triggerRenderer { get; private set; }
        /// <summary>
        /// Represents the default trigger collider scale.
        /// </summary>
        private Vector3 defaultScale = new Vector3(0.05f, 0.05f, 0.05f);
        /// <summary>
        /// Represents all parts that have this trigger in <see cref="Part.triggers"/> array.
        /// </summary>
        public List<Part> parts { get; internal set; }

        public GameObject installPoint { get; private set; }
        public GameObject partPivot { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of trigger. Note, Make sure to set up the trigger! <see cref="setUpTrigger(string, GameObject, Vector3, Quaternion, Vector3, bool)"/>
        /// </summary>
        public Trigger() { }

        /// <summary>
        /// Initializes a new instance and creates a trigger at the provided position, rotation and scale.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="parent">The parent gameobject of the trigger.</param>
        /// <param name="position">The position for the trigger. ('local' related to the parent).</param>
        /// <param name="eulerAngles">The rotation for the trigger. ('local') related to the parent.</param>
        /// <param name="scale">The scale for the trigger. NOTE: if null sets scale to 0.05 on all XYZ axes.</param>
        /// <param name="visible">Sets whether the trigger should be visualized or not.</param>
        public Trigger(string triggerName, GameObject parent, Vector3 position = default(Vector3), Vector3 eulerAngles = default(Vector3), Vector3? scale = null, bool visible = false)
        {
            // Written, 10.08.2018

            initTrigger(triggerName, parent, position, eulerAngles, scale, visible);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Exposes <see cref="onPartAssembledToTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        /// <param name="part">The part that was assembled.</param>
        internal void invokeAssembledEvent()
        {
            // Written, 12.06.2022

            onPartAssembledToTrigger?.Invoke(this);
        }
        /// <summary>
        /// Exposes <see cref="onPartDisassembledFromTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        /// <param name="part">The part that was disassembled.</param>
        internal void invokeDisassembledEvent()
        {
            // Written, 12.06.2022

            onPartDisassembledFromTrigger?.Invoke(this);
        }
        /// <summary>
        /// Exposes <see cref="onPartPreAssembledToTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        /// <param name="part">The part that was assembled.</param>
        internal void invokePreAssembledEvent()
        {
            // Written, 12.06.2022

            onPartPreAssembledToTrigger?.Invoke(this);
        }
        /// <summary>
        /// Exposes <see cref="onPartPreDisassembledFromTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        /// <param name="part">The part that was disassembled.</param>
        internal void invokePreDisassembledEvent()
        {
            // Written, 12.06.2022

            onPartPreDisassembledFromTrigger?.Invoke(this);
        }

        /// <summary>
        /// Initializes the trigger.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="_parent">The parent gameobject of the trigger.</param>
        /// <param name="position">The position for the trigger. ('local' related to the parent).</param>
        /// <param name="eulerAngles">The rotation for the trigger. ('local') related to the parent.</param>
        /// <param name="scale">The scale for the trigger.</param>
        /// <param name="visible">Sets whether the trigger should be visualized or not.</param>
        public void initTrigger(string triggerName, GameObject _parent, Vector3 position = default(Vector3), Vector3 eulerAngles = default(Vector3), Vector3? scale = null, bool visible = false)
        {
            // Written, 04.10.2018 | Updated, 09.2021 | 02.07.2022

            parent = _parent.transform;
            triggerPosition = position;
            triggerEuler = eulerAngles;
            createInstallPointGameObject();
            triggerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            triggerGameObject.transform.SetParent(installPoint.transform, false);
            triggerGameObject.name = triggerName;
            triggerGameObject.transform.localPosition = position.clone();
            triggerGameObject.transform.localEulerAngles = eulerAngles.clone();
            triggerGameObject.transform.localScale = Vector3.one;
            triggerCollider = triggerGameObject.GetComponent<BoxCollider>();
            triggerCollider.size = scale ?? defaultScale;
            triggerCollider.isTrigger = true;
            triggerRenderer = triggerGameObject.GetComponent<Renderer>();
            triggerRenderer.enabled = visible;
            triggerCallback = triggerGameObject.AddComponent<TriggerCallback>();
            triggerCallback.trigger = this;
            parts = new List<Part>();
        }
        /// <summary>
        /// Creates and assigns <see cref="installPoint"/>.
        /// </summary>
        private void createInstallPointGameObject()
        {
            // Written, 11.07.2022

            installPoint = new GameObject("InstallPoint");
            installPoint.transform.parent = parent;
            installPoint.transform.localPosition = Vector3.zero;
            installPoint.transform.localEulerAngles = Vector3.zero;
            installPoint.transform.localScale = Vector3.one;

            partPivot = new GameObject("Pivot");
            partPivot.transform.parent = installPoint.transform;
            partPivot.transform.localPosition = triggerPosition;
            partPivot.transform.localEulerAngles = triggerEuler;
            partPivot.transform.localScale = Vector3.one;
        }

        #endregion
    }
}
