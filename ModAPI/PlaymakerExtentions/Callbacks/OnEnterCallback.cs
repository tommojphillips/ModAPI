using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    public class OnEnterCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022

        public event Action onEnter;

        public OnEnterCallback(Action action, bool everyFrame) : base(everyFrame)
        {
            init(action);
        }
        public OnEnterCallback(Action action) : base()
        {
            init(action);
        }

        private void init(Action action) 
        {
            // Written, 13.06.2022

            Name = action.Method.Name;
            onEnter += action;
        }

        public override void OnEnter()
        {
            // Written, 13.06.2022

            onEnter?.Invoke();
            if (!everyFrame)
                Finish();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
