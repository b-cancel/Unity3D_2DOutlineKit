using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object2DOutlines
{
    [ExecuteInEditMode]
    public class convexOut : outline //this extends monobehavior because base extends monobehvaior
    {
        private bool awakeFinished_VEX;

        void OnValidate()
        {
            if (awakeFinished_VEX)
            {
                //Optimization
                UpdateSpriteEveryFrame = updateSpriteEveryFrame;

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
            }
        }

        //-----Clipping Mask Variables

        [Space(10)]
        [Header("Clipping Mask Variables")]
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
                    outline.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                else
                    outline.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }
        } //NOTE: used in update function... doesnt have to do anyting special for get and set...

        //-----Outline Variables-----

        GameObject outline;

        [Space(10)]
        [Header("Outline Variables")]
        [SerializeField, HideInInspector]
        bool active_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool Active_O
        {
            get { return active_O; }
            set
            {
                active_O = value;//update local value

                outline.SetActive(active_O);
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

                outline.GetComponent<SpriteRenderer>().color = color_O;
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

                outline.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
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
                value = (value >= .1f) ? value : .1f; //since our childrens' size depends on our porportion with them we avoid our size being 0
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

            //----------Cover Duplication Problem

            Transform psblOF_T = this.transform.Find("Outline Folder");
            if (psblOF_T != null) //transform found
            {
                GameObject psblOF_GO = psblOF_T.gameObject;
                if (psblOF_GO.transform.parent.gameObject == gameObject) //this gameobject ours
                    DestroyImmediate(psblOF_GO);
            }

            //----------Object Instantiation

            //-----Outline Folder [MUST BE FIRST]
            outlineGameObjectsFolder = new GameObject("Outline Folder");
            copyTransform(gameObject, outlineGameObjectsFolder);
            outlineGameObjectsFolder.transform.parent = this.transform;

            //-----Outline GameObject

            outline = new GameObject("The Outline");
            outline.transform.parent = outlineGameObjectsFolder.transform;
            copyTransform(gameObject, outline);
            outline.AddComponent<SpriteRenderer>();
            var tempMaterial = new Material(outline.GetComponent<SpriteRenderer>().sharedMaterial);
            tempMaterial.shader = Shader.Find("GUI/Text Shader");

            //different ABOVE
            outline.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial; //text shader to get silhouette of our sprite
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), outline.GetComponent<SpriteRenderer>());
            //different BELOW

            //-----Sprite Overlay
            spriteOverlay = new GameObject("Sprite Overlay");
            spriteOverlay.AddComponent<SpriteRenderer>();
            spriteOverlay.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;
            copyTransform(gameObject, spriteOverlay);
            spriteOverlay.transform.parent = outlineGameObjectsFolder.transform;
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

            //-----Sprite Mask
            clippingMask = new GameObject("Sprite Mask");
            clippingMask.AddComponent<SpriteMask>();
            copyTransform(gameObject, clippingMask);
            clippingMask.transform.parent = outlineGameObjectsFolder.transform;
            clippingMask.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite;

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

            base.Awake();

            awakeFinished_VEX = true;
        }

        //-------------------------UNIQUE CODE-------------------------

        void Update()
        {
            if (UpdateSpriteEveryFrame)
            {
                print("vex updating sprite every frame");

                //update sprite overlay
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

                //update clipping mask
                clippingMask.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite;

                //update outline
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), outline.GetComponent<SpriteRenderer>());
            }
        }

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

            outline.transform.localScale = new Vector3(localSizeX, localSizeY, 1);
        }
    }
}