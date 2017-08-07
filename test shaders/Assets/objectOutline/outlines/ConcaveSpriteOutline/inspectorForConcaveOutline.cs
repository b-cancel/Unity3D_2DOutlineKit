﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace objOutlines
{
    [RequireComponent(typeof(concaveOutline))]
    [ExecuteInEditMode]
    public class inspectorForConcaveOutline : MonoBehaviour
    {
        [Header("Do you Change Sprite Setting In Runtime?")]
        public bool updateSpriteEveryFrame;

        //-----Variables for Used in Awake-----

        /*
        [Header("*****ONLY used in the Awake Function*****")]
        [Header("Changing them will have an effect only when...")]
        [Header("(1) going between (Edit Mode) and (Play Mode)")]
        [Header("(2) duplicating the Object in either mode")]
        */

        //-----Debugging Variables

        [Header("*****Debugging Variables*****[D]")]
        public bool showOutline_GOs_InHierarchy_D;

        //-----Sprite Overlay Variables

        [Header("*****Sprite Overlay Variables*****[SO]")]
        public bool active_SO;
        public int orderInLayer_SO;
        public Color color_SO;

        //-----Clipping Mask Variables

        [Header("*****Clipping Mask Variables*****[CM]")]
        [Header("Performance Drop -> (1/4)")]
        [Header("Suggested TRUE when (1) Semi-Transparent Sprites (2) Inline Instead of Outline")]
        public bool clipCenter_CM;

        [Range(0, 1)]
        public float alphaCutoff_CM;

        public bool customRange_CM;
        public int frontLayer_CM;
        public int backLayer_CM;

        //-----Outline Variables

        [Header("*****Outline Variables*****[O]")]
        public bool active_O;

        public Color color_O;
        public int orderInLayer_O;
        [Range(0, 2.5f)]
        public float size_O; //NOTE: size is handled differently depending on your "outlineType_Scaling_or_Pushing"
        public bool scaleWithParentX_O;
        public bool scaleWithParentY_O;

        //-----Push Type Variables

        [Header("***Pushing Type Options[P]")]
        [Header("More Edges -> Less Performace")]
        public bool pushType_Regular_or_Custom_OP;

        //---regular

        [Header("*Regular Push[R]")] //->R<-
        public int objsMakingOutline_OPR; //also the count of gameobjects that make up the outline
        public float startAngle_OPR;
        public bool radialPush_OPR; //push objs to edge of circle or to edge of box

        //---custom

        [Header("*Custom Push[C]")] //->C<-
        public bool stdSize_OPC;

        void Start() //using Start() because this must run after Awake() of "outline2D"
        {
            //--- Grab Defaults Set in the Master Outline Script

            updateSpriteEveryFrame = gameObject.GetComponent<concaveOutline>().UpdateSpriteEveryFrame;

            //---Debugging Variables

            showOutline_GOs_InHierarchy_D = gameObject.GetComponent<concaveOutline>().ShowOutline_GOs_InHierarchy_D;

            //---Sprite Overlay

            active_SO = gameObject.GetComponent<concaveOutline>().Active_SO;
            orderInLayer_SO = gameObject.GetComponent<concaveOutline>().OrderInLayer_SO;
            color_SO = gameObject.GetComponent<concaveOutline>().Color_SO;

            //---Clipping Mask Variables

            clipCenter_CM = gameObject.GetComponent<concaveOutline>().ClipCenter_CM;
            alphaCutoff_CM = gameObject.GetComponent<concaveOutline>().AlphaCutoff_CM;
            customRange_CM = gameObject.GetComponent<concaveOutline>().CustomRange_CM;
            frontLayer_CM = gameObject.GetComponent<concaveOutline>().FrontLayer_CM;
            backLayer_CM = gameObject.GetComponent<concaveOutline>().BackLayer_CM;

            //---Outline Variables

            active_O = gameObject.GetComponent<concaveOutline>().Active_O;

            color_O = gameObject.GetComponent<concaveOutline>().Color_O;
            orderInLayer_O = gameObject.GetComponent<concaveOutline>().OrderInLayer_O;
            size_O = gameObject.GetComponent<concaveOutline>().Size_O;
            scaleWithParentX_O = gameObject.GetComponent<concaveOutline>().ScaleWithParentX_O;
            scaleWithParentY_O = gameObject.GetComponent<concaveOutline>().ScaleWithParentY_O;

            //---Push Type Variables

            pushType_Regular_or_Custom_OP = gameObject.GetComponent<concaveOutline>().PushType_Regular_or_Custom_OP;

            //regular

            objsMakingOutline_OPR = gameObject.GetComponent<concaveOutline>().ObjsMakingOutline_OPR;
            startAngle_OPR = gameObject.GetComponent<concaveOutline>().StartAngle_OPR;
            radialPush_OPR = gameObject.GetComponent<concaveOutline>().RadialPush_OPR;

            //custom

            stdSize_OPC = gameObject.GetComponent<concaveOutline>().StdSize_OPC;
        }

        void Update()
        {
            if (updateSpriteEveryFrame != gameObject.GetComponent<concaveOutline>().UpdateSpriteEveryFrame)
                gameObject.GetComponent<concaveOutline>().UpdateSpriteEveryFrame = updateSpriteEveryFrame;

            //---Debugging Variables

            if (showOutline_GOs_InHierarchy_D != gameObject.GetComponent<concaveOutline>().ShowOutline_GOs_InHierarchy_D)
                gameObject.GetComponent<concaveOutline>().ShowOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

            //--- Sprite Overlay Variables

            if (active_SO != gameObject.GetComponent<concaveOutline>().Active_SO)
                gameObject.GetComponent<concaveOutline>().Active_SO = active_SO;

            if (orderInLayer_SO != gameObject.GetComponent<concaveOutline>().OrderInLayer_SO)
                gameObject.GetComponent<concaveOutline>().OrderInLayer_SO = orderInLayer_SO;

            if (color_SO != gameObject.GetComponent<concaveOutline>().Color_SO)
                gameObject.GetComponent<concaveOutline>().Color_SO = color_SO;

            //--- Clipping Mask Variables

            if (clipCenter_CM != gameObject.GetComponent<concaveOutline>().ClipCenter_CM)
                gameObject.GetComponent<concaveOutline>().ClipCenter_CM = clipCenter_CM;

            if (alphaCutoff_CM != gameObject.GetComponent<concaveOutline>().AlphaCutoff_CM)
                gameObject.GetComponent<concaveOutline>().AlphaCutoff_CM = alphaCutoff_CM;

            if (customRange_CM != gameObject.GetComponent<concaveOutline>().CustomRange_CM)
                gameObject.GetComponent<concaveOutline>().CustomRange_CM = customRange_CM;

            if (frontLayer_CM != gameObject.GetComponent<concaveOutline>().FrontLayer_CM)
                gameObject.GetComponent<concaveOutline>().FrontLayer_CM = frontLayer_CM;

            if (backLayer_CM != gameObject.GetComponent<concaveOutline>().BackLayer_CM)
                gameObject.GetComponent<concaveOutline>().BackLayer_CM = backLayer_CM;

            //---Outline Variables

            if (active_O != gameObject.GetComponent<concaveOutline>().Active_O)
                gameObject.GetComponent<concaveOutline>().Active_O = active_O;

            if (color_O != gameObject.GetComponent<concaveOutline>().Color_O)
                gameObject.GetComponent<concaveOutline>().Color_O = color_O;

            if (orderInLayer_O != gameObject.GetComponent<concaveOutline>().OrderInLayer_O)
                gameObject.GetComponent<concaveOutline>().OrderInLayer_O = orderInLayer_O;

            if (size_O != gameObject.GetComponent<concaveOutline>().Size_O)
                gameObject.GetComponent<concaveOutline>().Size_O = size_O;

            if (scaleWithParentX_O != gameObject.GetComponent<concaveOutline>().ScaleWithParentX_O)
                gameObject.GetComponent<concaveOutline>().ScaleWithParentX_O = scaleWithParentX_O;

            if (scaleWithParentY_O != gameObject.GetComponent<concaveOutline>().ScaleWithParentY_O)
                gameObject.GetComponent<concaveOutline>().ScaleWithParentY_O = scaleWithParentY_O;

            //---Push Type Variables

            if (pushType_Regular_or_Custom_OP != gameObject.GetComponent<concaveOutline>().PushType_Regular_or_Custom_OP)
                gameObject.GetComponent<concaveOutline>().PushType_Regular_or_Custom_OP = pushType_Regular_or_Custom_OP;

            //Regular

            if (objsMakingOutline_OPR != gameObject.GetComponent<concaveOutline>().ObjsMakingOutline_OPR)
                gameObject.GetComponent<concaveOutline>().ObjsMakingOutline_OPR = objsMakingOutline_OPR;

            if (startAngle_OPR != gameObject.GetComponent<concaveOutline>().StartAngle_OPR)
                gameObject.GetComponent<concaveOutline>().StartAngle_OPR = startAngle_OPR;

            if (radialPush_OPR != gameObject.GetComponent<concaveOutline>().RadialPush_OPR)
                gameObject.GetComponent<concaveOutline>().RadialPush_OPR = radialPush_OPR;

            //Custom

            if (stdSize_OPC != gameObject.GetComponent<concaveOutline>().StdSize_OPC)
                gameObject.GetComponent<concaveOutline>().StdSize_OPC = stdSize_OPC;
        }
    }
}