using System.Collections;

using UnityEngine;

namespace TommoJProductions.ModApi.YieldInstructions
{
    /// <summary>
    /// Waits for seconds in real time.
    /// </summary>
    public class WaitForSecondsRealTime : IEnumerator
    {
        // Written, 11.09.2022

        private int _i;
        private float _startTime;

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
            if (_i == 0)
            {
                _startTime = Time.time;
            }
            _i++;
            return Time.time < _startTime + seconds;
        }
        /// <summary>
        /// Resets the wait.
        /// </summary>
        public void Reset() 
        {
            _i = 0;
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
