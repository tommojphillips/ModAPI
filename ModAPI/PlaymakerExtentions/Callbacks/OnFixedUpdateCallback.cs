using System;

namespace TommoJProductions.ModApi.PlaymakerExtentions.Callbacks
{
    /// <summary>
    /// Represents the on fixed update state action callback
    /// </summary>
    public class OnFixedUpdateCallback : FsmStateActionCallback
    {
        // Written, 13.06.2022


        /// <summary>
        /// The action to call onfixedupdate.
        /// </summary>
        public event Action onFixedUpdate;

        /// <summary>
        /// Inits
        /// </summary>
        /// <param name="onFixedUpdate">fixedupdate action</param>
        public OnFixedUpdateCallback(Action onFixedUpdate) : base()
        {
            init(onFixedUpdate);
        }
        /// <summary>
        /// Inits
        /// </summary>
        /// <param name="onFixedUpdate">fixedupdate action</param>
        /// <param name="everyFrame">every frame?</param>
        public OnFixedUpdateCallback(Action onFixedUpdate, bool everyFrame) : base(everyFrame)
        {
            init(onFixedUpdate);
        }

        private void init(Action action)
        {
            // Written, 13.06.2022

            Name = action.Method.Name;
            onFixedUpdate += action;
        }
        /// <summary>
        /// Invokes onfixed update action.
        /// </summary>
        public override void OnFixedUpdate()
        {
            onFixedUpdate?.Invoke();
            if (!everyFrame)
                Finish();
        }         
        /// <summary>
        /// Returns the name of the state action.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
