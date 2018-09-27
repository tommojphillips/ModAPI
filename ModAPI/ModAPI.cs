using UnityEngine;
using HutongGames.PlayMaker;

namespace ModAPI
{
    /// <summary>
    /// Represents useful properties for playmaker.
    /// </summary>
    public static class ModAPI
    {
        // Written, 10.08.2018

        #region Properties
        
        /// <summary>
        /// Represents the assemble assemble audio source.
        /// </summary>
        public static AudioSource assembleAudio
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents the disassemble audio source.
        /// </summary>
        public static AudioSource disassembleAudio
        {
            get;
            private set;
        }
        /// <summary>
        /// Represents whether the gui assemble icon is shown or not. (Tick Symbol)
        /// </summary>
        public static bool guiAssemble
        {
            get
            {
                return PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble").Value;
            }
            set
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble").Value = value;
            }
        }
        /// <summary>
        /// Represents whether the gui interaction icon is shown or not. (Hand Symbol)
        /// </summary>
        public static bool guiInteraction
        {
            get
            {
                return PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIinteraction").Value;
            }
            set
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIinteraction").Value = value;
            }
        }
        /// <summary>
        /// Gets the current player mode, 
        /// </summary>
        public static PlayerModeEnum getMode
        {
            get
            {
                PlayerModeEnum pme;
                string currentVechicle = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle").Value;
                switch (currentVechicle)
                {
                    case "":
                        pme = PlayerModeEnum.OnFoot;
                        break;
                    case "Satsuma":
                        pme = PlayerModeEnum.InSatsuma;
                        break;
                    default:
                         pme = PlayerModeEnum.InOther;
                        break;
                }
                return pme;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets, <see cref="assembleAudio"/> and <see cref="aisassembleAudio"/> to the audio.
        /// </summary>
        public static void intializeAssembleSounds()
        {
            // Written, 10.08.2018

            GameObject go = GameObject.Find("MasterAudio/CarBuilding/assemble");
            assembleAudio = go.GetComponent<AudioSource>();
            go = GameObject.Find("MasterAudio/CarBuilding/disassemble");
            disassembleAudio = go.GetComponent<AudioSource>();
        }

        #endregion
    }
}
