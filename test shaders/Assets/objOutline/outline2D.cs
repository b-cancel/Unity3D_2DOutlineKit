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
    public bool showOutline_GOs_InHierarchy; //show outline game objects in hierarchy

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
    public float size_1B;
    [Header("For Best Result -> (parentsScale.x == parentsScale.y)")]
    public bool scaleWithParent_1B;
    [Header("Performance Drop -> (depend on edge Count)---")]
    public bool customOutline_1B;

    [Header("-----Works IF (CustomOutline = FALSE)->R<-")] //->R<-
    [Header("Performance Drop -> (1/2)---")]
    public bool cornerOutline_1R;

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

    //-----Contains Data to Create Our NON-Custom Outlines

    Vector2[] stdShiftDirections;
    Vector2[] stdCornerShiftDirections;

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
        customOutline_1B = false;

        cornerOutline_1R = false;

        stdSize_1C = false;

        //--- Sprite Overlay

        active_SO = true;
        orderInLayer_SO = this.GetComponent<SpriteRenderer>().sortingOrder + 1; //by default in front
        color_SO = new Color(0, 0, 0, 0);

        //--- STANDARD directions (will be used if we are not using a custom outline)

        //n, e, s, w
        stdShiftDirections = new Vector2[4]
        {
                Vector2.up,
                Vector2.right,
                Vector2.down,
                Vector2.left
        };

        //ne, se, sw, nw
        stdCornerShiftDirections = new Vector2[4]
        {
                (Vector2.up + Vector2.right).normalized,
                (Vector2.down + Vector2.right).normalized,
                (Vector2.down + Vector2.left).normalized,
                (Vector2.up + Vector2.left).normalized
        };
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

            if (!customOutline_1B) //whatever Outline edges are currently exist will be used
            {
                if (activeOutline_1)
                    if(edges_1.Count != 4 || (PREVcustomOutline_1B != customOutline_1B))
                    {
                        clearEdgesAroundSprite();
                        foreach (var item in stdShiftDirections)
                            addOutline(item);
                    }
                        
                if (cornerOutline_1R)
                    if(edges_1.Count != 8 || (PREVcustomOutline_1B != customOutline_1B))
                    {
                        foreach (var item in stdCornerShiftDirections)
                            addOutline(item);
                    }        
            }
            //ELSE... we are using a custom outline... we add and remove outlines from it manually... we also add and remove directions from it manually...

            //----- Use Settings to make outline

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

                if (customOutline_1B)
                {
                    if (stdSize_1C)
                        entry.Key.transform.position = entry.Value.normalized; //use ONLY vector (1) direction
                    else
                        entry.Key.transform.position = entry.Value; //use ONLY vector (1) direction (2) magnitude
                }
                else
                {
                    entry.Key.transform.position = entry.Value.normalized; //use ONLY vector (1) direction

                    if (tempIndex < 4) //0 -> 3 (Regular outline) [side of right triangle]
                            entry.Key.transform.position *= size_1B;
                    else // 4 -> 7 (corner outlines) [hypotenuse of right triangle]
                            entry.Key.transform.position *= Mathf.Sqrt(2 * size_1B * size_1B);
                }

                if (scaleWithParent_1B)
                    entry.Key.transform.position *= averageScale;

                entry.Key.transform.position = this.transform.position + this.transform.rotation * entry.Key.transform.position;

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

        PREVcustomOutline_1B = customOutline_1B;
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
