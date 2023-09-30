using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MSCLoader.ModConsole;
using static UnityEngine.GUILayout;
using MSCLoader;
using TanjentOGG;
using HutongGames.PlayMaker.Actions;
using TommoJProductions.ModApi;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents an installable and boltable part.
    /// </summary>
    public class Part : MonoBehaviour, IHasBolts
    {
        // Written, 27.10.2018 | Updated, 09.2021

        #region Events

        /// <summary>
        /// Represents the on assemble event.invoked after assembly logic executes
        /// </summary>
        public event Action onAssemble;
        /// <summary>
        /// Represents the on disassemble event. invoked after disassembly logic executes
        /// </summary>
        public event Action onDisassemble;
        /// <summary>
        /// Represents the on pre assemble event. invoked before assembly logic executes
        /// </summary>
        public event Action onPreAssemble;
        /// <summary>
        /// Represents the on pre disassemble event. invoked before disassembly logic executes
        /// </summary>
        public event Action onPreDisassemble;
        /// <summary>
        /// Represents the on pre init part event. invoked after fields are assigned but before init part logic has run.
        /// </summary>
        public event Action onPreInitPart;
        /// <summary>
        /// Represents the on post init part event. invoked after init part logic has run.
        /// </summary>
        public event Action onPostInitPart;
        /// <summary>
        /// Represents the on part bolted. invoked when the part has been completely bolted..
        /// </summary>
        public event Action onPartBolted;
        /// <summary>
        /// Represents the on part unbolted. invoked when the part has been completely unbolted.
        /// </summary>
        public event Action onPartUnBolted;
        /// <summary>
        /// Represents the on bolt screwed. occurs when any of the bolts have been screwed (up or down).
        /// </summary>
        public event Action onBoltScrew;
        /// <summary>
        /// Represents the on part picked up event. occurs when this part has been picked up.
        /// </summary>
        public event Action onPartPickUp;
        /// <summary>
        /// Represents the on part thrown event. occurs when this part has been thrown.
        /// </summary>
        public event Action onPartThrow;
        /// <summary>
        /// Represents the on part dropped event. occurs when this part has been dropped.
        /// </summary>
        public event Action onPartDrop;
        /// <summary>
        /// Represents the on joint break event. occurs when this parts Joint breaks. invoked after disassembly logic executes.
        /// </summary>
        public event Action<float> onJointBreak;

        #endregion

        #region Fields

        private PartSaveInfo _defaultSaveInfo = new PartSaveInfo();

        private string _partID = "part";
        private bool _initialized;
        private bool _mouseOver;
        private bool _inTrigger;
        private bool _pickedUp;
        private bool _inherentlyPickedUp;
        private bool _hasBolts;
        private float _cachedMass;
        private float _tightnessStep;
        private float _maxTightnessTotal;
        private float _tightnessTotal;
        private PartSettings _settings;
        private PartSaveInfo _loadedSaveInfo;
        private TriggerData _triggerData;
        private Trigger _installPoint;
        private Bolt[] _bolts;
        private GameObject _boltParent;
        private Rigidbody _cachedRigidBody;
        private FixedJoint _joint;
        private Coroutine _triggerRoutine;

        #endregion

        #region Properties

        /// <summary>
        /// Represents the default save info for this part.
        /// </summary>
        public PartSaveInfo defaultSaveInfo
        {
            get => _defaultSaveInfo;
            set => _defaultSaveInfo = PartSaveInfo.copy(value);
        }
        /// <summary>
        /// Represents the parts settings.
        /// </summary>
        public PartSettings partSettings => _settings;
        /// <summary>
        /// Represents the runtime id of this part.
        /// </summary>
        public string partID => _partID;
        /// <summary>
        /// Represents if the part is completely bolted or not.
        /// </summary>
        public bool bolted => _tightnessTotal == _maxTightnessTotal;
        /// <summary>
        /// Represents if the part is installed or not.
        /// </summary>
        public bool installed => _loadedSaveInfo.installed;
        /// <summary>
        /// Represents if mouse over activated.
        /// </summary>
        public bool mouseOver => _mouseOver;
        /// <summary>
        /// Represents if this part has any bolts!
        /// </summary>
        public bool hasBolts => _hasBolts;
        /// <summary>
        /// <see langword="true"/> if this part or the install point its installed to has bolts.
        /// </summary>
        public bool hasAnyBolts => _hasBolts || (_loadedSaveInfo.installed ? _installPoint.hasBolts : false);
        /// <summary>
        /// Represents if this part is inherently picked up. (is a child of a part that is currently picked up)
        /// </summary>
        public bool inherentlyPickedUp
        { 
            get => _inherentlyPickedUp;
            internal set => _inherentlyPickedUp = value; 
        }
        /// <summary>
        /// Represents if this part is currently picked up.
        /// </summary>
        public bool pickedUp
        {
            get => _pickedUp;
            internal set => _pickedUp = value;
        }
        /// <summary>
        /// Represents if the part is in a trigger.
        /// </summary>
        public bool inTrigger => _inTrigger;
        /// <summary>
        /// Represents the  tightness step. calculated in <see cref="updateJointBreakForce"/>. how much the breakforce of the install point joint increases per bolt screw.
        /// </summary>
        public float tightnessStep => _tightnessStep;
        /// <summary>
        /// Represents max tightness of all bolts.(8 * <see cref="bolts"/>.Length). if any present on part. (<see cref="hasBolts"/>)
        /// </summary>
        public float maxTightnessTotal => _maxTightnessTotal;
        /// <summary>
        /// Represents tightness of all bolts (<see cref="BoltSaveInfo.boltTightness"/> * <see cref="bolts"/>.Length). if any present on part. (<see cref="hasBolts"/>)
        /// </summary>
        public float tightnessTotal => _tightnessTotal;

        /// <summary>
        /// Represents all bolts for the part.
        /// </summary>
        public Bolt[] bolts => _bolts;
        /// <summary>
        /// Represents the bolt parent. all bolts will be a child of this game object. NOTE null if no bolts are on this part or the part has bolts but none of them are relevant to the part alone. bolts could be assigned to a trigger.
        /// </summary>
        public GameObject boltParent => _boltParent;
        /// <summary>
        /// Represents the Parts Trigger type. The part will be installable to any Trigger with the same trigger type. (<see cref="TriggerData"/>)
        /// </summary>
        public TriggerData triggerData => _triggerData;
        /// <summary>
        /// Represents the fixed joint when using <see cref="AssembleType.joint"/> and installed.
        /// </summary>
        public FixedJoint joint => _joint;
        /// <summary>
        /// Represents the trigger routine.
        /// </summary>
        internal Coroutine triggerRoutine => _triggerRoutine;
        /// <summary>
        /// Represents the cached rigidbody.
        /// </summary>
        public Rigidbody cachedRigidBody => _cachedRigidBody;
        /// <summary>
        /// Represents the install point that the part is installed to. (Trigger).
        /// </summary>
        public Trigger installPoint => _installPoint;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Represents the joint break runtime call
        /// </summary>
        protected virtual void OnJointBreak(float breakForce)
        {
            disassemble();
            onJointBreak?.Invoke(breakForce);
        } 

        #endregion

        #region IEnumerators

        /// <summary>
        /// Represents the part in a trigger logic.
        /// </summary>
        /// <param name="trigger">The trigger callback that invoked this call.</param>
        public virtual IEnumerator partInTrigger(Trigger trigger)
        {
            // Written, undocumented | Modified, 02.07.2022

            while (_inTrigger && holdingCheck(trigger))
            {                
                yield return null;
                ModClient.guiAssemble = true;
                if (Input.GetMouseButtonDown(0))
                {
                    assemble(trigger);
                    break;
                }
            }
            ModClient.guiAssemble = false;
            stopTriggerRoutine();
        }       

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this part. with save info reference
        /// </summary>
        /// <param name="saveInfo">The save info to load this part with.</param>
        /// <param name="partSettingsRef">The part settings to load this part with.</param>
        /// <param name="triggerData">The Trigger Data. This Part can be installed to triggers that have the same Trigger Data.</param>
        /// <param name="boltRefs">The bolts for this part</param>
        public virtual void initPart(PartSaveInfo saveInfo, TriggerData triggerData, PartSettings partSettingsRef = default, Bolt[] boltRefs = null)
        {
            // Written, 08.09.2021

            vaildiatePart(partSettingsRef);

            _triggerData = triggerData;

            setPartID();
            _settings = new PartSettings(partSettingsRef);
            _cachedRigidBody = GetComponent<Rigidbody>();
            if (_cachedRigidBody)
            {
                _cachedMass = _cachedRigidBody.mass;
            }
            else
            {
                _cachedMass = 1;
            }

            setLoadedSaveInfo(saveInfo);

            onPreInitPart?.Invoke();

            if (_loadedSaveInfo.installed && _loadedSaveInfo.installPointId != null)
            {
                if (!Trigger.triggerDictionary[triggerData].TryGetValue(_loadedSaveInfo.installPointId, out _installPoint))
                {
                    Debug.Log($"[ModAPI] could not find install point '{_loadedSaveInfo.installPointId}' ({_partID})");
                    _loadedSaveInfo.installed = false;
                    _loadedSaveInfo.installPointId = null;
                }
            }

            initBolts(boltRefs);
            initBolts();

            if (_loadedSaveInfo.installed)
            {
                assemble(_installPoint, false);
            }
            else
            {
                makePartPickable(true);

                if (_settings.setPositionRotationOnInitialisePart)
                {
                    transform.position = _loadedSaveInfo.position;
                    transform.eulerAngles = _loadedSaveInfo.rotation;
                }
            }

            setPhysicsMaterials();

            ModClient.loadedParts.Add(this);
            _initialized = true;
            onPostInitPart?.Invoke();
        }

        /// <summary>
        /// Initializes this part with bolts and without save info reference.
        /// </summary>
        /// <param name="partSettingsRef">The part settings to load this part with.</param>
        /// <param name="boltRefs">The bolts for this part</param>
        /// <param name="triggerData">The Trigger Data. This Part can be installed to triggers that have the same Trigger Data.</param>
        public virtual void initPart(TriggerData triggerData, PartSettings partSettingsRef = default, Bolt[] boltRefs = null)
        {
            // Written, 19.08.2023

            initPart(null, triggerData, partSettingsRef, boltRefs);
        }
        /// <summary>
        /// installs this part to the install point.
        /// </summary>
        /// <param name="installPoint">The install point</param>
        /// <param name="playSound">Play assemble sound?</param>
        public virtual void assemble(Trigger installPoint, bool playSound = true)
        {
            // Written, 10.08.2018 | Modified, 09.06.2022

            if (installPoint.callback.part)
            {
                Debug.LogWarning($"[ModAPI] '{partID}' attempted to install to '{installPoint.triggerID}' but '{installPoint.callback.part.partID}' is installed there.");
                _loadedSaveInfo.installed = false;
                _loadedSaveInfo.installPointId = null;
                return;
            }

            onPreAssemble?.Invoke();
            _installPoint = installPoint;
            _installPoint.invokePreAssembledEvent(this);
            _loadedSaveInfo.installed = true;
            transform.parent = _installPoint.partPivot.transform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            if (hasAnyBolts)
            {
                _bolts.activateBolts(true);
            }
            enableColliderTrigger(false);
            switch (_settings.assembleType)
            {
                case AssembleType.static_rigidbodyDelete:
                    if (_cachedRigidBody)
                    {
                        _cachedMass = _cachedRigidBody.mass;
                        Destroy(_cachedRigidBody);
                    }
                    break;
                case AssembleType.static_rigidibodySetKinematic:
                    _cachedRigidBody.isKinematic = true;
                    break;
                case AssembleType.joint:
                    _joint = gameObject.AddComponent<FixedJoint>();
                    Rigidbody rb = _installPoint.installPoint.GetComponentInParent<Rigidbody>();
                    if (rb)
                    {
                        _joint.connectedBody = rb;
                    }
                    else
                    {
                        throw new Exception($"[Assemble.{name}] (Joint) Error assigning connected body. could not find a rigidbody in parent.");
                    }
                    updateJointBreakForce();
                    break;
            }
            makePartPickable(false);

            if (playSound)
            {
                ModClient.playSoundAt(transform, "CarBuilding", "assemble");
            }
            _installPoint.invokeAssembledEvent(this);
            onAssemble?.Invoke();
        }
        /// <summary>
        /// Disassemble this part from the installed point
        /// </summary>
        public virtual void disassemble(bool playSound = true)
        {
            // Written, 10.08.2018 | Modified, 25.09.2021 | 09.06.2022

            onPreDisassemble?.Invoke();
            _installPoint.invokePreDisassembledEvent(this);
            if (mouseOver)
            {
                mouseOverGuiDisassembleEnable(false);
            }

            transform.parent = null;

            if (hasAnyBolts)
            {
                _bolts.activateBolts(b => func(b));

                resetTotalBoltTightness();
                enableDisassembleLogic(true);

                bool func(Bolt b)
                {
                    b.setTightnessAndUpdateModel(0);
                    return false;
                }
            }
            enableColliderTrigger(true);

            _loadedSaveInfo.installed = false;

            switch (_settings.assembleType)
            {
                case AssembleType.static_rigidbodyDelete:
                    if (!_cachedRigidBody)
                    {
                        _cachedRigidBody = gameObject.AddComponent<Rigidbody>();
                    }
                    _cachedRigidBody.mass = _cachedMass;
                    break;
                case AssembleType.static_rigidibodySetKinematic:
                    _cachedRigidBody.isKinematic = false;
                    break;
                case AssembleType.joint:
                    Destroy(_joint);
                    break;
            }
            makePartPickable(true);

            if (playSound)
            {
                ModClient.playSoundAt(transform, "CarBuilding", "disassemble");
            }

            _installPoint.invokeDisassembledEvent(this);
            onDisassemble?.Invoke();
        }
        /// <summary>
        /// Gets save info for this part and its bolts (if the part has any). (pos, rot, installed, install index, tightness)
        /// </summary>
        public virtual PartSaveInfo getSaveInfo(bool getBoltInfo = true)
        {
            // Written, 18.10.2018 | Modified, 11.07.2022

            PartSaveInfo info = PartSaveInfo.copy(_loadedSaveInfo);
            info.position = transform.position;
            info.rotation = transform.eulerAngles;
            if (info.installed)
            {
                info.installPointId = _installPoint.triggerID;
            }
            else
            {
                info.installPointId = null;
            }
            if (getBoltInfo)
            {
                info.boltTightness = Bolt.getBoltSaveInfo(_bolts, _hasBolts);
            }
            return info;
        }  
        /// <summary>
        /// Sets the triggers collider enable (install point) to <paramref name="active"/>.
        /// </summary>
        /// <param name="active">active or not [part]</param>
        public virtual void enableColliderTrigger(bool active)
        {
            // Written, 18.09.2021 | Modified, 23.10.2021

            _installPoint.enableTriggerCollider(active);
        }
        /// <summary>
        /// Sets the triggers disasemble logic enable (install point).
        /// </summary>
        /// <param name="active">active or not [part]</param>
        public virtual void enableDisassembleLogic(bool active)
        {
            // Written, 11.09.2023

            _installPoint.enableDisassemblyLogic(active);
        }

        /// <summary>
        /// Makes the part a pickable item depending on the provided values.
        /// </summary>
        /// <param name="inPickable">Make part pickable?</param>
        public void makePartPickable(bool inPickable)
        {
            // Written, 14.08.2018 | Modified, 30.09.2021 | Modified, 04.06.2022

            if (inPickable)
            {
                gameObject.tag = "PART";
                gameObject.layer = _settings.notInstalledPartToLayer.layer();

                if (_cachedRigidBody)
                    _cachedRigidBody.collisionDetectionMode = _settings.collisionSettings.notInstalledCollisionDetectionMode;
            }
            else
            {
                gameObject.tag = "DontPickThis";
                gameObject.layer = _settings.installedPartToLayer.layer();

                if (_cachedRigidBody)
                    _cachedRigidBody.collisionDetectionMode = _settings.collisionSettings.installedCollisionDetectionMode;
            }
        }
        /// <summary>
        /// ends or starts a disassembly gui interaction.
        /// </summary>
        public void mouseOverGuiDisassembleEnable(bool enable)
        {
            // Written, 04.10.2018 | Modified, 18.09.2021

            _mouseOver = enable;
            ModClient.guiDisassemble = enable;
        }
        /// <summary>
        /// resets all bolts tightness on this part
        /// </summary>
        public void resetBoltTightness() 
        {
            // Written, 25.08.2022

            resetTotalBoltTightness();
            if (_hasBolts)
            {
                for (int i = 0; i < _bolts.Length; i++)
                {
                    _bolts[i].setTightnessAndUpdateModel(0);
                }
            }
            if (_installPoint.hasBolts)
            {
                for (int i = 0; i < _installPoint.bolts.Length; i++)
                {
                    _installPoint.bolts[i].setTightnessAndUpdateModel(0);
                }
            }
            updateJointBreakForce();
            enableDisassembleLogic(true);
            onPartUnBolted?.Invoke();
        }
        /// <summary>
        /// sets all bolt tightness to the max tightness.
        /// </summary>
        public void setMaxBoltTightness()
        {
            // Written, 25.08.2022

            resetTotalBoltTightness();

            if (_hasBolts)
            {
                for (int i = 0; i < _bolts.Length; i++)
                {
                    _tightnessTotal += bolts[i].maxTightness;
                    _bolts[i].setTightnessAndUpdateModel(bolts[i].maxTightness);
                }
            }
            if (_installPoint.hasBolts)
            {
                for (int i = 0; i < _installPoint.bolts.Length; i++)
                {
                    _tightnessTotal += _installPoint.bolts[i].maxTightness;
                    _installPoint.bolts[i].setTightnessAndUpdateModel(_installPoint.bolts[i].maxTightness);
                }
            }
            updateJointBreakForce();
            enableDisassembleLogic(false);
            mouseOverGuiDisassembleEnable(false);
            onPartBolted?.Invoke();

        }

        /// <summary>
        /// Vaildates the part and reports to mod console. called at: <see cref="initPart(PartSaveInfo, TriggerData, PartSettings, Bolt[])"/>.
        /// </summary>
        protected virtual void vaildiatePart(PartSettings partSettings)
        {
            // Written, 25.09.2021

            string e = String.Empty;

            if (_initialized)
            {
                e += $"error: Part, {name} is already initialized\n";
            }
            if (!ModClient.getBoltManager.assetsLoaded)
            {
                e += $"- Bolt assets aren't loaded. refused to initialize part, {name}\n";
            }
            if (e != String.Empty)
            {
                Error(e);
                throw new Exception(e);
            }
        }
        /// <summary>
        /// Updates <see cref="_tightnessTotal"/> from current state of <see cref="_bolts"/>. see: <see cref="BoltSaveInfo.boltTightness"/> and <see cref="BoltSaveInfo.addNutTightness"/>
        /// </summary>
        internal protected virtual void updateTotalBoltTightness() 
        {
            // Written, 10.07.2022

            resetTotalBoltTightness();

            updateMaxTotalBoltTightness();

            if (_hasBolts)
            {
                for (int i = 0; i < _bolts.Length; i++)
                {
                    addBoltTightness(_bolts[i]);
                }
            }

            if (_installPoint == null)
                return;

            if (_installPoint.hasBolts)
            {
                for (int i = 0; i < _installPoint.bolts.Length; i++)
                {
                    addBoltTightness(_installPoint.bolts[i]);
                }
            }

            void addBoltTightness(Bolt b)
            {
                _tightnessTotal += b.tightness;

                /*if (b.addNut)
                {
                    _tightnessTotal += b.nut.tightness;
                    _maxTightnessTotal += 8;
                }*/
            }
        }
        /// <summary>
        /// updates the max tightness of the bolts.
        /// </summary>
        protected virtual void updateMaxTotalBoltTightness() 
        {
            // Written, 24.09.2023

            _maxTightnessTotal = 0;

            if (_hasBolts)
            {
                for (int i = 0; i < _bolts.Length; i++)
                {
                    _maxTightnessTotal += bolts[i].maxTightness;
                }
            }

            if (_installPoint == null)
                return;

            if (_installPoint.hasBolts)
            {
                for (int i = 0; i < _installPoint.bolts.Length; i++)
                {
                    _maxTightnessTotal += _installPoint.bolts[i].maxTightness;
                }
            }
        }
        /// <summary>
        /// Updates joint breakforce. (bolts)
        /// </summary>
        internal protected virtual void updateJointBreakForce()
        {
            // Written, 08.07.2022

            if (_joint)
            {
                if (hasAnyBolts && _settings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce)
                {
                    _tightnessStep = (_settings.assemblyTypeJointSettings.breakForce - _settings.assemblyTypeJointSettings.breakForceMin) / _maxTightnessTotal;
                    _joint.breakForce = _tightnessStep * _tightnessTotal + _settings.assemblyTypeJointSettings.breakForceMin;
                }
                else
                {
                    _joint.breakForce = _settings.assemblyTypeJointSettings.breakForce;
                }
                _joint.breakTorque = _joint.breakForce / 2;
            }
        }  
        /// <summary>
        /// checks all bolts for their <see cref="Bolt.tightness"/>.
        /// invokes, <see cref="enableDisassembleLogic(bool)"/> and
        /// invokes <see cref="onPartBolted"/>, <see cref="onPartUnBolted"/> events.
        /// </summary>
        protected virtual void boltTightnessCheck() 
        {
            // Written, 08.07.2022

            if (_tightnessTotal >= _maxTightnessTotal * Mathf.Clamp(_settings.tightnessThreshold, 0.25f, 1))
            {
                enableDisassembleLogic(false);
                mouseOverGuiDisassembleEnable(false);
            }
            if (bolted)
            {
                onPartBolted?.Invoke();
            }            
            if (_tightnessTotal == 0)
            {
                enableDisassembleLogic(true);
                onPartUnBolted?.Invoke();
            }
        }
        /// <summary>
        /// Vaildiates bolt refs. called prior to initialization of bolts on the part.
        /// </summary>
        /// <param name="boltRefs">the bolt refs to vaildiate.</param>
        /// <returns>True if no issues were detected with the bolt references.</returns>
        protected virtual bool vaildiateBolts(Bolt[] boltRefs)
        {
            // Written, 08.10.2022

            string error = String.Empty;

            if (boltRefs != null)
            {
                if (boltRefs.Length == 0)
                {
                    error += "initBolts was called but no bolts were found (array length was zero)\n";
                }
            }
            else
            {
                error += "initBolts was called but no bolts were found (boltRef array was null)\n";
            }

            if (error != String.Empty)
            {
                Error($"Error initialializing Part, {name}. {error}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Check for deciding if part is in trigger to be installed
        /// </summary>
        /// <param name="trigger">the trigger.</param>
        protected virtual bool colliderCheck(Trigger trigger)
        {
            // Written, 09.06.2022

            if (!_settings.assembleCollider)
                return true;
            return trigger.gameObject == _settings.assembleCollider.gameObject;
        }
        /// <summary>
        /// Represents the holding check for a <see cref="Part"/>. Used for starting and checking trigger routine. <see cref="partInTrigger(Trigger)"/>
        /// </summary>
        /// <param name="trigger">The trigger</param>
        protected virtual bool holdingCheck(Trigger trigger) 
        {
            // Written, 02.07.2022

            if (!_settings.installEitherDirection)
            {
                return _pickedUp;
            }
            return _pickedUp || (trigger.gameObject.getBehaviourInParent<Part>(part => !part._loadedSaveInfo.installed)?._pickedUp ?? false);
        }

        /// <summary>
        /// Inits bolts.
        /// </summary>
        private void initBolts()
        {
            if (!hasAnyBolts)
                return;

            bool installed = _loadedSaveInfo.installed;

            updateTotalBoltTightnessAndActivate(installed);

            if (installed)
            {
                boltTightnessCheck();
            }
        }
        private void updateTotalBoltTightnessAndActivate(bool active)
        {
            resetTotalBoltTightness();

            if (active)
            {
                updateMaxTotalBoltTightness();
            }

            _bolts.activateBolts(b => f(b));

            if (_installPoint == null)
                return;

            _installPoint.bolts.activateBolts(b => f(b));

            bool f(Bolt b)
            {
                if (active)
                {
                    _tightnessTotal += b.tightness;
                }
                return active;
            }
        }
        /// <summary>
        /// Updates all bolt tightness.
        /// </summary>
        /// <param name="tightness">the tightness to set all bolts</param>
        private void setBoltTightness(int tightness, Action<Bolt> func = null)
        {
            // Written, 25.08.2022

            if (_hasBolts)
            {
                for (int i = 0; i < _bolts.Length; i++)
                {
                    func?.Invoke(_bolts[i]);
                    _bolts[i].setTightnessAndUpdateModel(tightness);
                }
            }
            if (_installPoint.hasBolts)
            {
                for (int i = 0; i < _installPoint.bolts.Length; i++)
                {
                    func?.Invoke(_installPoint.bolts[i]);
                    _installPoint.bolts[i].setTightnessAndUpdateModel(tightness);
                }
            }
            updateJointBreakForce();
        }
        /// <summary>
        /// Stops the trigger coroutine and sets <see cref="_inTrigger"/> to false. brute force
        /// </summary>
        private void stopTriggerRoutine()
        {
            // Written, 10.07.2022

            _inTrigger = false;
            StopCoroutine(_triggerRoutine);
            _triggerRoutine = null;
        }
        private void setLoadedSaveInfo(PartSaveInfo saveInfo)
        {
            // Written, 03.09.2023

            PartSaveInfo info;

            if (_settings.autoSave)
            {
                info = ModClient.getPartManager.partSaveManager.loadPart(_partID);
            }
            else
            {
                info = saveInfo;
            }

            if (info == null)
            {
                _loadedSaveInfo = _defaultSaveInfo;
                return;
            }
            _loadedSaveInfo = info.copy();
        }
        private void setPartID()
        {
            // Written, 03.09.2023

            _partID = gameObject.name.Replace(" ", "").Trim();
            ModClient.loadedParts.setID<Part>(ref _partID, p => p._partID.Contains(_partID));
        }
        /// <summary>
        /// initializes the bolts.
        /// </summary>
        /// <param name="bolts"></param>
        public void initBolts(Bolt[] bolts)
        {
            if (bolts != null && bolts.Length > 0)
            {
                _bolts = bolts;
                _hasBolts = true;
                _boltParent = Bolt.createBoltParentGameObject("PartBolts", transform);
                for (int i = 0; i < _bolts.Length; i++)
                {
                    int tightness = _loadedSaveInfo.boltTightness?[i] ?? 0;
                    _bolts[i].createBolt(_loadedSaveInfo.boltTightness[i], _boltParent.transform);
                    _bolts[i].onScrew += boltOnScrew;
                }
                Bolt.postVaildiateBolts(_bolts);
            }
        }
        private void resetTotalBoltTightness()
        {
            _tightnessTotal = 0;
        }
        private void setPhysicsMaterials()
        {
            // Written, 24.09.2023

            if (_settings.setPhysicsMaterialOnInitialisePart)
            {
                Collider[] colliders;
                switch (_settings.collisionSettings.physicMaterialType)
                {
                    case CollisionSettings.PhysicMaterialType.setOnAllFoundColliders:
                        colliders = GetComponents<Collider>().Where(col => !col.isTrigger).ToArray();
                        break;
                    default:
                    case CollisionSettings.PhysicMaterialType.setOnProvidedColliders:
                        colliders = _settings.collisionSettings.providedColliders;
                        break;
                }
                foreach (Collider collider in colliders)
                {
                    collider.material = _settings.collisionSettings.physicMaterial;
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// invoked when this part enters one of its triggers.
        /// </summary>
        /// <param name="trigger">the trigger callback that invoked this.</param>
        protected internal virtual void onTriggerEnter(Trigger trigger)
        {
            // Written, 04.10.2018 | Modified, 02.07.2022

            if (colliderCheck(trigger) && holdingCheck(trigger))
            {
                if (_triggerRoutine == null)
                {
                    _inTrigger = true;
                    _triggerRoutine = StartCoroutine(partInTrigger(trigger));
                }
            }
        }
        /// <summary>
        /// invoked when this part exits one of its triggers.
        /// </summary>
        /// <param name="trigger">the trigger callback that invoked this.</param>
        protected internal virtual void onTriggerExit(Trigger trigger)
        {
            // Written, 14.08.2018 | updated 18.09.2021

            if (colliderCheck(trigger))
            {
                if (_triggerRoutine != null)
                {
                    stopTriggerRoutine();
                }
                ModClient.guiAssemble = false;
            }
        }
        /// <summary>
        /// invoked when any bolt on this part is screwed in or out.
        /// </summary>
        protected internal virtual void boltOnScrew()
        {
            // Written, 08.07.2022

            updateTotalBoltTightness();
            if (!float.IsPositiveInfinity(_settings.assemblyTypeJointSettings.breakForce))
            {
                updateJointBreakForce();
            }
            boltTightnessCheck();
            onBoltScrew?.Invoke();
        }

        #endregion

        #region Exposed Event Invokes

        /// <summary>
        /// invokes the picked up event
        /// </summary>
        internal void invokePickedUpEvent()
        {
            // Written, 11.07.2022

            onPartPickUp?.Invoke();
        }
        /// <summary>
        /// invokes the thrown event.
        /// </summary>
        internal void invokeThrownEvent()
        {
            // Written, 11.07.2022

            onPartThrow?.Invoke();
        }
        /// <summary>
        /// invokes the dropped event.
        /// </summary>
        internal void invokeDroppedEvent()
        {
            // Written, 11.07.2022

            onPartDrop?.Invoke();
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets the gameobject of the part.
        /// </summary>
        /// <param name="p"></param>
        public static implicit operator GameObject(Part p) => p.gameObject;

        #endregion
    }
}
