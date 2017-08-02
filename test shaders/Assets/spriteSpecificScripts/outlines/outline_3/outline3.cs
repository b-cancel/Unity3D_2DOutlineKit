using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class outline3 : MonoBehaviour {

    //-----Variables for Used in Awake----- (currently NONE)

    //-----Debugging Variables-----

    GameObject outlineGameObjectsFolder; //contains all the outlines
                                         //IMPORTANT NOTE: currently only one outline is supported

    bool showOutline_GOs_InHierarchy_D;
    public bool ShowOutline_GOs_InHierarchy_D
    {
        get { return showOutline_GOs_InHierarchy_D; }
        set
        {
            showOutline_GOs_InHierarchy_D = value; //update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().showOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

            if (showOutline_GOs_InHierarchy_D)
                outlineGameObjectsFolder.hideFlags = HideFlags.None;
            else
                outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    //-----Outline Variables-----

    GameObject Outline;

    bool active_O;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public bool Active_O {
        get { return active_O; }
        set
        {
            active_O = value;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().active_O = active_O;

            //all edges set active
            if (edges_1 != null)
            {
                foreach (KeyValuePair<GameObject, Vector2> entry in edges_1)
                    entry.Key.SetActive(active_O);
            }
        }
    }

    [Space(10)]

    Color color_O;
    public Color Color_O
    {
        get { return color_O; }
        set
        {
            color_O = value;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().color_O = color_O;

            //update our edges with the new color
            if (edges_1 != null)
                foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                    dictVal.Key.GetComponent<SpriteRenderer>().color = color_O;
        }
    }

    int orderInLayer_O;
    public int OrderInLayer_O
    {
        get { return orderInLayer_O; }
        set
        {
            orderInLayer_O = value;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().orderInLayer_O = orderInLayer_O;

            //update our edges with the new color
            if (edges_1 != null)
                foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                    dictVal.Key.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
        }
    }

    float size_O;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public float Size_O {
        get { return size_O; }
        set
        {
            size_O = value;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().size_O = size_O;
        }
    }

    bool scaleWithParentX_O;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public bool ScaleWithParentX_O {
        get { return scaleWithParentX_O; }
        set
        {
            scaleWithParentX_O = value;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().scaleWithParentX_O = scaleWithParentX_O;
        }
    }

    bool scaleWithParentY_O;
    //NOTE: used in update function... doesnt have to do anyting special for get and set...
    public bool ScaleWithParentY_O {
        get { return scaleWithParentY_O; }
        set
        {
            scaleWithParentY_O = value;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().scaleWithParentY_O = scaleWithParentY_O;
        }
    }

    //-----Push Type Variables-----

    Dictionary<GameObject, Vector2> edges_1;

    bool pushType_Regular_or_Custom_OP;
    public bool PushType_Regular_or_Custom_OP
    {
        get { return pushType_Regular_or_Custom_OP; }
        set
        {
            pushType_Regular_or_Custom_OP = value; //update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().pushType_Regular_or_Custom_OP = pushType_Regular_or_Custom_OP;

            if (pushType_Regular_or_Custom_OP) //f -> t (CLEAR custom oultine data)
            {
                destroyEdgesThatCreateOutline();
                makeSureWeHaveCorrectNumberOfEdges();
            }
            else //t -> f (KEEP regular outline data)
                ;
        }
    }

    //---Regular

    int objsMakingOutline_OPR; //also the count of gameobjects that make up the outline
    public int ObjsMakingOutline_OPR {
        get { return objsMakingOutline_OPR; }
        set
        {
            objsMakingOutline_OPR = (value >= 0) ? value : 0;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().objsMakingOutline_OPR = objsMakingOutline_OPR;

            makeSureWeHaveCorrectNumberOfEdges();
        }
    }

    float startAngle_OPR;
    public float StartAngle_OPR {
        get { return startAngle_OPR; }
        set
        {
            startAngle_OPR = value;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().startAngle_OPR = startAngle_OPR;
        }
    }

    bool radialPush_OPR; //push objs to edge of circle or to edge of box
    public bool RadialPush_OPR {
        get { return radialPush_OPR; }
        set
        {
            radialPush_OPR = value;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().radialPush_OPR = radialPush_OPR;
        }
    }

    //---Custom

    bool stdSize_OPC;
    public bool StdSize_OPC {
        get { return stdSize_OPC; }
        set
        {
            stdSize_OPC = value;//update local value
            if (gameObject.GetComponent<inspectorForOutline3>() != null)//update inspector value
                gameObject.GetComponent<inspectorForOutline3>().stdSize_OPC = stdSize_OPC;
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
        Outline = new GameObject("Outline Around Sprite Folder");
        copyOriginalGO_Transforms(Outline);
        Outline.transform.parent = outlineGameObjectsFolder.transform;

        //*****Set Variable Defaults*****

        //----- Debugging

        ShowOutline_GOs_InHierarchy_D = false;

        //----- Outline

        Active_O = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size

        Color_O = Color.blue;
        OrderInLayer_O = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
        Size_O = .1f;
        ScaleWithParentX_O = false;
        ScaleWithParentY_O = false;

        //--- Push Type

        edges_1 = new Dictionary<GameObject, Vector2>();

        PushType_Regular_or_Custom_OP = true;

        //regular

        ObjsMakingOutline_OPR = 4;
        StartAngle_OPR = 0;
        RadialPush_OPR = true;

        //custom

        StdSize_OPC = false;
    }

    void Update()
    {
        //--- set our directions every frame to RESIST CHANGE
        if (PushType_Regular_or_Custom_OP)
        {
            //NOTE: only required if regular Outline = True
            float rotation = StartAngle_OPR;
            float angleBetweenAllEdges = (ObjsMakingOutline_OPR == 0) ? 360 : 360 / ObjsMakingOutline_OPR;

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
            if (PushType_Regular_or_Custom_OP)
            {
                entry.Key.transform.position = entry.Value.normalized; //use ONLY vector (1) direction

                if (RadialPush_OPR) //Radial Push
                    entry.Key.transform.position *= Size_O;
                else //Square Push
                    entry.Key.transform.position *= Mathf.Abs(Size_O / Mathf.Cos((Vector2.Angle(entry.Value, Vector2.up) % 90) * Mathf.Deg2Rad));
            }
            else
            {
                if (StdSize_OPC) //STANDARD size for all vectors
                    entry.Key.transform.position = entry.Value.normalized * Size_O; //use ONLY vector (1) direction
                else
                    entry.Key.transform.position = entry.Value; //use ONLY vector (1) direction (2) magnitude
            }

            //NOTE: as of now our position is still on a compass in the (0,0,0) position

            if (ScaleWithParentX_O)
                entry.Key.transform.position = new Vector2(entry.Key.transform.position.x * this.transform.localScale.x, entry.Key.transform.position.y);

            if (ScaleWithParentY_O)
                entry.Key.transform.position = new Vector2(entry.Key.transform.position.x, entry.Key.transform.position.y * this.transform.localScale.y);

            entry.Key.transform.position = this.transform.position + (this.transform.rotation * entry.Key.transform.position);

            tempIndex++;
        }

    }

    //--- Outline Edge List Edits

    public bool addOutline(Vector2 outlineDirection)
    {
        return addOutline(outlineDirection, false);
    }

    bool addOutline(Vector2 outlineDirection, bool sudo)
    {
        if (PushType_Regular_or_Custom_OP == false || sudo == true)
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
        else //if we have a REGULAR outline then we cant change the list of edges directly
            return false;
    }

    public bool removeOutline(GameObject edgeGO)
    {
        return removeOutline(edgeGO, false);
    }

    bool removeOutline(GameObject edgeGO, bool sudo)
    {
        if (PushType_Regular_or_Custom_OP == false || sudo == true) 
        {
            if (edges_1.ContainsKey(edgeGO))
            {
                edges_1.Remove(edgeGO);
                Destroy(edgeGO);
                return true;
            }
            else
                return false;
        }
        else //if we have a regular outline then we cant change the list of edges directly
            return false;
    }

    public bool editOutline(GameObject edgeGO, Vector2 newDirection)
    {
        return editOutline(edgeGO, newDirection, false);
    }

    bool editOutline(GameObject edgeGO, Vector2 newDirection, bool sudo)
    {
        if (PushType_Regular_or_Custom_OP == false || sudo == true)
        {
            if (edges_1.ContainsKey(edgeGO))
            {
                edges_1[edgeGO] = newDirection;
                return true;
            }
            else
                return false;
        }
        else //if we have a regular outline then we cant change the list of edges directly
            return false;
    }

    //--- helper functions

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

    void destroyEdgesThatCreateOutline()
    {
        if (edges_1 != null)
        {
            foreach (KeyValuePair<GameObject, Vector2> entry in edges_1)
                DestroyImmediate(entry.Key);
            edges_1.Clear();
        }
    }

    void makeSureWeHaveCorrectNumberOfEdges()
    {
        if (PushType_Regular_or_Custom_OP)
        {
            if (edges_1.Count != ObjsMakingOutline_OPR)
            {
                int iterations = Mathf.Abs(ObjsMakingOutline_OPR - edges_1.Count);

                if (edges_1.Count < ObjsMakingOutline_OPR)
                {
                    for (int i = 0; i < iterations; i++)
                        addOutline(Vector2.up, true);
                }
                else
                {
                    List<GameObject> keyList = new List<GameObject>(edges_1.Keys);
                    for (int i = 0 ; i < iterations; i++)
                        removeOutline(keyList[i], true);
                }
            }
            else
                ; //we are good to go
        }
        else
            ; //there are no correct ammount of edges... this is a custom outline
    }
}