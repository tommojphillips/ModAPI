using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    public class OnUpdateCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        public OnUpdateCallback(Action onUpdateAction, bool everyFrame) : base(onUpdateAction, everyFrame) { }

        public override void OnUpdate()
        {
            invokeAction();
        }
    }
}
