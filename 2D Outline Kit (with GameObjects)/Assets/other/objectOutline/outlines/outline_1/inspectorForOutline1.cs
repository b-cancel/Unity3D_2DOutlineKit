using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace objOutlines
{
    [RequireComponent(typeof(outline1))]
    [ExecuteInEditMode]
    public class inspectorForOutline1 : MonoBehaviour
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
        [Range(1, 5)]
        public float size_O; //NOTE: size is handled differently depending on your "outlineType_Scaling_or_Pushing"
        public bool scaleWithParentX_O;
        public bool scaleWithParentY_O;

        void Start() //using Start() because this must run after Awake() of "outline2D"
        {
            //--- Grab Defaults Set in the Master Outline Script

            //---Debugging Variables

            showOutline_GOs_InHierarchy_D = gameObject.GetComponent<outline1>().ShowOutline_GOs_InHierarchy_D;

            //---Sprite Overlay

            active_SO = gameObject.GetComponent<outline1>().Active_SO;
            orderInLayer_SO = gameObject.GetComponent<outline1>().OrderInLayer_SO;
            color_SO = gameObject.GetComponent<outline1>().Color_SO;

            //---Outline Variables

            active_O = gameObject.GetComponent<outline1>().Active_O;

            color_O = gameObject.GetComponent<outline1>().Color_O;
            orderInLayer_O = gameObject.GetComponent<outline1>().OrderInLayer_O;
            size_O = gameObject.GetComponent<outline1>().Size_O;
            scaleWithParentX_O = gameObject.GetComponent<outline1>().ScaleWithParentX_O;
            scaleWithParentY_O = gameObject.GetComponent<outline1>().ScaleWithParentY_O;
        }

        void Update()
        {
            //---Debugging Variables

            if (showOutline_GOs_InHierarchy_D != gameObject.GetComponent<outline1>().ShowOutline_GOs_InHierarchy_D)
                gameObject.GetComponent<outline1>().ShowOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

            //--- Sprite Overlay Variables

            if (active_SO != gameObject.GetComponent<outline1>().Active_SO)
                gameObject.GetComponent<outline1>().Active_SO = active_SO;

            if (orderInLayer_SO != gameObject.GetComponent<outline1>().OrderInLayer_SO)
                gameObject.GetComponent<outline1>().OrderInLayer_SO = orderInLayer_SO;

            if (color_SO != gameObject.GetComponent<outline1>().Color_SO)
                gameObject.GetComponent<outline1>().Color_SO = color_SO;

            //---Outline Variables

            if (active_O != gameObject.GetComponent<outline1>().Active_O)
                gameObject.GetComponent<outline1>().Active_O = active_O;

            if (color_O != gameObject.GetComponent<outline1>().Color_O)
                gameObject.GetComponent<outline1>().Color_O = color_O;

            if (orderInLayer_O != gameObject.GetComponent<outline1>().OrderInLayer_O)
                gameObject.GetComponent<outline1>().OrderInLayer_O = orderInLayer_O;

            if (size_O != gameObject.GetComponent<outline1>().Size_O)
                gameObject.GetComponent<outline1>().Size_O = size_O;

            if (scaleWithParentX_O != gameObject.GetComponent<outline1>().ScaleWithParentX_O)
                gameObject.GetComponent<outline1>().ScaleWithParentX_O = scaleWithParentX_O;

            if (scaleWithParentY_O != gameObject.GetComponent<outline1>().ScaleWithParentY_O)
                gameObject.GetComponent<outline1>().ScaleWithParentY_O = scaleWithParentY_O;
        }
    }
}