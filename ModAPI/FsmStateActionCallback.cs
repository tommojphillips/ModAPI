using HutongGames.PlayMaker;
using System;

namespace TommoJProductions.ModApi
{
    public class FsmStateActionCallback : FsmStateAction
    {
        public event Action onEnterCallback;

        public FsmStateActionCallback(Action action)
        {
            Name = action.Method.Name;
            onEnterCallback += action;
        }
        public override void OnEnter()
        {
            onEnterCallback?.Invoke();
            Finish();
        }
    }
}
