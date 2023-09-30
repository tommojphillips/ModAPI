using System;
using System.Collections;

namespace TommoJProductions.ModApi.YieldInstructions
{
    /// <summary>
    /// 
    /// </summary>
    public class WaitWhile : IEnumerator
    {
        private Func<bool> predicate;
        /// <summary>
        /// current
        /// </summary>
        public object Current { get { return null; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MoveNext() 
        {
            return !predicate.Invoke();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Reset() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public WaitWhile(Func<bool> predicate)
        {
            this.predicate = predicate;
        }
    }
}
