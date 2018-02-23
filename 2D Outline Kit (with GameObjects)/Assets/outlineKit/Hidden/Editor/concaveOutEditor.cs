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
        SerializedProperty showOutline_GOs_InHierarchy;

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

        SerializedProperty size_O;

        //-----conCAVE

        SerializedProperty pushType_O_CAVE;
        SerializedProperty stdSize_O_CAVE;

        //ONLY regular
        SerializedProperty edgeCount_O_CAVE_R;
        SerializedProperty startAngle_O_CAVE_R;
        SerializedProperty pushPattern_O_CAVE_R;

        SerializedProperty rectSize_O_CAVE_RS;
        SerializedProperty rectWidth_O_CAVE_RS;
        SerializedProperty rectHeight_O_CAVE_RS;

        void OnEnable()
        {
            //Optimization
            updateSprite = serializedObject.FindProperty("updateSprite");

            //Debugging
            showOutline_GOs_InHierarchy = serializedObject.FindProperty("showOutline_GOs_InHierarchy");

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

            size_O = serializedObject.FindProperty("size_O");

            //-------------------------Push Type Variables-------------------------

            pushType_O_CAVE = serializedObject.FindProperty("pushType_O_CAVE");
            stdSize_O_CAVE = serializedObject.FindProperty("stdSize_O_CAVE");

            //ONLY regular
            edgeCount_O_CAVE_R = serializedObject.FindProperty("edgeCount_O_CAVE_R");
            startAngle_O_CAVE_R = serializedObject.FindProperty("startAngle_O_CAVE_R");
            pushPattern_O_CAVE_R = serializedObject.FindProperty("pushPattern_O_CAVE_R");

            rectSize_O_CAVE_RS = serializedObject.FindProperty("rectSize_O_CAVE_RS");
            rectWidth_O_CAVE_RS = serializedObject.FindProperty("rectWidth_O_CAVE_RS");
            rectHeight_O_CAVE_RS = serializedObject.FindProperty("rectHeight_O_CAVE_RS");
        }

        public override void OnInspectorGUI()
        {
            //set flags
            EditorStyles.helpBox.wordWrap = true;

            //IDK
            if (GUI.changed)
                EditorUtility.SetDirty(target);

            //link up to our script
            concaveOut script = (concaveOut)target;

            //grab properties from scripts
            serializedObject.Update();

            //---Optimization
            EditorGUILayout.PropertyField(updateSprite, new GUIContent("We Update The Sprite"));

            if (script.UpdateSprite == spriteUpdateSetting.Manually)
                if (GUILayout.Button("Update Sprite"))
                    script.updateSpriteData();

            EditorGUILayout.Space(); ///-------------------------

            //---Debugging
            EditorGUILayout.PropertyField(showOutline_GOs_InHierarchy, new GUIContent("Show Outline In Hierarchy"));

            EditorGUILayout.Space(); ///-------------------------

            //---Sprite Overlay
            EditorGUILayout.PropertyField(active_SO, new GUIContent("Activate Sprite Overlay"));

            if (script.Active_SO)
            {
                EditorGUILayout.PropertyField(orderInLayer_SO, new GUIContent("   it's Order In Layer"));
                EditorGUILayout.PropertyField(color_SO, new GUIContent("   it's Color"));
            }

            EditorGUILayout.Space(); ///-------------------------

            //---Clipping Mask
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

            EditorGUILayout.Space(); ///-------------------------

            //---Sprite Outline
            EditorGUILayout.PropertyField(active_O, new GUIContent("Active Sprite Outline"));

            if (script.Active_O)
            {
                EditorGUILayout.PropertyField(color_O, new GUIContent("   it's Color"));
                EditorGUILayout.PropertyField(orderInLayer_O, new GUIContent("   it's Order In Layer"));
                EditorGUILayout.PropertyField(scaleWithParentX_O, new GUIContent("   Follow Parent X Scale"));
                EditorGUILayout.PropertyField(scaleWithParentY_O, new GUIContent("   Follow Parent Y Scale"));

                EditorGUILayout.Space(); ///-------------------------

                EditorGUILayout.PropertyField(pushType_O_CAVE, new GUIContent("--Push With A"));

                if (script.PushType_O_CAVE == push.regularPattern)
                {
                    EditorGUILayout.PropertyField(startAngle_O_CAVE_R, new GUIContent("---Rotation"));

                    EditorGUILayout.PropertyField(stdSize_O_CAVE, new GUIContent("---Use Pattern Size"));

                    if (script.StdSize_O_CAVE)
                    {
                        EditorGUILayout.PropertyField(size_O, new GUIContent("---it's STD Size"));

                        EditorGUILayout.PropertyField(pushPattern_O_CAVE_R, new GUIContent("----Push Pattern"));

                        if (script.PushPattern_O_CAVE_R == pushPattern.squarial)
                        {
                            EditorGUILayout.PropertyField(edgeCount_O_CAVE_R, new GUIContent("---Multiplier Of Edges"));

                            EditorGUILayout.PropertyField(rectSize_O_CAVE_RS, new GUIContent("-----Rect Type"));

                            if (script.RectSize_O_CAVE_RS == rectType.custom)
                            {
                                EditorGUILayout.PropertyField(rectWidth_O_CAVE_RS, new GUIContent("------Width"));
                                EditorGUILayout.PropertyField(rectHeight_O_CAVE_RS, new GUIContent("------Height"));
                            }
                        }
                        else
                            EditorGUILayout.PropertyField(edgeCount_O_CAVE_R, new GUIContent("---Number Of Edges"));
                    }
                    else
                        EditorGUILayout.HelpBox("*you can change each edge by using the script's public function \n(editEdgeMagnitude) \n*An Editor Based Solution is in the works", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.PropertyField(stdSize_O_CAVE, new GUIContent("--Use Standard Size"));

                    if (script.StdSize_O_CAVE)
                        EditorGUILayout.PropertyField(size_O, new GUIContent("---it's STD Size"));
                    else
                        EditorGUILayout.HelpBox("*you can change each edge by using the script's public function \n(editEdgeMagnitude) \n*An Editor Based Solution is in the works", MessageType.Info);

                    EditorGUILayout.HelpBox("*you can change each edge by using the script's public function \n(addEdge, removeEdge, and editEdge) \n*An Editor Based Solution is in the works", MessageType.Info);
                }
            }

            //apply modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}