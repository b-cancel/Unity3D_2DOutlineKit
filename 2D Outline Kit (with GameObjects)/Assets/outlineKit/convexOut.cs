using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object2DOutlines
{
    [System.Serializable, ExecuteInEditMode]
    public class convexOut : outline
    {
        [System.NonSerialized]
        private bool awakeFinished_VEX; //SHOULD NOT be serialized... if it is... OnValidate will run before it should

        void OnValidate()
        {
            if (awakeFinished_VEX)
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
                clipCenter_CM = value; //update local value

                //enable or disable mask
                newActiveCM = true;

                //update how our edge gameobjects interact with the mask
                if (clipCenter_CM == true)
                    thisOutline.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                else
                    thisOutline.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }
        } //NOTE: used in update function... doesnt have to do anyting special for get and set...
        [System.NonSerialized]
        private bool newActiveCM;

        //-----Outline Variables-----

        [SerializeField, HideInInspector]
        GameObject thisOutline;

        [Space(10)]
        [Header("OUTLINE VARIABLES-----")]
        [SerializeField, HideInInspector]
        bool active_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool Active_O
        {
            get { return active_O; }
            set
            {
                active_O = value; //update local value

                newActiveO = true; //hack in update
            }
        }
        [System.NonSerialized]
        private bool newActiveO;

        [SerializeField, HideInInspector]
        Color color_O;
        public Color Color_O
        {
            get { return color_O; }
            set
            {
                color_O = value;//update local value

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
                orderInLayer_O = value;//update local value

                thisOutline.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
            }
        }

        [SerializeField, HideInInspector]
        float size_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public float Size_O
        {
            get { return size_O; }
            set
            {
                value = (value >= 0) ? value : 0;
                size_O = value; //update local value 

                updateOutline();
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

                updateOutline();
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

                updateOutline();
            }
        }

        new void Awake()
        {
            awakeFinished_VEX = false;

            if (gameObject.transform.Find("Outline Folder") == null) //NOTE: will directly search children
            {
                //----------Objects Inits

                thisOutline = new GameObject("The Outline");
                Material tempMaterial = initPart1(gameObject, ref thisOutline, ref outlineGameObjectsFolder);
                thisOutline.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial; //DIFFERENT
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), thisOutline.GetComponent<SpriteRenderer>()); //DIFFERENT
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

                Size_O = 2f;

                //---Var Inits from base outline class
                base.Awake();
            }

            //---Hacks Inits (dont need serialization and you will never be fast enough to hit play unless they are false)
            newActiveCM = false;
            newActiveO = false;

            awakeFinished_VEX = true;
        }

        //--------------------------------------------------SLIGHTLY DIFFERENT CODE--------------------------------------------------

        new void Update()
        {
            if (awakeFinished_VEX)
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
                if (newActiveCM)
                {
                    clippingMask.GetComponent<SpriteMask>().enabled = clipCenter_CM;
                    newActiveCM = false;
                }

                if (newActiveO)
                {
                    thisOutline.SetActive(active_O);
                    newActiveO = false;
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