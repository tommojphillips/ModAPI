using HutongGames.PlayMaker;
using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    public class FsmStateActionCallback : FsmStateAction
    {
        // Written, 13.06.2022

        /// <summary>
        /// The action to invoke.
        /// </summary>
        public event Action onInvokeAction;

        /// <summary>
        /// Represents if should loop this untill state is inactive.
        /// </summary>
        public bool everyFrame = false;
        /// <summary>
        /// caches the action method name that is subbed to this action callback.
        /// </summary>
        public string debugActionName;

        /// <summary>
        /// inits new action callback
        /// </summary>
        /// <param name="action">the action to callback on</param>
        /// <param name="everyFrame">call action every frame?</param>
        public FsmStateActionCallback(Action action, bool everyFrame)
        {
            init(action, everyFrame);
        }

        /// <summary>
        /// inits this action callback.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="everyFrame"></param>
        private void init(Action action, bool everyFrame)
        {
            // Written, 13.06.2022

            this.everyFrame = everyFrame;
            Name = action.Method.Name;
            debugActionName = Name;
            onInvokeAction += action;
        }

        /// <summary>
        /// exposes the event <see cref="onInvokeAction"/>
        /// </summary>
        protected void invokeAction() 
        {
            onInvokeAction?.Invoke();
            if (!everyFrame)
                Finish();
        }

        /// <summary>
        /// Returns the name of the state action.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
