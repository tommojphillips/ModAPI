using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HutongGames.PlayMaker;

using MSCLoader;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// Represents the Parts Manager. Handles Part AutoSave, Drop, Throw and Pick up Logic.
    /// </summary>
    public class PartManager
    {
        #region Fields

        private bool _pickedPartSet = false;
        private bool _pickedPartInherentlySet = false;
        private Part _pickedPart; 
        private Part[] _inherentlyPickedParts;
        private GameObject _pickedObject;
        private SaveManager _partSaveManager;
        private Action _partLeaveAction;
        private FsmGameObject _pickedUpObject;

        #endregion

        #region Properties

        #endregion

        /// <summary>
        /// Reps all parts that were set in <see cref="partCheckFunction"/>. Reps all <see cref="Part"/> children in root <see cref="pickedPart"/>.
        /// </summary>
        public Part[] inherentlyPickedParts => _inherentlyPickedParts;
        /// <summary>
        /// Reps the root picked part. the part that the player is currently holding.
        /// </summary>
        public Part pickedPart => _pickedPart;
        /// <summary>
        /// Reps the picked object. the object that the player is currently holding.
        /// </summary>
        public GameObject pickedObject => _pickedObject;
        /// <summary>
        /// Represents the instance of <see cref="SaveManager"/> that handles part saving.
        /// </summary>
        public SaveManager partSaveManager => _partSaveManager;
        internal bool pickedPartSet => _pickedPartSet;
        internal bool inherentlyPickedPartsSet => _pickedPartInherentlySet;

        #region IEnumerators

        private IEnumerator partCheckFunction()
        {
            // Written, 09.06.2022

            yield return null;
            _pickedObject = _pickedUpObject.Value;
            if (_pickedObject)
            {
                _pickedPart = _pickedObject.GetComponent<Part>();
                if (_pickedPart)
                {
                    _pickedPartSet = true;
                    _pickedPart.pickedUp = true;
                    _pickedPart.invokePickedUpEvent();
                }

                List<Part> inherentParts = new List<Part>();
                IEnumerator<Part> parts = _pickedObject.getBehavioursInChildrenAsync<Part>();
                int count = 0;
                while (parts.MoveNext())
                {
                    count++;
                    inherentParts.Add(parts.Current);
                    yield return parts;
                }
                _inherentlyPickedParts = inherentParts.ToArray();

                if (count > 0)
                {
                    _pickedPartInherentlySet = true;
                    foreach (Part p in _inherentlyPickedParts)
                    {
                        p.gameObject.sendToLayer(LayerMasksEnum.Wheel);
                        p.inherentlyPickedUp = true;
                    }
                }
                ModClient.invokePickUpEvent(_pickedObject);
            }
        }

        #endregion

        #region Methods

        internal void load()
        {
            // Written, 02.07.2022

            _partSaveManager = new SaveManager();
            _pickedUpObject = ModClient.getPickedUpGameObject;
            PlayMakerFSM handPickup = ModClient.getHandPickUpFsm;

            // injecting part picked, drop and throw functions.
            handPickup.GetState("Part picked").insertNewAction(onPickedUp, 5);
            handPickup.GetState("Drop part").prependNewAction(onPartDropped);
            handPickup.GetState("Throw part").prependNewAction(onPartThrown);

            // inject save function (For Auto Save)
            GameObject.Find("ITEMS").GetPlayMaker("SaveItems").GetState("Save game").prependNewAction(onSave);
        }

        private void inherentlyPickedPartReset()
        {
            // Written, 09.06.2022

            foreach (Part p in _inherentlyPickedParts)
            {
                p.inherentlyPickedUp = false;
                p.makePartPickable(!p.installed);
            }
            _inherentlyPickedParts = null;
            _pickedPartInherentlySet = false;
        }
        private void objectLeaveHand(Action partLeaveEvent, Action<GameObject> objectLeaveEvent)
        {
            // Written, 14.06.2022

            if (_pickedPartSet)
            {
                partLeaveEvent?.Invoke();
                _pickedPart.pickedUp = false;
                _pickedPart = null;
                _pickedPartSet = false;
            }
            if (_pickedPartInherentlySet)
            {
                inherentlyPickedPartReset();
            }
            objectLeaveEvent.Invoke(_pickedObject);
            _pickedObject = null;
        }

        #endregion

        #region Event Handlers

        private void onSave()
        {
            // Written, 02.07.2023

            Debug.Log("[ModApiLoader] saving autosave parts");

            using (ES2Writer writer = ES2Writer.Create(_partSaveManager.saveFile))
            {
                _partSaveManager.saveValue("ModApiFullVersion", VersionInfo.FULL_VERSION, writer);
                List<Part> parts = ModClient.loadedParts;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i].partSettings.autoSave)
                    {
                        _partSaveManager.savePart(parts[i], writer);
                    }
                }
                List<Trigger> triggers = Trigger.loadedTriggers;
                for (int i = 0; i < triggers.Count; i++)
                {
                    _partSaveManager.saveTrigger(triggers[i], writer);
                }

                writer.Save();
            }
        }
        private void onPickedUp()
        {
            // Written, 11.06.2022

            ModClient.levelManager.StartCoroutine(partCheckFunction());
        }
        private void onPartDropped()
        {
            // Written, 11.06.2022

            if (_pickedPart)
                _partLeaveAction = _pickedPart.invokeDroppedEvent;
            else
                _partLeaveAction = null;
            objectLeaveHand(_partLeaveAction, ModClient.invokeDropEvent);
        }
        private void onPartThrown()
        {
            // Written, 11.06.2022

            if (_pickedPart)
                _partLeaveAction = _pickedPart.invokeThrownEvent;
            else
                _partLeaveAction = null;
            objectLeaveHand(_partLeaveAction, ModClient.invokeThrowEvent);
        }

        #endregion
    }
}
