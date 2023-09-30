using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions
{
    /// <summary>
    /// Represents the on enter state action callback
    /// </summary>
    public class OnEnterCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">the action to perfrom on enter.</param>
        /// <param name="everyFrame">should this on fixed enter call be invoked every frame?</param>
        public OnEnterCallback(Action action, bool everyFrame) : base(action, everyFrame) { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="func">the action to perfrom on enter. returns weather this state action is finished or not.</param>
        /// <param name="everyFrame">should this on fixed enter call be invoked every frame?</param>
        public OnEnterCallback(Func<bool> func, bool everyFrame) : base(func, everyFrame) { }
        /// <summary>
        /// Represents the callback type. ON ENTER
        /// </summary>
        public override CallbackTypeEnum callbackType => CallbackTypeEnum.onEnter;
        /// <summary>
        /// On Enter Method.
        /// </summary>
        public override void OnEnter()
        {
            // Written, 13.06.2022

            invokeAction();
        }
    }
}
