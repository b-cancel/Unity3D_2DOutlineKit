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
                ObjsMakingOutline_OPR = objsMakingOutline_OPR;
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
                        foreach (KeyValuePair<GameObject, Vector2> dictVal in outlineEdges)
                            dictVal.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    if (outlineEdges != null)
                        foreach (KeyValuePair<GameObject, Vector2> dictVal in outlineEdges)
                            dictVal.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
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
                    foreach (KeyValuePair<GameObject, Vector2> entry in outlineEdges)
                        entry.Key.SetActive(active_O);
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
                    foreach (KeyValuePair<GameObject, Vector2> dictVal in outlineEdges)
                        dictVal.Key.GetComponent<SpriteRenderer>().color = color_O;
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
                    foreach (KeyValuePair<GameObject, Vector2> dictVal in outlineEdges)
                        dictVal.Key.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
            }
        }

        //*****CHECK CODE BELOW

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

                updatePositionsOfEdges();
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

                updatePositionsOfEdges();
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

                updatePositionsOfEdges();
            }
        }

        //-------------------------Push Type Variables-------------------------

        [SerializeField, HideInInspector]
        Dictionary<GameObject, Vector2> outlineEdges;

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

                if (pushType_OP == push.regularPattern) //f -> t (CLEAR custom oultine data)
                {
                    destroyEdgesThatCreateOutline();

                    makeSureWeHaveCorrectNumberOfEdges();

                    updateNormalEdgeVectors(); //NOTE: this also updates the positions ofthe edges
                }
                else //t -> f (KEEP regular outline data)
                    updatePositionsOfEdges();
            }
        }

        //---Regular

        [SerializeField, HideInInspector]
        int objsMakingOutline_OPR; //also the count of gameobjects that make up the outline
        public int ObjsMakingOutline_OPR
        {
            get { return objsMakingOutline_OPR; }
            set
            {
                objsMakingOutline_OPR = (value >= 0) ? value : 0; //update local value

                makeSureWeHaveCorrectNumberOfEdges();

                updateNormalEdgeVectors();
            }
        }

        //****CHECK CODE ABOVE

        [SerializeField, HideInInspector]
        float startAngle_OPR;
        public float StartAngle_OPR
        {
            get { return startAngle_OPR; }
            set
            {
                startAngle_OPR = value; //update local value

                updateNormalEdgeVectors();
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

                updatePositionsOfEdges();
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

                updatePositionsOfEdges();
            }
        }

        new void Awake()
        {
            awakeFinished_CAVE = false;

            if (gameObject.transform.Find("Outline Folder") == null)
            {
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
                ObjsMakingOutline_OPR = 8;
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

        //-------------------------UNIQUE CODE-------------------------

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
        }

        public void updateSpriteData()
        {
            //update sprite overlay
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

            //update clipping mask
            copySpriteRendererDataToClipMask(this.gameObject, clippingMask);

            //update outline
            if (outlineEdges != null)
                foreach (KeyValuePair<GameObject, Vector2> entry in outlineEdges)
                    copySpriteRendererData(this.GetComponent<SpriteRenderer>(), entry.Key.GetComponent<SpriteRenderer>());
        }

        //****CHECK CODE BELOW

        //-------------------------ONLY PUSH TYPE OUTLINE-------------------------

        //USES...
        //(PushType_OP == regular)... use (PushPattern_OPR)
        //(PushType_OP == custom)... use (StdSize_OPC) 
        //(PushType_OP == either)... use (Size_O) (ScaleWithParentX_O) (ScaleWithParentY_O)
        void updatePositionsOfEdges()
        {
            if (outlineEdges != null)
                foreach (KeyValuePair<GameObject, Vector2> entry in outlineEdges)
                    updateEdgePosition(entry.Key, entry.Value);
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
                    anEdge.transform.position *= Mathf.Abs(Size_O / (Mathf.Cos((Vector2.Angle(vect, Vector2.up) % 90) * Mathf.Deg2Rad)));
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

        //--- helper functions

        void destroyEdgesThatCreateOutline()
        {
            if (outlineEdges != null) //precautionary check
            {
                foreach (KeyValuePair<GameObject, Vector2> entry in outlineEdges)
                    DestroyImmediate(entry.Key);
                outlineEdges.Clear();
            }
        }

        void makeSureWeHaveCorrectNumberOfEdges()
        {
            if (PushType_OP == push.regularPattern)
            {
                if(outlineEdges != null) //precautionary check
                {
                    if (outlineEdges.Count != ObjsMakingOutline_OPR)
                    {
                        int iterations = Mathf.Abs(ObjsMakingOutline_OPR - outlineEdges.Count);

                        if (outlineEdges.Count < ObjsMakingOutline_OPR)
                        {
                            for (int i = 0; i < iterations; i++)
                                addOutline(Vector2.up, true); //NOTE: the lines will update with a simply Vector3.Up... BUT after this we update our directions and then update all of the lines directions... then all the positions...
                        }
                        else
                        {
                            List<GameObject> keyList = new List<GameObject>(outlineEdges.Keys);
                            for (int i = 0; i < iterations; i++)
                                removeOutline(keyList[i], true);
                        }
                    }
                    //ELSE... we have the correct number of edges
                }
            }
            //ELSE... follow custom outline rules
        }

        void updateNormalEdgeVectors()
        {
            if (PushType_OP == push.regularPattern)
            {
                //NOTE: only required if regular Outline = True
                float rotation = StartAngle_OPR;
                float angleBetweenAllEdges = (ObjsMakingOutline_OPR == 0) ? 360 : 360 / ObjsMakingOutline_OPR;

                if(outlineEdges != null) //precautionary check
                {
                    List<GameObject> edgesKeys = new List<GameObject>(outlineEdges.Keys);
                    foreach (var aKey in edgesKeys)
                    {
                        float oldMagnitude = outlineEdges[aKey].magnitude;
                        //NOTE: your direction is calculated from a compass (your obj rotation is not taken into consideration till later)
                        Vector3 newDirection = Quaternion.AngleAxis(rotation, Vector3.forward) * Vector3.up;

                        outlineEdges[aKey] = newDirection.normalized * oldMagnitude;

                        rotation += angleBetweenAllEdges;
                    }

                    updatePositionsOfEdges(); //we have new directions so we must recalculate our positions because they are based on our directions
                }
            }
            //ELSE... follow custom outline rules
        }

        //-------------------------Outline Edge List Edits-------------------------

        public bool addOutline(Vector2 outlineDirection)
        {
            return addOutline(outlineDirection, false);
        }

        //USES... PushType_OP | Color_O | orderInLayer_O | ClipCenter_CM | Active_O
        bool addOutline(Vector2 outlineDirection, bool sudo)
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

        public bool removeOutline(GameObject edgeGO)
        {
            return removeOutline(edgeGO, false);
        }

        //USES... PushType_OP
        bool removeOutline(GameObject edgeGO, bool sudo)
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

        public bool editOutline(GameObject edgeGO, Vector2 newDirection)
        {
            return editOutline(edgeGO, newDirection, false);
        }

        //USES... PushType_OP
        bool editOutline(GameObject edgeGO, Vector2 newDirection, bool sudo)
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
    }
}