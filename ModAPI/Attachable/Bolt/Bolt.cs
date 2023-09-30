using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

using TommoJProductions.ModApi;

using Object = UnityEngine.Object;
using System.Runtime.Remoting.Messaging;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// The Base Bolt. All basic bolt function of a bolt. Overridable.
    /// </summary>
    public class Bolt
    {
        #region Events

        /// <summary>
        /// Represents the post onscrew event. occurs when this bolt is screwed in or out. called after all other bolt events are called. <see cref="onLoose"/>, <see cref="onTight"/>.
        /// </summary>
        public event Action onScrew;
        /// <summary>
        /// Represents the on loose event. occurs when this bolt becomes loose (screwed to loose state)
        /// </summary>
        public event Action onLoose;
        /// <summary>
        /// Represents the on tight event. occurs when this bolt becomes tight (screwed to tight state)
        /// </summary>
        public event Action onTight;
        /// <summary>
        /// Represents the out loose event. occurs when this bolt was loose but was screw in (screwed from loose state)
        /// </summary>
        public event Action outLoose;
        /// <summary>
        /// Represents the out tight event. occurs when this bolt was tight but was screw out. (screwed from tight state)
        /// </summary>
        public event Action outTight;

        #endregion

        #region Fields

        /// <summary>
        /// The spanner wait time.
        /// </summary>
        public static float spannerWaitTime = 0.28f;
        /// <summary>
        /// The rachet wait time.
        /// </summary>
        public static float rachetWaitTime = 0.08f;

        private string _boltID = "bolt";
        private Coroutine _routine;
        private int _scrollDirection;
        private int _scrollInput;
        private bool _initialized;
        private protected int _tightness = 0;

        private protected bool _wait = false;
        private protected Vector3 _startPosition;
        private protected Vector3 _startEulerAngles;
        private protected Transform _parent;
        private protected GameObject _model;
        private protected BoltCallback _callback;
        private protected int _maxTightness = 8;
        
        /// <summary>
        /// if <see langword="true"/>, scrolling on this bolt will do nothing.
        /// </summary>
        public bool ignoreInput;

        #endregion

        #region Properties

        /// <summary>
        /// The bolts settings.
        /// </summary>
        public virtual BoltSettings settings { get; protected set; }
        /// <summary>
        /// The bolt waiting value. when true, the bolt has been tightened/loosened and is waiting for x amount of time base on the tool used before continuing to 
        /// update the model transform, play the bolt audio, invoke events bolt tighten/loosen events.
        /// </summary>
        public virtual bool wait { get => _wait; set => _wait = value; }
        /// <summary>
        /// The last scroll input.
        /// </summary>
        public virtual int scrollInput { get => _scrollInput; set => _scrollInput = value; }
        /// <summary>
        /// The bolts tightness (0 - 8)
        /// </summary>
        public virtual int tightness { get => _tightness; set => _tightness = value; }
        /// <summary>
        /// Returns true if this bolt is tight.
        /// </summary>
        public virtual bool isTight => tightness >= _maxTightness;
        /// <summary>
        /// Returns true if this bolt is loose.
        /// </summary>
        public virtual bool isLoose => tightness <= 0;
        /// <summary>
        /// The start position of the bolt.
        /// </summary>
        public virtual Vector3 startPosition { get => _startPosition; set => _startPosition = value; }
        /// <summary>
        /// The start rotation of the bolt.
        /// </summary>
        public virtual Vector3 startEulerAngles { get => _startEulerAngles; set => _startEulerAngles = value; }
        /// <summary>
        /// The position vector step (direction * step). How much and in what direction does the bolt move per tightness step.
        /// </summary>
        public virtual Vector3 positionVectorStep => settings.posDirection * settings.posStep;
        /// <summary>
        /// The rotation vector (direction * step). How much and in what direction does the bolt rotate per tightness step.
        /// </summary>
        public virtual Vector3 rotationVectorStep => settings.rotDirection * settings.rotStep;
        /// <summary>
        /// The max tightness of the bolt.
        /// </summary>
        public virtual int maxTightness => _maxTightness;
              
        /// <summary>
        /// The Bolts ID
        /// </summary>
        public string boltID => _boltID;
        /// <summary>
        /// The bolt model
        /// </summary>
        public GameObject model => _model;
        /// <summary>
        /// The callback
        /// </summary>
        public BoltCallback callback => _callback;
        /// <summary>
        /// the bolt func routine.
        /// </summary>
        public Coroutine routine => _routine;


        #endregion

        #region Constructors

        public Bolt() { }
        /// <summary>
        /// Assigns bolt values.
        /// </summary>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public Bolt(BoltSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default)
        {
            init(0, settings, position, eulerAngles);
        }
        /// <summary>
        /// Assigns bolt values.
        /// </summary>
        /// <param name="tightness">The the tightness of the bolt.</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public Bolt(int tightness, BoltSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default)
        {
            init(tightness, settings, position, eulerAngles);
        }

        #endregion

        #region IEnumerators

        /// <summary>
        /// Waits specified time then updates model transform, invokes events, plays bolt sound.
        /// </summary>
        /// <param name="waitTime">the wait time.</param>
        /// <param name="tightness">the tightness.</param>
        /// <returns>USAGE: <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/></returns>
        private protected virtual IEnumerator updateTightness(float waitTime, int tightness)
        {
            // Written, 17.09.2023

            float startTime = Time.realtimeSinceStartup;
            float time;

            do
            {
                time = Time.realtimeSinceStartup - startTime;
                yield return null;
            }
            while (time < waitTime);

            wait = false;

            if (isTight)
                invokeOutTight();
            else if (isLoose)
                invokeOutLoose();

            setTightnessAndUpdateModel(tightness);

            if (isTight)
                invokeOnTight();
            else if (isLoose)
                invokeOnLoose();

            invokeOnScrew();

            ModClient.playSoundAtInterupt(_parent, "CarBuilding", "bolt_screw");

        }
        /// <summary>
        /// The bolt logic. runs continuously while looking at the bolt. Stops every bolt from having a UnityEngine Update call.
        /// </summary>
        /// <param name="bcb">The callback that invoked this.</param>
        /// <returns>USAGE: <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/></returns>
        private protected virtual IEnumerator boltFunction(BoltCallback bcb)
        {
            bool hasRatchet = ModClient.getPlayerHasRatchet.Value;
            FsmBool ratchetSwitch = ModClient.getRatchetSwitch;
            float waitTime;

            while (bcb.boltCheck)
            {
                yield return null;

                _scrollInput = (int)Input.mouseScrollDelta.y;

                if (_scrollInput == 0 || wait || ignoreInput)
                {
                    continue;
                }

                if (hasRatchet)
                {
                    if (ratchetSwitch.Value)
                        _scrollDirection = 1;
                    else
                        _scrollDirection = -1;
                    waitTime = rachetWaitTime;
                }
                else
                {
                    _scrollDirection = Mathf.Clamp(_scrollInput, -1, 1);
                    waitTime = spannerWaitTime;
                }

                int tempTightness = stepTightness(_scrollDirection);

                if (tightness != tempTightness)
                {
                    wait = true;
                    bcb.StartCoroutine(updateTightness(waitTime, tempTightness));
                }
            }
        }

        #endregion

        /// <summary>
        /// Steps the current step by 1 in either direction and returns the value. clamped between 0 - 8.
        /// </summary>
        /// <param name="direction">Increase or decrease the tightness.</param>
        public virtual int stepTightness(int direction) => Mathf.Clamp(tightness + direction, 0, _maxTightness);
        /// <summary>
        /// Assigns bolt values. (tightness, settings, start (initial) transform.
        /// </summary>
        /// <param name="tightness">The tightness of the bolt.</param>
        /// <param name="baseSettings">the bolt settings.</param>
        /// <param name="position">the position of the bolt.</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public virtual void init(int tightness, BoltSettings baseSettings, Vector3 position, Vector3 eulerAngles)
        {
            // Written, 16.09.2023

            this.tightness = tightness;
            settings = baseSettings.copy();
            startPosition = position;
            startEulerAngles = eulerAngles;
        }
        /// <summary>
        /// Sets up the bolt model base on the settings. (model type, transform)
        /// </summary>
        protected virtual void setupBoltModel()
        {
            // Written, 29.10.2022

            if (settings.customPrefab != null)
            {
                _model = Object.Instantiate(settings.customPrefab);
            }
            else
            {
                switch (settings.type)
                {
                    case BoltType.nut:
                        _model = Object.Instantiate(ModClient.getBoltManager.nutPrefab);
                        break;
                    case BoltType.screw:
                        _model = Object.Instantiate(ModClient.getBoltManager.screwPrefab);
                        break;
                    case BoltType.longBolt:
                        _model = Object.Instantiate(ModClient.getBoltManager.longBoltPrefab);
                        break;
                    default:
                    case BoltType.shortBolt:
                        _model = Object.Instantiate(ModClient.getBoltManager.shortBoltPrefab);
                        break;
                }
            }
            _model.name = settings.name ?? _boltID;
            _model.transform.SetParent(_parent);

            _callback = _model.AddComponent<BoltCallback>();
            _callback.bolt = this;
            _callback.boltSize = settings.size;
        }
        /// <summary>
        /// Sets tightness and updates the model postion.
        /// </summary>
        /// <param name="tightnessValue">bolt tightness</param>
        public virtual void setTightnessAndUpdateModel(int tightnessValue)
        {
            // Written, 13.09.2023

            tightness = tightnessValue;
            updateModelPosition();
        }
        /// <summary>
        /// Updates the model position and rotation based on start and tightness values.
        /// </summary>
        public virtual void updateModelPosition()
        {
            // Written, 16.09.2023

            if (_model)
            {
                _model.transform.localPosition = startPosition + positionVectorStep * tightness;
                _model.transform.localEulerAngles = startEulerAngles + rotationVectorStep * tightness;
            }
        }
        /// <summary>
        /// Initializes this bolt. setups model and sets the parent to param, <paramref name="parent"/>.
        /// </summary>
        /// <param name="tightness">bolt tightness</param>
        /// <param name="parent">The parent of the bolt.</param>
        public virtual void createBolt(int tightness, Transform parent)
        {
            // Written, 02.07.2022

            if (_initialized)
                return;

            ModClient.loadedBolts.setID<Bolt>(ref _boltID, p => p._boltID.Contains(_boltID));
            _parent = parent;
            setupBoltModel();
            setTightnessAndUpdateModel(tightness);
            _model.SetActive(false);
            ModClient.loadedBolts.Add(this);
            _initialized = true;
        }
        /// <summary>                 
        /// Destorys the bolt.                
        /// </summary>
        public virtual void destoryBolt()
        {
            // Written, 23.09.2023

            if (!_initialized)
                return;

            Object.Destroy(_model);
            int index = ModClient.loadedBolts.indexOf(b => b._boltID == _boltID);
            if (index > -1)
            {
                ModClient.loadedBolts.RemoveAt(index);
            }
            _initialized = false;
        }
        /// <summary>
        /// De/Activates the model.
        /// </summary>
        /// <param name="active">activate or not.</param>
        public virtual void activateBolt(bool active) 
        {
            // Written, 17.09.2023

            model.SetActive(active);
        }
        /// <summary>
        /// Enables / Disables bolt logic.
        /// </summary>
        /// <param name="enabled">Set enabled or not</param>
        public virtual void enableBoltLogic(bool enabled)
        {
            // Written, 11.09.2023

            _callback.enabled = enabled;
        }
        /// <summary>
        /// On mouse enter. Starts the bolt coroutine and sets <see cref="routine"/> to that coroutine.
        /// </summary>
        /// <param name="bcb">The callback that invoked the bolt mouse enter event.</param>
        public virtual void bcb_mouseEnter(BoltCallback bcb)
        {
            // Written, 29.10.2022

            if (_routine == null)
            {
                _routine = _callback.StartCoroutine(boltFunction(bcb));
            }
        }
        /// <summary>
        /// On mouse exit. Stops the bolt coroutine and sets <see cref="routine"/> to <see langword="null"/>.
        /// </summary>
        /// <param name="bcb">The callback that invoked the mouse exit event.</param>
        public virtual void bcb_mouseExit(BoltCallback bcb)
        {
            // Written, 29.10.2022

            if (_routine != null)
            {
                _callback.StopCoroutine(_routine);
                _routine = null;
            }
        }

        /// <summary>
        /// Invokes the <see cref="onScrew"/> event.
        /// </summary>
        public virtual void invokeOnScrew() => onScrew?.Invoke();
        /// <summary>
        /// Invokes the <see cref="onTight"/> event.
        /// </summary>
        public virtual void invokeOnTight() => onTight?.Invoke();
        /// <summary>
        /// Invokes the <see cref="onLoose"/> event.
        /// </summary>
        public virtual void invokeOnLoose() => onLoose?.Invoke();
        /// <summary>
        /// Invokes the <see cref="outTight"/> event.
        /// </summary>
        public virtual void invokeOutTight() => outTight?.Invoke();
        /// <summary>
        /// Invokes the <see cref="outLoose"/> event.
        /// </summary>
        public virtual void invokeOutLoose() => outLoose?.Invoke();

        #region Static Methods

        /// <summary>
        /// Vaildiates bolts after bolts have been initialized.
        /// </summary>
        /// <returns>returns true if no errors were detected with the bolts.</returns>
        /// <exception cref="Exception">Throws an exception is any issues were detected. to prevent part from initializing.</exception>
        public static bool postVaildiateBolts(Bolt[] bolts)
        {
            // Written, 08.10.2022

            string error = String.Empty;

            for (int i = 0; i < bolts.Length; i++)
            {
                if (bolts[i].settings.size == BoltSize.hand)
                {
                    if (!bolts[i].model.isOnLayer(LayerMasksEnum.Parts))
                    {
                        error += $"bolt ref ({i}) has a bolt size of type 'hand' but is not on the layer 'Parts' Player will not be able to interact with this bolt/add nut.\n";
                    }
                }
                else
                {
                    if (!bolts[i].model.isOnLayer(LayerMasksEnum.Bolts))
                    {
                        error += $"bolt ref ({i}) has a bolt size of a type other than 'hand' but is not on the layer 'Bolts' Player will not be able to interact with this bolt/add nut.\n";
                    }
                }
            }

            if (error != String.Empty)
            {
                ModConsole.Error(error);
                throw new Exception(error);
            }
            return true;
        }
        /// <summary>
        /// Gets bolt save info in an array.
        /// </summary>
        /// <returns>All tightness values in an int array.</returns>
        public static int[] getBoltSaveInfo(Bolt[] bolts, bool hasBolts)
        {
            // Written, 27.08.2022

            if (bolts == null)
                return null;

            int[] tightness = new int[bolts.Length];

            if (hasBolts)
            {
                for (int i = 0; i < bolts.Length; i++)
                {
                    /*Bolt b = bolts[i] as Bolt;

                    if (b != null && b.addNut)
                    {
                        tightness[i] = b.tightness + b.nut.tightness;
                    }
                    else
                    {
                        tightness[i] = bolts[i].tightness;
                    }*/
                    tightness[i] = bolts[i].tightness;
                }
            }
            return tightness;
        }
        /// <summary>
        /// Creates a Bolt Parent on <paramref name="transform"/>.
        /// </summary>
        /// <returns>The Bolt Parent <see cref="GameObject"/>.</returns>
        public static GameObject createBoltParentGameObject(string boltParentName, Transform transform)
        {
            // Written, 03.07.2022

            GameObject boltParent = new GameObject(boltParentName);
            boltParent.transform.SetParent(transform);
            boltParent.transform.localPosition = Vector3.zero;
            boltParent.transform.localEulerAngles = Vector3.zero;

            return boltParent;
        }

        #endregion
    }
}
