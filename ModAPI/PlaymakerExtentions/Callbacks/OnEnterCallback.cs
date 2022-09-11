using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    public class OnEnterCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        public OnEnterCallback(Action action, bool everyFrame) : base(action, everyFrame) { }

        public override void OnEnter()
        {
            // Written, 13.06.2022

            invokeAction();
        }
    }
}
