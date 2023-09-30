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
        /// Constructor
        /// </summary>
        /// <param name="onFixedUpdate">fixedupdate action</param>
        /// <param name="everyFrame">should this on fixed update call be invoked every frame?</param>
        public OnFixedUpdateCallback(Action onFixedUpdate, bool everyFrame) : base(onFixedUpdate, everyFrame) { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="func">the action to perfrom on fixed update. returns weather this state action is finished or not.</param>
        /// <param name="everyFrame">should this on fixed update call be invoked every frame?</param>
        public OnFixedUpdateCallback(Func<bool> func, bool everyFrame) : base(func, everyFrame) { }
        /// <summary>
        /// Represents the callback type. ON FIXED UPDATE
        /// </summary>
        public override CallbackTypeEnum callbackType => CallbackTypeEnum.onFixedUpdate;



        /// <summary>
        /// Fixed Update Method
        /// </summary>
        public override void OnFixedUpdate()
        {
            invokeAction();
        }
    }
}
