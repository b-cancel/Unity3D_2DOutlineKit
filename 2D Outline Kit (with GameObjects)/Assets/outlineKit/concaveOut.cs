using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object2DOutlines
{
    [System.Serializable, ExecuteInEditMode]
    public class concaveOut : outline
    {
        [System.NonSerialized]
        private bool _awakeFinished_CAVE; //SHOULD NOT be serialized... if it is... OnValidate will run before it should

        [SerializeField]
        private bool notFirstRun; //NOTE: this relies on the fact that DEFAULT bool value is FALSE

        void OnValidate()
        {
            if (_awakeFinished_CAVE)
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

                Size_O = size_O;

                //-----conCAVE
                EdgeCount_O_CAVE_R = edgeCount_O_CAVE_R;
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
                clipCenter_CM = value;

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
                active_O = value;

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
                color_O = value;

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
                orderInLayer_O = value;

                //update our edges with the new order in layer
                if (outlineEdges != null)
                    foreach (var pair in outlineEdges)
                        pair.go.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
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

                _newEdgeCount = true;
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

                _newEdgeCount = true;
            }
        }

        [SerializeField, HideInInspector]
        float size_O; //NOTE: this size refers to the world space thickness of the outline
        public float Size_O
        {
            get { return size_O; }
            set
            {
                value = (value >= 0) ? value : 0;
                size_O = value;

                _newEdgeCount = true;
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
        [Header("PUSH TYPE VARIABLES-----")]
        [SerializeField, HideInInspector]
        private Vector2 _0Rotation; //what we consider 0 rotation (current Vector3.right)

        [SerializeField, HideInInspector]
        int edgeCount_O_CAVE_R; //also the count of gameobjects that make up the outline
        public int EdgeCount_O_CAVE_R
        {
            get { return edgeCount_O_CAVE_R; }
            set
            {
                edgeCount_O_CAVE_R = (value >= 0) ? value : 0;

                _newEdgeCount = true;
            }
        }

        [SerializeField, HideInInspector]
        float rotation_O_CAVE;
        public float Rotation_O_CAVE
        {
            get { return rotation_O_CAVE; }
            set
            {
                rotation_O_CAVE = value; 

                _newEdgeCount = true;
            }
        }

        void Awake()
        {
            _awakeFinished_CAVE = false;

            //---Hacks Inits (dont need serialization and you will never be fast enough to hit play unless they are false)
            _newEdgeCount = false;
            _newActiveCM = false;
            _newActiveO = false;

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

                ManualReset();

                notFirstRun = true;
            }
            else
            {
                outEdgesHelper = new Dictionary<GameObject, int>(); //this is required again since dictionaries are not serializable
                for (int i = 0; i < outlineEdges.Count; i++)
                    outEdgesHelper.Add(outlineEdges[i].go, i);

                //----------Some Strange Bug Fix

                //NOTE: for reasons unknown our gameobjects would lose their materials after spawning a prefab where the original used to create it had the material
                var tempMaterial = new Material(gameObject.GetComponent<SpriteRenderer>().sharedMaterial);
                tempMaterial.shader = Shader.Find("GUI/Text Shader");

                spriteOverlay.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;

                foreach(var pair in outlineEdges)
                    pair.go.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;
            }

            _awakeFinished_CAVE = true;
        }

        //assign good values to inspector
        public new void ManualReset()
        {
            //----------Variable Inits

            //---Var Inits from base outline class
            base.ManualReset();

            //---Clipping Mask Vars
            ClipCenter_CM = true;

            //---Outline Vars
            Active_O = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size
            Color_O = Color.red;
            OrderInLayer_O = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
            ScaleWithParentX_O = true;
            ScaleWithParentY_O = true;

            Size_O = .15f;

            //-----conCAVE Type Vars
            EdgeCount_O_CAVE_R = 8;
            Rotation_O_CAVE = 0;
        }

        //--------------------------------------------------SLIGHTLY DIFFERENT CODE--------------------------------------------------

        new void Update()
        {
            if (_awakeFinished_CAVE)
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
                    updateEdgeCount();
                    updateEdgeRotationsALL();
                    _newEdgeCount = false;
                }

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

        void OnDisable()
        {
        }

        void OnDestroy()
        {
            DestroyImmediate(outlineGameObjectsFolder);

            //TODO... here we also break our relationship with our parent
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

        //-------------------------ONLY PUSH TYPE OUTLINE-------------------------

        void updateEdgeCount()
        {
            if (outlineEdges != null) 
            {
                if (outlineEdges.Count != EdgeCount_O_CAVE_R)
                {
                    int totalDifferences = Mathf.Abs(EdgeCount_O_CAVE_R - outlineEdges.Count);

                    if (outlineEdges.Count < EdgeCount_O_CAVE_R)
                    {
                        for (int i = 0; i < totalDifferences; i++)
                            addEdge(_0Rotation);
                    }
                    else
                    {
                        for (int i = 0; i < totalDifferences; i++)
                            removeEdge(outlineEdges[outlineEdges.Count - 1].go); //remove from the back
                    }
                }
                //ELSE... we have the correct number of edges
            }
            //ELSE... no outline to update
        }

        //UPDATE ---ROTATIONS--- 
        void updateEdgeRotationsALL() //AUTOMATICALLY... calls updateEdgePositions()
        {
            if (outlineEdges != null)
            {
                float edgeRotation = 0;

                float angleBetweenAllEdges = (EdgeCount_O_CAVE_R == 0) ? 0 : 360 / (float)EdgeCount_O_CAVE_R;
                foreach (var pair in outlineEdges)
                {
                    float oldMag = pair.v2.magnitude;
                    Vector3 newDirection = Quaternion.AngleAxis(edgeRotation, Vector3.forward) * _0Rotation;
                    editEdge(pair.go, newDirection.normalized * oldMag); //this will also updateEdgePosition()
                    edgeRotation += angleBetweenAllEdges;
                }
            }
        }

        //-------------------------FOR BOTH PushTypes

        //USES... same as "updateEdgePosition()"
        void updateEdgePositionsALL()
        {
            if (outlineEdges != null)
            {
                foreach (var pair in outlineEdges)
                    updateEdgePosition(pair.go, pair.v2);
            }
        }

        //UPDATE ---MAGNITUDE--- 
        void updateEdgePosition(GameObject anEdge, Vector2 vect)
        {
            if (outlineEdges != null && outEdgesHelper != null && anEdge != null) 
            {
                outlineEdges[outEdgesHelper[anEdge]].v2 = vect;

                //NOTE: we push from origin (0,0,0)

                Vector3 newVect = vect.normalized * Size_O;

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
        }

        //-------------------------Outline Edge List Edits-------------------------

        //-------------------------PRIVATE

        //USES... PushType_OP | Color_O | orderInLayer_O | ClipCenter_CM | Active_O
        bool addEdge(Vector2 outlineDirection)
        {
            if (outlineEdges != null && outEdgesHelper != null && thisOutline != null)
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
                updateEdgePosition(tempSpriteCopy, outlineDirection);

                //set active state
                tempSpriteCopy.SetActive(Active_O);

                return true;
            }
            else
                return false;
        }

        bool removeEdge(GameObject edgeGO)
        {
            if (outlineEdges != null && outEdgesHelper != null && edgeGO != null)
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

        bool editEdge(GameObject edgeGO, Vector2 newDirection)
        {
            if (outEdgesHelper != null && edgeGO != null)
            {
                if (outEdgesHelper.ContainsKey(edgeGO))
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
    }
}