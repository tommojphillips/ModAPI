using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions
{
    /// <summary>
    /// Represents the on gui fsm state action callback.
    /// </summary>
    public class OnGuiCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="onGuiAction">the action to perfrom on gui.</param>
        /// <param name="everyFrame">should this on gui call be invoked every frame?</param>
        public OnGuiCallback(Action onGuiAction, bool everyFrame) : base(onGuiAction, everyFrame) { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="func">the action to perfrom on gui. returns weather this state action is finished or not.</param>
        /// <param name="everyFrame">should this on gui call be invoked every frame?</param>
        public OnGuiCallback(Func<bool> func, bool everyFrame) : base(func, everyFrame) { }
        /// <summary>
        /// Represents the callback type. ON GUI
        /// </summary>
        public override CallbackTypeEnum callbackType => CallbackTypeEnum.onGui;
        /// <summary>
        /// GUI Method
        /// </summary>
        public override void OnGUI()
        {
            invokeAction();
        }
    }
}
