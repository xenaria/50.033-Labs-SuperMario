using UnityEngine;
using System;
namespace Game.DebugTools
{
    /// <summary>
    /// Marks a method to be drawn as a button in the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class InspectorButtonAttribute : PropertyAttribute { }
}
