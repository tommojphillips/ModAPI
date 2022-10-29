using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    // Written, 11.05.2022


    /// <summary>
    /// Represents all settings for a bolt.
    /// </summary>
    public class BoltSettings
    {
        /// <summary>
        /// for initializing a new instance of bolt with custom name.
        /// </summary>
        public string name = null;

        /// <summary>
        /// Represents the bolts type.
        /// </summary>
        public BoltType boltType = BoltType.shortBolt;
        /// <summary>
        /// Represents the tool size required to un/tighten this bolt
        /// </summary>
        public BoltSize boltSize = BoltSize._7mm;
        /// <summary>
        /// Represents the position step. (how quick the position of the bolt moves in <see cref="posDirection"/>. when un/tightening)
        /// </summary>
        public float posStep = 0.0005f;
        /// <summary>
        /// Represents the rot step. (how quick the bolt rotates when un/tightening.
        /// </summary>
        public float rotStep = 30;
        /// <summary>
        /// Represents the tightness step.
        /// </summary>
        public float tightnessStep = 1;
        /// <summary>
        /// Represents the max tightness.
        /// </summary>
        public float maxTightness = 8;
        /// <summary>
        /// Represents positional direction of bolt.
        /// </summary>
        public Vector3 posDirection = Vector3.forward;
        /// <summary>
        /// Represents the rotational direction of bolt.
        /// </summary>
        public Vector3 rotDirection = Vector3.forward;
        /// <summary>
        /// Represents the custom bolt prefab to use. NOTE: Set <see cref="boltType"/> to <see cref="BoltType.custom"/>.
        /// </summary>
        public GameObject customBoltPrefab = null;
        /// <summary>
        /// if true, <see cref="Bolt.boltFunction"/> does not check if tightness has changed (tightness is either completely tight or loose). therefore invokes bolt event/s anyway. eg => if bolt is already tight and user tries to tighten more. it will call <see cref="Bolt.onTight"/> anyway.
        /// </summary>
        public bool ignoreTightnessChangedCheck = false;
        /// <summary>
        /// if true adds a nut to the other side of the bolt. note both bolt and nut will need to be tightened.
        /// </summary>
        public bool addNut = false;
        /// <summary>
        /// Settings for <see cref="addNut"/> setting. 
        /// </summary>
        public AddNutSettings addNutSettings = default;
        /// <summary>
        /// if true, highlights the bolt green when the bolt is activated.
        /// </summary>
        public bool highlightBoltWhenActive = true;
        /// <summary>
        /// If true, Bolt is visible and logic is active when uninstalled. only relevant when <see cref="visibleWhenUninstalled"/> is <see langword="true"/>.
        /// </summary>
        public bool activeWhenUninstalled = false;
        /// <summary>
        /// if not null. the bolt will be parented to this transform instead of the boltParent gameObject on the bolt's part.
        /// </summary>
        public Transform customParent = null;
        /// <summary>
        /// If true, parents the bolt to the trigger at the specified index, <see cref="parentBoltToTriggerIndex"/>.
        /// </summary>
        public bool parentBoltToTrigger = false;
        /// <summary>
        /// Represents the trigger index to parent the bolt on.
        /// </summary>
        public int parentBoltToTriggerIndex = 0;

        /// <summary>
        /// Initializes new instance of b settings with default values.
        /// </summary>
        public BoltSettings() { }
        /// <summary>
        /// inits new instance of bolt settings and copies instance values.
        /// </summary>
        /// <param name="s">the instance of bolt settings to copy.</param>
        public BoltSettings(BoltSettings s)
        {
            if (s != null)
            {
                name = s.name;
                boltType = s.boltType;
                boltSize = s.boltSize;
                posStep = s.posStep;
                rotStep = s.rotStep;
                tightnessStep = s.tightnessStep;
                maxTightness = s.maxTightness;
                posDirection = s.posDirection;
                rotDirection = s.rotDirection;
                customBoltPrefab = s.customBoltPrefab;
                ignoreTightnessChangedCheck = s.ignoreTightnessChangedCheck;
                addNut = s.addNut;
                addNutSettings = new AddNutSettings(s.addNutSettings);
                highlightBoltWhenActive = s.highlightBoltWhenActive;
                activeWhenUninstalled = s.activeWhenUninstalled;
                customParent = s.customParent;
                parentBoltToTrigger = s.parentBoltToTrigger;
                parentBoltToTriggerIndex = s.parentBoltToTriggerIndex;
            }
        }
    }
}
