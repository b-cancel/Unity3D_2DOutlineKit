using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Programmer: Bryan Cancel
 * Last Updated: 7/22/17
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
 * LIMITATION 1: since I am using the sprite to create an outline... if the sprite SOURCE is semi transparent then the outline and the overlay will also be semi transparent
 * SOLUTION 1: use shader that grab the silhouette of the sprite as a solid color regardless of semi transparency and use that... (I wasn't able to find said shader... and I dont know HLSL)
 */

[ExecuteInEditMode]
public class outline2D : MonoBehaviour {

    //-----Variables for Used in Awake-----

    [Header("ONLY used in the Awake Function")]
    [Header("Changing them will have an effect only when...")]
    [Header("(1) ging between (Edit Mode) and (Play Mode)")]
    [Header("(2) duplicating the Object in either mode")]
    public bool copyVarsFromSource;

    //-----Variables for entire Outline

    GameObject outlineGameObjectsFolder; 

    GameObject spriteMaskGO;

    public bool showGameObjectsInHierarchy;

    //NOTE: we delete and add MANY objets when we use this boolean
    //SO... to hide the outline temporarily its better to lower the color opacity instead of using this boolean
    public bool outlineAroundSprite;

    [Header("Works IFF (clipCenter_AS = true)---")]
    [Range(0,1)]
    public float spriteMaskGO_AlphaCutoff; 

    //-----Variables for Line Around Our Sprite

    GameObject aroundSpriteFolder; 
    Dictionary<GameObject, Vector2> edgesAroundSprite; 

    [Header("-----Outline Variables-----")]

    [Space(10)]

    public Color color_AS;
    public int outlineSortingLayer;

    [Header("For Best Result -> (parentsScale.x == parentsScale.y)")]
    [Header("Works IFF (customOutlineAroundSprite = false)")]
    public bool scaleWithParent;

    [Header("Works IFF (customOutlineAroundSprite = false)")]
    public float size_AS; //NOTE: this refers to the distance in world space between the sprite edge and the outlines furthest edge

    [Header("Performance Drop -> (1/4)---")]
    [Header("Check when using Semi-Transparent Sprites")]
    public bool clipCenter_AS;

    [Header("Performance Drop -> (1/2)---")]
    [Header("Works IFF (outlineAroundSprite = true)")]
    public bool cornerOutlineAroundSprite; 

    [Header("Performance Drop -> (depend on edge Count)---")]
    [Header("Works IFF (outlineAroundSprite = true)")]
    public bool customOutlineAroundSprite;

    [Header("For Best Result -> (parentsScale.x == parentsScale.y)")]
    [Header("Works IFF (outlineAroundSprite = true)")]
    public bool customScaleWithParent;

    //-----Edges && Displacement Vectors

    /*
     * NOTE: this must be private... 
     * if people are allowed to change the quantity of GOs...
     * WITHOUT the proper functions
     * you WILL create a massive memory leak
     */
    private List<GameObject> gameObject_to;
    [Header("-----Edges && Displacement Vectors-----")]
    public List<Vector2> to_vector;

    //-----Overlay Variables

    GameObject spriteOverlay;

    [Header("-----Overlay Variables-----")]
    public bool activeSpriteOverlay;
    public int overlaySortingOrder;

    [Header("Works IFF (tintOverSprite = true")]
    public Color spriteOverlayColor;

    //---private

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
        aroundSpriteFolder = new GameObject("Outline Around Sprite Folder");
        copyOriginalGO_Transforms(aroundSpriteFolder);
        aroundSpriteFolder.transform.parent = outlineGameObjectsFolder.transform;

        //Sprite Mask
        spriteMaskGO = new GameObject("Sprite Mask");
        spriteMaskGO.AddComponent<SpriteMask>();
        copyOriginalGO_Transforms(spriteMaskGO);
        spriteMaskGO.transform.parent = outlineGameObjectsFolder.transform;

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

        showGameObjectsInHierarchy = false;
        outlineAroundSprite = true;
        spriteMaskGO_AlphaCutoff = .25f;
        edgesAroundSprite = new Dictionary<GameObject, Vector2>();

        //--- outside defaults

        color_AS = Color.blue;
        outlineSortingLayer = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //outline is behind our sprite
        scaleWithParent = false;
        size_AS = .1f;

        clipCenter_AS = false;
        cornerOutlineAroundSprite = false;
        customOutlineAroundSprite = false;
        customScaleWithParent = false;

        gameObject_to = new List<GameObject>();
        to_vector = new List<Vector2>();

        //--- Sprite Overlay

        activeSpriteOverlay = true;
        overlaySortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 1;
        spriteOverlayColor = new Color(0, 0, 0, 0);

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

    bool PREVcustomOutlineAroundSprite;

    void Update()
    {
        //--- Take Care of Tint

        if (activeSpriteOverlay)
        {
            spriteOverlay.SetActive(true);

            //set sprite renderer data
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

            //set sorting order
            spriteOverlay.GetComponent<SpriteRenderer>().sortingOrder = overlaySortingOrder;

            //set color
            spriteOverlay.GetComponent<SpriteRenderer>().color = spriteOverlayColor;
        }
        else
            spriteOverlay.SetActive(false);

        //--- toggle seeing the objects that create our outline in hierarchy

        if (showGameObjectsInHierarchy)
            outlineGameObjectsFolder.hideFlags = HideFlags.None;
        else
            outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;

        //--- Outline Around Sprite

        if (outlineAroundSprite)
        {
            //----- Handle Sprite Mask

            if (clipCenter_AS)
                updateSpriteMask();
            else
                if (spriteMaskGO != null)
                    spriteMaskGO.GetComponent<SpriteMask>().enabled = false;

            //----- Create out Outline Edges IFF needed

            if (!customOutlineAroundSprite) //whatever Outline edges are currently exist will be used
            {
                if (outlineAroundSprite)
                    if(edgesAroundSprite.Count != 4 || (PREVcustomOutlineAroundSprite != customOutlineAroundSprite))
                    {
                        clearEdgesAroundSprite();
                        foreach (var item in stdShiftDirections)
                            addOutline(item);
                    }
                        
                if (cornerOutlineAroundSprite)
                    if(edgesAroundSprite.Count != 8 || (PREVcustomOutlineAroundSprite != customOutlineAroundSprite))
                    {
                        foreach (var item in stdCornerShiftDirections)
                            addOutline(item);
                    }        
            }
            //ELSE... we are using a custom outline... we add and remove outlines from it manually... we also add and remove directions from it manually...

            //----- Use Settings to make outline

            int tempIndex = 0;
            foreach (KeyValuePair<GameObject, Vector2> entry in edgesAroundSprite)
            {
                //--- set sprite renderer data
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), entry.Key.GetComponent<SpriteRenderer>());

                //--- set masking
                if (clipCenter_AS)
                    entry.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                else
                    entry.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

                //--- set sorting order
                entry.Key.GetComponent<SpriteRenderer>().sortingOrder = outlineSortingLayer;

                //--- set position 
                if (customOutlineAroundSprite)
                {
                    if(customScaleWithParent)
                        entry.Key.transform.position = this.transform.position + (this.transform.rotation * ((Vector3)entry.Value * ( (this.transform.localScale.x + this.transform.localScale.y) / 2) ));
                    else
                        entry.Key.transform.position = this.transform.position + (this.transform.rotation * (Vector3)entry.Value);
                }
                else
                {
                    if(tempIndex < 4) //0 -> 3 (Regular outline)
                    {  
                        if (scaleWithParent)
                            entry.Key.transform.position = this.transform.position + (this.transform.rotation * (Vector3)(entry.Value.normalized * (size_AS * ((this.transform.localScale.x + this.transform.localScale.y) / 2)))); //side of right triangle
                        else
                            entry.Key.transform.position = this.transform.position + (this.transform.rotation * (Vector3)(entry.Value.normalized * size_AS)); //side of right triangle
                    }
                    else // 4 -> 7 (corner outlines)
                    {
                        if(scaleWithParent)
                            entry.Key.transform.position = this.transform.position + (this.transform.rotation * (Vector3)(entry.Value.normalized * (Mathf.Sqrt(2 * size_AS * size_AS) * ((this.transform.localScale.x + this.transform.localScale.y) / 2)))); //hypotenuse
                        else
                            entry.Key.transform.position = this.transform.position + (this.transform.rotation * (Vector3)(entry.Value.normalized * Mathf.Sqrt(2 * size_AS * size_AS))); //hypotenuse
                    }
                }

                //--- set color
                entry.Key.GetComponent<SpriteRenderer>().color = color_AS;

                tempIndex++;
            }
        }
        else //we dont want AROUND outline
        {
            if (spriteMaskGO != null)
                spriteMaskGO.GetComponent<SpriteMask>().enabled = false;

            clearEdgesAroundSprite();
        }

        PREVcustomOutlineAroundSprite = customOutlineAroundSprite;
    }

    //-----------------------Helper Functions

    void updateSpriteMask()
    {
        spriteMaskGO.GetComponent<SpriteMask>().enabled = true; //if we want to update it we can imply that we want it on...
        spriteMaskGO.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite;
        spriteMaskGO.GetComponent<SpriteMask>().alphaCutoff = spriteMaskGO_AlphaCutoff;
    }

    void clearEdgesAroundSprite()
    {
        if (edgesAroundSprite.Count != 0)
        {
            foreach (KeyValuePair<GameObject, Vector2> entry in edgesAroundSprite)
                DestroyImmediate(entry.Key);
            edgesAroundSprite.Clear();
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
        if (aroundSpriteFolder != null)
        {
            GameObject tempSpriteCopy = new GameObject();
            tempSpriteCopy.AddComponent<SpriteRenderer>();

            copyOriginalGO_Transforms(tempSpriteCopy);

            //assign our parent
            tempSpriteCopy.transform.parent = aroundSpriteFolder.transform;

            //use text shader so that we only conserve the silhouette of our sprite
            var tempMaterial = new Material(tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial);
            tempMaterial.shader = Shader.Find("GUI/Text Shader");
            tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;

            //save our data
            edgesAroundSprite.Add(tempSpriteCopy, outlineDirection);

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
        if (edgesAroundSprite.ContainsKey(edgeGO))
        {
            edgesAroundSprite.Remove(edgeGO);
            return true;
        }
        else
            return false;
    }

    public bool editOutline(GameObject edgeGO, Vector2 newDirection)
    {
        if (edgesAroundSprite.ContainsKey(edgeGO))
        {
            edgesAroundSprite[edgeGO] = newDirection;
            return true;
        }
        else
            return false;
    }
}
