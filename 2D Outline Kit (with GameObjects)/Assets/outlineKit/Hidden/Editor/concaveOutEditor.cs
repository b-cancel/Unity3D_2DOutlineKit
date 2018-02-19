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

        //Overlay
        SerializedProperty active_SO;
        SerializedProperty orderInLayer_SO;
        SerializedProperty color_SO;

        //Clipping Mask
        SerializedProperty clipCenter_CM;
        SerializedProperty alphaCutoff_CM;
        SerializedProperty customRange_CM;
        SerializedProperty frontLayer_CM;
        SerializedProperty backLayer_CM;

        //Outline
        SerializedProperty active_O;
        SerializedProperty color_O;
        SerializedProperty orderInLayer_O;
        SerializedProperty scaleWithParentX_O;
        SerializedProperty scaleWithParentY_O;

        //-------------------------Push Type Variables-------------------------(only rotation)

        SerializedProperty pushType_OP;
        SerializedProperty stdSize_OP;
        SerializedProperty size_OP;

        //ONLY regular
        SerializedProperty edgeCount_OPR;
        SerializedProperty startAngle_OPR;
        SerializedProperty pushPattern_OPR;

        SerializedProperty rectSize_OPRS;
        SerializedProperty rectWidth_OPRS;
        SerializedProperty rectHeight_OPRS;

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
            scaleWithParentX_O = serializedObject.FindProperty("scaleWithParentX_O");
            scaleWithParentY_O = serializedObject.FindProperty("scaleWithParentY_O");

            //-------------------------Push Type Variables-------------------------

            pushType_OP = serializedObject.FindProperty("pushType_OP");
            stdSize_OP = serializedObject.FindProperty("stdSize_OP");
            size_OP = serializedObject.FindProperty("size_OP");

            //ONLY regular
            edgeCount_OPR = serializedObject.FindProperty("edgeCount_OPR");
            startAngle_OPR = serializedObject.FindProperty("startAngle_OPR");
            pushPattern_OPR = serializedObject.FindProperty("pushPattern_OPR");

            rectSize_OPRS = serializedObject.FindProperty("rectSize_OPRS");
            rectWidth_OPRS = serializedObject.FindProperty("rectWidth_OPRS");
            rectHeight_OPRS = serializedObject.FindProperty("rectHeight_OPRS");
        }

        public override void OnInspectorGUI()
        {
            if (GUI.changed)
                EditorUtility.SetDirty(target);

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
                EditorGUILayout.PropertyField(scaleWithParentX_O, new GUIContent("   Follow Parent X Scale"));
                EditorGUILayout.PropertyField(scaleWithParentY_O, new GUIContent("   Follow Parent Y Scale"));
            }

            //-------------------------Push Type Variables-------------------------

            EditorGUILayout.PropertyField(pushType_OP, new GUIContent("Push With A"));

            if (script.PushType_OP == push.regularPattern)
                EditorGUILayout.PropertyField(stdSize_OP, new GUIContent("   Use Pattern Size")); //run update outline for everything below
            else
                EditorGUILayout.PropertyField(stdSize_OP, new GUIContent("   Use Standard Size")); //run update outline for everything below

            if (script.StdSize_OP)
            {
                EditorGUILayout.PropertyField(size_OP, new GUIContent("      it's STD Size")); //run update outline for everything below
            }

            if (script.PushType_OP == push.regularPattern)
            {
                EditorGUILayout.PropertyField(edgeCount_OPR, new GUIContent("   # Of Edges"));
                EditorGUILayout.PropertyField(startAngle_OPR, new GUIContent("   Rotation"));

                if (script.StdSize_OP)
                {
                    EditorGUILayout.PropertyField(pushPattern_OPR, new GUIContent("   Push Pattern"));

                    if(script.PushPattern_OPR == pushPattern.squarial)
                    {
                        EditorGUILayout.PropertyField(rectSize_OPRS, new GUIContent("      Rect Type"));

                        if (script.RectSize_OPRS == rectType.custom)
                        {
                            EditorGUILayout.PropertyField(rectWidth_OPRS, new GUIContent("         Width"));
                            EditorGUILayout.PropertyField(rectHeight_OPRS, new GUIContent("         Height"));
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
