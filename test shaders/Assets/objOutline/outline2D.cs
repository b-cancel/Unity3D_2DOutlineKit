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

    [Header("*****ONLY used in the Awake Function*****")]
    [Header("Changing them will have an effect only when...")]
    [Header("(1) going between (Edit Mode) and (Play Mode)")]
    [Header("(2) duplicating the Object in either mode")]

    //-----Variables for ALL Outlines-----

    GameObject outlineGameObjectsFolder; //contains all the outlines
                                         //IMPORTANT NOTE: currently only one outline is supported

    [Header("*****Variables For ALL Outlines*****")]
    public bool showOutline_GOs_InHierarchy;

    //-----Sprite Mask->(SM)<-

    GameObject GO_with_SpriteMask; //gameobject with sprite mask

    [Header("-----Use Masking To Clip Center->SM<-")]
    [Header("Performance Drop -> (1/4)")]
    public bool clipCenter_SM; //NOTE: Suggested TRUE when making an outline around Semi-Transparent Sprites

    [Range(0,1)]
    public float alphaCutoff_SM;

    public bool customRange_SM;
    public int frontLayer_SM;
    public int backLayer_SM;

    //-----Variables for Outline 1----->1<-

    GameObject folder_1; 
    Dictionary<GameObject, Vector2> edges_1;

    [Header("-----Variables For ONE Outline----->1<-")]
    public bool activeOutline_1; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size

    [Space(10)]

    [Header("-----For Regular or Custom Outline->B<-")]
    public Color color_1B;
    public int orderInLayer_1B;
    [Range(0, 5)]
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

    //-----Edges && Displacement Vectors

    //TODO... create these for expo and testing purposes

    //-----Overlay Variables
    
    //NOTE: We create this object on "Awake"

    GameObject spriteOverlay;

    [Header("*****Overlay Variables*****[SO]")]
    public bool active_SO;
    public int orderInLayer_SO;
    public Color color_SO;

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

        showOutline_GOs_InHierarchy = false;
        clipCenter_SM = false; 
        alphaCutoff_SM = .25f;

        customRange_SM = false;
        frontLayer_SM = 0;
        backLayer_SM = 1;

        //--- outside defaults

        edges_1 = new Dictionary<GameObject, Vector2>();

        activeOutline_1 = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size

        color_1B = Color.blue;
        orderInLayer_1B = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
        size_1B = .1f;
        scaleWithParent_1B = false;
        regularOutline = true;

        objsMakingOutline_R = 8;
        startAngle_R = 0;
        radialPush_R = true;

        stdSize_1C = false;

        //--- Sprite Overlay

        active_SO = true;
        orderInLayer_SO = this.GetComponent<SpriteRenderer>().sortingOrder + 1; //by default in front
        color_SO = new Color(0, 0, 0, 0);
    }

    bool PREVcustomOutline_1B;

    void Update()
    {
        //--- Take Care of Overlay

        if (active_SO)
        {
            spriteOverlay.SetActive(true);

            //set color
            spriteOverlay.GetComponent<SpriteRenderer>().color = color_SO;

            //set sprite renderer data
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

            //set sorting order
            spriteOverlay.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_SO;
        }
        else
            spriteOverlay.SetActive(false);

        //--- toggle seeing the objects that create our outline in hierarchy

        if (showOutline_GOs_InHierarchy)
            outlineGameObjectsFolder.hideFlags = HideFlags.None;
        else
            outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;

        //--- Outline Around Sprite

        if (activeOutline_1)
        {
            //----- Handle Sprite Mask

            if (clipCenter_SM)
                updateSpriteMask();
            else
                if (GO_with_SpriteMask != null)
                    GO_with_SpriteMask.GetComponent<SpriteMask>().enabled = false;

            //----- Create out Outline Edges IFF needed

            if (regularOutline)
            {
                //make sure we have our required ammount of outlines to RESIST CHANGE
                if (edges_1.Count != objsMakingOutline_R)
                {
                    clearEdgesAroundSprite();
                    for (int i = 0; i < objsMakingOutline_R; i++)
                        addOutline(Vector2.up);
                }
            }
            //ELSE... we are using a custom outline... we add and remove outlines from it manually... we also add and remove directions from it manually...

            //--- set our directions every frame to RESIST CHANGE
            if (regularOutline)
            {
                //NOTE: only required if regular Outline = True
                float rotation = startAngle_R;
                float angleBetweenAllEdges = (objsMakingOutline_R == 0) ? 360 : 360 / objsMakingOutline_R;

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

                //--- set masking
                if (clipCenter_SM)
                    entry.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                else
                    entry.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

                //--- set sorting order
                entry.Key.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_1B;

                //--- set position 
                float averageScale = (this.transform.localScale.x + this.transform.localScale.y) / 2;

                if (regularOutline)
                {
                    entry.Key.transform.position = entry.Value.normalized; //use ONLY vector (1) direction

                    if (radialPush_R)
                        entry.Key.transform.position *= size_1B;
                    else
                    {
                        float currRotation = Vector2.Angle(entry.Value, Vector2.up) % 90;

                        float currSize;
                        if(Mathf.Approximately(currRotation,0))
                            currSize = size_1B;
                        else
                            currSize = Mathf.Abs(size_1B / Mathf.Cos(currRotation * Mathf.Deg2Rad));

                        entry.Key.transform.position *= currSize;          
                    }
                }
                else
                {
                    if (stdSize_1C) //STANDARD size for all vectors
                        entry.Key.transform.position = entry.Value.normalized * size_1B; //use ONLY vector (1) direction
                    else
                        entry.Key.transform.position = entry.Value; //use ONLY vector (1) direction (2) magnitude
                }

                if (scaleWithParent_1B)
                    entry.Key.transform.position *= averageScale;

                entry.Key.transform.position = this.transform.position + (this.transform.rotation * entry.Key.transform.position);

                //--- set color
                entry.Key.GetComponent<SpriteRenderer>().color = color_1B;

                tempIndex++;
            }
        }
        else //we dont want AROUND outline
        {
            if (GO_with_SpriteMask != null)
                GO_with_SpriteMask.GetComponent<SpriteMask>().enabled = false;

            clearEdgesAroundSprite();
        }

        PREVcustomOutline_1B = regularOutline;
    }

    //-----------------------Helper Functions

    void updateSpriteMask()
    {
        GO_with_SpriteMask.GetComponent<SpriteMask>().enabled = true; //if we want to update it we can imply that we want it on...
        GO_with_SpriteMask.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite;
        GO_with_SpriteMask.GetComponent<SpriteMask>().alphaCutoff = alphaCutoff_SM;
        GO_with_SpriteMask.GetComponent<SpriteMask>().isCustomRangeActive = customRange_SM;
        GO_with_SpriteMask.GetComponent<SpriteMask>().frontSortingLayerID = frontLayer_SM;
        GO_with_SpriteMask.GetComponent<SpriteMask>().backSortingLayerID = backLayer_SM;
    }

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
