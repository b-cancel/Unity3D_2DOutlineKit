using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace object2DOutlines
{
    [CustomEditor(typeof(convexOut))]
    public class convexOutEditor : Editor
    {

        //Optimization
        SerializedProperty updateSpriteEveryFrame;

        //Debugging
        SerializedProperty showOutline_GOs_InHierarchy_D;

        //Sprite Outline
        SerializedProperty active_SO;
        SerializedProperty orderInLayer_SO;
        SerializedProperty color_SO;

        //Clipping Mask
        SerializedProperty clipCenter_CM;
        SerializedProperty alphaCutoff_CM;
        SerializedProperty customRange_CM;
        SerializedProperty frontLayer_CM;
        SerializedProperty backLayer_CM;

        //Sprite Overlay
        SerializedProperty active_O;
        SerializedProperty color_O;
        SerializedProperty orderInLayer_O;
        SerializedProperty size_O;
        SerializedProperty scaleWithParentX_O;
        SerializedProperty scaleWithParentY_O;

        void OnEnable()
        {
            //Optimization
            updateSpriteEveryFrame = serializedObject.FindProperty("updateSpriteEveryFrame");

            //Debugging
            showOutline_GOs_InHierarchy_D = serializedObject.FindProperty("showOutline_GOs_InHierarchy_D");

            //Sprite Outline
            active_SO = serializedObject.FindProperty("active_SO");
            orderInLayer_SO = serializedObject.FindProperty("orderInLayer_SO");
            color_SO = serializedObject.FindProperty("color_SO");

            //Clipping Mask
            clipCenter_CM = serializedObject.FindProperty("clipCenter_CM");
            alphaCutoff_CM = serializedObject.FindProperty("alphaCutoff_CM");
            customRange_CM = serializedObject.FindProperty("customRange_CM");
            frontLayer_CM = serializedObject.FindProperty("frontLayer_CM");
            backLayer_CM = serializedObject.FindProperty("backLayer_CM");

            //Sprite Overlay
            active_O = serializedObject.FindProperty("active_O");
            color_O = serializedObject.FindProperty("color_O");
            orderInLayer_O = serializedObject.FindProperty("orderInLayer_O");
            size_O = serializedObject.FindProperty("size_O");
            scaleWithParentX_O = serializedObject.FindProperty("scaleWithParentX_O");
            scaleWithParentY_O = serializedObject.FindProperty("scaleWithParentY_O");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //Optimization
            EditorGUILayout.PropertyField(updateSpriteEveryFrame);

            //Debugging
            EditorGUILayout.PropertyField(showOutline_GOs_InHierarchy_D);

            //Sprite Outline
            EditorGUILayout.PropertyField(active_SO);
            EditorGUILayout.PropertyField(orderInLayer_SO);
            EditorGUILayout.PropertyField(color_SO);

            //Clipping Mask
            EditorGUILayout.PropertyField(clipCenter_CM);
            EditorGUILayout.PropertyField(alphaCutoff_CM);
            EditorGUILayout.PropertyField(customRange_CM);
            EditorGUILayout.PropertyField(frontLayer_CM);
            EditorGUILayout.PropertyField(backLayer_CM);

            //Sprite Overlay
            EditorGUILayout.PropertyField(active_O);
            EditorGUILayout.PropertyField(color_O);
            EditorGUILayout.PropertyField(orderInLayer_O);
            EditorGUILayout.PropertyField(size_O); //run update outline for everything below
            EditorGUILayout.PropertyField(scaleWithParentX_O);
            EditorGUILayout.PropertyField(scaleWithParentY_O);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
