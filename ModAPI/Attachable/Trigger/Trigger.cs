using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents a trigger for a part.
    /// </summary>
    public class Trigger : IHasBolts
    {
        // Written, 10.08.2018 | Updated, 11.2021

        #region Events

        /// <summary>
        /// Represents the on part assembled event. occurs when a part is assembled to this trigger (instance). occurs after assembly logic executes
        /// </summary>
        public event Action<Trigger, Part> onPartAssembledToTrigger;
        /// <summary>
        /// Represents the on part disassembled event. occurs when a part is disassembled from this trigger (instance). occurs after disassembly logic executes
        /// </summary>
        public event Action<Trigger, Part> onPartDisassembledFromTrigger;
        /// <summary>
        /// Represents the on part pre assembled event. occurs when a part is assembled to this trigger (instance). occurs after index of trigger is evaluated but before any assembly logic executes.
        /// </summary>
        public event Action<Trigger, Part> onPartPreAssembledToTrigger;
        /// <summary>
        /// Represents the on part pre disassembled event. occurs when a part is disassembled from this trigger (instance). occurs before any disassembly logic executes.
        /// </summary>
        public event Action<Trigger, Part> onPartPreDisassembledFromTrigger;

        #endregion

        #region Static Fields

        /// <summary>
        /// Represents all loaded/created triggers. in GAME
        /// </summary>
        public static List<Trigger> loadedTriggers = new List<Trigger>();
        /// <summary>
        /// Represents All <see cref="Trigger"/>s that are in game. by trigger type (<see cref="TriggerData"/>) then by key (<see cref="Trigger.triggerID"/>)
        /// </summary>
        public static Dictionary<TriggerData, Dictionary<string, Trigger>> triggerDictionary = new Dictionary<TriggerData, Dictionary<string, Trigger>>();

        #endregion

        #region Private Fields

        /// <summary>
        /// Represents all for the trigger.
        /// </summary>
        private Bolt[] _bolts;
        /// <summary>
        /// Represents if this trigger has any bolts!
        /// </summary>
        private bool _hasBolts = false;
        /// <summary>
        /// Represents the trigger game object.
        /// </summary>
        private GameObject _trigger;
        /// <summary>
        /// Represents the trigger callback reference on the triggers gameobject.
        /// </summary>
        private TriggerCallback _callback;
        /// <summary>
        /// Represents the triggers collider.
        /// </summary>
        private SphereCollider _collider;
        /// <summary>
        /// Represents the install point gameobject. the trigger and part pivot are children of this gameobject.
        /// </summary>
        private GameObject _installPoint;
        /// <summary>
        /// Represents the parts pivot. parts that are installed to this trigger will be parented here when installed.
        /// </summary>
        private GameObject _partPivot;
        /// <summary>
        /// Represents the triggers bolt parent. all bolts on the part that are only relevant to this trigger are parented to this gameobject. Null if no bolts are assigned to this trigger.
        /// </summary>
        private GameObject _boltParent;
        /// <summary>
        /// Represents settings for the trigger.
        /// </summary>
        private TriggerSettings _settings;
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Represents the trigger game object.
        /// </summary>
        public GameObject gameObject => _trigger;
        /// <summary>
        /// Represents the trigger callback reference on the triggers gameobject.
        /// </summary>
        public TriggerCallback callback => _callback;
        /// <summary>
        /// Represents the install point gameobject. the trigger and part pivot are children of this gameobject.
        /// </summary>
        public GameObject installPoint => _installPoint;
        /// <summary>
        /// Represents the parts pivot. parts that are installed to this trigger will be parented here when installed.
        /// </summary>
        public GameObject partPivot => _partPivot;
        /// <summary>
        /// Represents the triggers bolt parent. all bolts on the part that are only relevant to this trigger are parented to this gameobject. Null if no bolts are assigned to this trigger.
        /// </summary>
        public GameObject boltParent => _boltParent;        
        /// <summary>
        /// Represents all for the trigger.
        /// </summary>
        public Bolt[] bolts => _bolts;
        /// <summary>
        /// Represents if this trigger has any bolts!
        /// </summary>
        public bool hasBolts => _hasBolts;
        /// <summary>
        /// Represents the triggers position.
        /// </summary>
        public Vector3 triggerPosition => _trigger.transform.localPosition;
        /// <summary>
        /// Represents the triggers rotation.
        /// </summary>
        public Vector3 triggerEuler => _trigger.transform.localEulerAngles;
        /// <summary>
        /// Represents the triggers scale.
        /// </summary>
        public Vector3 triggerSize => _trigger.transform.localScale;
        /// <summary>
        /// Represents the pivot position.
        /// </summary>
        public Vector3 pivotPosition => _partPivot.transform.localPosition;
        /// <summary>
        /// Represents the pivot rotation.
        /// </summary>
        public Vector3 pivotEuler => _partPivot.transform.localEulerAngles;
        /// <summary>
        /// Represents the triggers name.
        /// </summary>
        public string triggerID => _settings.triggerID;
        /// <summary>
        /// Represents the triggers collider.
        /// </summary>
        public SphereCollider collider => _collider;
        /// <summary>
        /// Represents settings for the trigger.
        /// </summary>
        public TriggerSettings settings => _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of trigger. Note, Make sure to set up the trigger! <see cref="initTrigger"/>
        /// </summary>
        public Trigger() { }

        /// <summary>
        /// Initializes a new instance and creates a trigger at the provided position, rotation and scale.
        /// </summary>
        /// <param name="parent">The parent transform of the install point (part pivot and trigger).</param>
        /// <param name="settings"></param>
        /// <param name="boltRefs"></param>
        public Trigger(GameObject parent, TriggerSettings settings, Bolt[] boltRefs = null)
        {
            // Written, 21.08.2023

            initTrigger(parent, settings);
            initBolts(boltRefs);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Initializes the trigger.
        /// </summary>
        /// <param name="parent">The parent transform of the install point (part pivot and trigger).</param>
        /// <param name="settings">Trigger settings.</param>
        public void initTrigger(GameObject parent, TriggerSettings settings)
        {
            // Written, 04.10.2018 | Updated, 09.2021 | 08.11.2022 | 20.08.2023

            createInstallPointGameObject(parent.transform);
            createTriggerGameObject(settings.triggerData);

            loadSettings(settings);

            Dictionary<string, Trigger> dictionary;
            if (triggerDictionary.ContainsKey(settings.triggerData))
            {
                dictionary = triggerDictionary[settings.triggerData];
            }
            else
            {
                dictionary = new Dictionary<string, Trigger>();
                triggerDictionary.Add(settings.triggerData, dictionary);
            }
            dictionary.Keys.setID<string>(ref _settings.triggerID, t => t.Contains(_settings.triggerID));


            dictionary.Add(_settings.triggerID, this);
            triggerDictionary[settings.triggerData] = dictionary;
            loadedTriggers.Add(this);
        }

        private void loadSettings(TriggerSettings settings)
        {
            // Written, 11.09.2023

            _settings = new TriggerSettings(settings);

            _settings.triggerID = _settings.triggerID.Replace("Trigger", "").Replace("trigger", "").Replace(" ", "");

            _trigger.transform.localPosition = _settings.triggerPosition;
            _trigger.transform.localEulerAngles = _settings.triggerEuler;
            _trigger.transform.localScale = Vector3.one;

            _collider.radius = _settings.triggerRadius;

            if (_settings.useTriggerTransformData)
            {
                _partPivot.transform.localPosition = _settings.triggerPosition;
                _partPivot.transform.localEulerAngles = _settings.triggerEuler;
            }
            else
            {
                _partPivot.transform.localPosition = _settings.pivotPosition;
                _partPivot.transform.localEulerAngles = _settings.pivotEuler;
            }
        }

        /// <summary>
        /// Gets trigger save info 
        /// </summary>
        public virtual TriggerSaveInfo getSaveInfo()
        {
            // Written, 27.08.2022

            TriggerSaveInfo info = new TriggerSaveInfo();

            info.boltTightness = Bolt.getBoltSaveInfo(_bolts, _hasBolts);

            return info;
        }
        /// <summary>
        /// Changes <see cref="Collider.enabled"/>. (<see cref="_collider"/>)
        /// </summary>
        /// <param name="enabled">Set enabled or not</param>
        public void enableTriggerCollider(bool enabled)
        {
            // Written, 11.09.2023

            _collider.enabled = enabled;
        }
        /// <summary>
        /// Enables / Disables disassembly logic.
        /// </summary>
        /// <param name="enabled">Set enabled or not</param>
        public void enableDisassemblyLogic(bool enabled)
        {
            // Written, 11.09.2023

            _callback.enabled = enabled;
        }
        /// <summary>
        /// initializes bolts.
        /// </summary>
        /// <param name="bolts"></param>
        public void initBolts(Bolt[] bolts)
        {
            if (bolts != null && bolts.Length > 0)
            {
                _bolts = bolts;
                _hasBolts = true;
                _boltParent = Bolt.createBoltParentGameObject("TriggerBolts", partPivot.transform);
                                
                int[] infos = ModClient.getPartManager.partSaveManager.loadBolts(triggerID);

                for (int i = 0; i < _bolts.Length; i++)
                {
                    int tightness = infos?[i] ?? 0;
                    _bolts[i].createBolt(tightness,_boltParent.transform);
                }
                Bolt.postVaildiateBolts(_bolts);
            }
        }
        /// <summary>
        /// Exposes <see cref="onPartAssembledToTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        internal void invokeAssembledEvent(Part assembledPart)
        {
            // Written, 12.06.2022

            onPartAssembledToTrigger?.Invoke(this, assembledPart);
        }
        /// <summary>
        /// Exposes <see cref="onPartDisassembledFromTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>rrr
        internal void invokeDisassembledEvent(Part disassembledPart)
        {
            // Written, 12.06.2022

            onPartDisassembledFromTrigger?.Invoke(this, disassembledPart);
        }
        /// <summary>
        /// Exposes <see cref="onPartPreAssembledToTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        internal void invokePreAssembledEvent(Part assembledPart)
        {
            // Written, 12.06.2022

            _callback.setPart(assembledPart);

            if (hasBolts)
            {
                for (int i = 0; i < bolts.Length; i++)
                {
                    bolts[i].onScrew += assembledPart.boltOnScrew;
                    bolts[i].activateBolt(true);
                }
                boltParent.transform.SetParent(assembledPart.transform, false);
                boltParent.transform.localPosition = Vector3.zero;
                boltParent.transform.localEulerAngles = Vector3.zero;
            }

            onPartPreAssembledToTrigger?.Invoke(this, assembledPart);
        }
        /// <summary>
        /// Exposes <see cref="onPartPreDisassembledFromTrigger"/>'s <see cref="Action.Invoke"/> method.
        /// </summary>
        internal void invokePreDisassembledEvent(Part disassembledPart)
        {
            // Written, 12.06.2022

            if (hasBolts)
            {
                for (int i = 0; i < bolts.Length; i++)
                {
                    bolts[i].onScrew -= disassembledPart.boltOnScrew;
                    bolts[i].setTightnessAndUpdateModel(0);
                    bolts[i].activateBolt(false);
                }
                boltParent.transform.SetParent(partPivot.transform, false);
            }
            _callback.setPart(null);
            onPartPreDisassembledFromTrigger?.Invoke(this, disassembledPart);
        }

        private void createTriggerGameObject(TriggerData triggerData)
        {
            // Written, 08.10.2022

            // GameObject
            _trigger = new GameObject("Trigger", typeof(SphereCollider));
            _trigger.transform.SetParent(_installPoint.transform, false);
            // Collider
            _collider = _trigger.GetComponent<SphereCollider>();
            _collider.isTrigger = true;
            // Callback
            _callback = _trigger.AddComponent<TriggerCallback>();
            _callback.trigger = this;
            _callback.triggerData = triggerData;
        }

        /// <summary>
        /// Creates and assigns <see cref="_installPoint"/>.
        /// </summary>
        private void createInstallPointGameObject(Transform parent)
        {
            // Written, 11.07.2022

            _installPoint = new GameObject("InstallPoint");
            _installPoint.transform.parent = parent;
            _installPoint.transform.localPosition = Vector3.zero;
            _installPoint.transform.localEulerAngles = Vector3.zero;
            _installPoint.transform.localScale = Vector3.one;

            _partPivot = new GameObject("Pivot");
            _partPivot.transform.parent = _installPoint.transform;
            _partPivot.transform.localScale = Vector3.one;
        }

        #endregion
    }
}
