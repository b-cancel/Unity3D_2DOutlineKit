using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Assumptions
 * 
 * 1. you wont change the sprite during runtime
 */ 

public class outline2D : MonoBehaviour {

    [Header("Certain Settings Reduce Performace by 4... these will be marked with [!!!!]")]

    //-----Variables for entire Outline

    GameObject outlineGameObjectsFolder;

    GameObject spriteMaskGO;
    [Range(0,1)]
    public float spriteMaskGO_AlphaCutoff;

    public bool showAddedGameObjectsInHierarchy;

    public bool outlineAroundSprite;

    //-----Variables for Line Around Our Sprite

    GameObject aroundSpriteFolder;
    GameObject[] objsAroundSprite;

    [Tooltip("[!!!!] If we have a semi transparent sprite and we dont want the outline to effects its transparency -> True")]
    public bool clipCenter_AS;

    public float size_AS;
    public Color color_AS;

    void Awake()
    {
        //--- global defaults

        outlineGameObjectsFolder = new GameObject("outLineGOs");
        outlineGameObjectsFolder.transform.parent = this.transform;
        outlineGameObjectsFolder.transform.position = this.transform.position; //TODO... should happen auto

        spriteMaskGO_AlphaCutoff = .1f;

        showAddedGameObjectsInHierarchy = true;

        outlineAroundSprite = true;

        //--- outside defaults

        clipCenter_AS = false;

        size_AS = .1f;
        color_AS = Color.blue; 
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
            //create our folders and objects for the first time if needed
            createAroundObjects();

            //----- Use Settings to make outline

            pushSpritesToEdges(objsAroundSprite, size_AS);

            for (int i = 0; i < objsAroundSprite.Length; i++)
            {
                //set masking properties
                if (clipCenter_AS)
                {
                    objsAroundSprite[i].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                    createSpriteMask();
                    updateSpriteMask();
                }
                else
                    objsAroundSprite[i].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

                //set color
                objsAroundSprite[i].GetComponent<SpriteRenderer>().color = color_AS;
            }
        }
        else //we dont want AROUND outline
        {
            if (aroundSpriteFolder != null) //remove data
            {
                Destroy(aroundSpriteFolder);
                objsAroundSprite = null;
            }
            //ELSE... data already removed
        }

        //--- wipe the spriteMaskGO if possible for performance purposes

        if (outlineAroundSprite == false || (outlineAroundSprite == true && clipCenter_AS == false))
            if (spriteMaskGO != null)
                Destroy(spriteMaskGO);
    }

    //-----------------------Helper Functions

    void pushSpritesToEdges(GameObject[] sprites, float size)
    {
        sprites[0].transform.position = this.transform.position + new Vector3(0, +size); //n
        sprites[1].transform.position = this.transform.position + new Vector3(+size, 0); //e
        sprites[2].transform.position = this.transform.position + new Vector3(0, -size); //s
        sprites[3].transform.position = this.transform.position + new Vector3(-size, 0); //w
    }

    void createAroundObjects()
    {
        //--- create folder to store all copies AROUND us [IF needed]

        if (aroundSpriteFolder == null)
        {
            aroundSpriteFolder = new GameObject();
            aroundSpriteFolder.transform.parent = outlineGameObjectsFolder.transform;
            aroundSpriteFolder.transform.position = outlineGameObjectsFolder.transform.position; //TODO... should happen auto
        }

        //--- create simple outline with 4 copies of ourselves [IF needed]

        if (objsAroundSprite == null)
        {
            objsAroundSprite = new GameObject[4];
            for (int i = 0; i < objsAroundSprite.Length; i++)
            {
                GameObject tempSpriteCopy = new GameObject();
                tempSpriteCopy.AddComponent<SpriteRenderer>();

                //copy our sprite renderer data
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), tempSpriteCopy.GetComponent<SpriteRenderer>());

                //we can be in (front) ---or--- (behind) our sprite depending on the type of outline we are
                tempSpriteCopy.GetComponent<SpriteRenderer>().sortingOrder += -1;

                //we are a child of our parent folder for easy access
                tempSpriteCopy.transform.parent = aroundSpriteFolder.transform;

                //use text shader so that we only conserve the silhouette of our sprite
                tempSpriteCopy.GetComponent<SpriteRenderer>().material.shader = Shader.Find("GUI/Text Shader");

                objsAroundSprite[i] = tempSpriteCopy;
            }
        }
    }

    void createSpriteMask()
    {
        if(spriteMaskGO == null)
        {
            spriteMaskGO = new GameObject();
            spriteMaskGO.AddComponent<SpriteMask>();
            spriteMaskGO.transform.parent = outlineGameObjectsFolder.transform;
            spriteMaskGO.transform.position = outlineGameObjectsFolder.transform.position; //TODO... should happen auto
        }
    }

    void updateSpriteMask()
    {
        spriteMaskGO.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite;
        spriteMaskGO.GetComponent<SpriteMask>().alphaCutoff = spriteMaskGO_AlphaCutoff;
    }

    //NOTE: you only need to copy over a variable if its not its default value (for later optimization)
    void copySpriteRendererData(SpriteRenderer from, SpriteRenderer to)
    {
        to.adaptiveModeThreshold = from.adaptiveModeThreshold;
        //COLOR is irelevant... we dont have the same color as our sprite...
        to.drawMode = from.drawMode;
        to.flipX = from.flipX;
        to.flipY = from.flipY;
        //NOTE: outlines use a different clipping then their parents...
        to.size = from.size;
        to.sprite = from.sprite;
        to.tileMode = from.tileMode;

        to.sortingOrder = from.sortingOrder;

        //NOTE: Inherited Members -> Properties... not currently being copied over
    }

    /*
        //Locating shaders reference
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default"); 
     */
}
