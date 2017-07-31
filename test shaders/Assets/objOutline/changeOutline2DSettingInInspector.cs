using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class changeOutline2DSettingInInspector : MonoBehaviour {

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

    [Header("*****Outline Variables*****[O]")]
    public bool active_O;

    public Color color_O;
    public int orderInLayer_O;
    [Range(0, 5)]
    public float size_O; //NOTE: size is handled differently depending on your "outlineType_Scaling_or_Pushing"
    public bool scaleWithParentX_O;
    public bool scaleWithParentY_O;

    public bool outlineType_Scaling_or_Pushing_O;

    [Header("***Scaling Type Options[S]")]
    //NOTE: this would let you create inlines if desired
    public float ClippingMaskScale_OS; //TODO... 

    [Header("***Pushing Type Options[P]")]
    [Header("More Edges -> Less Performace")]
    public bool pushType_Regular_or_Custom_OP;

    [Header("*Regular Push[R]")] //->R<-
    public int objsMakingOutline_OPR; //also the count of gameobjects that make up the outline
    public float startAngle_OPR;
    public bool radialPush_OPR; //push objs to edge of circle or to edge of box

    [Header("*Custom Push[C]")] //->C<-
    public bool stdSize_OPC;

    void Start() //using Start() because this must run after Awake() of "outline2D"
    {
        //--- Grab Defaults Set in the Master Outline Script

        //Debugging Variables

        showOutline_GOs_InHierarchy_D = gameObject.GetComponent<outline2D>().ShowOutline_GOs_InHierarchy_D;

        //Sprite Overlay Variables

        active_SO = gameObject.GetComponent<outline2D>().Active_SO;
        orderInLayer_SO = gameObject.GetComponent<outline2D>().OrderInLayer_SO;
        color_SO = gameObject.GetComponent<outline2D>().Color_SO;

        //Clipping Mask Variables

        clipCenter_CM = gameObject.GetComponent<outline2D>().ClipCenter_CM;
        alphaCutoff_CM = gameObject.GetComponent<outline2D>().AlphaCutoff_CM;
        customRange_CM = gameObject.GetComponent<outline2D>().CustomRange_CM;
        frontLayer_CM = gameObject.GetComponent<outline2D>().FrontLayer_CM;
        backLayer_CM = gameObject.GetComponent<outline2D>().BackLayer_CM;

        //Outline Variables

        active_O = gameObject.GetComponent<outline2D>().Active_O;

        color_O = gameObject.GetComponent<outline2D>().Color_O;
        orderInLayer_O = gameObject.GetComponent<outline2D>().OrderInLayer_O;
        size_O = gameObject.GetComponent<outline2D>().Size_O;
        scaleWithParentX_O = gameObject.GetComponent<outline2D>().ScaleWithParentX_O;
        scaleWithParentY_O = gameObject.GetComponent<outline2D>().ScaleWithParentY_O;
        outlineType_Scaling_or_Pushing_O = gameObject.GetComponent<outline2D>().OutlineType_Scaling_or_Pushing_O;

        pushType_Regular_or_Custom_OP = gameObject.GetComponent<outline2D>().RegularOutline;

        objsMakingOutline_OPR = gameObject.GetComponent<outline2D>().ObjsMakingOutline_R;
        startAngle_OPR = gameObject.GetComponent<outline2D>().StartAngle_R;
        radialPush_OPR = gameObject.GetComponent<outline2D>().RadialPush_R;

        stdSize_OPC = gameObject.GetComponent<outline2D>().StdSize_1C;
    }
	
	void Update () {

        print("bounds x: " + gameObject.GetComponent<SpriteRenderer>().bounds.size.x);
        print("bounds y: " + gameObject.GetComponent<SpriteRenderer>().bounds.size.y);

        //--- Check for a new value and react to it

        //Debugging Variables

        if (showOutline_GOs_InHierarchy_D != gameObject.GetComponent<outline2D>().ShowOutline_GOs_InHierarchy_D)
            gameObject.GetComponent<outline2D>().ShowOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

        //--- Sprite Overlay Variables

        if (active_SO != gameObject.GetComponent<outline2D>().Active_SO)
            gameObject.GetComponent<outline2D>().Active_SO = active_SO;

        if (orderInLayer_SO != gameObject.GetComponent<outline2D>().OrderInLayer_SO)
            gameObject.GetComponent<outline2D>().OrderInLayer_SO = orderInLayer_SO;

        if (color_SO != gameObject.GetComponent<outline2D>().Color_SO)
            gameObject.GetComponent<outline2D>().Color_SO = color_SO;

        //--- Clipping Mask Variables

        if (clipCenter_CM != gameObject.GetComponent<outline2D>().ClipCenter_CM)
            gameObject.GetComponent<outline2D>().ClipCenter_CM = clipCenter_CM;

        if (alphaCutoff_CM != gameObject.GetComponent<outline2D>().AlphaCutoff_CM)
            gameObject.GetComponent<outline2D>().AlphaCutoff_CM = alphaCutoff_CM;

        if (customRange_CM != gameObject.GetComponent<outline2D>().CustomRange_CM)
            gameObject.GetComponent<outline2D>().CustomRange_CM = customRange_CM;

        if (frontLayer_CM != gameObject.GetComponent<outline2D>().FrontLayer_CM)
            gameObject.GetComponent<outline2D>().FrontLayer_CM = frontLayer_CM;

        if (backLayer_CM != gameObject.GetComponent<outline2D>().BackLayer_CM)
            gameObject.GetComponent<outline2D>().BackLayer_CM = backLayer_CM;

        //----- Outline Variables

        if (active_O != gameObject.GetComponent<outline2D>().Active_O)
            gameObject.GetComponent<outline2D>().Active_O = active_O;

        if (color_O != gameObject.GetComponent<outline2D>().Color_O)
            gameObject.GetComponent<outline2D>().Color_O = color_O;

        if (orderInLayer_O != gameObject.GetComponent<outline2D>().OrderInLayer_O)
            gameObject.GetComponent<outline2D>().OrderInLayer_O = orderInLayer_O;

        if (size_O != gameObject.GetComponent<outline2D>().Size_O)
            gameObject.GetComponent<outline2D>().Size_O = size_O;

        if (scaleWithParentX_O != gameObject.GetComponent<outline2D>().ScaleWithParentX_O)
            gameObject.GetComponent<outline2D>().ScaleWithParentX_O = scaleWithParentX_O;

        if (scaleWithParentY_O != gameObject.GetComponent<outline2D>().ScaleWithParentY_O)
            gameObject.GetComponent<outline2D>().ScaleWithParentY_O = scaleWithParentY_O;

        if (outlineType_Scaling_or_Pushing_O != gameObject.GetComponent<outline2D>().OutlineType_Scaling_or_Pushing_O)
            gameObject.GetComponent<outline2D>().OutlineType_Scaling_or_Pushing_O = outlineType_Scaling_or_Pushing_O;

        //---

        if (pushType_Regular_or_Custom_OP != gameObject.GetComponent<outline2D>().RegularOutline)
            gameObject.GetComponent<outline2D>().RegularOutline = pushType_Regular_or_Custom_OP;

        //---R

        if (objsMakingOutline_OPR != gameObject.GetComponent<outline2D>().ObjsMakingOutline_R)
            gameObject.GetComponent<outline2D>().ObjsMakingOutline_R = objsMakingOutline_OPR;

        if (startAngle_OPR != gameObject.GetComponent<outline2D>().StartAngle_R)
            gameObject.GetComponent<outline2D>().StartAngle_R = startAngle_OPR;

        if (radialPush_OPR != gameObject.GetComponent<outline2D>().RadialPush_R)
            gameObject.GetComponent<outline2D>().RadialPush_R = radialPush_OPR;

        //---C

        if (stdSize_OPC != gameObject.GetComponent<outline2D>().StdSize_1C)
            gameObject.GetComponent<outline2D>().StdSize_1C = stdSize_OPC;
    }
}
