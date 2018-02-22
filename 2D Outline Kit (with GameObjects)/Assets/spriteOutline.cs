using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object2DOutlines
{
    [System.Serializable, ExecuteInEditMode]
    public class spriteOutline : outline
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

        //NOTE: any function that ends in VEX... will not run if unless your "SpriteType_O" == conVEX
        //ditto for any function that ends in CAVE

        //NOTE: initially set SpriteType_O runs once from Awake -> sets our type to conVEX
        //then runs again from OnValidate() -> new set is ignored
        //but for reasons unkonw the first time it changes our type... it doesnt run convex in udpdate hack


        [SerializeField, HideInInspector]
        private bool _awakeFinished;

        void OnValidate()
        {
            if (_awakeFinished)
            {
                //Optimization
                UpdateSprite = updateSprite;

                //Debugging
                ShowOutline_GOs_InHierarchy = showOutline_GOs_InHierarchy;

                //Sprite Overlay
                Active_SO = active_SO;
                OrderInLayer_SO = orderInLayer_SO;
                Color_SO = color_SO;

                //Clipping Mask
                ClipCenter_CM = clipCenter_CM;
                AlphaCutoff_CM = alphaCutoff_CM;
                CustomRange_CM = customRange_CM;
                FrontLayer_CM = frontLayer_CM;
                BackLayer_CM = backLayer_CM;

                //Sprite Outline
                Active_O = active_O;
                Color_O = color_O;
                OrderInLayer_O = orderInLayer_O;
                ScaleWithParentX_O = scaleWithParentX_O;
                ScaleWithParentY_O = scaleWithParentY_O;
                SpriteType_O = spriteType_O;
                Size_O = size_O;

                //-----conCAVE

                PushType_O_CAVE = pushType_O_CAVE;
                StdSize_O_CAVE = stdSize_O_CAVE;

                //ONLY regular
                EdgeCount_O_CAVE_R = edgeCount_O_CAVE_R;
                StartAngle_O_CAVE_R = startAngle_O_CAVE_R;
                PushPattern_O_CAVE_R = pushPattern_O_CAVE_R;

                //ONLY regular && squarial
                RectSize_O_CAVE_RS = rectSize_O_CAVE_RS;
                RectWidth_O_CAVE_RS = rectWidth_O_CAVE_RS;
                RectHeight_O_CAVE_RS = rectHeight_O_CAVE_RS;
            }
        }

        //-----Clipping Mask Variables

        [Header("CLIPPING MASK VARIABLES-----")]
        [SerializeField, HideInInspector]
        bool clipCenter_CM;
        public bool ClipCenter_CM
        {
            get { return clipCenter_CM; }
            set
            {
                clipCenter_CM = value;

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

        [Header("OUTLINE VARIABLES-----")]
        [SerializeField, HideInInspector]
        bool active_O;
        public bool Active_O
        {
            get { return active_O; }
            set
            {
                active_O = value;

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
                color_O = value;

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
                orderInLayer_O = value;

                //update our edges with the new color
                if (outlineEdges != null)
                    foreach (var GameObject in outlineEdges.Keys)
                        GameObject.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
            }
        }

        [SerializeField, HideInInspector]
        bool scaleWithParentX_O;
        public bool ScaleWithParentX_O
        {
            get { return scaleWithParentX_O; }
            set
            {
                scaleWithParentX_O = value;

                //the one that is currently active will run
                updateOutlineVEX();
                updateOutlineCAVE();
            }
        }

        [SerializeField, HideInInspector]
        bool scaleWithParentY_O;
        public bool ScaleWithParentY_O
        {
            get { return scaleWithParentY_O; }
            set
            {
                scaleWithParentY_O = value;

                //the one that is currently active will run
                updateOutlineVEX();
                updateOutlineCAVE();
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
                        newVEXinit = true;
                    else
                        newCAVEinit = true;
                }
                else
                    print("it was equal");
            }
        }
        [SerializeField, HideInInspector]
        private bool newVEXinit;
        [SerializeField, HideInInspector]
        private bool newCAVEinit;

        //---CONCAVE and CONVEX use this differently

        [SerializeField, HideInInspector]
        float size_O; //NOTE: this size refers to the world space thickness of the outline
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public float Size_O
        {
            get { return size_O; }
            set
            {
                value = (value >= 0) ? value : 0;
                size_O = value;

                //the one that is currently active will run
                updateOutlineVEX();
                updateOutlineCAVE();
            }
        }

        //-------------------------Push Type Variables-------------------------

        [SerializeField, HideInInspector]
        Dictionary<GameObject, Vector2> outlineEdges;

        [SerializeField, HideInInspector]
        private bool newEdgeCount;

        [Header("PUSH TYPE VARIABLES-----")]
        [SerializeField, HideInInspector]
        push pushType_O_CAVE;
        public push PushType_O_CAVE
        {
            get { return pushType_O_CAVE; }
            set
            {
                pushType_O_CAVE = value;

                newEdgeCount = true;
            }
        }

        [SerializeField, HideInInspector]
        bool stdSize_O_CAVE; //NOTE: this size refers to the world space thickness of the outline
        public bool StdSize_O_CAVE
        {
            get { return stdSize_O_CAVE; }
            set
            {
                stdSize_O_CAVE = value;

                updateOutlineCAVE();
            }
        }

        //---Regular

        [SerializeField, HideInInspector]
        private static Vector2 _0Rotation; //what we consider 0 rotation (current Vector3.right)

        [SerializeField, HideInInspector]
        int edgeCount_O_CAVE_R; //also the count of gameobjects that make up the outline
        public int EdgeCount_O_CAVE_R
        {
            get { return edgeCount_O_CAVE_R; }
            set
            {
                edgeCount_O_CAVE_R = (value >= 0) ? value : 0; //update local value

                newEdgeCount = true;
            }
        }

        [SerializeField, HideInInspector]
        float startAngle_O_CAVE_R;
        public float StartAngle_O_CAVE_R
        {
            get { return startAngle_O_CAVE_R; }
            set
            {
                startAngle_O_CAVE_R = value; //update local value

                updateOutlineCAVE();
            }
        }

        [SerializeField, HideInInspector]
        pushPattern pushPattern_O_CAVE_R; //push objs to edge of circle or to edge of box
        public pushPattern PushPattern_O_CAVE_R
        {
            get { return pushPattern_O_CAVE_R; }
            set
            {
                pushPattern_O_CAVE_R = value; //update local value

                updateEdgeRotationsCAVE();
            }
        }

        [SerializeField, HideInInspector]
        rectType rectSize_O_CAVE_RS;
        public rectType RectSize_O_CAVE_RS
        {
            get { return rectSize_O_CAVE_RS; }
            set
            {
                rectSize_O_CAVE_RS = value;

                //update height and width... which will update our outline
                if (rectSize_O_CAVE_RS == rectType.regular)
                {
                    RectWidth_O_CAVE_RS = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
                    RectHeight_O_CAVE_RS = gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
                }
            }
        }

        [SerializeField, HideInInspector]
        float rectWidth_O_CAVE_RS;
        public float RectWidth_O_CAVE_RS
        {
            get { return rectWidth_O_CAVE_RS; }
            set
            {
                rectWidth_O_CAVE_RS = value;

                updateEdgeRotationsCAVE();
            }
        }

        [SerializeField, HideInInspector]
        float rectHeight_O_CAVE_RS;
        public float RectHeight_O_CAVE_RS
        {
            get { return rectHeight_O_CAVE_RS; }
            set
            {
                rectHeight_O_CAVE_RS = value;

                updateEdgeRotationsCAVE();
            }
        }

        new void Awake()
        {
            _awakeFinished = false;

            if (gameObject.transform.Find("Outline Folder") == null)
            {
                _0Rotation = Vector3.right; //what we consider 0 rotation (current Vector3.right)

                //----------Objects Inits

                thisOutline = new GameObject("The Outline");
                Material tempMaterial = initPart1(gameObject, ref thisOutline, ref outlineGameObjectsFolder);
                DestroyImmediate(thisOutline.GetComponent<SpriteRenderer>()); //DIFFERENT
                initPart2(gameObject, ref outlineGameObjectsFolder, ref spriteOverlay, ref clippingMask, ref tempMaterial);

                outlineEdges = new Dictionary<GameObject, Vector2>(); //(MUST be before any inits)

                //----------Hack Inits

                newEdgeCount = false;
                newActiveCM = false;
                newActiveO = false;
                newCAVEinit = false;
                newVEXinit = false;

                //----------Main Init (MUST be after hack inits)
                SpriteType_O = spriteType.conVEX; //"SpriteType_O" and the first value of the "spriteType" enum MUST NOT MATCH for things to work properly
                //NOTE: many variables are also initialized when this is set

                //----------Variable Inits

                //---Clipping Mask Vars
                ClipCenter_CM = true;

                //---Outline Vars
                Active_O = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size
                Color_O = Color.red;
                OrderInLayer_O = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
                ScaleWithParentX_O = true;
                ScaleWithParentY_O = true;

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

            print("---------");

            _awakeFinished = true;
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
                updateEdgeCountCAVE();
                updateEdgeRotationsCAVE();
                newEdgeCount = false;
                print("---------");
            }

            //required hack
            if (newVEXinit)
            {
                print("**********");

                //--------------wipe out conCAVE outline
                spriteType_O = spriteType.conCAVE; //DIRECT SETTING... be careful [nothing]
                pushType_O_CAVE = push.regularPattern; //DIRECT SETTING... be careful [nothing]
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

                newVEXinit = false;

                print("we should now be VEX " + SpriteType_O.ToString());

                print("**********");
            }

            if (newCAVEinit)
            {
                print("**********");

                //---------------set the new value
                spriteType_O = spriteType.conCAVE; //DIRECT SETTING... be careful [nothing]

                //--------------wipe out conVEX outline
                EdgeCount_O_CAVE_R = 0; //spriteType_O MUST be conCAVE for this to run

                //--------------spawn in conCAVE outline

                //---------------initialize variables

                Size_O = .25f;

                PushType_O_CAVE = push.regularPattern; //NOTE: newEdgeCount next frame
                StdSize_O_CAVE = true;

                //ONLY regular
                EdgeCount_O_CAVE_R = 8;
                StartAngle_O_CAVE_R = 0;
                PushPattern_O_CAVE_R = pushPattern.squarial;

                //ONLY regular && squarial
                RectSize_O_CAVE_RS = rectType.regular;
                //width and height set by the above

                newCAVEinit = false;

                print("we should now be CAVE " + SpriteType_O.ToString());

                print("**********");
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
                    foreach (var GameObject in outlineEdges.Keys) //ALSO works for concave type (by simyply having 1 edge here)
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
                foreach (var GameObject in outlineEdges.Keys) //ALSO works for concave type (by simyply having 1 edge here)
                    copySpriteRendererData(this.GetComponent<SpriteRenderer>(), GameObject.GetComponent<SpriteRenderer>());
        }

        //--------------------------------------------------SUPER DIFFERENT CODE--------------------------------------------------

        //-------------------------ONLY conVEX-------------------------

        void updateOutlineVEX()
        {
            if (SpriteType_O == spriteType.conVEX)
            {
                if (outlineEdges != null)
                {
                    print("vex update START ???");

                    List<GameObject> GOs = new List<GameObject>(outlineEdges.Keys);
                    foreach (var GameObject in GOs) //NOTE: this should only run for 1 single "edge"
                    {
                        float localSizeX;
                        float localSizeY;

                        //REQUIRED because: when we want our size to be able to be == to 0
                        //when size is 0 there should essentially be a "solidly colored sprite behind our current sprite"
                        //without this adjustment to get a "solidly colored sprite behind our current sprite" our size needs to be 1
                        float adjustedSize = Size_O + 1;

                        if (ScaleWithParentX_O)
                            localSizeX = adjustedSize;
                        else
                            localSizeX = adjustedSize / this.transform.localScale.x;

                        if (ScaleWithParentY_O)
                            localSizeY = adjustedSize;
                        else
                            localSizeY = adjustedSize / this.transform.localScale.y;

                        GameObject.transform.localScale = new Vector3(localSizeX, localSizeY, 1);
                    }

                    print("vex update END ???");
                }
                //ELSE... the outline doesnt yet exist
            }
            //ELSE... this function should not be running
        }

        //-------------------------ONLY conCAVE-------------------------

        //-------------------------ONLY PushType == push.regularPattern

        void updateEdgeCountCAVE()
        {
            if (SpriteType_O == spriteType.conCAVE)
            {
                if (PushType_O_CAVE == push.regularPattern)
                {
                    if (outlineEdges != null) //precautionary check
                    {
                        print("cave update START ???");

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
                                {
                                    List<GameObject> keyList = new List<GameObject>(outlineEdges.Keys);
                                    removeEdge(keyList[keyList.Count - 1], true); //remove from the back
                                }
                            }
                        }
                        //ELSE... we have the correct number of edges

                        print("cave update END ???");
                    }
                    //ELSE... the outline doesnt yet exist
                }
                //ELSE... follow custom outline rules
            }
            //ELSE... this function should not be running
        }

        //UPDATE ---ROTATIONS--- BASED ON PATTERN
        void updateEdgeRotationsCAVE() //AUTOMATICALLY... calls updateEdgePositions()
        {
            if (SpriteType_O == spriteType.conCAVE)
            {
                if (PushType_O_CAVE == push.regularPattern)
                {
                    if (outlineEdges != null) //precautionary check
                    {
                        //StartAngle_OPR is used to rotate the shape after this step
                        float edgeRotation = (PushPattern_O_CAVE_R == pushPattern.radial) ?
                            0 //for RADIAL... start at _0Rotation
                            : (Mathf.Rad2Deg * Mathf.Atan2((RectHeight_O_CAVE_RS / 2), (RectWidth_O_CAVE_RS / 2))); //for SQUARIAL... start by placing the first edge in the first corner (1st quadrant)
                        float angleBetweenAllEdges = (EdgeCount_O_CAVE_R == 0) ? 0 : 360 / (float)EdgeCount_O_CAVE_R;

                        List<GameObject> GOs = new List<GameObject>(outlineEdges.Keys);
                        foreach (var GameObject in GOs)
                        {
                            float oldMagnitude = outlineEdges[GameObject].magnitude;
                            Vector3 newDirection = Quaternion.AngleAxis(edgeRotation, Vector3.forward) * _0Rotation;
                            editEdge(GameObject, newDirection.normalized * oldMagnitude, true); //this will also updateEdgePosition()
                            edgeRotation += angleBetweenAllEdges;
                        }
                    }
                    //ELSE... the outline doesnt yet exist
                }
                //ELSE... follow custom outline rules
            }
            //ELSE... this function should not be running
        }

        //-------------------------FOR BOTH PushTypes

        //USES... same as "updateEdgePosition()"
        void updateOutlineCAVE()
        {
            if (SpriteType_O == spriteType.conCAVE)
            {
                if (outlineEdges != null)
                {
                    List<GameObject> GOs = new List<GameObject>(outlineEdges.Keys);
                    foreach (var GameObject in GOs)
                        updateEdgePositionCAVE(GameObject, outlineEdges[GameObject]);
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
            if (SpriteType_O == spriteType.conCAVE)
            {
                if (outlineEdges != null)
                {
                    outlineEdges[anEdge] = vect;

                    //NOTE: we push from origin (0,0,0)

                    Vector3 newVect = Vector3.zero;
                    if (PushType_O_CAVE == push.regularPattern)
                    {
                        if (StdSize_O_CAVE)
                        {
                            if (PushPattern_O_CAVE_R == pushPattern.radial) //Radial Push
                                newVect = vect.normalized * Size_O;
                            else //Squarial Push
                            {
                                //do this pretending 
                                //that the Rect Width length goes from -x to x
                                //and that the Rect Length goes from -y to y

                                float cornerAngle = (Mathf.Rad2Deg * Mathf.Atan2((RectHeight_O_CAVE_RS / 2), (RectWidth_O_CAVE_RS / 2))); //top right corner
                                //TODO... might have to repair below with conversion into degree
                                float cornerHypotenuse = (RectHeight_O_CAVE_RS / 2) / Mathf.Sin(cornerAngle * Mathf.Deg2Rad);

                                float AF_Up = Vector3.Angle(Vector3.up, vect.normalized);
                                float AF_Right = Vector3.Angle(Vector3.right, vect.normalized);
                                float AF_Down = Vector3.Angle(Vector3.down, vect.normalized);
                                float AF_Left = Vector3.Angle(Vector3.left, vect.normalized);

                                float newMagnitude = 0;

                                //--find what quadrant this outline is in
                                if (AF_Left > 90 && AF_Down > 90) //1st quad
                                {
                                    if (AF_Right < cornerAngle) //base ourselves on width
                                        newMagnitude = (RectWidth_O_CAVE_RS / 2) / (Mathf.Cos(AF_Right * Mathf.Deg2Rad));
                                    else if (AF_Right > cornerAngle) //base ourselves on height
                                        newMagnitude = (RectHeight_O_CAVE_RS / 2) / (Mathf.Cos((90 - AF_Right) * Mathf.Deg2Rad));
                                    else
                                        newMagnitude = cornerHypotenuse;
                                }
                                else if (AF_Right > 90 && AF_Down > 90) //2nd quad
                                {
                                    if (AF_Left < cornerAngle) //base ourselves on width
                                        newMagnitude = (RectWidth_O_CAVE_RS / 2) / (Mathf.Cos(AF_Left * Mathf.Deg2Rad));
                                    else if (AF_Left > cornerAngle) //base ourselves on height
                                        newMagnitude = (RectHeight_O_CAVE_RS / 2) / (Mathf.Cos((90 - AF_Left) * Mathf.Deg2Rad));
                                    else
                                        newMagnitude = cornerHypotenuse;
                                }
                                else if (AF_Right > 90 && AF_Up > 90) //3rd quad
                                {
                                    if (AF_Left < cornerAngle) //base ourselves on width
                                        newMagnitude = (RectWidth_O_CAVE_RS / 2) / (Mathf.Cos(AF_Left * Mathf.Deg2Rad));
                                    else if (AF_Left > cornerAngle) //base ourselves on height
                                        newMagnitude = (RectHeight_O_CAVE_RS / 2) / (Mathf.Cos((90 - AF_Left) * Mathf.Deg2Rad));
                                    else
                                        newMagnitude = cornerHypotenuse;
                                }
                                else //4th quad
                                {
                                    if (AF_Right < cornerAngle) //base ourselves on width
                                        newMagnitude = (RectWidth_O_CAVE_RS / 2) / (Mathf.Cos(AF_Right * Mathf.Deg2Rad));
                                    else if (AF_Right > cornerAngle) //base ourselves on height
                                        newMagnitude = (RectHeight_O_CAVE_RS / 2) / (Mathf.Cos((90 - AF_Right) * Mathf.Deg2Rad));
                                    else
                                        newMagnitude = cornerHypotenuse;
                                }

                                newVect = vect.normalized * newMagnitude;

                                newVect = newVect.normalized * (newVect.magnitude * Size_O);
                            }
                        }
                        else
                            newVect = vect.normalized;

                        //Rotate the RADIAL or SQUARIAL pattern the number of StartAngle_OPR
                        float oldMagnitude = newVect.magnitude;
                        Vector3 newDirection = Quaternion.AngleAxis(StartAngle_O_CAVE_R, Vector3.forward) * newVect.normalized;
                        newVect = newDirection.normalized * oldMagnitude;
                    }
                    else
                    {
                        if (StdSize_O_CAVE) //STANDARD size for all vectors
                            newVect = vect.normalized * Size_O; //use ONLY vector (1) direction
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
                //ELSE... the outline doesnt yet exist
            }
            //ELSE... this function should not be running
        }

        //-------------------------Outline Edge List Edits-------------------------
        //So... these are used by BOTH (1) regular and (2) custom patterns
        //So... don't update the outline as a whole... 
        //the function that called these GIVEN REGULAR PATTERN should call the functions required to update the outline or pattern as a whole

        //-------------------------PRIVATE

        //USES... PushType_OP | Color_O | orderInLayer_O | ClipCenter_CM | Active_O
        bool addEdge(Vector2 outlineDirection, bool sudo)
        {
            if (PushType_O_CAVE == push.customPattern || sudo == true)
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
                    updateEdgePositionCAVE(tempSpriteCopy, outlineDirection);

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
            if (PushType_O_CAVE == push.customPattern || sudo == true)
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

        void removeAllEdges()
        {
            if (outlineEdges != null)
            {
                List<GameObject> GOs = new List<GameObject>(outlineEdges.Keys);
                foreach (var GameObject in GOs)
                    removeEdge(GameObject);
            }
            //ELSE... the outline doesnt yet exist
        }

        //USES... PushType_OP
        bool editEdge(GameObject edgeGO, Vector2 newDirection, bool sudo)
        {
            if (PushType_O_CAVE == push.customPattern || sudo == true)
            {
                if (outlineEdges != null) //precautionary check
                {
                    if (outlineEdges.ContainsKey(edgeGO))
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
                    updateEdgePositionCAVE(edgeGO, outlineEdges[edgeGO].normalized * newMag);

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