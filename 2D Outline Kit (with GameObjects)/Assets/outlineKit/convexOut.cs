using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object2DOutlines
{
    [System.Serializable, ExecuteInEditMode]
    public class convexOut : outline
    {
        [System.NonSerialized]
        private bool _awakeFinished_VEX; //SHOULD NOT be serialized... if it is... OnValidate will run before it should

        [SerializeField]
        private bool notFirstRun; //NOTE: this releis on the fact that DEFAULT bool value is FALSE

        void OnValidate()
        {
            if (_awakeFinished_VEX)
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
                Size_O = size_O;
                ScaleWithParentX_O = scaleWithParentX_O;
                ScaleWithParentY_O = scaleWithParentY_O;
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
                    thisOutline.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                else
                    thisOutline.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
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

                _newActiveO = true; //hack in update
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

                thisOutline.GetComponent<SpriteRenderer>().color = color_O;
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

                thisOutline.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
            }
        }

        [SerializeField, HideInInspector]
        float size_O;
        public float Size_O
        {
            get { return size_O; }
            set
            {
                value = (value >= 0) ? value : 0;
                size_O = value; 

                updateOutline();
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

                updateOutline();
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

                updateOutline();
            }
        }

        void Awake()
        {
            _awakeFinished_VEX = false;

            //---Hacks Inits
            _newActiveCM = false;
            _newActiveO = false;

            if (notFirstRun == false) //NOTE: will directly search children
            {
                //----------Objects Inits

                thisOutline = new GameObject("The Outline");
                Material tempMaterial = initPart1(gameObject, ref thisOutline, ref outlineGameObjectsFolder);
                thisOutline.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial; //DIFFERENT
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), thisOutline.GetComponent<SpriteRenderer>());//DIFFERENT
                initPart2(gameObject, ref outlineGameObjectsFolder, ref spriteOverlay, ref clippingMask, ref tempMaterial);

                ManualReset();

                notFirstRun = true;
            }
            else
            {
                //----------Some Strange Bug Fix

                //NOTE: for reasons unknown our gameobjects would lose their materials after spawning a prefab where the original used to create it had the material
                var tempMaterial = new Material(gameObject.GetComponent<SpriteRenderer>().sharedMaterial);
                tempMaterial.shader = Shader.Find("GUI/Text Shader");

                spriteOverlay.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;
                thisOutline.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;
            }

            _awakeFinished_VEX = true;
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

            Size_O = 2f;
        }

        //--------------------------------------------------SLIGHTLY DIFFERENT CODE--------------------------------------------------

        new void Update()
        {
            if (_awakeFinished_VEX)
            {
                switch (UpdateSprite)
                {
                    case spriteUpdateSetting.EveryFrame: updateSpriteData(); break;
                    case spriteUpdateSetting.AfterEveryChange:
                        if (spriteChanged(this.GetComponent<SpriteRenderer>()))
                            updateSpriteData();
                        break;
                }

                //required hacks because of warnings
                if (_newActiveCM)
                {
                    clippingMask.GetComponent<SpriteMask>().enabled = clipCenter_CM;
                    _newActiveCM = false;
                }

                if (_newActiveO)
                {
                    thisOutline.SetActive(active_O);
                    _newActiveO = false;
                }

                base.Update();
            }
        }

        void OnDisable()
        {
            print("disabled");
        }

        void OnDestroy()
        {
            print("destroy");

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
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), thisOutline.GetComponent<SpriteRenderer>());
        }

        //--------------------------------------------------SUPER DIFFERENT CODE--------------------------------------------------

        void updateOutline()
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

            thisOutline.transform.localScale = new Vector3(localSizeX, localSizeY, 1);
        }
    }
}