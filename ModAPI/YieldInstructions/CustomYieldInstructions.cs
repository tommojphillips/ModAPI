using System;
using System.Collections;
using UnityEngine;

namespace TommoJProductions.ModApi.YieldInstructions
{
    /// <summary>
    /// Represents helpful custom yield instructions.
    /// </summary>
    public class CustomYieldInstructions
    {
        /// <summary>
        /// Waits for the function to returns true. then continues. 
        /// </summary>
        /// <param name="behaviour">The MonoBehaviour to run the Coroutine on.</param>
        /// <param name="func">The funcion that returns whether to contine or not.</param>
        /// <returns>USAGE: yield return <see cref="waitWhile(MonoBehaviour, Func{bool})"/></returns>
        public static Coroutine waitWhile(MonoBehaviour behaviour, Func<bool> func) => behaviour.StartCoroutine(new WaitWhile(func));
        /// <summary>
        /// Waits for the specified amount of time. then continues.
        /// </summary>
        /// <param name="behaviour">The MonoBehaviour to run the Coroutine on.</param>
        /// <param name="seconds">The amount of seconds to wait for.</param>
        /// <returns>USAGE: yield return <see cref="waitForSecondsRealTime(MonoBehaviour, float)"/></returns>
        public static IEnumerator waitForSecondsRealTime(MonoBehaviour behaviour, float seconds) 
        {
            yield return new WaitForSecondsRealTime(seconds);
        }
    }
}
