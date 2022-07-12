using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    public class OnGuiCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022



        public event Action onGui;

        public OnGuiCallback(Action onGuiAction) : base()
        {
            init(onGuiAction);
        }

        public OnGuiCallback(Action onGuiAction, bool everyFrame) : base(everyFrame)
        {
            init(onGuiAction);
        }

        void init(Action action)
        {
            // Written, 13.06.2022

            Name = action.Method.Name;
            onGui += action;
        }
        public override void OnGUI()
        {
            onGui?.Invoke();
            if (!everyFrame)
                Finish();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
