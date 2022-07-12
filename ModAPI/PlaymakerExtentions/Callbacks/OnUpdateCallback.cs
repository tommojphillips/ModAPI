using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    public class OnUpdateCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022


        public event Action onUpdate;

        public OnUpdateCallback(Action onUpdateAction) : base()
        {
            init(onUpdateAction);
        }

        public OnUpdateCallback(Action onUpdateAction, bool everyFrame) : base(everyFrame)
        {
            init(onUpdateAction);
        }

        private void init(Action action) 
        {
            // Written, 13.06.2022

            Name = action.Method.Name;
            onUpdate += action;
        }

        public override void OnUpdate()
        {
            onUpdate?.Invoke();
            if (!everyFrame)
                Finish();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
