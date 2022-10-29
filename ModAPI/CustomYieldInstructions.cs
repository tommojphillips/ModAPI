using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace TommoJProductions.ModApi
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
        public bool MoveNext() { return predicate(); }
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

    /// <summary>
    /// Waits for seconds in real time.
    /// </summary>
    public class WaitForSecondsRealTime : IEnumerator
    {
        // Written, 11.09.2022

        private int i;
        private float startTime;

        /// <summary>
        /// Represents the number of seconds to wait in realtime for.
        /// </summary>
        public float seconds = 0;
        /// <summary>
        /// Reps the current
        /// </summary>
        public object Current => null;

        /// <summary>
        /// Moves next
        /// </summary>
        /// <returns></returns>
        public bool MoveNext() 
        {
            if (i == 0)
            {
                startTime = Time.unscaledTime;
            }
            i++;
            return Time.unscaledTime < startTime + seconds;
        }
        /// <summary>
        /// Resets the wait.
        /// </summary>
        public void Reset() 
        {
            i = 0;
        }
        /// <summary>
        /// inits new instance of wait for seconds real time.
        /// </summary>
        /// <param name="seconds">The number of seconds to wait in realtime for.</param>
        public WaitForSecondsRealTime(float seconds)
        {
            this.seconds = seconds;
        }
    }
}
