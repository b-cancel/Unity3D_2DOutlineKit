using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object2DOutlines
{
    public enum push { regularPattern, customPattern }; //ONLY for concave outline
    public enum pushPattern { radial, squarial}; //ONLY for concave outline

    [System.Serializable, ExecuteInEditMode]
    public class concaveOut : outline
    {
        //NOTE: (PushType = push.regularPattern) only the direction of the vector is used to calculate out outline
        //ELSE... both our direction and rotation will be used (unless you specify a standard size... in which case once again only the rotation will be used)

        //NOTE: updateEdgeCount()... calls updateEdgeRotations()...
        //NOTE: updateEdgeRotations()... calls updateEdgePositions()...

        [SerializeField, HideInInspector]
        private bool awakeFinished_CAVE;

        void OnValidate()
        {
            if (awakeFinished_CAVE)
            {
                //Optimization
                UpdateSprite = updateSprite;

                //Debugging
                ShowOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

                //Sprite Outline
                Active_SO = active_SO;
                OrderInLayer_SO = orderInLayer_SO;
                Color_SO = color_SO;

                //Clipping Mask
                ClipCenter_CM = clipCenter_CM;
                AlphaCutoff_CM = alphaCutoff_CM;
                CustomRange_CM = customRange_CM;
                FrontLayer_CM = frontLayer_CM;
                BackLayer_CM = backLayer_CM;

                //Sprite Overlay
                Active_O = active_O;
                Color_O = color_O;
                OrderInLayer_O = orderInLayer_O;
                Size_O = size_O;
                ScaleWithParentX_O = scaleWithParentX_O;
                ScaleWithParentY_O = scaleWithParentY_O;

                //ONLY push
                PushType_OP = pushType_OP;
                EdgeCount_OPR = edgeCount_OPR;
                StartAngle_OPR = startAngle_OPR;
                PushPattern_OPR = pushPattern_OPR;
                StdSize_OPC = stdSize_OPC;
            }
        }

        //-----Clipping Mask Variables

        [Space(10)]
        [Header("CLIPPING MASK VARIABLES-----")]
        [SerializeField, HideInInspector]
        bool clipCenter_CM;
        public bool ClipCenter_CM
        {
            get { return clipCenter_CM; }
            set
            {
                clipCenter_CM = value; //update local value

                //enable or disable mask
                clippingMask.GetComponent<SpriteMask>().enabled = clipCenter_CM;

                //update how our edge gameobjects interact with the mask
                if (clipCenter_CM == true)
                {
                    if (outlineEdges != null)
                        foreach (var GameObject in outlineEdges.Keys)
                            GameObject.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    if (outlineEdges != null)
                        foreach (var GameObject in outlineEdges.Keys)
                            GameObject.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                }
            }
        } //NOTE: used in update function... doesnt have to do anyting special for get and set...

        //-----Outline Variables-----

        [SerializeField, HideInInspector]
        GameObject thisOutline;

        [Space(10)]
        [Header("OUTLINE VARIABLES-----")]
        [SerializeField, HideInInspector]
        bool active_O;
        public bool Active_O
        {
            get { return active_O; }
            set
            {
                active_O = value;//update local value

                //all edges set active
                if (outlineEdges != null)
                    foreach (var GameObject in outlineEdges.Keys)
                        GameObject.SetActive(active_O);
            }
        }

        [SerializeField, HideInInspector]
        Color color_O;
        public Color Color_O
        {
            get { return color_O; }
            set
            {
                color_O = value;//update local value

                //update our edges with the new color
                if (outlineEdges != null)
                    foreach (var GameObject in outlineEdges.Keys)
                        GameObject.GetComponent<SpriteRenderer>().color = color_O;
            }
        }

        [SerializeField, HideInInspector]
        int orderInLayer_O;
        public int OrderInLayer_O
        {
            get { return orderInLayer_O; }
            set
            {
                orderInLayer_O = value;//update local value

                //update our edges with the new color
                if (outlineEdges != null)
                    foreach (var GameObject in outlineEdges.Keys)
                        GameObject.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
            }
        }

        [SerializeField, HideInInspector]
        float size_O; //NOTE: this size refers to the world space thickness of the outline
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public float Size_O
        {
            get { return size_O; }
            set
            {
                value = (value >= 0) ? value : 0;
                size_O = value;//update local value

                updateEdgePositionsALL();
            }
        }

        [SerializeField, HideInInspector]
        bool scaleWithParentX_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool ScaleWithParentX_O
        {
            get { return scaleWithParentX_O; }
            set
            {
                scaleWithParentX_O = value;//update local value

                updateEdgePositionsALL();
            }
        }

        [SerializeField, HideInInspector]
        bool scaleWithParentY_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool ScaleWithParentY_O
        {
            get { return scaleWithParentY_O; }
            set
            {
                scaleWithParentY_O = value;//update local value

                updateEdgePositionsALL();
            }
        }

        //-------------------------Push Type Variables-------------------------

        [SerializeField, HideInInspector]
        Dictionary<GameObject, Vector2> outlineEdges;

        [SerializeField, HideInInspector]
        private bool mustCallEdgeCount;

        [Space(10)]
        [Header("PUSH TYPE VARIABLES-----")]
        [SerializeField, HideInInspector]
        push pushType_OP;
        public push PushType_OP
        {
            get { return pushType_OP; }
            set
            {
                pushType_OP = value; //update local value

                mustCallEdgeCount = true;
            }
        }

        //---Regular

        [SerializeField, HideInInspector]
        private static Vector2 _0Rotation; //what we consider 0 rotation (current Vector3.right)

        [SerializeField, HideInInspector]
        int edgeCount_OPR; //also the count of gameobjects that make up the outline
        public int EdgeCount_OPR
        {
            get { return edgeCount_OPR; }
            set
            {
                edgeCount_OPR = (value >= 0) ? value : 0; //update local value

                mustCallEdgeCount = true;
            }
        }

        [SerializeField, HideInInspector]
        float startAngle_OPR;
        public float StartAngle_OPR
        {
            get { return startAngle_OPR; }
            set
            {
                startAngle_OPR = value; //update local value

                updateEdgeRotationsALL();
            }
        }

        [SerializeField, HideInInspector]
        pushPattern pushPattern_OPR; //push objs to edge of circle or to edge of box
        public pushPattern PushPattern_OPR
        {
            get { return pushPattern_OPR; }
            set
            {
                pushPattern_OPR = value; //update local value

                updateEdgePositionsALL();
            }
        }

        //---Custom

        [SerializeField, HideInInspector]
        bool stdSize_OPC;
        public bool StdSize_OPC
        {
            get { return stdSize_OPC; }
            set
            {
                stdSize_OPC = value; //update local value

                updateEdgePositionsALL();
            }
        }

        new void Awake()
        {
            awakeFinished_CAVE = false;

            if (gameObject.transform.Find("Outline Folder") == null)
            {
                _0Rotation = Vector3.right; //what we consider 0 rotation (current Vector3.right)

                //----------Objects Inits

                thisOutline = new GameObject("The Outline");
                Material tempMaterial = initPart1(gameObject, ref thisOutline, ref outlineGameObjectsFolder);
                DestroyImmediate(thisOutline.GetComponent<SpriteRenderer>()); //DIFFERENT
                initPart2(gameObject, ref outlineGameObjectsFolder, ref spriteOverlay, ref clippingMask, ref tempMaterial);

                //----------Variable Inits

                //---Clipping Mask Vars
                ClipCenter_CM = true;

                //---Outline Vars
                Active_O = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size
                Color_O = Color.red;
                OrderInLayer_O = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
                Size_O = 2f;
                ScaleWithParentX_O = true;
                ScaleWithParentY_O = true;

                //-----Push Type Vars
                outlineEdges = new Dictionary<GameObject, Vector2>();
                PushType_OP = push.regularPattern;

                //---Regular
                EdgeCount_OPR = 8;
                StartAngle_OPR = 0;
                PushPattern_OPR = pushPattern.squarial;
                //---Custom
                StdSize_OPC = false;

                //---Var Inits from base outline class
                base.Awake();
            }
            else
            {
                outlineGameObjectsFolder = gameObject.transform.Find("Outline Folder").gameObject;
                thisOutline = outlineGameObjectsFolder.transform.Find("The Outline").gameObject;
                spriteOverlay = outlineGameObjectsFolder.transform.Find("Sprite Overlay").gameObject;
                clippingMask = outlineGameObjectsFolder.transform.Find("Sprite Mask").gameObject;
            }

            awakeFinished_CAVE = true;
        }

        //--------------------------------------------------SLIGHTLY DIFFERENT CODE--------------------------------------------------

        void Update()
        {
            switch (UpdateSprite)
            {
                case spriteUpdateSetting.EveryFrame: updateSpriteData(); break;
                case spriteUpdateSetting.AfterEveryChange:
                    if (spriteChanged(this.GetComponent<SpriteRenderer>()))
                        updateSpriteData();
                    break;
            }

            //NOTE: this is a required "HACK"
            //this is because this variables change might trigger a DestroyImmediate
            //DestoryImmediate MUST be used when a script runs in edit mode... but it cant be called by OnValidate... 
            //so this is a "HACK" to make it work without it (technically) being inside OnValidate()
            if (mustCallEdgeCount)
            {
                updateEdgeCount();
                mustCallEdgeCount = false;
            }
        }

        public void updateSpriteData()
        {
            //update sprite overlay
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

            //update clipping mask
            copySpriteRendererDataToClipMask(this.gameObject, clippingMask);

            //update outline
            if (outlineEdges != null)
                foreach (var GameObject in outlineEdges.Keys)
                    copySpriteRendererData(this.GetComponent<SpriteRenderer>(), GameObject.GetComponent<SpriteRenderer>());
        }

        //--------------------------------------------------SUPER DIFFERENT CODE--------------------------------------------------

        //-------------------------ONLY PUSH TYPE OUTLINE-------------------------

        //-------------------------ONLY PushType == push.regularPattern

        void updateEdgeCount() //AUTOMATICALLY... calls updateEdgeRotations()
        {
            if (PushType_OP == push.regularPattern)
            {
                if(outlineEdges != null) //precautionary check
                {
                    if (outlineEdges.Count != EdgeCount_OPR)
                    {
                        int totalDifferences = Mathf.Abs(EdgeCount_OPR - outlineEdges.Count);

                        if (outlineEdges.Count < EdgeCount_OPR)
                        {
                            for (int i = 0; i < totalDifferences; i++)
                                addEdge(_0Rotation, true);
                        }
                        else
                        {
                            for (int i = 0; i < totalDifferences; i++)
                            {
                                List<GameObject> keyList = new List<GameObject>(outlineEdges.Keys);
                                removeEdge(keyList[keyList.Count-1], true); //remove from the back
                            }     
                        }

                        updateEdgeRotationsALL(); //BECAUSE... the count of edges changed
                    }
                    //ELSE... we have the correct number of edges
                }
            }
            //ELSE... follow custom outline rules
        }

        void updateEdgeRotationsALL() //AUTOMATICALLY... calls updateEdgePositions()
        {
            if (PushType_OP == push.regularPattern)
            {
                if(outlineEdges != null) //precautionary check
                {
                    float edgeRotation = StartAngle_OPR;
                    float angleBetweenAllEdges = (EdgeCount_OPR == 0) ? 0 : 360 / EdgeCount_OPR;

                    List<GameObject> GOs = new List<GameObject>(outlineEdges.Keys);
                    foreach (var GameObject in GOs)
                    {
                        float oldMagnitude = outlineEdges[GameObject].magnitude;
                        Vector3 newDirection = Quaternion.AngleAxis(edgeRotation, Vector3.forward) * _0Rotation;
                        editEdge(GameObject, newDirection.normalized * oldMagnitude, true);

                        edgeRotation += angleBetweenAllEdges;
                    }

                    updateEdgePositionsALL(); //BECAUSE... the direction of every edge changed
                }
            }
            //ELSE... follow custom outline rules
        }

        //-------------------------FOR BOTH PushTypes

        //USES... same as "updateEdgePosition()"
        void updateEdgePositionsALL()
        {
            if (outlineEdges != null)
                foreach (var GameObject in outlineEdges.Keys)
                    updateEdgePosition(GameObject, outlineEdges[GameObject]);
        }

        //USES...
        //(PushType_OP == regular)... use (PushPattern_OPR)
        //(PushType_OP == custom)... use (StdSize_OPC) 
        //(PushType_OP == either)... use (Size_O) (ScaleWithParentX_O) (ScaleWithParentY_O)
        void updateEdgePosition(GameObject anEdge, Vector2 vect)
        {
            if (PushType_OP == push.regularPattern)
            {
                anEdge.transform.position = vect.normalized; //use ONLY vector (1) direction

                if (PushPattern_OPR == pushPattern.radial) //Radial Push
                    anEdge.transform.position *= Size_O;
                else //Square Push (note: SQUARE not RECTANGLE) [Hypotenuse = Adjecent / cos(theta)]
                    anEdge.transform.position *= Mathf.Abs(Size_O / (Mathf.Cos((Vector2.Angle(vect, _0Rotation) % 90) * Mathf.Deg2Rad)));
            }
            else
            {
                if (StdSize_OPC) //STANDARD size for all vectors
                    anEdge.transform.position = vect.normalized * Size_O; //use ONLY vector (1) direction
                else
                    anEdge.transform.position = vect; //use ONLY vector (1) direction (2) magnitude
            }

            //NOTE: as of now our position is still on a compass in the (0,0,0) position

            if (ScaleWithParentX_O)
                anEdge.transform.position = new Vector2(anEdge.transform.position.x * this.transform.localScale.x, anEdge.transform.position.y);

            if (ScaleWithParentY_O)
                anEdge.transform.position = new Vector2(anEdge.transform.position.x, anEdge.transform.position.y * this.transform.localScale.y);

            //take into consideration the position and rotation of the sprite we are an edge for

            anEdge.transform.position = this.transform.position + (this.transform.rotation * anEdge.transform.position);
        }

        //-------------------------Outline Edge List Edits-------------------------
        //So... these are used by BOTH (1) regular and (2) custom patterns
        //So... don't update the outline as a whole... 
        //the function that called these GIVEN REGULAR PATTERN should call the functions required to update the outline or pattern as a whole

        //-------------------------PRIVATE

        //USES... PushType_OP | Color_O | orderInLayer_O | ClipCenter_CM | Active_O
        bool addEdge(Vector2 outlineDirection, bool sudo)
        {
            if (PushType_OP == push.customPattern || sudo == true)
            {
                if (thisOutline != null) //precautionary check
                {
                    //NOTE: here we must manually take into account, color, order in layer, activeness

                    GameObject tempSpriteCopy = new GameObject();
                    tempSpriteCopy.AddComponent<SpriteRenderer>();

                    copyTransform(gameObject, tempSpriteCopy);

                    //assign our parent
                    tempSpriteCopy.transform.parent = thisOutline.transform;

                    //use text shader so that we only conserve the silhouette of our sprite
                    var tempMaterial = new Material(tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial);
                    tempMaterial.shader = Shader.Find("GUI/Text Shader");
                    tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;

                    //set color
                    tempSpriteCopy.GetComponent<SpriteRenderer>().color = Color_O;

                    //set sorting layer
                    tempSpriteCopy.GetComponent<SpriteRenderer>().sortingOrder = OrderInLayer_O;

                    //set sprite renderer data
                    copySpriteRendererData(this.GetComponent<SpriteRenderer>(), tempSpriteCopy.GetComponent<SpriteRenderer>());

                    //set interaction with mask
                    if (ClipCenter_CM)
                        tempSpriteCopy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                    else
                        tempSpriteCopy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

                    //save our data
                    outlineEdges.Add(tempSpriteCopy, outlineDirection);

                    //update position that is affected by... size, scale par x, scale par y
                    updateEdgePosition(tempSpriteCopy, outlineDirection);

                    //set active state
                    tempSpriteCopy.SetActive(Active_O);

                    return true;
                }
                else
                    return false;
            }
            else //if we have a REGULAR outline then we cant change the list of edges directly
                return false;
        }

        //USES... PushType_OP
        bool removeEdge(GameObject edgeGO, bool sudo)
        {
            if (PushType_OP == push.customPattern || sudo == true)
            {
                if (outlineEdges != null) //precautionary check
                {
                    if (outlineEdges.ContainsKey(edgeGO))
                    {
                        outlineEdges.Remove(edgeGO);
                        DestroyImmediate(edgeGO);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else //if we have a regular outline then we cant change the list of edges directly
                return false;
        }

        //USES... PushType_OP
        bool editEdge(GameObject edgeGO, Vector2 newDirection, bool sudo)
        {
            if (PushType_OP == push.customPattern || sudo == true)
            {
                if (outlineEdges != null) //precautionary check
                {
                    if (outlineEdges.ContainsKey(edgeGO))
                    {
                        outlineEdges[edgeGO] = newDirection;

                        //update position that is affected by... size, scale par x, scale par y
                        updateEdgePosition(edgeGO, newDirection);

                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else //if we have a regular outline then we cant change the list of edges directly
                return false;
        }

        //-------------------------PUBLIC

        public bool addEdge(Vector2 outlineDirection) //you WONT BE ABLE to use me unless your (PushType == push.customPattern)
        {
            return addEdge(outlineDirection, false);
        }

        public bool removeEdge(GameObject edgeGO) //you WONT BE ABLE to use me unless your (PushType == push.customPattern)
        {
            return removeEdge(edgeGO, false);
        }

        public bool editEdge(GameObject edgeGO, Vector2 newDirection) //you WONT BE ABLE to use me unless your (PushType == push.customPattern)
        {
            return editEdge(edgeGO, newDirection, false);
        }
    }
}