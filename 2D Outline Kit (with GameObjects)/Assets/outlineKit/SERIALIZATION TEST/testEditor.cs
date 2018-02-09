using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace object2DOutlines
{
    [CustomEditor(typeof(testOut))]
    public class testEditor : Editor
    {
        //Optimization
        SerializedProperty updateSprite;

        //Debugging
        SerializedProperty showOutline_GOs_InHierarchy_D;

        //Sprite Outline
        SerializedProperty active_O;
        SerializedProperty color_O;

        void OnEnable()
        {
            //Optimization
            updateSprite = serializedObject.FindProperty("updateSprite");

            //Debugging
            showOutline_GOs_InHierarchy_D = serializedObject.FindProperty("showOutline_GOs_InHierarchy_D");

            //outline vars
            active_O = serializedObject.FindProperty("active_O");
            color_O = serializedObject.FindProperty("color_O");
        }

        public override void OnInspectorGUI()
        {
            if (GUI.changed)
                EditorUtility.SetDirty(target);

            testOut script = (testOut)target;

            serializedObject.Update();

            //Optimization
            EditorGUILayout.PropertyField(updateSprite, new GUIContent("We Update The Sprite"));

            if (script.UpdateSprite == spriteUpdateSetting.Manually)
                if (GUILayout.Button("Update Sprite"))
                    script.updateSpriteData();

            //Debugging
            EditorGUILayout.PropertyField(showOutline_GOs_InHierarchy_D, new GUIContent("Show Outline In Hierarchy"));

            //Sprite Outline
            EditorGUILayout.PropertyField(active_O, new GUIContent("Active Sprite Outline"));
            if (script.Active_O)
            {
                EditorGUILayout.PropertyField(color_O, new GUIContent("   it's Color"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
