using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    public class OnGuiCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        public OnGuiCallback(Action onGuiAction, bool everyFrame) : base(onGuiAction, everyFrame) { }

        public override void OnGUI()
        {
            invokeAction();
        }
    }
}
