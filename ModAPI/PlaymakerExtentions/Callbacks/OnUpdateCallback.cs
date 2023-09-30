using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions
{
    /// <summary>
    /// Represents the on update fsm state action callback.
    /// </summary>
    public class OnUpdateCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="onUpdateAction">the action to perfrom on update.</param>
        /// <param name="everyFrame">should this on update call be invoked every frame?</param>
        public OnUpdateCallback(Action onUpdateAction, bool everyFrame) : base(onUpdateAction, everyFrame) { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="func">the action to perfrom on update. return weather this state action is finished or not.</param>
        /// <param name="everyFrame">should this on update call be invoked every frame?</param>
        public OnUpdateCallback(Func<bool> func, bool everyFrame) : base(func, everyFrame) { }
        /// <summary>
        /// Represents the callback type. ON UPDATE
        /// </summary>
        public override CallbackTypeEnum callbackType => CallbackTypeEnum.onUpdate;

        /// <summary>
        /// update method.
        /// </summary>
        public override void OnUpdate()
        {
            invokeAction();
        }
    }
}
