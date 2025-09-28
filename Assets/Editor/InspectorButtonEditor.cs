using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Game.DebugTools;

// works on ANY GameObject
[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class InspectorButtonEditor : Editor
{
    // cache parameter values per method
    private Dictionary<string, object[]> methodParams = new Dictionary<string, object[]>();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        var type = target.GetType();
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            if (!System.Attribute.IsDefined(method, typeof(InspectorButtonAttribute)))
                continue;

            var parameters = method.GetParameters();
            string methodKey = type.FullName + "." + method.Name;

            if (!methodParams.ContainsKey(methodKey))
                methodParams[methodKey] = new object[parameters.Length];

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(method.Name), EditorStyles.boldLabel);

            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                object currentValue = methodParams[methodKey][i];
                methodParams[methodKey][i] = DrawParameterField(p, currentValue);
            }

            if (GUILayout.Button("Run"))
            {
                foreach (var t in targets)
                {
                    method.Invoke(t, methodParams[methodKey]);
                    EditorUtility.SetDirty(t);
                }
            }

            EditorGUILayout.EndVertical();
        }
    }

    private object DrawParameterField(ParameterInfo p, object value)
    {
        var type = p.ParameterType;

        // arrays and lists
        if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
        {
            System.Type elementType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];

            // convert to list
            var list = new List<object>();
            if (value is System.Array arr)
                list.AddRange(arr.Cast<object>());
            else if (value is System.Collections.IEnumerable enumerable && !(value is string))
                foreach (var item in enumerable) list.Add(item);

            EditorGUILayout.BeginVertical("box");
            int newSize = EditorGUILayout.IntField($"{p.Name} Size", list.Count);

            while (newSize > list.Count) list.Add(GetDefault(elementType));
            while (newSize < list.Count) list.RemoveAt(list.Count - 1);

            for (int i = 0; i < list.Count; i++)
                list[i] = DrawFieldForType(elementType, list[i], $"{p.Name} [{i}]");

            EditorGUILayout.EndVertical();

            if (type.IsArray)
                return list.ToArray();
            else
            {
                var genericList = (System.Collections.IList)System.Activator.CreateInstance(type);
                foreach (var item in list) genericList.Add(item);
                return genericList;
            }
        }

        // single field
        return DrawFieldForType(type, value, p.Name);
    }

    private object DrawFieldForType(System.Type type, object value, string label)
    {
        // primitive params
        if (type == typeof(int))
            return EditorGUILayout.IntField(label, (int)(value ?? 0));

        if (type == typeof(float))
            return EditorGUILayout.FloatField(label, (float)(value ?? 0f));

        if (type == typeof(bool))
            return EditorGUILayout.Toggle(label, (bool)(value ?? false));

        if (type == typeof(string))
            return EditorGUILayout.TextField(label, (string)(value ?? ""));

        // enum and object reference
        if (type.IsEnum)
            return EditorGUILayout.EnumPopup(label, (System.Enum)(value ?? System.Enum.GetValues(type).GetValue(0)));

        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            return EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, type, true);

        // Unity extended types
        if (type == typeof(Vector2))
            return EditorGUILayout.Vector2Field(label, value != null ? (Vector2)value : Vector2.zero);

        if (type == typeof(Vector3))
            return EditorGUILayout.Vector3Field(label, value != null ? (Vector3)value : Vector3.zero);

        if (type == typeof(Vector4))
            return EditorGUILayout.Vector4Field(label, value != null ? (Vector4)value : Vector4.zero);

        if (type == typeof(Color))
            return EditorGUILayout.ColorField(label, value != null ? (Color)value : Color.white);

        if (type == typeof(LayerMask))
            return EditorGUILayout.LayerField(label, (int)(value ?? 0));

        EditorGUILayout.LabelField($"{label} (Unsupported: {type.Name})");
        return value;
    }

    private object GetDefault(System.Type type)
    {
        if (type.IsValueType) return System.Activator.CreateInstance(type);
        return null;
    }
}
