using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions
{
    public class OnEnterCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        public OnEnterCallback(Action action, bool everyFrame) : base(action, everyFrame) { }

        public OnEnterCallback(Func<bool> func, bool everyFrame) : base(func, everyFrame) { }

        public override CallbackTypeEnum callbackType => CallbackTypeEnum.onEnter;

        public override void OnEnter()
        {
            // Written, 13.06.2022

            invokeAction();
        }
    }
}
