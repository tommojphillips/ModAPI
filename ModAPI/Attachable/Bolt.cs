using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    public class Bolt
    {
        // Written, 11.05.2022

        #region Events

        public event Action onScrew;
        public event Action onLoose;
        public event Action onTight;
        public event Action outLoose;
        public event Action outTight;

        #endregion 

        #region Fields

        public float posStep = 0.0005f;
        public float rotStep = 30;
        public float tightnessStep = 1;
        public float maxTightness = 8;
        public Vector3 posDirection = Vector3.forward;
        public Vector3 rotDirection = Vector3.forward;
        public GameObject boltGo;

        private Vector3 startPosition;
        private Vector3 startEulerAngles;
        

        #endregion

        #region Properties

        public int tightness { get; internal set; }
        public bool tight => tightness >= maxTightness;
        public bool loose => tightness <= 0;

        private Vector3 positionDelta => posDirection * posStep;
        private Vector3 rotationDelta => rotDirection * rotStep;

        #endregion

        #region Unity Runtime Invoked Methods

        private void Awake()
        {
            startPosition = boltGo.transform.localPosition;
            startEulerAngles = boltGo.transform.localEulerAngles;
        }
        private void Start()
        {
            initBolt();
            updateNutPosRot();
        }
        private void Update()
        {
            if (boltGo.isPlayerLookingAt() && ModClient.isInHandMode)
            {
                float scrollInput = Input.mouseScrollDelta.y;

                if (Mathf.Abs(scrollInput) > 0)
                {
                    int tempTightness = (int)Mathf.Clamp(tightness + (scrollInput * tightnessStep), 0, maxTightness);
                    if (tempTightness != tightness)
                    {
                        if (tight)
                            outTight?.Invoke();
                        else if (loose)
                            outLoose?.Invoke();
                        tightness = tempTightness;
                        updateNutPosRot();
                        if (tight)
                            onTight?.Invoke();
                        else if (loose)
                            onLoose?.Invoke();
                        else
                            onScrew?.Invoke();
                        MasterAudio.PlaySound3DAndForget("CarBuilding", boltGo.transform, variationName: "bolt_screw");
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void initBolt() 
        {
        
        }
        private void updateNutPosRot()
        {
            boltGo.transform.localPosition = startPosition + positionDelta * tightness;
            boltGo.transform.localEulerAngles = startEulerAngles + rotationDelta * tightness;
        }

        #endregion
    }
}
