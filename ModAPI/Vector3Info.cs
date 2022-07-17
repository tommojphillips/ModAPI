using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TommoJProductions.ModApi
{
    /// <summary>
    /// Represents a only x, y, z from a Vector3
    /// </summary>
    public class Vector3Info
    {
        // Written, 15.07.2022

        public float x { get; set; } = 0;
        public float y { get; set; } = 0;
        public float z { get; set; } = 0;

        /// <summary>
        /// inits a new info with zeros.
        /// </summary>
        public Vector3Info() { }

        /// <summary>
        /// inits a new vector3info with vector3 values.
        /// </summary>
        /// <param name="v"></param>
        public Vector3Info(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        /// <summary>
        /// converts a vector3.
        /// </summary>
        /// <param name="v">the vector3 to convert.</param>
        /// <returns>new instance Vector3Info</returns>
        public static Vector3Info getInfo(Vector3 v)
        {
            return new Vector3Info(v);
        }

        public static Vector3 operator *(Vector3Info v1, Vector3 v2)
        {
            Vector3 v = Vector3.zero;
            Vector3 v1_ = (Vector3)v1;
            v.x = (v1_ * v2.x).x;
            v.y = (v1_ * v2.y).y;
            v.z = (v1_ * v2.z).z;
            return v;
        }
        public static Vector3Info operator +(Vector3Info v1, Vector3 v2) => new Vector3Info(v1.toVector3() + v2);
        public static Vector3Info operator -(Vector3Info v1, Vector3 v2) => new Vector3Info(v1.toVector3() - v2);
        public static bool operator ==(Vector3Info v1, Vector3 v2) => v1.toVector3() == v2;
        public static bool operator !=(Vector3Info v1, Vector3 v2) => v1.toVector3() != v2;
        public static implicit operator Vector3(Vector3Info v) => v.toVector3();
        public static implicit operator Vector3Info(Vector3 v) => new Vector3Info(v);
    }
}
