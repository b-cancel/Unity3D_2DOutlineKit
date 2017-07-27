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

    //-----Variables for Used in Awake-----

    //-----Variables for ALL Outlines-----

    GameObject outlineGameObjectsFolder; //contains all the outlines
                                         //IMPORTANT NOTE: currently only one outline is supported

    bool showOutline_GOs_InHierarchy;
    public bool ShowOutline_GOs_InHierarchy
    {
        get { return showOutline_GOs_InHierarchy; }
        set
        {
            if (value)
                outlineGameObjectsFolder.hideFlags = HideFlags.None;
            else
                outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;

            showOutline_GOs_InHierarchy = value;
        }
    }

    //-----Sprite Mask->(SM)<-

    GameObject GO_with_SpriteMask; //gameobject with sprite mask

    bool clipCenter_SM;
    public bool ClipCenter_SM
    {
        get { return clipCenter_SM; }
        set
        {
            if (clipCenter_SM != value) //NEW value
            {
                //enable or disable mask
                GO_with_SpriteMask.GetComponent<SpriteMask>().enabled = value;

                //update how our edge gameobjects interact with the mask
                if (value == true)
                {
                    foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                        dictVal.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                        dictVal.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                }
            }
            else
                ;// print("we should not have run");

            clipCenter_SM = value;
        }
    } //NOTE: used in update function... doesnt have to do anyting special for get and set...

    [Range(0,1)]
    float alphaCutoff_SM;
    public float AlphaCutoff_SM
    {
        get { return alphaCutoff_SM; }
        set
        {
            GO_with_SpriteMask.GetComponent<SpriteMask>().alphaCutoff = value;

            alphaCutoff_SM = value;
        }
    }

    bool customRange_SM;
    public bool CustomRange_SM
    {
        get { return customRange_SM; }
        set
        {
            GO_with_SpriteMask.GetComponent<SpriteMask>().isCustomRangeActive = value;

            customRange_SM = value;
        }
    }

    int frontLayer_SM;
    public int FrontLayer_SM
    {
        get { return frontLayer_SM; }
        set
        {
            GO_with_SpriteMask.GetComponent<SpriteMask>().frontSortingLayerID = value;

            frontLayer_SM = value;
        }
    }

    int backLayer_SM;
    public int BackLayer_SM
    {
        get { return backLayer_SM; }
        set
        {
            GO_with_SpriteMask.GetComponent<SpriteMask>().backSortingLayerID = value;

            backLayer_SM = value;
        }
    }

    //-----Variables for Outline 1----->1<-

    GameObject folder_1; 
    Dictionary<GameObject, Vector2> edges_1;

    bool activeOutline_1;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public bool ActiveOutline_1 { get { return activeOutline_1; } set { activeOutline_1 = value; } }  

    [Space(10)]
    
    Color color_1B;
    public Color Color_1B
    {
        get { return color_1B; }
        set
        {
            //update our edges with the new color
            foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                dictVal.Key.GetComponent<SpriteRenderer>().color = value;

            color_1B = value;
        }
    }

    int orderInLayer_1B;
    public int OrderInLayer_1B
    {
        get { return orderInLayer_1B; }
        set
        {
            //update our edges with the new color
            foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                dictVal.Key.GetComponent<SpriteRenderer>().sortingOrder = value;

            orderInLayer_1B = value;
        }
    }

    float size_1B;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public float Size_1B { get { return size_1B; } set { size_1B = value; } }

    bool scaleWithParent_1B;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public bool ScaleWithParent_1B { get { return scaleWithParent_1B; } set { scaleWithParent_1B = value; } }

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
        folder_1 = new GameObject("Outline Around Sprite Folder");
        copyOriginalGO_Transforms(folder_1);
        folder_1.transform.parent = outlineGameObjectsFolder.transform;

        //Sprite Mask
        GO_with_SpriteMask = new GameObject("Sprite Mask");
        GO_with_SpriteMask.AddComponent<SpriteMask>();
        copyOriginalGO_Transforms(GO_with_SpriteMask);
        GO_with_SpriteMask.transform.parent = outlineGameObjectsFolder.transform;

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

        //--- global defaults

        ShowOutline_GOs_InHierarchy = false;
        ClipCenter_SM = false; 
        AlphaCutoff_SM = .25f;

        CustomRange_SM = false;
        FrontLayer_SM = 0; //by defaults maps to "default" layer
        BackLayer_SM = 0; //by defaults maps to "default" layer

        //--- outside defaults

        edges_1 = new Dictionary<GameObject, Vector2>();

        ActiveOutline_1 = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size

        Color_1B = Color.blue;
        OrderInLayer_1B = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
        Size_1B = .1f;
        ScaleWithParent_1B = false;
        RegularOutline = true;

        ObjsMakingOutline_R = 8;
        StartAngle_R = 0;
        RadialPush_R = true;

        StdSize_1C = false;

        //--- Sprite Overlay

        Active_SO = true;
        OrderInLayer_SO = this.GetComponent<SpriteRenderer>().sortingOrder + 1; //by default in front
        Color_SO = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        //--- Take Care of Overlay

        if (Active_SO)
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

        //--- Outline Around Sprite

        if (ActiveOutline_1)
        {
            //TODO... we might not need to check this every frame... maybe manually call an update?
            GO_with_SpriteMask.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite; 

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
                float averageScale = (this.transform.localScale.x + this.transform.localScale.y) / 2;

                if (RegularOutline)
                {
                    entry.Key.transform.position = entry.Value.normalized; //use ONLY vector (1) direction

                    if (RadialPush_R)
                        entry.Key.transform.position *= Size_1B;
                    else
                    {
                        float currRotation = Vector2.Angle(entry.Value, Vector2.up) % 90;

                        float currSize;
                        if(Mathf.Approximately(currRotation,0))
                            currSize = Size_1B;
                        else
                            currSize = Mathf.Abs(Size_1B / Mathf.Cos(currRotation * Mathf.Deg2Rad));

                        entry.Key.transform.position *= currSize;          
                    }
                }
                else
                {
                    if (StdSize_1C) //STANDARD size for all vectors
                        entry.Key.transform.position = entry.Value.normalized * Size_1B; //use ONLY vector (1) direction
                    else
                        entry.Key.transform.position = entry.Value; //use ONLY vector (1) direction (2) magnitude
                }

                if (ScaleWithParent_1B)
                    entry.Key.transform.position *= averageScale;

                entry.Key.transform.position = this.transform.position + (this.transform.rotation * entry.Key.transform.position);

                tempIndex++;
            }
        }
        else //we dont want AROUND outline
        {
            if (GO_with_SpriteMask.GetComponent<SpriteMask>().enabled == true)
                GO_with_SpriteMask.GetComponent<SpriteMask>().enabled = false;

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
        if (folder_1 != null)
        {
            GameObject tempSpriteCopy = new GameObject();
            tempSpriteCopy.AddComponent<SpriteRenderer>();

            copyOriginalGO_Transforms(tempSpriteCopy);

            //assign our parent
            tempSpriteCopy.transform.parent = folder_1.transform;

            //use text shader so that we only conserve the silhouette of our sprite
            var tempMaterial = new Material(tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial);
            tempMaterial.shader = Shader.Find("GUI/Text Shader");
            tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;

            //how we interact with the mask
            if (ClipCenter_SM == true)
                tempSpriteCopy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            else
                tempSpriteCopy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

            //set color
            tempSpriteCopy.GetComponent<SpriteRenderer>().color = Color_1B;

            //set sorting layer
            tempSpriteCopy.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_1B;

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
