using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions
{
    public class OnGuiCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        public OnGuiCallback(Action onGuiAction, bool everyFrame) : base(onGuiAction, everyFrame) { }

        public OnGuiCallback(Func<bool> func, bool everyFrame) : base(func, everyFrame) { }

        public override CallbackTypeEnum callbackType => CallbackTypeEnum.onGui;

        public override void OnGUI()
        {
            invokeAction();
        }
    }
}
