using HutongGames.PlayMaker;
using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FsmStateActionCallback : FsmStateAction
    {
        // Written, 13.06.2022

        public abstract CallbackTypeEnum callbackType { get; }

        /// <summary>
        /// The action to invoke.
        /// </summary>
        public event Action onInvokeAction;
        /// <summary>
        /// The func to invoke.
        /// </summary>
        public event Func<bool> inInvokeAction;

        public Delegate action;

        private bool? invokedFinish;

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
        /// inits new action callback
        /// </summary>
        /// <param name="func">the func to callback on. the func should return if fsmAction should finish.</param>
        /// <param name="everyFrame">call action every frame?</param>
        public FsmStateActionCallback(Func<bool> func, bool everyFrame)
        {
            init(func, everyFrame);
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
            this.action = action;
        }
        /// <summary>
        /// inits this action callback.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="everyFrame"></param>
        private void init(Func<bool> func, bool everyFrame)
        {
            // Written, 13.06.2022

            this.everyFrame = everyFrame;
            Name = func.Method.Name;
            debugActionName = Name;
            inInvokeAction += func;
            this.action = func;
        }

        /// <summary>
        /// exposes the event <see cref="onInvokeAction"/>
        /// </summary>
        protected void invokeAction() 
        {
            // Modified, 07.12.2022

            onInvokeAction?.Invoke();
            invokedFinish = inInvokeAction?.Invoke();
            if (!everyFrame || invokedFinish == true)
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
