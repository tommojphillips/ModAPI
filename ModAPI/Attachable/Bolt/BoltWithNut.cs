using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// 
    /// </summary>
    public class BoltWithNut : Bolt
    {
        private Bolt _nut;
        private BoltWithNutSettings _settings;

        /// <summary>
        /// The add nut.
        /// </summary>
        public Bolt nut => _nut;
        /// <summary>
        /// The base bolt settings for this bolt
        /// </summary>
        public override BoltSettings settings => _settings;
        /// <summary>
        /// Bolt with Nut specific settings. derived from <see cref="BoltSettings"/>.
        /// </summary>
        public BoltWithNutSettings boltWithNutSettings => _settings;
        /// <summary>
        /// The total tightness of the bolt and nut.
        /// </summary>
        public override int tightness 
        {
            get => base.tightness + _nut.tightness;

            set 
            {
                if (value < 8)
                {
                    base.tightness = value;
                    _nut.tightness = 0;
                }
                else
                {
                    base.tightness = 8;
                    _nut.tightness = value - 8;
                }
            }             
        }
        /// <summary>
        /// The max tightness of the bolt and nut.
        /// </summary>
        public override int maxTightness => 16;

        #region Constructors

        /// <summary>
        /// Initialies this bolt.
        /// </summary>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public BoltWithNut(BoltWithNutSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default) : base()
        {
            // Written, 16.09.2023

            init(settings, position, eulerAngles, 0);
        }
        /// <summary>
        /// Initialies this bolt with a tightness value.
        /// </summary>
        /// <param name="tightness">The the tightness of the bolt.</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        public BoltWithNut(int tightness, BoltWithNutSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default) : base()
        {
            // Written, 16.09.2023

            init(settings, position, eulerAngles, tightness);
        }
        /// <summary>
        /// Initialies this bolt with a nut offset overload. only use if you intend on this bolt having an 'add nut'
        /// </summary>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        /// <param name="nutOffsetOverride">the nut offset overload</param>
        public BoltWithNut(BoltWithNutSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default, float? nutOffsetOverride = null) : base()
        {
            // Written, 16.09.2023

            init(settings, position, eulerAngles, 0, nutOffsetOverride);
        }
        /// <summary>
        /// Initialies this bolt with a tightness value and a nut offset overload. only use if you intend on this bolt having an 'add nut'
        /// </summary>
        /// <param name="tightness">The the tightness of the bolt.</param>
        /// <param name="settings">the settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        /// <param name="nutOffsetOverride">the nut offset overload</param>
        public BoltWithNut(int tightness, BoltWithNutSettings settings = default, Vector3 position = default, Vector3 eulerAngles = default, float? nutOffsetOverride = null) : base()
        {
            // Written, 16.09.2023

            init(settings, position, eulerAngles, 0, nutOffsetOverride);
        }

        #endregion

        /// <summary>
        /// Initializes this bolt. assigns field values with an option of setting a overload for nut offset. Called in Constructors.
        /// Use only if this bolt is using add nut, otherwise this does nothing extra.
        /// </summary>
        /// <param name="tightness">The the tightness of the bolt.</param>
        /// <param name="settings">The settings for the bolt.</param>
        /// <param name="position">the position of the bolt</param>
        /// <param name="eulerAngles">the rotation of the bolt.</param>
        /// <param name="nutOffsetOverride">the nut offset overload</param>
        private protected void init(BoltWithNutSettings settings, Vector3 position, Vector3 eulerAngles, int tightness, float? nutOffsetOverride = null)
        {
            // Written, 16.09.2023

            startPosition = position;
            startEulerAngles = eulerAngles;

            _settings = settings;

            BoltSettings nutSettings = new BoltSettings();
            nutSettings.posDirection = settings.posDirection * -1;
            nutSettings.rotDirection = settings.rotDirection * -1;
            nutSettings.posStep = settings.posStep;
            nutSettings.rotStep = settings.rotStep;
            nutSettings.activeWhenUninstalled = settings.activeWhenUninstalled;
            nutSettings.type = BoltType.nut;
            nutSettings.size = settings.nutSettings.size;
            nutSettings.name = "nut";

            Vector3 nutPosition = startPosition + (positionVectorStep * 8 * 2) + (_settings.offset * settings.posDirection);
            _nut = new Bolt(nutSettings, nutPosition, startEulerAngles);
            this.tightness = tightness;

            _nut.onLoose += onNutLoose;
            _nut.outLoose += outNutLoose;
            _nut.onScrew += nutOnScrew;

            if (nutOffsetOverride != null)
            {
                _settings.offset = (float)nutOffsetOverride;
            }
        }
        private void nutOnScrew()
        {
            invokeOnScrew();
        }
        /// <summary>
        /// Nut out Loose. Starts ignoring bolt input
        /// </summary>
        private void outNutLoose()
        {
            ignoreInput = true;
        }
        /// <summary>
        /// Nut on Loose. Stops ignoring bolt input
        /// </summary>
        private void onNutLoose()
        {
            ignoreInput = false;
        }

        /// <summary>
        /// inits bolt and add nut.
        /// </summary>
        /// <param name="parent">the parent for the bolt.</param>
        /// <param name="tightness">bolt tightness</param>
        public override void createBolt(int tightness, Transform parent)
        {
            // Written, 30.09.2023

            if (tightness < 8)
            {
                base.createBolt(tightness, parent);
                _nut.createBolt(0, parent);
            }
            else
            {
                base.createBolt(8, parent);
                _nut.createBolt(tightness - 8, parent);
            }
        }

        /// <summary>
        /// De/Activates the bolt.
        /// </summary>
        /// <param name="active">Activate the bolt or not.</param>
        public override void activateBolt(bool active)
        {
            // Written, 17.09.2023

            base.activateBolt(active);
            _nut.model.SetActive(active);
        }
        /// <summary>
        /// Updates the model position and rotation based on start and tightness values.
        /// </summary>
        public override void updateModelPosition()
        {
            // Written, 16.09.2023

            if (_model)
            {
                _model.transform.localPosition = startPosition + positionVectorStep * _tightness;
                _model.transform.localEulerAngles = startEulerAngles + rotationVectorStep * _tightness;
            }

            if (_nut.model)
            { 
                _nut.updateModelPosition();
            }

            if (tightness > 8)
            {
                ignoreInput = true;
                _nut.ignoreInput = false;
            }
            else if (tightness < 8)
            {
                ignoreInput = false;
                _nut.ignoreInput = true;
            }
            else
            {
                ignoreInput = false;
                _nut.ignoreInput = false;
            }
        }
    }
}
