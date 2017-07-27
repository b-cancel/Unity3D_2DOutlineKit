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

    //-----Variables for ALL Outlines-----

    [Header("*****Variables For ALL Outlines*****")]
    public bool showOutline_GOs_InHierarchy;

    //-----Sprite Mask->(SM)<-
    [Header("-----Use Masking To Clip Center->SM<-")]
    [Header("Performance Drop -> (1/4)")]
    [Header("Suggested TRUE when making an outline around Semi-Transparent Sprites")]
    public bool clipCenter_SM; 

    [Range(0, 1)]
    public float alphaCutoff_SM;

    public bool customRange_SM;
    public int frontLayer_SM;
    public int backLayer_SM;

    [Header("-----Variables For ONE Outline----->1<-")]
    public bool activeOutline_1;

    [Header("-----For Regular or Custom Outline->B<-")]
    public Color color_1B;
    public int orderInLayer_1B;
    [Range(0,5)]
    public float size_1B;
    [Header("For Best Result -> (parentsScale.x == parentsScale.y)")]
    public bool scaleWithParent_1B;

    [Header("Performance Drop -> (depend on edge Count)---")]
    public bool regularOutline;

    [Header("-----Works IF (CustomOutline = FALSE)->R<-")] //->R<-
    public int objsMakingOutline_R; //also the count of gameobjects that make up the outline
    public float startAngle_R;
    public bool radialPush_R; //push objs to edge of circle or to edge of box

    [Header("-----Works IF (CustomOutline = TRUE)->C<-")] //->C<-
    public bool stdSize_1C;

    [Header("*****Overlay Variables*****[SO]")]
    public bool active_SO;
    public int orderInLayer_SO;
    public Color color_SO;

    void Start() //using Start() because this must run after Awake() of "outline2D"
    {
        //--- Grab Defaults Set in the Master Outline Script

        //Variables for ALL Outlines-----

        showOutline_GOs_InHierarchy = gameObject.GetComponent<outline2D>().ShowOutline_GOs_InHierarchy;

        //Sprite Mask->(SM)<-

        clipCenter_SM = gameObject.GetComponent<outline2D>().ClipCenter_SM;
        alphaCutoff_SM = gameObject.GetComponent<outline2D>().AlphaCutoff_SM;
        customRange_SM = gameObject.GetComponent<outline2D>().CustomRange_SM;
        frontLayer_SM = gameObject.GetComponent<outline2D>().FrontLayer_SM;
        backLayer_SM = gameObject.GetComponent<outline2D>().BackLayer_SM;

        //Variables for Outline 1----->1<-

        activeOutline_1 = gameObject.GetComponent<outline2D>().ActiveOutline_1;

        color_1B = gameObject.GetComponent<outline2D>().Color_1B;
        orderInLayer_1B = gameObject.GetComponent<outline2D>().OrderInLayer_1B;
        size_1B = gameObject.GetComponent<outline2D>().Size_1B;
        scaleWithParent_1B = gameObject.GetComponent<outline2D>().ScaleWithParent_1B;

        regularOutline = gameObject.GetComponent<outline2D>().RegularOutline;

        objsMakingOutline_R = gameObject.GetComponent<outline2D>().ObjsMakingOutline_R;
        startAngle_R = gameObject.GetComponent<outline2D>().StartAngle_R;
        radialPush_R = gameObject.GetComponent<outline2D>().RadialPush_R;

        stdSize_1C = gameObject.GetComponent<outline2D>().StdSize_1C;
    }
	
	void Update () {

        //--- Check for a new value and react to it

        //Variables for ALL Outlines-----

        if (showOutline_GOs_InHierarchy != gameObject.GetComponent<outline2D>().ShowOutline_GOs_InHierarchy)
            gameObject.GetComponent<outline2D>().ShowOutline_GOs_InHierarchy = showOutline_GOs_InHierarchy;

        //Sprite Mask->(SM)<-

        if (clipCenter_SM != gameObject.GetComponent<outline2D>().ClipCenter_SM)
            gameObject.GetComponent<outline2D>().ClipCenter_SM = clipCenter_SM;

        if (alphaCutoff_SM != gameObject.GetComponent<outline2D>().AlphaCutoff_SM)
            gameObject.GetComponent<outline2D>().AlphaCutoff_SM = alphaCutoff_SM;

        if (customRange_SM != gameObject.GetComponent<outline2D>().CustomRange_SM)
            gameObject.GetComponent<outline2D>().CustomRange_SM = customRange_SM;

        if (frontLayer_SM != gameObject.GetComponent<outline2D>().FrontLayer_SM)
            gameObject.GetComponent<outline2D>().FrontLayer_SM = frontLayer_SM;

        if (backLayer_SM != gameObject.GetComponent<outline2D>().BackLayer_SM)
            gameObject.GetComponent<outline2D>().BackLayer_SM = backLayer_SM;

        //Variables for Outline 1----->1<-

        if (activeOutline_1 != gameObject.GetComponent<outline2D>().ActiveOutline_1)
            gameObject.GetComponent<outline2D>().ActiveOutline_1 = activeOutline_1;

        if (color_1B != gameObject.GetComponent<outline2D>().Color_1B)
            gameObject.GetComponent<outline2D>().Color_1B = color_1B;

        if (orderInLayer_1B != gameObject.GetComponent<outline2D>().OrderInLayer_1B)
            gameObject.GetComponent<outline2D>().OrderInLayer_1B = orderInLayer_1B;

        if (size_1B != gameObject.GetComponent<outline2D>().Size_1B)
            gameObject.GetComponent<outline2D>().Size_1B = size_1B;

        if (scaleWithParent_1B != gameObject.GetComponent<outline2D>().ScaleWithParent_1B)
            gameObject.GetComponent<outline2D>().ScaleWithParent_1B = scaleWithParent_1B;

        //---

        if (regularOutline != gameObject.GetComponent<outline2D>().RegularOutline)
            gameObject.GetComponent<outline2D>().RegularOutline = regularOutline;

        //---R

        if (objsMakingOutline_R != gameObject.GetComponent<outline2D>().ObjsMakingOutline_R)
            gameObject.GetComponent<outline2D>().ObjsMakingOutline_R = objsMakingOutline_R;

        if (startAngle_R != gameObject.GetComponent<outline2D>().StartAngle_R)
            gameObject.GetComponent<outline2D>().StartAngle_R = startAngle_R;

        if (radialPush_R != gameObject.GetComponent<outline2D>().RadialPush_R)
            gameObject.GetComponent<outline2D>().RadialPush_R = radialPush_R;

        //---C

        if (stdSize_1C != gameObject.GetComponent<outline2D>().StdSize_1C)
            gameObject.GetComponent<outline2D>().StdSize_1C = stdSize_1C;
    }
}
