using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: there are POTENTIAL optimization issues because of the POTENTIAL use of delegates
//for now the goal is to simply get this to work

namespace object2DOutlines
{
    [ExecuteInEditMode]
    public class convexC : MonoBehaviour
    {
        bool awakeFinished;

        void OnValidate()
        {
            if (awakeFinished)
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

        //-----Family Relationships-----

        [Header("Family Relationships")]
        public GameObject parentGOWithScript; //what does this do?
        GameObject prevParentGOWithScript; //what does this do?

        public List<GameObject> children;

        //-----Optimization-----

        [SerializeField, HideInInspector]
        bool updateSpriteEveryFrame;
        public bool UpdateSpriteEveryFrame
        {
            get { return updateSpriteEveryFrame; }
            set { updateSpriteEveryFrame = value; }
        }

        //-----Debugging Variables-----

        GameObject outlineGameObjectsFolder; //contains all the outlines
                                             //IMPORTANT NOTE: currently only one outline is supported

        [SerializeField, HideInInspector]
        bool showOutline_GOs_InHierarchy_D;
        public bool ShowOutline_GOs_InHierarchy_D
        {
            get { return showOutline_GOs_InHierarchy_D; }
            set
            {
                showOutline_GOs_InHierarchy_D = value; //update local value

                if (showOutline_GOs_InHierarchy_D)
                    outlineGameObjectsFolder.hideFlags = HideFlags.None;
                else
                    outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        //-----Overlay Variables-----

        GameObject spriteOverlay;

        [SerializeField, HideInInspector]
        private bool active_SO;
        public bool Active_SO
        {
            get { return active_SO; }
            set
            {
                active_SO = value; //update local value

                spriteOverlay.SetActive(active_SO);
            }
        }

        [SerializeField, HideInInspector]
        int orderInLayer_SO;
        public int OrderInLayer_SO
        {
            get { return orderInLayer_SO; }
            set
            {
                orderInLayer_SO = value; //update local value

                spriteOverlay.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_SO;
            }
        }

        [SerializeField, HideInInspector]
        Color color_SO;
        public Color Color_SO
        {
            get { return color_SO; }
            set
            {
                color_SO = value; //update local value

                spriteOverlay.GetComponent<SpriteRenderer>().color = color_SO;
            }
        }

        //-----Clipping Mask Variables

        GameObject clippingMask; //gameobject with sprite mask

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
                    outlineGO.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                else
                    outlineGO.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }
        } //NOTE: used in update function... doesnt have to do anyting special for get and set...

        [SerializeField, HideInInspector]
        float alphaCutoff_CM;
        public float AlphaCutoff_CM
        {
            get { return alphaCutoff_CM; }
            set
            {
                alphaCutoff_CM = value; //update local value

                clippingMask.GetComponent<SpriteMask>().alphaCutoff = alphaCutoff_CM;
            }
        }

        [SerializeField, HideInInspector]
        bool customRange_CM;
        public bool CustomRange_CM
        {
            get { return customRange_CM; }
            set
            {
                customRange_CM = value; //update local value

                clippingMask.GetComponent<SpriteMask>().isCustomRangeActive = customRange_CM;
            }
        }

        [SerializeField, HideInInspector]
        int frontLayer_CM;
        public int FrontLayer_CM
        {
            get { return frontLayer_CM; }
            set
            {
                frontLayer_CM = value; //update local value

                clippingMask.GetComponent<SpriteMask>().frontSortingLayerID = frontLayer_CM;
            }
        }

        [SerializeField, HideInInspector]
        int backLayer_CM;
        public int BackLayer_CM
        {
            get { return backLayer_CM; }
            set
            {
                backLayer_CM = value; //update local value

                clippingMask.GetComponent<SpriteMask>().backSortingLayerID = backLayer_CM;
            }
        }

        //-----Outline Variables-----

        GameObject outlineGO;

        [SerializeField, HideInInspector]
        bool active_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool Active_O
        {
            get { return active_O; }
            set
            {
                active_O = value;//update local value

                outlineGO.SetActive(active_O);
            }
        }

        [Space(10)]

        [SerializeField, HideInInspector]
        Color color_O;
        public Color Color_O
        {
            get { return color_O; }
            set
            {
                color_O = value;//update local value

                outlineGO.GetComponent<SpriteRenderer>().color = color_O;
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

                outlineGO.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;
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

        void Awake()
        {
            awakeFinished = false;

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

            awakeFinished = true;
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

            outlineGO.transform.localScale = new Vector3(localSizeX, localSizeY, 1);
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

        //-------------------------parent child reltionship code[PCRC]-------------------------

        //NOTE: according to microsoft delegates are x8 time slower than methods... so eventually make sure that the switch isnt converting the code and using it as a sort of delegate
        //I.O.W. make sure the switch case isnt become a dictionary from Enums -> Delegates when compiled... cuz that would be really slow
        void passToChildren(varToUpdate varEnum)
        {
            for (int i = 0; i < children.Count; i++) //loop through all of our children
            {
                if (children[i] != null) //if this child still exists
                {
                    //TODO... iterate through different script with these same variables "convexC" and "concaveC" and possible others
                    //replace if statemetn for "for loop"

                    if (children[i].GetComponent<convexC>() != null)
                    {
                        switch (varEnum)
                        {
                            //Optimization
                            case varToUpdate.USEF: children[i].GetComponent<convexC>().UpdateSpriteEveryFrame = UpdateSpriteEveryFrame; break;
                            //Debugging
                            case varToUpdate.SOGIH: children[i].GetComponent<convexC>().ShowOutline_GOs_InHierarchy_D = ShowOutline_GOs_InHierarchy_D; break;
                            //Sprite Overlay
                            case varToUpdate.A_SO: children[i].GetComponent<convexC>().Active_SO = Active_SO; break;
                            case varToUpdate.OIL_SO: children[i].GetComponent<convexC>().OrderInLayer_SO = OrderInLayer_SO; break;
                            case varToUpdate.C_SO: children[i].GetComponent<convexC>().Color_SO = Color_SO; break;
                            //Clipping Mask
                            case varToUpdate.CC_CM: children[i].GetComponent<convexC>().ClipCenter_CM = ClipCenter_CM; break;
                            case varToUpdate.AC_CM: children[i].GetComponent<convexC>().AlphaCutoff_CM = AlphaCutoff_CM; break;
                            case varToUpdate.CR_CM: children[i].GetComponent<convexC>().CustomRange_CM = CustomRange_CM; break;
                            case varToUpdate.FL_CM: children[i].GetComponent<convexC>().FrontLayer_CM = FrontLayer_CM; break;
                            case varToUpdate.BL_CM: children[i].GetComponent<convexC>().BackLayer_CM = BackLayer_CM; break;
                            //Sprite Outline
                            case varToUpdate.A_O: children[i].GetComponent<convexC>().Active_O = Active_O; break;
                            case varToUpdate.C_O: children[i].GetComponent<convexC>().Color_O = Color_O; break;
                            case varToUpdate.OIL_O: children[i].GetComponent<convexC>().OrderInLayer_O = OrderInLayer_O; break;
                            case varToUpdate.S_O: children[i].GetComponent<convexC>().Size_O = Size_O; break;
                            case varToUpdate.SWPX_O: children[i].GetComponent<convexC>().ScaleWithParentX_O = ScaleWithParentX_O; break;
                            case varToUpdate.SWPY_O: children[i].GetComponent<convexC>().ScaleWithParentY_O = ScaleWithParentY_O; break;
                        };
                    }
                }
                else
                {
                    children.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}