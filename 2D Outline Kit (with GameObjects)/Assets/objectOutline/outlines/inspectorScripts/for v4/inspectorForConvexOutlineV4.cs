using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace objOutlines
{
    [RequireComponent(typeof(convexOutlineV4))]
    [ExecuteInEditMode]
    public class inspectorForConvexOutlineV4 : MonoBehaviour
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
        [Range(0, 5)]
        public float size_O; //NOTE: size is handled differently depending on your "outlineType_Scaling_or_Pushing"
        public bool scaleWithParentX_O;
        public bool scaleWithParentY_O;
        public bool forceRetainPorpWithChildren;

        void Start() //using Start() because this must run after Awake() of "outline2D"
        {
            //--- Grab Defaults Set in the Master Outline Script

            updateSpriteEveryFrame = gameObject.GetComponent<convexOutlineV4>().UpdateSpriteEveryFrame;

            //---Debugging Variables

            showOutline_GOs_InHierarchy_D = gameObject.GetComponent<convexOutlineV4>().ShowOutline_GOs_InHierarchy_D;

            //---Sprite Overlay

            active_SO = gameObject.GetComponent<convexOutlineV4>().Active_SO;
            orderInLayer_SO = gameObject.GetComponent<convexOutlineV4>().OrderInLayer_SO;
            color_SO = gameObject.GetComponent<convexOutlineV4>().Color_SO;

            //---Clipping Mask Variables

            clipCenter_CM = gameObject.GetComponent<convexOutlineV4>().ClipCenter_CM;
            alphaCutoff_CM = gameObject.GetComponent<convexOutlineV4>().AlphaCutoff_CM;
            customRange_CM = gameObject.GetComponent<convexOutlineV4>().CustomRange_CM;
            frontLayer_CM = gameObject.GetComponent<convexOutlineV4>().FrontLayer_CM;
            backLayer_CM = gameObject.GetComponent<convexOutlineV4>().BackLayer_CM;

            //---Outline Variables

            active_O = gameObject.GetComponent<convexOutlineV4>().Active_O;

            color_O = gameObject.GetComponent<convexOutlineV4>().Color_O;
            orderInLayer_O = gameObject.GetComponent<convexOutlineV4>().OrderInLayer_O;
            size_O = gameObject.GetComponent<convexOutlineV4>().Size_O;
            scaleWithParentX_O = gameObject.GetComponent<convexOutlineV4>().ScaleWithParentX_O;
            scaleWithParentY_O = gameObject.GetComponent<convexOutlineV4>().ScaleWithParentY_O;
            forceRetainPorpWithChildren = gameObject.GetComponent<convexOutlineV4>().ForceRetainPorpWithChildren;
        }

        void Update()
        {
            if (updateSpriteEveryFrame != gameObject.GetComponent<convexOutlineV4>().UpdateSpriteEveryFrame)
                gameObject.GetComponent<convexOutlineV4>().UpdateSpriteEveryFrame = updateSpriteEveryFrame;

            //---Debugging Variables

            if (showOutline_GOs_InHierarchy_D != gameObject.GetComponent<convexOutlineV4>().ShowOutline_GOs_InHierarchy_D)
                gameObject.GetComponent<convexOutlineV4>().ShowOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

            //--- Sprite Overlay Variables

            if (active_SO != gameObject.GetComponent<convexOutlineV4>().Active_SO)
                gameObject.GetComponent<convexOutlineV4>().Active_SO = active_SO;

            if (orderInLayer_SO != gameObject.GetComponent<convexOutlineV4>().OrderInLayer_SO)
                gameObject.GetComponent<convexOutlineV4>().OrderInLayer_SO = orderInLayer_SO;

            if (color_SO != gameObject.GetComponent<convexOutlineV4>().Color_SO)
                gameObject.GetComponent<convexOutlineV4>().Color_SO = color_SO;

            //--- Clipping Mask Variables

            if (clipCenter_CM != gameObject.GetComponent<convexOutlineV4>().ClipCenter_CM)
                gameObject.GetComponent<convexOutlineV4>().ClipCenter_CM = clipCenter_CM;

            if (alphaCutoff_CM != gameObject.GetComponent<convexOutlineV4>().AlphaCutoff_CM)
                gameObject.GetComponent<convexOutlineV4>().AlphaCutoff_CM = alphaCutoff_CM;

            if (customRange_CM != gameObject.GetComponent<convexOutlineV4>().CustomRange_CM)
                gameObject.GetComponent<convexOutlineV4>().CustomRange_CM = customRange_CM;

            if (frontLayer_CM != gameObject.GetComponent<convexOutlineV4>().FrontLayer_CM)
                gameObject.GetComponent<convexOutlineV4>().FrontLayer_CM = frontLayer_CM;

            if (backLayer_CM != gameObject.GetComponent<convexOutlineV4>().BackLayer_CM)
                gameObject.GetComponent<convexOutlineV4>().BackLayer_CM = backLayer_CM;

            //---Outline Variables

            if (active_O != gameObject.GetComponent<convexOutlineV4>().Active_O)
                gameObject.GetComponent<convexOutlineV4>().Active_O = active_O;

            if (color_O != gameObject.GetComponent<convexOutlineV4>().Color_O)
                gameObject.GetComponent<convexOutlineV4>().Color_O = color_O;

            if (orderInLayer_O != gameObject.GetComponent<convexOutlineV4>().OrderInLayer_O)
                gameObject.GetComponent<convexOutlineV4>().OrderInLayer_O = orderInLayer_O;

            if (size_O != gameObject.GetComponent<convexOutlineV4>().Size_O)
                gameObject.GetComponent<convexOutlineV4>().Size_O = size_O;

            if (scaleWithParentX_O != gameObject.GetComponent<convexOutlineV4>().ScaleWithParentX_O)
                gameObject.GetComponent<convexOutlineV4>().ScaleWithParentX_O = scaleWithParentX_O;

            if (scaleWithParentY_O != gameObject.GetComponent<convexOutlineV4>().ScaleWithParentY_O)
                gameObject.GetComponent<convexOutlineV4>().ScaleWithParentY_O = scaleWithParentY_O;

            if (forceRetainPorpWithChildren != gameObject.GetComponent<convexOutlineV4>().ForceRetainPorpWithChildren)
                gameObject.GetComponent<convexOutlineV4>().ForceRetainPorpWithChildren = forceRetainPorpWithChildren;
        }
    }
}