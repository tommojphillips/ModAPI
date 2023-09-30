using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace TommoJProductions.ModApi.Attachable
{
    /// <summary>
    /// <see cref="ScriptableObject"/>. Parts (<see cref="Part.triggerData"/>) can be installed to triggers (<see cref="TriggerCallback.triggerData"/>) that have the same <see cref="TriggerData"/>.
    /// </summary>
    public class TriggerData : ScriptableObject
    {
        /// <summary>
        /// Creates a new instance of trigger data with an id of <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID of this trigger data.</param>
        public static TriggerData createTriggerData(string id)
        {
            TriggerData data = ScriptableObject.CreateInstance<TriggerData>();
            data._id = id;
            data.name = data._id;

            return data;
        }

        private string _id;

        /// <summary>
        /// Represents the ID of this TriggerData.
        /// </summary>
        public string id => _id;
    }
}
