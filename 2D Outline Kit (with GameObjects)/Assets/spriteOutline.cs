using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object2DOutlines
{
    [System.Serializable, ExecuteInEditMode]
    public class spriteOutline : outline
    {
        [System.NonSerialized]
        private bool _awakeFinished; //SHOULD NOT be serialized... if it is... OnValidate will run before it should

        [SerializeField]
        private bool notFirstRun; //NOTE: this relies on the fact that DEFAULT bool value is FALSE

        void OnValidate()
        {
            if (_awakeFinished)
            {
                //Optimization
                UpdateSprite = updateSprite;

                //Debugging
                ShowOutline_GOs_InHierarchy = showOutline_GOs_InHierarchy;

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

                SpriteType_O = spriteType_O;

                Size_O = size_O;

                //-----conCAVE
                PatternType_O_CAVE = patternType_O_CAVE;

                //pattern type == radial
                EdgeCount_O_CAVE_R = edgeCount_O_CAVE_R;

                //pattern type == BOTH
                StdSize_O_CAVE = stdSize_O_CAVE;
                //---SIZE goes here in inspector
                Rotation_O_CAVE = rotation_O_CAVE;
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
                _newActiveCM = true;

                //update how our edge gameobjects interact with the mask
                if (clipCenter_CM == true)
                {
                    if (outlineEdges != null)
                        foreach (var pair in outlineEdges)
                            pair.go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    if (outlineEdges != null)
                        foreach (var pair in outlineEdges)
                            pair.go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                }
            }
        }
        [System.NonSerialized]
        private bool _newActiveCM;

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

                _newActiveO = true;
            }
        }
        [System.NonSerialized]
        private bool _newActiveO;

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
                    foreach (var pair in outlineEdges)
                        pair.go.GetComponent<SpriteRenderer>().color = color_O;
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
                    foreach (var pair in outlineEdges)
                        pair.go.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
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

                _newEdgeCount = true; //TODO... we shouldn't need this here
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

                _newEdgeCount = true; //TODO... we shouldn't need this here
            }
        }

        //---DECIDES BETWEEN OUTLINE TYPE
        [SerializeField, HideInInspector]
        spriteType spriteType_O;
        public spriteType SpriteType_O
        {
            get { return spriteType_O; }
            set
            {
                print("+++++SET SPRITE TYPE+++++");
                if (value != spriteType_O)
                {
                    print("Set to: " + value);
                    if (value == spriteType.conVEX)
                        _newVEXinit = true;
                    else
                        _newCAVEinit = true;
                }
                else
                    print("it was equal");
            }
        }
        [System.NonSerialized]
        private bool _newVEXinit;
        [System.NonSerialized]
        private bool _newCAVEinit;

        [SerializeField, HideInInspector]
        float size_O;
        public float Size_O
        {
            get { return size_O; }
            set
            {
                value = (value >= 0) ? value : 0;
                size_O = value;//update local value

                _newEdgeCount = true; //TODO... we shouldn't need this here
            }
        }

        //-------------------------Push Type Variables-------------------------

        [SerializeField, HideInInspector]
        List<GO_to_Vector2> outlineEdges;

        //non serializable dictionary simply makes lookups quick
        Dictionary<GameObject, int> outEdgesHelper;

        [System.NonSerialized]
        private bool _newEdgeCount;

        [Space(10)]
        [Header("conCAVE TYPE VARIABLES-----")]
        [SerializeField, HideInInspector]
        pushPattern patternType_O_CAVE;
        public pushPattern PatternType_O_CAVE
        {
            get { return patternType_O_CAVE; }
            set
            {
                patternType_O_CAVE = value; //update local value

                _newEdgeCount = true;
            }
        }

        [SerializeField, HideInInspector]
        private Vector2 _0Rotation; //what we consider 0 rotation (current Vector3.right)

        [SerializeField, HideInInspector]
        int edgeCount_O_CAVE_R; //also the count of gameobjects that make up the outline
        public int EdgeCount_O_CAVE_R
        {
            get { return edgeCount_O_CAVE_R; }
            set
            {
                edgeCount_O_CAVE_R = (value >= 0) ? value : 0; //update local value

                _newEdgeCount = true;
            }
        }

        [SerializeField, HideInInspector]
        bool stdSize_O_CAVE; //NOTE: this size refers to the world space thickness of the outline
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool StdSize_O_CAVE
        {
            get { return stdSize_O_CAVE; }
            set
            {
                stdSize_O_CAVE = value;//update local value

                _newEdgeCount = true; //TODO... we shouldn't need this here
            }
        }

        [SerializeField, HideInInspector]
        float rotation_O_CAVE;
        public float Rotation_O_CAVE
        {
            get { return rotation_O_CAVE; }
            set
            {
                rotation_O_CAVE = value; //update local value

                _newEdgeCount = true; //TODO... we shouldn't need this here
            }
        }

        void Awake()
        {
            _awakeFinished = false;

            //---Hacks Inits (dont need serialization and you will never be fast enough to hit play unless they are false)
            _newEdgeCount = false;
            _newActiveCM = false;
            _newActiveO = false;
            _newCAVEinit = false;
            _newVEXinit = false;

            if (notFirstRun == false)
            {
                _0Rotation = Vector3.right; //what we consider 0 rotation (current Vector3.right)

                //----------Objects Inits

                thisOutline = new GameObject("The Outline");
                Material tempMaterial = initPart1(gameObject, ref thisOutline, ref outlineGameObjectsFolder);
                DestroyImmediate(thisOutline.GetComponent<SpriteRenderer>()); //DIFFERENT
                initPart2(gameObject, ref outlineGameObjectsFolder, ref spriteOverlay, ref clippingMask, ref tempMaterial);

                outEdgesHelper = new Dictionary<GameObject, int>();
                outlineEdges = new List<GO_to_Vector2>();

                notFirstRun = true;
            }
            else
            {
                //fill our dictionary again from our serializable data (then we use them together)
                outEdgesHelper = new Dictionary<GameObject, int>();
                for (int i = 0; i < outlineEdges.Count; i++)
                    outEdgesHelper.Add(outlineEdges[i].go, i);
            }

            _awakeFinished = true;
        }

        new void Reset()
        {
            //----------Object Linkages (I dont know why this is required since the linkages should be serialized but for some reason they break)

            outlineGameObjectsFolder = gameObject.transform.Find("Outline Folder").gameObject;
            thisOutline = outlineGameObjectsFolder.transform.Find("The Outline").gameObject;
            spriteOverlay = outlineGameObjectsFolder.transform.Find("Sprite Overlay").gameObject;
            clippingMask = outlineGameObjectsFolder.transform.Find("Sprite Mask").gameObject;

            //----------Variable Inits

            //---Var Inits from base outline class
            base.Reset();

            //---Main VEX vs CAVE Init (MUST be after hack inits)
            SpriteType_O = spriteType.conVEX; //"SpriteType_O" and the first value of the "spriteType" enum MUST NOT MATCH for things to work properly
                                              //NOTE: many variables are also initialized when this is set

            //---Clipping Mask Vars
            ClipCenter_CM = true;

            //---Outline Vars
            Active_O = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size
            Color_O = Color.red;
            OrderInLayer_O = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
            ScaleWithParentX_O = true;
            ScaleWithParentY_O = true;

            /*
             * These should be set only spriteType
            Size_O = .15f;

            //-----conCAVE Type Vars
            PatternType_O_CAVE = pushPattern.radial;
            EdgeCount_O_CAVE_R = 8;
            StdSize_O_CAVE = true;
            Rotation_O_CAVE = 0;
            */
        }

        //--------------------------------------------------SLIGHTLY DIFFERENT CODE--------------------------------------------------

        new void Update()
        {
            if (_awakeFinished)
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
                if (_newEdgeCount)
                {
                    updateEdgeCountCAVE();
                    updateEdgeRotationsCAVE();
                    _newEdgeCount = false;
                }

                if (_newVEXinit || _newCAVEinit)
                {
                    if (_newVEXinit && _newCAVEinit)
                        print("ERROR... i cant set the outline to both");
                    else
                    {
                        if (_newVEXinit)
                        {
                            print("**********VEX");

                            //--------------wipe out conCAVE outline
                            spriteType_O = spriteType.conCAVE; //DIRECT SETTING... be careful [nothing]
                            patternType_O_CAVE = pushPattern.radial; //DIRECT SETTING... be careful [nothing]
                            edgeCount_O_CAVE_R = 0; //DIRECT SETTING... be careful (spriteType_O MUST be conCAVE for this to run) [nothing]
                            updateEdgeCountCAVE();

                            //--------------spawn in conVEX outline
                            _0Rotation = Vector3.zero;
                            edgeCount_O_CAVE_R = 1; //DIRECT SETTING... be careful (spriteType_O MUST be conCAVE for this to run) [nothing]
                            updateEdgeCountCAVE();
                            _0Rotation = Vector3.right;

                            //---------------set the new value
                            spriteType_O = spriteType.conVEX; //DIRECT SETTING... be careful [nothing]

                            //---------------initialize variables
                            Size_O = 2f;

                            print("we should now be VEX " + SpriteType_O.ToString());

                            print("**********VEX");
                        }
                        else
                        {
                            print("**********CAVE");

                            //--------------wipe out conVEX outline
                            spriteType_O = spriteType.conCAVE; //DIRECT SETTING... be careful [nothing]
                            patternType_O_CAVE = pushPattern.radial; //DIRECT SETTING... be careful [nothing]
                            edgeCount_O_CAVE_R = 0; //spriteType_O MUST be conCAVE for this to run
                            updateEdgeCountCAVE();
                            print("edgecount " + outlineEdges.Count);

                            //--------------spawn in conCAVE outline
                            edgeCount_O_CAVE_R = 8; //spriteType_O MUST be conCAVE for this to run
                            updateEdgeCountCAVE();
                            print("edgecount " + outlineEdges.Count);

                            //---------------initialize variables
                            Size_O = .25f;

                            PatternType_O_CAVE = pushPattern.radial;

                            //ONLY regular
                            EdgeCount_O_CAVE_R = 8;

                            //BOTH
                            StdSize_O_CAVE = true;
                            //---SIZE goes here in inspector
                            Rotation_O_CAVE = 0;

                            print("we should now be CAVE " + SpriteType_O.ToString());

                            print("**********CAVE");
                        }
                    }

                    //reset vars
                    _newVEXinit = false;
                    _newCAVEinit = false;
                }
                else //both FALSE
                    print("sprite type " + SpriteType_O.ToString());

                //required hack because of warnings
                if (_newActiveCM)
                {
                    clippingMask.GetComponent<SpriteMask>().enabled = clipCenter_CM;
                    _newActiveCM = false;
                }

                if (_newActiveO)
                {
                    //all edges set active
                    if (outlineEdges != null)
                        foreach (var pair in outlineEdges)
                            pair.go.SetActive(active_O);
                }

                base.Update();
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
                foreach (var pair in outlineEdges)
                    copySpriteRendererData(this.GetComponent<SpriteRenderer>(), pair.go.GetComponent<SpriteRenderer>());
        }

        //--------------------------------------------------SUPER DIFFERENT CODE--------------------------------------------------

        //-------------------------ONLY conVEX-------------------------

        //TODO... addd updateOutlineVEX()

        //-------------------------ONLY conCAVE-------------------------

        //-------------------------ONLY RADIAL

        void updateEdgeCountCAVE()
        {
            if (outlineEdges != null)
            {
                if(SpriteType_O == spriteType.conCAVE)
                {
                    if (PatternType_O_CAVE == pushPattern.radial)
                    {
                        if (outlineEdges.Count != EdgeCount_O_CAVE_R)
                        {
                            int totalDifferences = Mathf.Abs(EdgeCount_O_CAVE_R - outlineEdges.Count);

                            if (outlineEdges.Count < EdgeCount_O_CAVE_R)
                            {
                                for (int i = 0; i < totalDifferences; i++)
                                    addEdge(_0Rotation, true);
                            }
                            else
                            {
                                for (int i = 0; i < totalDifferences; i++)
                                    removeEdge(outlineEdges[outlineEdges.Count - 1].go, true); //remove from the back
                            }
                        }
                        //ELSE... we have the correct number of edges
                    }
                    //ELSE... follow custom outline rules
                }
                //ELSE... this function is should not be running in conVEX mode
            }
            //ELSE... no outline to update
        }

        //UPDATE ---ROTATIONS--- BASED ON PATTERN
        void updateEdgeRotationsCAVE() //AUTOMATICALLY... calls updateEdgePositions()
        {
            if (outlineEdges != null)
            {
                if(SpriteType_O == spriteType.conCAVE)
                {
                    if (PatternType_O_CAVE == pushPattern.radial)
                    {
                        float edgeRotation = 0;

                        float angleBetweenAllEdges = (EdgeCount_O_CAVE_R == 0) ? 0 : 360 / (float)EdgeCount_O_CAVE_R;
                        foreach (var pair in outlineEdges)
                        {
                            float oldMag = pair.v2.magnitude;
                            Vector3 newDirection = Quaternion.AngleAxis(edgeRotation, Vector3.forward) * _0Rotation;
                            editEdge(pair.go, newDirection.normalized * oldMag, true); //this will also updateEdgePosition()
                            edgeRotation += angleBetweenAllEdges;
                        }
                    }
                    //ELSE... use custom outline rules
                }
                //ELSE.... this function should not be running
            }
            //ELSE... no outline to modify yet
        }

        //-------------------------FOR BOTH pushPatterns

        //USES... same as "updateEdgePosition()"
        void updateOutlineCAVE()
        {
            if(SpriteType_O == spriteType.conCAVE)
            {
                if (outlineEdges != null)
                {
                    foreach (var pair in outlineEdges)
                        updateEdgePositionCAVE(pair.go, pair.v2);
                }
                //ELSE... the outline doesnt yet exist
            }
            //ELSE... this function should not be running
        }

        //USES...
        //(PushType_OP == regular)... use (PushPattern_OPR)
        //(PushType_OP == custom)... use (StdSize_OPC) 
        //(PushType_OP == either)... use (Size_O) (ScaleWithParentX_O) (ScaleWithParentY_O)

        //UPDATE ---MAGNITUDE--- BASED ON PATTERN
        void updateEdgePositionCAVE(GameObject anEdge, Vector2 vect)
        {
            if (outlineEdges != null && outEdgesHelper != null && anEdge != null)
            {
                if(SpriteType_O == spriteType.conCAVE)
                {
                    outlineEdges[outEdgesHelper[anEdge]].v2 = vect;

                    //NOTE: we push from origin (0,0,0)

                    Vector3 newVect = Vector3.zero;
                    if (PatternType_O_CAVE == pushPattern.radial)
                    {
                        if (StdSize_O_CAVE)
                            newVect = vect.normalized * Size_O;
                        else
                            newVect = vect.normalized;
                    }
                    else
                    {
                        if (StdSize_O_CAVE) //STANDARD size for all vectors
                            newVect = vect.normalized * Size_O; //use ONLY vector (1) direction
                        else
                            newVect = vect; //use ONLY vector (1) direction (2) magnitude
                    }

                    //Rotate the Edges
                    float oldMagnitude = newVect.magnitude;
                    Vector3 newDirection = Quaternion.AngleAxis(Rotation_O_CAVE, Vector3.forward) * newVect.normalized;
                    newVect = newDirection.normalized * oldMagnitude;

                    //Apply the newVect Value
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
                //ELSE... this function should not be running
            }
        }

        //-------------------------Outline Edge List Edits-------------------------
        //So... these are used by BOTH (1) regular and (2) custom patterns
        //So... don't update the outline as a whole... 
        //the function that called these GIVEN REGULAR PATTERN should call the functions required to update the outline or pattern as a whole

        //-------------------------PRIVATE

        //USES... PushType_OP | Color_O | orderInLayer_O | ClipCenter_CM | Active_O
        bool addEdge(Vector2 outlineDirection, bool sudo)
        {
            if (outlineEdges != null && outEdgesHelper != null && thisOutline != null)
            {
                if (PatternType_O_CAVE == pushPattern.custom || sudo == true)
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
                    outEdgesHelper.Add(tempSpriteCopy, outlineEdges.Count);
                    outlineEdges.Add(new GO_to_Vector2(tempSpriteCopy, outlineDirection));

                    //update position that is affected by... size, scale par x, scale par y
                    updateEdgePositionCAVE(tempSpriteCopy, outlineDirection);

                    //set active state
                    tempSpriteCopy.SetActive(Active_O);

                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        bool removeEdge(GameObject edgeGO, bool sudo)
        {
            if (outlineEdges != null && outEdgesHelper != null && edgeGO != null)
            {
                if (PatternType_O_CAVE == pushPattern.custom || sudo == true)
                {
                    if (outEdgesHelper.ContainsKey(edgeGO))
                    {
                        outlineEdges.RemoveAt(outEdgesHelper[edgeGO]);
                        outEdgesHelper.Remove(edgeGO);

                        DestroyImmediate(edgeGO);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        bool editEdge(GameObject edgeGO, Vector2 newDirection, bool sudo)
        {
            if (outEdgesHelper != null && edgeGO != null)
            {
                if (PatternType_O_CAVE == pushPattern.custom || sudo == true)
                {
                    if (outEdgesHelper.ContainsKey(edgeGO))
                    {
                        updateEdgePositionCAVE(edgeGO, newDirection);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        //-------------------------PUBLIC (all of these are bool simply so the user knows whether or not a particular operation was successfull)

        public bool addEdge(Vector2 outlineDirection) //you WONT BE ABLE to use me unless your (PushType == push.customPattern)
        {
            return addEdge(outlineDirection, false);
        }

        public bool removeEdge(GameObject edgeGO) //you WONT BE ABLE to use me unless your (PushType == push.customPattern)
        {
            return removeEdge(edgeGO, false);
        }

        public bool removeAllEdges()
        {
            if (outlineEdges != null)
            {
                for(int i=0; i<outlineEdges.Count; i++)
                    removeEdge(outlineEdges[i].go);
                return true;
            }
            return
                false;
        }

        public bool editEdgeMagnitude(GameObject edgeGO, float newMag)
        {
            if (outlineEdges != null && outEdgesHelper != null && edgeGO != null)
            {
                editEdge(edgeGO, outlineEdges[outEdgesHelper[edgeGO]].v2.normalized * newMag);
                return true;
            }
            else
                return false;
        }

        public bool editEdge(GameObject edgeGO, Vector2 newDirection) //you WONT BE ABLE to use me unless your (PushType == push.customPattern)
        {
            return editEdge(edgeGO, newDirection, false);
        }
    }
}