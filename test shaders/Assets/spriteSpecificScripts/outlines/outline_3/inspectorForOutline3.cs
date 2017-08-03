using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace objOutlines
{
    [RequireComponent(typeof(outline3))]
    [ExecuteInEditMode]
    public class inspectorForOutline3 : MonoBehaviour
    {

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

            //---Debugging Variables

            showOutline_GOs_InHierarchy_D = gameObject.GetComponent<outline3>().ShowOutline_GOs_InHierarchy_D;

            //---Sprite Overlay

            active_SO = gameObject.GetComponent<outline3>().Active_SO;
            orderInLayer_SO = gameObject.GetComponent<outline3>().OrderInLayer_SO;
            color_SO = gameObject.GetComponent<outline3>().Color_SO;

            //---Outline Variables

            active_O = gameObject.GetComponent<outline3>().Active_O;

            color_O = gameObject.GetComponent<outline3>().Color_O;
            orderInLayer_O = gameObject.GetComponent<outline3>().OrderInLayer_O;
            size_O = gameObject.GetComponent<outline3>().Size_O;
            scaleWithParentX_O = gameObject.GetComponent<outline3>().ScaleWithParentX_O;
            scaleWithParentY_O = gameObject.GetComponent<outline3>().ScaleWithParentY_O;

            //---Push Type Variables

            pushType_Regular_or_Custom_OP = gameObject.GetComponent<outline3>().PushType_Regular_or_Custom_OP;

            //regular

            objsMakingOutline_OPR = gameObject.GetComponent<outline3>().ObjsMakingOutline_OPR;
            startAngle_OPR = gameObject.GetComponent<outline3>().StartAngle_OPR;
            radialPush_OPR = gameObject.GetComponent<outline3>().RadialPush_OPR;

            //custom

            stdSize_OPC = gameObject.GetComponent<outline3>().StdSize_OPC;
        }

        void Update()
        {
            //---Debugging Variables

            if (showOutline_GOs_InHierarchy_D != gameObject.GetComponent<outline3>().ShowOutline_GOs_InHierarchy_D)
                gameObject.GetComponent<outline3>().ShowOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

            //--- Sprite Overlay Variables

            if (active_SO != gameObject.GetComponent<outline3>().Active_SO)
                gameObject.GetComponent<outline3>().Active_SO = active_SO;

            if (orderInLayer_SO != gameObject.GetComponent<outline3>().OrderInLayer_SO)
                gameObject.GetComponent<outline3>().OrderInLayer_SO = orderInLayer_SO;

            if (color_SO != gameObject.GetComponent<outline3>().Color_SO)
                gameObject.GetComponent<outline3>().Color_SO = color_SO;

            //---Outline Variables

            if (active_O != gameObject.GetComponent<outline3>().Active_O)
                gameObject.GetComponent<outline3>().Active_O = active_O;

            if (color_O != gameObject.GetComponent<outline3>().Color_O)
                gameObject.GetComponent<outline3>().Color_O = color_O;

            if (orderInLayer_O != gameObject.GetComponent<outline3>().OrderInLayer_O)
                gameObject.GetComponent<outline3>().OrderInLayer_O = orderInLayer_O;

            if (size_O != gameObject.GetComponent<outline3>().Size_O)
                gameObject.GetComponent<outline3>().Size_O = size_O;

            if (scaleWithParentX_O != gameObject.GetComponent<outline3>().ScaleWithParentX_O)
                gameObject.GetComponent<outline3>().ScaleWithParentX_O = scaleWithParentX_O;

            if (scaleWithParentY_O != gameObject.GetComponent<outline3>().ScaleWithParentY_O)
                gameObject.GetComponent<outline3>().ScaleWithParentY_O = scaleWithParentY_O;

            //---Push Type Variables

            if (pushType_Regular_or_Custom_OP != gameObject.GetComponent<outline3>().PushType_Regular_or_Custom_OP)
                gameObject.GetComponent<outline3>().PushType_Regular_or_Custom_OP = pushType_Regular_or_Custom_OP;

            //Regular

            if (objsMakingOutline_OPR != gameObject.GetComponent<outline3>().ObjsMakingOutline_OPR)
                gameObject.GetComponent<outline3>().ObjsMakingOutline_OPR = objsMakingOutline_OPR;

            if (startAngle_OPR != gameObject.GetComponent<outline3>().StartAngle_OPR)
                gameObject.GetComponent<outline3>().StartAngle_OPR = startAngle_OPR;

            if (radialPush_OPR != gameObject.GetComponent<outline3>().RadialPush_OPR)
                gameObject.GetComponent<outline3>().RadialPush_OPR = radialPush_OPR;

            //Custom

            if (stdSize_OPC != gameObject.GetComponent<outline3>().StdSize_OPC)
                gameObject.GetComponent<outline3>().StdSize_OPC = stdSize_OPC;
        }
    }
}