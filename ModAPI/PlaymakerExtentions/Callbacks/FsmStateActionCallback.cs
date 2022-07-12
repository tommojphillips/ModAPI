using HutongGames.PlayMaker;
using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    public class FsmStateActionCallback : FsmStateAction
    {
        // Written, 13.06.2022

        /// <summary>
        /// Represents if should loop this untill state is inactive.
        /// </summary>
        public bool everyFrame = false;

        public FsmStateActionCallback(bool everyFrame)
        {
            init(everyFrame);
        }
        public FsmStateActionCallback()
        {
            init(false);
        }

        private void init(bool _everyFrame)
        {
            everyFrame = _everyFrame;
        }
    }
}
