using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using MSCLoader;

using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents Bolt Manager. Logic. 
    /// </summary>
    public class BoltManager
    {
        // Written, 11.09.2023

        private bool _assetsLoaded = false;
        private FsmGameObject _bolt;
        private BoltCallback _currentCallback;
        private BoltCallback _lookingAtCallback;

        private FsmGameObject _raycastHit;

        /// <summary>
        /// Represents the short bolt prefab.
        /// </summary>
        public GameObject shortBoltPrefab;
        /// <summary>
        /// Represents the long bolt prefab.
        /// </summary>
        public GameObject longBoltPrefab;
        /// <summary>
        /// Represents the screw prefab.
        /// </summary>
        public GameObject screwPrefab;
        /// <summary>
        /// Represents the nut prefab.
        /// </summary>
        public GameObject nutPrefab;

        /// <summary>
        /// Represents if the bolt assets are loaded or not.
        /// </summary>
        internal bool assetsLoaded => _assetsLoaded;

        /// <summary>
        /// Represents the current detected bolt gameobject. only works in tool mode.
        /// </summary>
        internal FsmGameObject bolt => _bolt;
        /// <summary>
        /// represents the current bolt callback that the player is looking at.
        /// </summary>
        internal BoltCallback currentCallback => _currentCallback;
        /// <summary>
        /// represents the set bolt callback that the player is looking at.
        /// </summary>
        internal BoltCallback lookingAtCallback => _lookingAtCallback;


        internal void load() 
        {
            // Written, 11.09.2023

            loadBoltAssets();
            injectBoltCheckToolMode();
            injectBoltCheckHandMode();

            _raycastHit = ModClient.getRaycastHitGameObject;
        }

        private void loadBoltAssets()
        {
            // Written, 02.07.2022

            if (!_assetsLoaded)
            {
                AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(ModApi.Properties.Resources.modapi);
                nutPrefab = ab.LoadAsset("nut.prefab") as GameObject;
                shortBoltPrefab = ab.LoadAsset("short bolt.prefab") as GameObject;
                longBoltPrefab = ab.LoadAsset("long bolt.prefab") as GameObject;
                screwPrefab = ab.LoadAsset("screw.prefab") as GameObject;
                ab.Unload(false);
                _assetsLoaded = true;
            }

            string message = $"[ModApLoader] Bolt Assets Loaded\n" +
                $"Nut           Prefab: {nutPrefab}\n" +
                $"Screw         Prefab: {screwPrefab}\n" +
                $"Short Bolt    Prefab: {shortBoltPrefab}\n" +
                $"Long Bolt     Prefab: {longBoltPrefab}";

            Debug.Log(message);
        }
        private void boltCheck(GameObject raycast)
        {
            _currentCallback = null;

            if (raycast)
            {
                _currentCallback = raycast.GetComponent<BoltCallback>();
            }
            resetBolt();
            if (_currentCallback)
            {
                _currentCallback.onBoltEnter();
            }
            _lookingAtCallback = _currentCallback;
        }
        private void injectBoltCheckHandMode()
        {
            // Written, 08.10.2022

            PlayMakerFSM pickUp = ModClient.getHandPickUpFsm;

            pickUp.GetState("Look for object").appendNewAction(handModeBoltCheck, CallbackTypeEnum.onUpdate, true);
        }
        private void injectBoltCheckToolMode()
        {
            // Written, 25.08.2022

            Transform fpsCamera = ModClient.getFPS.transform;
            Transform toolLogic = fpsCamera.FindChild("2Spanner/Raycast");
            Transform selectItem = fpsCamera.FindChild("SelectItem");
            PlayMakerFSM raycast = toolLogic.GetPlayMaker("Raycast");
            PlayMakerFSM check = toolLogic.GetPlayMaker("Check");
            PlayMakerFSM selection = selectItem.GetPlayMaker("Selection");

            _bolt = raycast.FsmVariables.FindFsmGameObject("Bolt");
            check.GetState("Check bolt Name").appendNewAction(toolModeBoltCheck);
            selection.GetState("Reset tool").appendNewAction(resetBolt);
        }

        #region Event handlers

        /// <summary>
        /// Occurs when the <see cref="_raycastHit"/> gameobject reference changes. Handles <see cref="BoltCallback.onBoltEnter"/>/<see cref="BoltCallback.onBoltExit"/> calls. invoked by injected playmaker state.
        /// </summary>
        private void handModeBoltCheck()
        {
            boltCheck(_raycastHit.Value);
        }
        /// <summary>
        /// Occurs when the <see cref="_bolt"/> gameobject reference changes. Handles <see cref="BoltCallback.onBoltEnter"/>/<see cref="BoltCallback.onBoltExit"/> calls. invoked by injected playmaker state.
        /// </summary>
        private void toolModeBoltCheck()
        {
            // Written, 25.08.2022

            boltCheck(_bolt.Value);
        }
        /// <summary>
        /// Occurs when the <see cref="_bolt"/> gameobject reference changes or when the reset tool check takes place when player has changed to hand mode.. Handles <see cref="BoltCallback.onBoltExit"/> calls. invoked by injected playmaker state.
        /// </summary>
        private void resetBolt()
        {
            // Written, 25.08.2022

            if (_lookingAtCallback)
            {
                _lookingAtCallback.onBoltExit();
                _lookingAtCallback = null;
            }
        }

        #endregion
    }
}
