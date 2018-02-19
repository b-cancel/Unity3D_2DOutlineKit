using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object2DOutlines
{
    public enum push { regularPattern, customPattern }; //ONLY for concave outline
    public enum pushPattern { radial, squarial}; //ONLY for concave outline
    public enum rectType { regular, custom};

    [System.Serializable, ExecuteInEditMode]
    public class concaveOut : outline
    {
        //NOTE: updateEdgeCount()... calls updateEdgeRotations()...
        //NOTE: updateEdgeRotations()... calls updateEdgePositions()...

        //NOTE: (pushType == regular) -> ONLY controls rotation of edges
        //IF(pushType == regular) && (stdSize == true) -> can select patternType (Radial or Squarial)
        //IF(pushType == regular) && (stdSize == false) -> you can individually edit the magnitude of each edge [TODO in editor] 
        //---currently doable with "editEdge(GameObject edgeGO, Vector2 newDirection)"
        //ELSE IF (pushType == custom) -> you can individually edit the rotation and magnitude of each edge (AND add and remove edges) [TODO in editor] 
        //---currentlty doable with "editEdgeMagnitude(GameObject edgeGO, float newMag)"

        //NOTE: IF(RectSize == regulat) -> use the height and width of our source sprite
        //ELSE -> use our own custom height and width

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
                ScaleWithParentX_O = scaleWithParentX_O;
                ScaleWithParentY_O = scaleWithParentY_O;

                //ONLY push
                PushType_OP = pushType_OP;
                StdSize_OP = stdSize_OP;
                Size_OP = size_OP;
                EdgeCount_OPR = edgeCount_OPR;
                StartAngle_OPR = startAngle_OPR;
                PushPattern_OPR = pushPattern_OPR;
                RectSize_OPRS = rectSize_OPRS;
                RectWidth_OPRS = rectWidth_OPRS;
                RectHeight_OPRS = rectHeight_OPRS;
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
                newActiveCM = true;

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
        }
        [SerializeField, HideInInspector]
        private bool newActiveCM;

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

                newActiveO = true;
            }
        }
        [SerializeField, HideInInspector]
        private bool newActiveO;

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
        private bool newEdgeCount;

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

                newEdgeCount = true;
            }
        }

        [SerializeField, HideInInspector]
        bool stdSize_OP; //NOTE: this size refers to the world space thickness of the outline
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool StdSize_OP
        {
            get { return stdSize_OP; }
            set
            {
                stdSize_OP = value;//update local value

                updateEdgePositionsALL();
            }
        }

        [SerializeField, HideInInspector]
        float size_OP; //NOTE: this size refers to the world space thickness of the outline
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public float Size_OP
        {
            get { return size_OP; }
            set
            {
                value = (value >= 0) ? value : 0;
                size_OP = value;//update local value

                updateEdgePositionsALL();
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

                newEdgeCount = true;
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

                updateEdgePositionsALL();
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

                updateEdgeRotationsALL();
            }
        }

        [SerializeField, HideInInspector]
        rectType rectSize_OPRS;
        public rectType RectSize_OPRS
        {
            get { return rectSize_OPRS;  }
            set
            {
                rectSize_OPRS = value;

                //update height and width... which will update our outline
                if(rectSize_OPRS == rectType.regular)
                {
                    RectWidth_OPRS = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
                    RectHeight_OPRS = gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
                }
            }
        }

        [SerializeField, HideInInspector]
        float rectWidth_OPRS;
        public float RectWidth_OPRS
        {
            get { return rectWidth_OPRS; }
            set
            {
                rectWidth_OPRS = value;

                updateEdgeRotationsALL();
            }
        }

        [SerializeField, HideInInspector]
        float rectHeight_OPRS;
        public float RectHeight_OPRS
        {
            get { return rectHeight_OPRS; }
            set
            {
                rectHeight_OPRS = value;

                updateEdgeRotationsALL();
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
                ScaleWithParentX_O = true;
                ScaleWithParentY_O = true;

                //-----Push Type Vars
                outlineEdges = new Dictionary<GameObject, Vector2>();
                PushType_OP = push.regularPattern;
                StdSize_OP = true;
                Size_OP = 2f;
                //---Regular
                EdgeCount_OPR = 8;
                StartAngle_OPR = 0;
                PushPattern_OPR = pushPattern.squarial;

                RectSize_OPRS = rectType.regular;
                //width and height set by the above

                //---Hack Inits
                newEdgeCount = false;
                newActiveCM = false;
                newActiveO = false;

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

        new void Update()
        {
            switch (UpdateSprite)
            {
                case spriteUpdateSetting.EveryFrame: updateSpriteData(); break;
                case spriteUpdateSetting.AfterEveryChange:
                    if (spriteChanged(this.GetComponent<SpriteRenderer>()))
                        updateSpriteData();
                    break;
            }

            //required hack because of error
            //this is because this variables change might trigger a DestroyImmediate
            //DestoryImmediate MUST be used when a script runs in edit mode... but it cant be called by OnValidate... 
            //so this is a "HACK" to make it work without it (technically) being inside OnValidate()
            if (newEdgeCount)
            {
                updateEdgeCount();
                newEdgeCount = false;
            }

            //required hack because of warnings
            if (newActiveCM)
            {
                clippingMask.GetComponent<SpriteMask>().enabled = clipCenter_CM;
                newActiveCM = false;
            }

            if (newActiveO)
            {
                //all edges set active
                if (outlineEdges != null)
                    foreach (var GameObject in outlineEdges.Keys)
                        GameObject.SetActive(active_O);
            }

            base.Update();
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

        //UPDATE ---ROTATIONS--- BASED ON PATTERN
        void updateEdgeRotationsALL() //AUTOMATICALLY... calls updateEdgePositions()
        {
            if (PushType_OP == push.regularPattern)
            {
                if(outlineEdges != null) //precautionary check
                {
                    //StartAngle_OPR is used to rotate the shape after this step
                    float edgeRotation = (PushPattern_OPR == pushPattern.radial) ?
                        0 //for RADIAL... start at _0Rotation
                        : (Mathf.Rad2Deg * Mathf.Atan2((RectHeight_OPRS/2), (RectWidth_OPRS/2))); //for SQUARIAL... start by placing the first edge in the first corner (1st quadrant)
                    float angleBetweenAllEdges = (EdgeCount_OPR == 0) ? 0 : 360 / (float)EdgeCount_OPR;

                    List<GameObject> GOs = new List<GameObject>(outlineEdges.Keys);
                    foreach (var GameObject in GOs)
                    {
                        float oldMagnitude = outlineEdges[GameObject].magnitude;
                        Vector3 newDirection = Quaternion.AngleAxis(edgeRotation, Vector3.forward) * _0Rotation;
                        editEdge(GameObject, newDirection.normalized * oldMagnitude, true); //this will also updateEdgePosition()
                        edgeRotation += angleBetweenAllEdges;
                    }
                }
            }
            //ELSE... follow custom outline rules
        }

        //-------------------------FOR BOTH PushTypes

        //USES... same as "updateEdgePosition()"
        void updateEdgePositionsALL()
        {
            if (outlineEdges != null)
            {
                List<GameObject> GOs = new List<GameObject>(outlineEdges.Keys);
                foreach (var GameObject in GOs)
                    updateEdgePosition(GameObject, outlineEdges[GameObject]);
            }
        }

        //USES...
        //(PushType_OP == regular)... use (PushPattern_OPR)
        //(PushType_OP == custom)... use (StdSize_OPC) 
        //(PushType_OP == either)... use (Size_O) (ScaleWithParentX_O) (ScaleWithParentY_O)

        //UPDATE ---MAGNITUDE--- BASED ON PATTERN
        void updateEdgePosition(GameObject anEdge, Vector2 vect)
        {
            outlineEdges[anEdge] = vect;

            //NOTE: we push from origin (0,0,0)

            Vector3 newVect = Vector3.zero;
            if (PushType_OP == push.regularPattern)
            {
                if (StdSize_OP)
                {
                    if (PushPattern_OPR == pushPattern.radial) //Radial Push
                        newVect = vect.normalized * Size_OP;
                    else //Squarial Push
                    {
                        //do this pretending 
                        //that the Rect Width length goes from -x to x
                        //and that the Rect Length goes from -y to y

                        float cornerAngle = (Mathf.Rad2Deg * Mathf.Atan2((RectHeight_OPRS / 2), (RectWidth_OPRS / 2))); //top right corner
                        //TODO... might have to repair below with conversion into degree
                        float cornerHypotenuse = (RectHeight_OPRS / 2) / Mathf.Sin(cornerAngle * Mathf.Deg2Rad);

                        float AF_Up = Vector3.Angle(Vector3.up, vect.normalized);
                        float AF_Right = Vector3.Angle(Vector3.right, vect.normalized);
                        float AF_Down = Vector3.Angle(Vector3.down, vect.normalized);
                        float AF_Left = Vector3.Angle(Vector3.left, vect.normalized);

                        float newMagnitude = 0;

                        //--find what quadrant this outline is in
                        if (AF_Left > 90 && AF_Down > 90) //1st quad
                        {
                            if (AF_Right < cornerAngle) //base ourselves on width
                                newMagnitude = (RectWidth_OPRS/2) / (Mathf.Cos(AF_Right * Mathf.Deg2Rad));
                            else if (AF_Right > cornerAngle) //base ourselves on height
                                newMagnitude = (RectHeight_OPRS/2) / (Mathf.Cos((90 - AF_Right) * Mathf.Deg2Rad));
                            else
                                newMagnitude = cornerHypotenuse;
                        }
                        else if(AF_Right > 90 && AF_Down > 90) //2nd quad
                        {
                            if (AF_Left < cornerAngle) //base ourselves on width
                                newMagnitude = (RectWidth_OPRS / 2) / (Mathf.Cos(AF_Left * Mathf.Deg2Rad));
                            else if (AF_Left > cornerAngle) //base ourselves on height
                                newMagnitude = (RectHeight_OPRS / 2) / (Mathf.Cos((90 - AF_Left) * Mathf.Deg2Rad));
                            else
                                newMagnitude = cornerHypotenuse;
                        }
                        else if(AF_Right > 90 && AF_Up > 90) //3rd quad
                        {
                            if (AF_Left < cornerAngle) //base ourselves on width
                                newMagnitude = (RectWidth_OPRS / 2) / (Mathf.Cos(AF_Left * Mathf.Deg2Rad));
                            else if (AF_Left > cornerAngle) //base ourselves on height
                                newMagnitude = (RectHeight_OPRS / 2) / (Mathf.Cos((90 - AF_Left) * Mathf.Deg2Rad));
                            else
                                newMagnitude = cornerHypotenuse;
                        }
                        else //4th quad
                        {
                            if (AF_Right < cornerAngle) //base ourselves on width
                                newMagnitude = (RectWidth_OPRS / 2) / (Mathf.Cos(AF_Right * Mathf.Deg2Rad));
                            else if (AF_Right > cornerAngle) //base ourselves on height
                                newMagnitude = (RectHeight_OPRS / 2) / (Mathf.Cos((90 - AF_Right) * Mathf.Deg2Rad));
                            else
                                newMagnitude = cornerHypotenuse;
                        }

                        newVect = vect.normalized * newMagnitude;

                        newVect = newVect.normalized * (newVect.magnitude * Size_OP);
                    }
                }
                else
                    newVect = vect.normalized;

                //Rotate the RADIAL or SQUARIAL pattern the number of StartAngle_OPR
                float oldMagnitude = newVect.magnitude;
                Vector3 newDirection = Quaternion.AngleAxis(StartAngle_OPR, Vector3.forward) * newVect.normalized;
                newVect = newDirection.normalized * oldMagnitude;
            }
            else
            {
                if (StdSize_OP) //STANDARD size for all vectors
                    newVect = vect.normalized * Size_OP; //use ONLY vector (1) direction
                else
                    newVect = vect; //use ONLY vector (1) direction (2) magnitude
            }
            anEdge.transform.position = newVect;

            //NOTE: as of now our position is still on a compass in the (0,0,0) position

            //take into consideration the SCALE of the sprite we are an edge for
            if (ScaleWithParentX_O)
                anEdge.transform.position = new Vector2(anEdge.transform.position.x * this.transform.localScale.x, anEdge.transform.position.y);
            if (ScaleWithParentY_O)
                anEdge.transform.position = new Vector2(anEdge.transform.position.x, anEdge.transform.position.y * this.transform.localScale.y);

            //take into consideration the POSITION and ROTATION of the sprite we are an edge for
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

        public bool editEdgeMagnitude(GameObject edgeGO, float newMag)
        {
            if (outlineEdges != null) //precautionary check
            {
                if (outlineEdges.ContainsKey(edgeGO))
                {
                    updateEdgePosition(edgeGO, outlineEdges[edgeGO].normalized * newMag);

                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
    }
}