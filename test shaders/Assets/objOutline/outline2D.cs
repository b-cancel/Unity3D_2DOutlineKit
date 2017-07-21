using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: pay attention to the "HEADERS" and the NOTES in this document

//[ExecuteInEditMode]
public class outline2D : MonoBehaviour {
   
    //-----Variables for entire Outline

    GameObject outlineGameObjectsFolder; 

    GameObject spriteMaskGO;

    public bool showAddedGameObjectsInHierarchy;

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

    //---private

    Vector2[] stdShiftDirections;
    Vector2[] stdCornerShiftDirections;

    void Awake()
    {
        //--- Object Instantiation

        //Outline Folder
        outlineGameObjectsFolder = new GameObject("outLineGOs");
        outlineGameObjectsFolder.transform.parent = this.transform;
        outlineGameObjectsFolder.transform.position = this.transform.position; //TODO... should happen auto

        //Sprite Mask
        spriteMaskGO = new GameObject();
        spriteMaskGO.AddComponent<SpriteMask>();
        spriteMaskGO.transform.parent = outlineGameObjectsFolder.transform;
        spriteMaskGO.transform.position = outlineGameObjectsFolder.transform.position; //TODO... should happen auto

        //--- global defaults

        showAddedGameObjectsInHierarchy = false;
        outlineAroundSprite = true;
        spriteMaskGO_AlphaCutoff = .25f;
        edgesAroundSprite = new Dictionary<GameObject, Vector2>();

        //--- outside defaults

        color_AS = Color.blue;
        outlineSortingLayer = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //outline is behind our sprite
        size_AS = .1f;

        clipCenter_AS = false;
        cornerOutlineAroundSprite = false;
        customOutlineAroundSprite = false;

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

    void Update()
    {
        //--- toggle seeing the objects that create our outline in hierarchy

        if (showAddedGameObjectsInHierarchy)
            outlineGameObjectsFolder.hideFlags = HideFlags.None;
        else
            outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;

        //--- Outline Around Sprite

        if (outlineAroundSprite)
        {
            //----- Handle Edges Folder

            if (aroundSpriteFolder == null)
            {
                aroundSpriteFolder = new GameObject();
                aroundSpriteFolder.transform.parent = outlineGameObjectsFolder.transform;
                aroundSpriteFolder.transform.position = outlineGameObjectsFolder.transform.position; //TODO... should happen auto
            }
            //ELSE... object folder already exists

            //----- Handle Sprite Mask

            if (clipCenter_AS)
                updateSpriteMask();
            else
                if (spriteMaskGO != null)
                    spriteMaskGO.GetComponent<SpriteMask>().enabled = false;

            //----- Create out Outline Edges IFF needed

            if (!customOutlineAroundSprite) //whatever Outline edges are currently exist will be used
            {
                //TODO... optimize code below without causing erros 

                clearEdgesAroundSprite();

                if (outlineAroundSprite)
                    foreach (var item in stdShiftDirections)
                        addOutline(item);

                if (cornerOutlineAroundSprite)
                    foreach (var item in stdCornerShiftDirections)
                        addOutline(item);
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
                if(customOutlineAroundSprite)
                    entry.Key.transform.position = this.transform.position + (Vector3)entry.Value;
                else
                {
                    if(tempIndex < 4) //0 -> 3 (Regular outline)
                        entry.Key.transform.position = this.transform.position + (Vector3)(entry.Value.normalized * size_AS); //side of right triangle
                    else // 4 -> 7 (corner outlines)
                        entry.Key.transform.position = this.transform.position + (Vector3)(entry.Value.normalized * Mathf.Sqrt(2 * size_AS * size_AS)); //hypotenuse
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

            if (aroundSpriteFolder != null) //remove ALL Outline Data
            {
                Destroy(aroundSpriteFolder);
                clearEdgesAroundSprite();
            }
            //ELSE... folder object already dead
        }
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
        foreach (KeyValuePair<GameObject, Vector2> entry in edgesAroundSprite)
            Destroy(entry.Key);
        edgesAroundSprite.Clear();
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

            //we are a child of our parent folder for easy access
            tempSpriteCopy.transform.parent = aroundSpriteFolder.transform;

            //use text shader so that we only conserve the silhouette of our sprite
            tempSpriteCopy.GetComponent<SpriteRenderer>().material.shader = Shader.Find("GUI/Text Shader");

            //save our data
            edgesAroundSprite.Add(tempSpriteCopy, outlineDirection);

            return true;
        }
        else
            return false;
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
