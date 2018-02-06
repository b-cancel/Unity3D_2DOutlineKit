using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace object2DOutlines
{
    [CustomEditor(typeof(concaveOut))]
    public class concaveOutEditor : Editor
    {

        //Optimization
        SerializedProperty updateSprite;

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

        //-------------------------Push Type Variables-------------------------

        SerializedProperty pushType_OP;

        //ONLY regular
        SerializedProperty objsMakingOutline_OPR;
        SerializedProperty startAngle_OPR;
        SerializedProperty radialPush_OPR;

        //ONLY custom
        SerializedProperty stdSize_OPC;

        void OnEnable()
        {
            //Optimization
            updateSprite = serializedObject.FindProperty("updateSprite");

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

            //-------------------------Push Type Variables-------------------------

            pushType_OP = serializedObject.FindProperty("pushType_OP");

            //ONLY regular
            objsMakingOutline_OPR = serializedObject.FindProperty("objsMakingOutline_OPR");
            startAngle_OPR = serializedObject.FindProperty("startAngle_OPR");
            radialPush_OPR = serializedObject.FindProperty("radialPush_OPR");

            //ONLY custom
            stdSize_OPC = serializedObject.FindProperty("stdSize_OPC");
        }

        public override void OnInspectorGUI()
        {
            concaveOut script = (concaveOut)target;

            serializedObject.Update();

            //Optimization
            EditorGUILayout.PropertyField(updateSprite, new GUIContent("We Update The Sprite"));


            if (script.UpdateSprite == spriteUpdateSetting.Manually)
                if (GUILayout.Button("Update Sprite"))
                    script.updateSpriteData();

            //Debugging
            EditorGUILayout.PropertyField(showOutline_GOs_InHierarchy_D, new GUIContent("Show Outline In Hierarchy"));

            //Sprite Overlay
            EditorGUILayout.PropertyField(active_SO, new GUIContent("Activate Sprite Overlay"));
            if (script.Active_SO)
            {
                EditorGUILayout.PropertyField(orderInLayer_SO, new GUIContent("   it's Order In Layer"));
                EditorGUILayout.PropertyField(color_SO, new GUIContent("   it's Color"));
            }

            //Clipping Mask

            EditorGUILayout.PropertyField(clipCenter_CM, new GUIContent("Support Semi-Transparency"));
            if (script.ClipCenter_CM)
            {
                EditorGUILayout.PropertyField(alphaCutoff_CM, new GUIContent("   it's Alpha Cut-Off"));
                EditorGUILayout.PropertyField(customRange_CM, new GUIContent("   Use A Custom Range"));
                if (script.CustomRange_CM)
                {
                    EditorGUILayout.PropertyField(frontLayer_CM, new GUIContent("      it's Front Layer"));
                    EditorGUILayout.PropertyField(backLayer_CM, new GUIContent("      it's Back Layer"));
                }
            }

            //Sprite Outline
            EditorGUILayout.PropertyField(active_O, new GUIContent("Active Sprite Outline"));
            if (script.Active_O)
            {
                EditorGUILayout.PropertyField(color_O, new GUIContent("   it's Color"));
                EditorGUILayout.PropertyField(orderInLayer_O, new GUIContent("   it's Order In Layer"));
                EditorGUILayout.PropertyField(size_O, new GUIContent("   it's Size")); //run update outline for everything below
                EditorGUILayout.PropertyField(scaleWithParentX_O, new GUIContent("   Follow Parent X Scale"));
                EditorGUILayout.PropertyField(scaleWithParentY_O, new GUIContent("   Follow Parent Y Scale"));
            }

            //-------------------------Push Type Variables-------------------------

            EditorGUILayout.PropertyField(pushType_OP, new GUIContent("Push With A"));

            if(script.PushType_OP == push.regularPattern)
            {
                EditorGUILayout.PropertyField(objsMakingOutline_OPR, new GUIContent("   # Of Outline Objects"));
                EditorGUILayout.PropertyField(startAngle_OPR, new GUIContent("   Start Angle For Pattern"));
                EditorGUILayout.PropertyField(radialPush_OPR, new GUIContent("   Push Pattern"));
            }
            else
            {
                EditorGUILayout.PropertyField(stdSize_OPC, new GUIContent("   Should All Pushes Be A Standard Size"));
            } 

            serializedObject.ApplyModifiedProperties();
        }
    }
}
