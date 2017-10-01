using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace objOutlines
{
    [ExecuteInEditMode]
    public class convexOutlineV2 : MonoBehaviour
    {
        //--- Optimization

        bool updateSpriteEveryFrame;
        public bool UpdateSpriteEveryFrame
        {
            get { return updateSpriteEveryFrame; }
            set { updateSpriteEveryFrame = value; }
        }

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
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().showOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

                if (showOutline_GOs_InHierarchy_D)
                    outlineGameObjectsFolder.hideFlags = HideFlags.None;
                else
                    outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        //-----Overlay Variables-----

        GameObject spriteOverlay;

        bool active_SO;
        public bool Active_SO
        {
            get { return active_SO; }
            set
            {
                active_SO = value; //update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().active_SO = active_SO;

                spriteOverlay.SetActive(active_SO);
            }
        }

        int orderInLayer_SO;
        public int OrderInLayer_SO
        {
            get { return orderInLayer_SO; }
            set
            {
                orderInLayer_SO = value; //update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().orderInLayer_SO = orderInLayer_SO;

                spriteOverlay.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_SO;
            }
        }

        Color color_SO;
        public Color Color_SO
        {
            get { return color_SO; }
            set
            {
                color_SO = value; //update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().color_SO = color_SO;

                spriteOverlay.GetComponent<SpriteRenderer>().color = color_SO;
            }
        }

        //-----Clipping Mask Variables

        GameObject clippingMask; //gameobject with sprite mask

        bool clipCenter_CM;
        public bool ClipCenter_CM
        {
            get { return clipCenter_CM; }
            set
            {
                clipCenter_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().clipCenter_CM = clipCenter_CM;

                //enable or disable mask
                clippingMask.GetComponent<SpriteMask>().enabled = clipCenter_CM;

                //update how our edge gameobjects interact with the mask
                if (clipCenter_CM == true)
                    outlineGO.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                else
                    outlineGO.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }
        } //NOTE: used in update function... doesnt have to do anyting special for get and set...

        float alphaCutoff_CM;
        public float AlphaCutoff_CM
        {
            get { return alphaCutoff_CM; }
            set
            {
                alphaCutoff_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().alphaCutoff_CM = alphaCutoff_CM;

                clippingMask.GetComponent<SpriteMask>().alphaCutoff = alphaCutoff_CM;
            }
        }

        bool customRange_CM;
        public bool CustomRange_CM
        {
            get { return customRange_CM; }
            set
            {
                customRange_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().customRange_CM = customRange_CM;

                clippingMask.GetComponent<SpriteMask>().isCustomRangeActive = customRange_CM;
            }
        }

        int frontLayer_CM;
        public int FrontLayer_CM
        {
            get { return frontLayer_CM; }
            set
            {
                frontLayer_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().frontLayer_CM = frontLayer_CM;

                clippingMask.GetComponent<SpriteMask>().frontSortingLayerID = frontLayer_CM;
            }
        }

        int backLayer_CM;
        public int BackLayer_CM
        {
            get { return backLayer_CM; }
            set
            {
                backLayer_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().backLayer_CM = backLayer_CM;

                clippingMask.GetComponent<SpriteMask>().backSortingLayerID = backLayer_CM;
            }
        }

        //-----Outline Variables-----

        GameObject outlineGO;

        bool active_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool Active_O
        {
            get { return active_O; }
            set
            {
                active_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().active_O = active_O;

                outlineGO.SetActive(active_O);
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
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().color_O = color_O;

                outlineGO.GetComponent<SpriteRenderer>().color = color_O;
            }
        }

        int orderInLayer_O;
        public int OrderInLayer_O
        {
            get { return orderInLayer_O; }
            set
            {
                orderInLayer_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().orderInLayer_O = orderInLayer_O;

                outlineGO.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
            }
        }

        float size_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public float Size_O
        {
            get { return size_O; }
            set
            {
                float oldSize = size_O;

                value = (value >= .1f) ? value : .1f; //since our childrens' size depends on our porportion with them we avoid our size being 0
                size_O = value; //update local value 

                //actually set our own size once we have considered the needs of our children
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().size_O = size_O;

                updateOutline();
            }
        }

        bool scaleWithParentX_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool ScaleWithParentX_O
        {
            get { return scaleWithParentX_O; }
            set
            {
                scaleWithParentX_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().scaleWithParentX_O = scaleWithParentX_O;

                updateOutline();
            }
        }

        bool scaleWithParentY_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool ScaleWithParentY_O
        {
            get { return scaleWithParentY_O; }
            set
            {
                scaleWithParentY_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConvexOutlineV2>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConvexOutlineV2>().scaleWithParentY_O = scaleWithParentY_O;

                updateOutline();
            }
        }

        void Awake()
        {
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
            copyOriginalGO_Transforms(outlineGameObjectsFolder);
            outlineGameObjectsFolder.transform.parent = this.transform;

            //-----Outline GameObject
            outlineGO = new GameObject("Our Outline");
            outlineGO.AddComponent<SpriteRenderer>();
            //set material
            var tempMaterial = new Material(outlineGO.GetComponent<SpriteRenderer>().sharedMaterial);
            tempMaterial.shader = Shader.Find("GUI/Text Shader");
            outlineGO.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial; //text shader to get silhouette of our sprite
            //parenting and positioning
            copyOriginalGO_Transforms(outlineGO);
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), outlineGO.GetComponent<SpriteRenderer>());
            outlineGO.transform.parent = outlineGameObjectsFolder.transform;

            //-----Sprite Overlay
            spriteOverlay = new GameObject("Sprite Overlay");
            spriteOverlay.AddComponent<SpriteRenderer>();
            spriteOverlay.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;
            //parenting and positioning
            copyOriginalGO_Transforms(spriteOverlay);
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());
            spriteOverlay.transform.parent = outlineGameObjectsFolder.transform;

            //-----Sprite Mask
            clippingMask = new GameObject("Sprite Mask");
            clippingMask.AddComponent<SpriteMask>();
            //parenting and positioning
            copyOriginalGO_Transforms(clippingMask);
            clippingMask.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite;
            clippingMask.transform.parent = outlineGameObjectsFolder.transform;

            //*****Set Variable Defaults*****

            //--- Optimization

            UpdateSpriteEveryFrame = true;

            //----- Debugging

            ShowOutline_GOs_InHierarchy_D = false;

            //--- Sprite Overlay

            Active_SO = false;
            OrderInLayer_SO = this.GetComponent<SpriteRenderer>().sortingOrder + 1; //by default in front
            Color_SO = new Color(0, 0, 1, .5f);

            //--- Clipping Mask

            ClipCenter_CM = true;
            AlphaCutoff_CM = .25f;

            CustomRange_CM = false;
            FrontLayer_CM = 0; //by defaults maps to "default" layer
            BackLayer_CM = 0; //by defaults maps to "default" layer

            //----- Outline

            Active_O = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size

            Color_O = Color.blue;
            OrderInLayer_O = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
            Size_O = 1.1f;
            ScaleWithParentX_O = true;
            ScaleWithParentY_O = true;
        }

        void Update()
        {
            if (UpdateSpriteEveryFrame)
            {
                //update sprite overlay
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

                //update outline
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), outlineGO.GetComponent<SpriteRenderer>());

                //update clipping mask
                clippingMask.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite;
            }
        }

        //Update the Variables shared by all outline types
        public void updateUniversalVars()
        {
            //--- Sprite Overlay
            Active_SO = Active_SO;
            OrderInLayer_SO = OrderInLayer_SO;
            Color_SO = Color_SO;

            //----- Outline
            Active_O = Active_O;
            Color_O = Color_O;
            OrderInLayer_O = OrderInLayer_O; //by default behind
            Size_O = Size_O;
            ScaleWithParentX_O = ScaleWithParentX_O;
            ScaleWithParentY_O = ScaleWithParentY_O;
        }

        void updateOutline()
        {
            bool sizeBasedOnScale = true; //NOT set different size types for different reasons

            float localSizeX;
            float localSizeY;

            if (sizeBasedOnScale) //---PAIRS WITH CONCAVE--- | ---CONSISTENT--- 
            {
                //REQUIRED because: when we want our size to be able to be == to 0
                //when size is 0 their should essentially be a "solidly colored sprite behind our current sprite"
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

                outlineGO.transform.localScale = new Vector3(localSizeX, localSizeY, 1);
            }
            else //---DOESNT PAIR WITH CONCAVE--- | ---NOT CONSISTENT--- because it works by basing itself off of the size of the AABB square given by the bounds given by the spriterenderer 
            {

            }
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
    }
}