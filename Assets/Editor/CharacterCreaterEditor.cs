
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterCreator))]
[CanEditMultipleObjects]
public class CharacterCreatorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Draw default inspector first
        DrawDefaultInspector();

        // Add some spacing
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Character"))
        {
            foreach (var t in targets)
            {
                ((CharacterCreator)t).SpawnCharacter();
            }
        }

        // Button 2
        if (GUILayout.Button("Kill Character"))
        {
            foreach (var t in targets)
                ((CharacterCreator)t).KillOneCharacter();
        }


    }
}
#endif