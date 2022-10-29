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
        /// Represents the default trigger collider size.
        /// </summary>
        public readonly Vector3 defaultSize = new Vector3(0.05f, 0.05f, 0.05f);

        /// <summary>
        /// Represents the parent for the trigger.
        /// </summary>
        public Transform parent { get; private set; }
        /// <summary>
        /// Represents the triggers position.
        /// </summary>
        public Vector3 triggerPosition
        {
            get => triggerGameObject.transform.localPosition;
            set => triggerGameObject.transform.localPosition = value;
        }
        /// <summary>
        /// Represents the triggers rotation.
        /// </summary>
        public Vector3 triggerEuler
        {
            get => triggerGameObject.transform.localEulerAngles;
            set => triggerGameObject.transform.localEulerAngles = value;
        }
        /// <summary>
        /// Represents the triggers scale.
        /// </summary>
        public Vector3 triggerSize
        {
            get => triggerGameObject.transform.localScale;
            set => triggerGameObject.transform.localScale = value;
        }
        /// <summary>
        /// Represents the pivot position.
        /// </summary>
        public Vector3 pivotPosition
        {
            get => partPivot.transform.localPosition;
            set => partPivot.transform.localPosition = value;
        }
        /// <summary>
        /// Represents the pivot rotation.
        /// </summary>
        public Vector3 pivotEuler
        {
            get => partPivot.transform.localEulerAngles;
            set => partPivot.transform.localEulerAngles = value;
        }
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
        /// Represents all parts that have this trigger in <see cref="Part.triggers"/> array.
        /// </summary>
        public List<Part> parts { get; private set; } = new List<Part>();

        /// <summary>
        /// Represents the install point gameobject. the trigger and part pivot are children of this gameobject.
        /// </summary>
        public GameObject installPoint { get; private set; }
        /// <summary>
        /// Represents the parts pivot. parts that are installed to this trigger will be parented here when installed.
        /// </summary>
        public GameObject partPivot { get; private set; }
        /// <summary>
        /// Represents the triggers name.
        /// </summary>
        public string triggerName { get; private set; }
        /// <summary>
        /// Represents the triggers bolt parent. all bolts on the part that are only relevant to this trigger are parented to this gameobject. Null if no bolts are assigned to this trigger.
        /// </summary>
        public GameObject boltParent { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of trigger. Note, Make sure to set up the trigger! <see cref="initTrigger"/>
        /// </summary>
        public Trigger() { }

        /// <summary>
        /// Initializes a new instance and creates a trigger at the provided position, rotation and scale.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="parent">The parent transform of the install point (part pivot and trigger).</param>
        /// <param name="triggerPosition">The position of the part when installed and position of trigger. ('local' relative to the parent)</param>
        /// <param name="triggerEuler">The rotation of the part when installed and rotation of trigger. ('local') relative to the parent.</param>
        /// <param name="triggerSize">The scale for the trigger. NOTE: if null sets scale to 0.05 on all XYZ axes.</param>
        public Trigger(string triggerName, GameObject parent, Vector3 triggerPosition = default, Vector3 triggerEuler = default, Vector3? triggerSize = null)
        {
            // Written, 08.10.2022

            initTrigger(triggerName, parent, triggerPosition, triggerEuler, triggerSize, triggerPosition, triggerEuler);
        }
        /// <summary>
        /// Initializes a new instance and creates a trigger at the provided position, rotation and scale.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="parent">The parent transform of the install point (part pivot and trigger).</param>
        /// <param name="triggerPosition">The position for the trigger. ('local' related to the parent).</param>
        /// <param name="triggerEuler">The rotation for the trigger. ('local') related to the parent.</param>
        /// <param name="triggerSize">The scale for the trigger. NOTE: if null sets scale to 0.05 on all XYZ axes.</param>
        /// <param name="pivotPosition">The position of the part when installed. ('local' relative to the parent)</param>
        /// <param name="pivotEuler">The rotation of the part when installed. ('local') relative to the parent.</param>
        public Trigger(GameObject parent, string triggerName, Vector3 pivotPosition = default, Vector3 pivotEuler = default, Vector3 triggerPosition = default, Vector3 triggerEuler = default, Vector3? triggerSize = null)
        {
            // Written, 08.10.2022

            initTrigger(triggerName, parent, triggerPosition, triggerEuler, triggerSize, pivotPosition, pivotEuler);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Exposes <see cref="onPartAssembledToTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        internal void invokeAssembledEvent()
        {
            // Written, 12.06.2022

            onPartAssembledToTrigger?.Invoke(this);
        }
        /// <summary>
        /// Exposes <see cref="onPartDisassembledFromTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        internal void invokeDisassembledEvent()
        {
            // Written, 12.06.2022

            onPartDisassembledFromTrigger?.Invoke(this);
        }
        /// <summary>
        /// Exposes <see cref="onPartPreAssembledToTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        internal void invokePreAssembledEvent()
        {
            // Written, 12.06.2022

            onPartPreAssembledToTrigger?.Invoke(this);
        }
        /// <summary>
        /// Exposes <see cref="onPartPreDisassembledFromTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        internal void invokePreDisassembledEvent()
        {
            // Written, 12.06.2022

            onPartPreDisassembledFromTrigger?.Invoke(this);
        }

        /// <summary>
        /// Initializes the trigger.
        /// </summary>
        /// <param name="triggerName">The name of the trigger.</param>
        /// <param name="parent">The parent transform of the install point (part pivot and trigger).</param>
        /// <param name="triggerPosition">The position of the trigger. ('local' relative to the parent).</param>
        /// <param name="triggerEuler">The rotation of the trigger. ('local') relative to the parent.</param>
        /// <param name="triggerSize">The size of the triggers colldier.</param>
        /// <param name="pivotPosition">The position of the part when installed. ('local' relative to the parent)</param>
        /// <param name="pivotEuler">The rotation of the part when installed. ('local') relative to the parent.</param>
        public void initTrigger(string triggerName, GameObject parent, Vector3 triggerPosition = default, Vector3 triggerEuler = default, Vector3? triggerSize = null, Vector3 pivotPosition = default, Vector3 pivotEuler = default)
        {
            // Written, 04.10.2018 | Updated, 09.2021 | 08.11.2022

            this.triggerName = triggerName.Replace("Trigger", "").Replace("trigger", "");

            createInstallPointGameObject(parent.transform);
            createTriggerGameObject();

            this.triggerPosition = triggerPosition.clone();
            this.triggerEuler = triggerEuler.clone();
            this.triggerSize = triggerSize?.clone() ?? defaultSize;
            this.pivotPosition = pivotPosition.clone();
            this.pivotEuler = pivotEuler.clone();
        }

        private void createTriggerGameObject()
        {
            // Written, 08.10.2022

            triggerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            triggerGameObject.transform.SetParent(installPoint.transform, false);
            triggerGameObject.name = "Trigger";
            triggerCollider = triggerGameObject.GetComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerRenderer = triggerGameObject.GetComponent<Renderer>();
            triggerRenderer.enabled = false;
            triggerCallback = triggerGameObject.AddComponent<TriggerCallback>();
            triggerCallback.trigger = this;
        }

        /// <summary>
        /// Creates and assigns <see cref="installPoint"/>.
        /// </summary>
        private void createInstallPointGameObject(Transform parent)
        {
            // Written, 11.07.2022

            installPoint = new GameObject(triggerName + "_InstallPoint");
            installPoint.transform.parent = parent;
            installPoint.transform.localPosition = Vector3.zero;
            installPoint.transform.localEulerAngles = Vector3.zero;
            installPoint.transform.localScale = Vector3.one;

            partPivot = new GameObject("Pivot");
            partPivot.transform.parent = installPoint.transform;
            partPivot.transform.localScale = Vector3.one;
        }

        #endregion
    }
}
