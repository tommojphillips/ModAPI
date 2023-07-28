using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions
{
    /// <summary>
    /// Represents the on fixed update state action callback
    /// </summary>
    public class OnFixedUpdateCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        /// <summary>
        /// Inits
        /// </summary>
        /// <param name="onFixedUpdate">fixedupdate action</param>
        public OnFixedUpdateCallback(Action onFixedUpdate, bool everyFrame) : base(onFixedUpdate, everyFrame) { }

        public OnFixedUpdateCallback(Func<bool> func, bool everyFrame) : base(func, everyFrame) { }

        public override CallbackTypeEnum callbackType => CallbackTypeEnum.onFixedUpdate;



        /// <summary>
        /// Invokes onfixed update action.
        /// </summary>
        public override void OnFixedUpdate()
        {
            invokeAction();
        }
    }
}
