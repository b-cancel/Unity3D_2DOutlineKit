using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Programmer: Bryan Cancel
 * Last Updated: 7/22/17
 * 
 * FUSE -vs- OVERLAP
 *      *This Refers to object A and object B with 2 different outlines
 *      FUSE -> by DEFAULT outline A will "Interact" with outline B... 
 *          so it seems like object A and object B are merging into 1 sprite
 *      OVERLAP -> IF obj A and obj B have oultines with Masks that affect different layers
 *          (and ofcourse the outline are being clipped within their own perspective range)
 *          outline A will overlap outline B [If (outline A orderInLayer) > (outline B orderInLayer)]
 * 
 * NOTE: Duplication of the Object in (Edit -or- Play) Mode will create the object but with all the DEFAULT outline settings
 * 
 * NOTE: this Object "Executes In Edit Mode" For the Sole purpose of chossing your settings
 *          HOWEVER... when you are actually using the asset you MUST change the variables from code
 *          
 *          REASON... this is because this object is [NOT SERIALIZABLE]... so unity will clone it and use its copy in game mode
 *          REASON ITS NOT ADDED... its alot more trouble that its worth and can cause significant problems if not used properly
 *        
 * 
 * LIMITATION 1: since I am using the sprite to create an outline... if the sprite SOURCE is semi transparent then the outline then the overlay will also be semi transparent
 *               in areas where the outline is semitransparent the opacities might no be the same
 * SOLUTION 1: use shader that grabs the silhouette of the sprite as a solid color regardless of semi transparency and use that... (I wasn't able to find said shader... and I dont know HLSL)
 */

[ExecuteInEditMode]
public class outline2D : MonoBehaviour {

    //-----Variables for Used in Awake----- (currently NONE)

    //-----Variables for ALL Outlines-----

    GameObject outlineGameObjectsFolder; //contains all the outlines
                                         //IMPORTANT NOTE: currently only one outline is supported

    bool showOutline_GOs_InHierarchy_D;
    public bool ShowOutline_GOs_InHierarchy_D
    {
        get { return showOutline_GOs_InHierarchy_D; }
        set
        {
            if (value)
                outlineGameObjectsFolder.hideFlags = HideFlags.None;
            else
                outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;

            showOutline_GOs_InHierarchy_D = value;
        }
    }

    //-----Overlay Variables

    //NOTE: We create this object on "Awake"

    GameObject spriteOverlay;

    bool active_SO;
    public bool Active_SO
    {
        get { return active_SO; }
        set
        {
            spriteOverlay.SetActive(value);

            active_SO = value;
        }
    }

    int orderInLayer_SO;
    public int OrderInLayer_SO
    {
        get { return orderInLayer_SO; }
        set
        {
            spriteOverlay.GetComponent<SpriteRenderer>().sortingOrder = value;

            orderInLayer_SO = value;
        }
    }

    Color color_SO;
    public Color Color_SO
    {
        get { return color_SO; }
        set
        {
            spriteOverlay.GetComponent<SpriteRenderer>().color = value;

            color_SO = value;
        }
    }

    //-----Clipping Mask Variables

    GameObject ClippingMask; //gameobject with sprite mask

    bool clipCenter_CM;
    public bool ClipCenter_CM
    {
        get { return clipCenter_CM; }
        set
        {
            if (clipCenter_CM != value) //NEW value
            {
                //enable or disable mask
                ClippingMask.GetComponent<SpriteMask>().enabled = value;

                //update how our edge gameobjects interact with the mask
                if (value == true)
                {
                    if(edges_1 != null)
                        foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                            dictVal.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    if (edges_1 != null)
                        foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                            dictVal.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                }
            }
            else
                ;// print("we should not have run");

            clipCenter_CM = value;
        }
    } //NOTE: used in update function... doesnt have to do anyting special for get and set...

    [Range(0,1)]
    float alphaCutoff_CM;
    public float AlphaCutoff_CM
    {
        get { return alphaCutoff_CM; }
        set
        {
            ClippingMask.GetComponent<SpriteMask>().alphaCutoff = value;

            alphaCutoff_CM = value;
        }
    }

    bool customRange_CM;
    public bool CustomRange_CM
    {
        get { return customRange_CM; }
        set
        {
            ClippingMask.GetComponent<SpriteMask>().isCustomRangeActive = value;

            customRange_CM = value;
        }
    }

    int frontLayer_CM;
    public int FrontLayer_CM
    {
        get { return frontLayer_CM; }
        set
        {
            ClippingMask.GetComponent<SpriteMask>().frontSortingLayerID = value;

            frontLayer_CM = value;
        }
    }

    int backLayer_CM;
    public int BackLayer_CM
    {
        get { return backLayer_CM; }
        set
        {
            ClippingMask.GetComponent<SpriteMask>().backSortingLayerID = value;

            backLayer_CM = value;
        }
    }

    //-----Outline Variables

    GameObject Outline; 

    bool active_O;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public bool Active_O { get { return active_O; } set { active_O = value; } }  

    [Space(10)]
    
    Color color_O;
    public Color Color_O
    {
        get { return color_O; }
        set
        {
            //update our edges with the new color
            if (edges_1 != null)
                foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                    dictVal.Key.GetComponent<SpriteRenderer>().color = value;

            color_O = value;
        }
    }

    int orderInLayer_O;
    public int OrderInLayer_O
    {
        get { return orderInLayer_O; }
        set
        {
            //update our edges with the new color
            if (edges_1 != null)
                foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                    dictVal.Key.GetComponent<SpriteRenderer>().sortingOrder = value;

            orderInLayer_O = value;
        }
    }

    float size_O;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public float Size_O { get { return size_O; } set { size_O = value; } }

    bool scaleWithParentX_O;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public bool ScaleWithParentX_O { get { return scaleWithParentX_O; } set { scaleWithParentX_O = value; } }

    bool scaleWithParentY_O;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public bool ScaleWithParentY_O { get { return scaleWithParentY_O; } set { scaleWithParentY_O = value; } }

    bool outlineType_Scaling_or_Pushing_O;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public bool OutlineType_Scaling_or_Pushing_O { get { return outlineType_Scaling_or_Pushing_O; } set { outlineType_Scaling_or_Pushing_O = value; } }

    //---Scale Type

    //---Push Type

    Dictionary<GameObject, Vector2> edges_1;

    bool regularOutline;
    public bool RegularOutline
    {
        get { return regularOutline; }
        set
        {
            if (regularOutline != value) //NEW value
            {
                if (regularOutline == true) //t -> f (KEEP regular outline data)
                    ;
                else //f -> t (CLEAR custom oultine data)
                    clearEdgesAroundSprite();
            }
            else
                ; // print("we should not have run");

            regularOutline = value; //apply new value
        }
    }

    int objsMakingOutline_R; //also the count of gameobjects that make up the outline
    public int ObjsMakingOutline_R { get { return objsMakingOutline_R; } set { objsMakingOutline_R = value; } }

    float startAngle_R;
    public float StartAngle_R { get { return startAngle_R; } set { startAngle_R = value; } }

    bool radialPush_R; //push objs to edge of circle or to edge of box
    public bool RadialPush_R { get { return radialPush_R; } set { radialPush_R = value; } }

    bool stdSize_1C;
    public bool StdSize_1C { get { return stdSize_1C; } set { stdSize_1C = value; } }

    //-----Edges && Displacement Vectors

    //TODO... create these for expo and testing purposes

    void Awake()
    {
        //--- Cover Duplication Problem
        
        Transform psblOF_T = this.transform.Find("Outline Folder");
        if (psblOF_T != null) //transform found
        {
            GameObject psblOF_GO = psblOF_T.gameObject;
            if (psblOF_GO.transform.parent.gameObject == gameObject) //this gameobject ours
                DestroyImmediate(psblOF_GO);
        }

        //--- Object Instantiation

        //Outline Folder [MUST BE FIRST]
        outlineGameObjectsFolder = new GameObject("Outline Folder");
        copyOriginalGO_Transforms(outlineGameObjectsFolder);
        outlineGameObjectsFolder.transform.parent = this.transform;

        //Outline Around Sprite Object Folder
        Outline = new GameObject("Outline Around Sprite Folder");
        copyOriginalGO_Transforms(Outline);
        Outline.transform.parent = outlineGameObjectsFolder.transform;

        //Sprite Mask
        ClippingMask = new GameObject("Sprite Mask");
        ClippingMask.AddComponent<SpriteMask>();
        copyOriginalGO_Transforms(ClippingMask);
        ClippingMask.transform.parent = outlineGameObjectsFolder.transform;

        //Sprite Overlay
        spriteOverlay = new GameObject("Sprite Overlay");
        spriteOverlay.AddComponent<SpriteRenderer>();
        //---
        var tempMaterial = new Material(spriteOverlay.GetComponent<SpriteRenderer>().sharedMaterial);
        tempMaterial.shader = Shader.Find("GUI/Text Shader");
        spriteOverlay.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;
        //---
        copyOriginalGO_Transforms(spriteOverlay);
        spriteOverlay.transform.parent = outlineGameObjectsFolder.transform;

        //*****Set Variable Defaults*****

        //--- Debugging

        ShowOutline_GOs_InHierarchy_D = false;

        //--- Sprite Overlay

        Active_SO = true;
        OrderInLayer_SO = this.GetComponent<SpriteRenderer>().sortingOrder + 1; //by default in front
        Color_SO = new Color(0, 0, 0, 0);

        //--- Clipping Mask

        ClipCenter_CM = false; 
        AlphaCutoff_CM = .25f;

        CustomRange_CM = false;
        FrontLayer_CM = 0; //by defaults maps to "default" layer
        BackLayer_CM = 0; //by defaults maps to "default" layer

        //----- Outline

        Active_O = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size

        Color_O = Color.blue;
        OrderInLayer_O = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
        Size_O = .1f;
        ScaleWithParentX_O = false;
        ScaleWithParentY_O = false;
        OutlineType_Scaling_or_Pushing_O = true;

        //--- Scale Type

        //--- Push Type

        edges_1 = new Dictionary<GameObject, Vector2>();

        RegularOutline = true;

        ObjsMakingOutline_R = 4;
        StartAngle_R = 0;
        RadialPush_R = true;

        StdSize_1C = false;
    }

    void Update()
    {
        //--- Take Care of Overlay

        if (Active_SO)
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

        //--- Outline Around Sprite

        if (Active_O)
        {
            //TODO... we might not need to check this every frame... maybe manually call an update?
            ClippingMask.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite; 

            //----- Create out Outline Edges IFF needed

            if (RegularOutline)
            {
                //make sure we have our required ammount of outlines to RESIST CHANGE
                if (edges_1.Count != ObjsMakingOutline_R)
                {
                    if(edges_1.Count != 0)
                        clearEdgesAroundSprite();

                    for (int i = 0; i < ObjsMakingOutline_R; i++)
                        addOutline(Vector2.up);
                }
            }
            //ELSE... we are using a custom outline... we add and remove outlines from it manually... we also add and remove directions from it manually...

            //--- set our directions every frame to RESIST CHANGE
            if (RegularOutline)
            {
                //NOTE: only required if regular Outline = True
                float rotation = StartAngle_R;
                float angleBetweenAllEdges = (ObjsMakingOutline_R == 0) ? 360 : 360 / ObjsMakingOutline_R;

                List<GameObject> edgesKeys = new List<GameObject>(edges_1.Keys);
                foreach (var aKey in edgesKeys)
                {
                    float oldMagnitude = edges_1[aKey].magnitude;
                    //NOTE: your direction is calculated from a compass (your obj rotation is not taken into consideration till later)
                    Vector3 newDirection = Quaternion.AngleAxis(rotation, Vector3.forward) * Vector3.up; 

                    edges_1[aKey] = newDirection.normalized * oldMagnitude;

                    rotation += angleBetweenAllEdges;
                }
            }

            int tempIndex = 0;
            foreach (KeyValuePair<GameObject, Vector2> entry in edges_1)
            {
                //--- set sprite renderer data
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), entry.Key.GetComponent<SpriteRenderer>());

                //--- set position 
                if (RegularOutline)
                {
                    entry.Key.transform.position = entry.Value.normalized; //use ONLY vector (1) direction

                    if (RadialPush_R) //Radial Push
                        entry.Key.transform.position *= Size_O;
                    else //Square Push
                        entry.Key.transform.position *= Mathf.Abs(Size_O / Mathf.Cos( (Vector2.Angle(entry.Value, Vector2.up) % 90) * Mathf.Deg2Rad));          
                }
                else
                {
                    if (StdSize_1C) //STANDARD size for all vectors
                        entry.Key.transform.position = entry.Value.normalized * Size_O; //use ONLY vector (1) direction
                    else
                        entry.Key.transform.position = entry.Value; //use ONLY vector (1) direction (2) magnitude
                }

                //NOTE: as of now our position is still on a compass in the (0,0,0) position

                if (ScaleWithParentX_O)
                    entry.Key.transform.position = new Vector2(entry.Key.transform.position.x * this.transform.localScale.x, entry.Key.transform.position.y);

                if(ScaleWithParentY_O)
                    entry.Key.transform.position = new Vector2(entry.Key.transform.position.x, entry.Key.transform.position.y * this.transform.localScale.y);

                entry.Key.transform.position = this.transform.position + (this.transform.rotation * entry.Key.transform.position);

                tempIndex++;
            }
        }
        else //we dont want AROUND outline
        {
            if (ClippingMask.GetComponent<SpriteMask>().enabled == true)
                ClippingMask.GetComponent<SpriteMask>().enabled = false;

            clearEdgesAroundSprite();
        }
    }

    //-----------------------Helper Functions

    void clearEdgesAroundSprite()
    {
        if (edges_1.Count != 0)
        {
            foreach (KeyValuePair<GameObject, Vector2> entry in edges_1)
                DestroyImmediate(entry.Key);
            edges_1.Clear();
        }
    }

    //NOTE: you only need to copy over a variable if its not its default value (for later optimization)
    void copySpriteRendererData(SpriteRenderer from, SpriteRenderer to)
    {
        to.adaptiveModeThreshold = from.adaptiveModeThreshold;
        //COLOR -> set elsewhere
        to.drawMode = from.drawMode;
        to.flipX = from.flipX;
        to.flipY = from.flipY;
        //MASK INTERACTION -> set elsewhere
        to.size = from.size;
        to.sprite = from.sprite;
        to.tileMode = from.tileMode;

        //NOTE: Inherited Members -> Properties... not currently being copied over
    }

    public bool addOutline(Vector2 outlineDirection)
    {
        if (Outline != null)
        {
            GameObject tempSpriteCopy = new GameObject();
            tempSpriteCopy.AddComponent<SpriteRenderer>();

            copyOriginalGO_Transforms(tempSpriteCopy);

            //assign our parent
            tempSpriteCopy.transform.parent = Outline.transform;

            //use text shader so that we only conserve the silhouette of our sprite
            var tempMaterial = new Material(tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial);
            tempMaterial.shader = Shader.Find("GUI/Text Shader");
            tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;

            //how we interact with the mask
            if (ClipCenter_CM == true)
                tempSpriteCopy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            else
                tempSpriteCopy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

            //set color
            tempSpriteCopy.GetComponent<SpriteRenderer>().color = Color_O;

            //set sorting layer
            tempSpriteCopy.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;

            //save our data
            edges_1.Add(tempSpriteCopy, outlineDirection);

            return true;
        }
        else
            return false;
    }

    void copyOriginalGO_Transforms(GameObject copierGO)
    {
        //place ourselves in the same place as our parent (for transform copying purposes...)
        if (this.transform.parent != null)
            copierGO.transform.parent = this.transform.parent.gameObject.transform;
        //ELSE... our parent is in the root and currently so are we...

        //copy over all transforms
        copierGO.transform.localScale = this.transform.localScale;
        copierGO.transform.localPosition = this.transform.localPosition;
        copierGO.transform.localRotation = this.transform.localRotation;
    }
    
    public bool removeOutline(GameObject edgeGO)
    {
        if (edges_1.ContainsKey(edgeGO))
        {
            edges_1.Remove(edgeGO);
            return true;
        }
        else
            return false;
    }

    public bool editOutline(GameObject edgeGO, Vector2 newDirection)
    {
        if (edges_1.ContainsKey(edgeGO))
        {
            edges_1[edgeGO] = newDirection;
            return true;
        }
        else
            return false;
    }
}
