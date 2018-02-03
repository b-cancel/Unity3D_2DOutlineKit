using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//be careful when changing the setting of the source sprite listed below
//(sprite, flip, and draw mode)
//sprite --> repair by updating the sprite data when we change sprites
//draw mode --> repair by recalculation size after draw mode change
//flip --> repair by instead changing the roation of the sprite mask

//CHECK... sprite overlay, clipping mask, outline objects


namespace object2DOutlines
{
    //---This Enum makes it easier for us to pass our variables to our children (helps keep code clean)
    public enum varToUpdate
    {
        //----------Variables For All Outline Types

        //-----Optimization
        USEF, //UpdateSpriteEveryFrame
        //-----Debugging
        SOGIH, //ShowOutline_GOs_InHierarchy_D
        //-----Sprite Overlay
        A_SO, //Active_SO
        OIL_SO, //OrderInLayer_SO
        C_SO, //Color_SO
        //-----Clipping Mask
        CC_CM, //ClipCenter_CM
        AC_CM, //AlphaCutoff_CM
        CR_CM, //CustomRange_CM
        FL_CM, //FrontLayer_CM
        BL_CM, //BackLayer_CM
        //-----Sprite Outline
        A_O, //Active_O
        C_O, //Color_O
        OIL_O, //OrderInLayer_O
        S_O, //Size_O
        SWPX_O, //ScaleWithParentX_O
        SWPY_O //ScaleWithParentY_O

        //----------Variables For ONLY (Concave/Push) Outline Type

        //NONE
    };

    [ExecuteInEditMode]
    public class outline : MonoBehaviour
    {
        //--- Optimization

        [Header("Optimization Variables")]
        [SerializeField, HideInInspector]
        internal bool updateSpriteEveryFrame;
        public bool UpdateSpriteEveryFrame
        {
            get { return updateSpriteEveryFrame; }
            set { updateSpriteEveryFrame = value; }
        }

        //-----Debugging Variables-----

        //IMPORTANT NOTE: currently only one outline is supported
        internal GameObject outlineGameObjectsFolder; //contains all the outlines

        [Space(10)]
        [Header("Debugging Variables")]
        [SerializeField, HideInInspector]
        internal bool showOutline_GOs_InHierarchy_D;
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

        internal GameObject spriteOverlay;

        [Space(10)]
        [Header("Sprite Overlay Variables")]
        [SerializeField, HideInInspector]
        internal bool active_SO;
        public bool Active_SO
        {
            get { return active_SO; }
            set
            {
                active_SO = value; //update local value

                spriteOverlay.GetComponent<SpriteRenderer>().enabled = active_SO;
            }
        }

        [SerializeField, HideInInspector]
        internal int orderInLayer_SO;
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
        internal Color color_SO;
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

        internal GameObject clippingMask; //gameobject with sprite mask

        [SerializeField, HideInInspector]
        internal float alphaCutoff_CM;
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
        internal bool customRange_CM;
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
        internal int frontLayer_CM;
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
        internal int backLayer_CM;
        public int BackLayer_CM
        {
            get { return backLayer_CM; }
            set
            {
                backLayer_CM = value; //update local value

                clippingMask.GetComponent<SpriteMask>().backSortingLayerID = backLayer_CM;
            }
        }

        public void Awake()
        {
            //----------Variable Inits

            //--- Optimization
            UpdateSpriteEveryFrame = true;

            //----- Debugging
            ShowOutline_GOs_InHierarchy_D = false;

            //--- Sprite Overlay
            Active_SO = false;
            OrderInLayer_SO = this.GetComponent<SpriteRenderer>().sortingOrder + 1; //by default in front
            Color_SO = new Color(0, 0, 1, .5f);

            //--- Clipping Mask
            AlphaCutoff_CM = .25f;
            CustomRange_CM = false;
            FrontLayer_CM = 0; //by defaults maps to "default" layer
            BackLayer_CM = 0; //by defaults maps to "default" layer
        }

        //--------------------------------------------------STATIC FUNCTIONS--------------------------------------------------

        //-------------------------Used By All Outlines-------------------------

        public static Material initPart1(GameObject main, ref GameObject thisOutline, ref GameObject folder)
        {
            //----------Cover Duplication Problem

            Transform psblOF_T = main.transform.Find("Outline Folder");
            if (psblOF_T != null) //transform found
            {
                GameObject psblOF_GO = psblOF_T.gameObject;
                if (psblOF_GO.transform.parent.gameObject == main) //this gameobject ours
                    DestroyImmediate(psblOF_GO);
            }

            //----------Object Instantiation

            //-----Outline Folder [MUST BE FIRST]
            folder = new GameObject("Outline Folder");
            copyTransform(main, folder);
            folder.transform.parent = main.transform;

            //-----Outline GameObject
            thisOutline.transform.parent = folder.transform;
            copyTransform(main, thisOutline);
            thisOutline.AddComponent<SpriteRenderer>();
            var tempMaterial = new Material(thisOutline.GetComponent<SpriteRenderer>().sharedMaterial);
            tempMaterial.shader = Shader.Find("GUI/Text Shader");

            return tempMaterial;
        }

        public static void initPart2(GameObject main, ref GameObject folder, ref GameObject overlay, ref GameObject clipMask, ref Material tempMaterial)
        {
            //-----Sprite Overlay
            overlay = new GameObject("Sprite Overlay");
            overlay.AddComponent<SpriteRenderer>();
            overlay.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;
            copyTransform(main, overlay);
            overlay.transform.parent = folder.transform;
            copySpriteRendererData(main.GetComponent<SpriteRenderer>(), overlay.GetComponent<SpriteRenderer>());

            //-----Sprite Mask
            clipMask = new GameObject("Sprite Mask");
            clipMask.AddComponent<SpriteMask>();
            copyTransform(main, clipMask);
            clipMask.transform.parent = folder.transform;
            clipMask.GetComponent<SpriteMask>().sprite = main.GetComponent<SpriteRenderer>().sprite;
        }

        public static void copyTransform(GameObject original, GameObject copy)
        {
            //place ourselves in the same place as our parent (for transform copying purposes...)
            if (original.transform.parent != null)
                copy.transform.parent = (original.transform.parent).gameObject.transform;
            //ELSE... our parent is in the root and currently so are we...

            //copy over all transforms
            copy.transform.localScale = original.transform.localScale;
            copy.transform.localPosition = original.transform.localPosition;
            copy.transform.localRotation = original.transform.localRotation;
        }

        //NOTE: you only need to copy over a variable if its not its default value (for later optimization)
        public static void copySpriteRendererData(SpriteRenderer from, SpriteRenderer to)
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

        //-------------------------Used by Families-------------------------

        //NOTE: according to microsoft delegates are x8 time slower than methods... so eventually make sure that the switch isnt converting the code and using it as a sort of delegate
        //I.O.W. make sure the switch case isnt become a dictionary from Enums -> Delegates when compiled... cuz that would be really slow
        public static void passToChildren(varToUpdate varEnum, GameObject parent, List<GameObject> children)
        {
            for (int i = 0; i < children.Count; i++) //loop through all of our children
            {
                if (children[i] != null) //if this child still exists
                {
                    //TODO... make this work for both script convexOut -and- concaveOut

                    if (children[i].GetComponent<convexOut>() != null)
                    {
                        switch (varEnum)
                        {
                            //Optimization
                            case varToUpdate.USEF: children[i].GetComponent<convexOut>().UpdateSpriteEveryFrame = parent.GetComponent<convexOut>().UpdateSpriteEveryFrame; break;
                            //Debugging
                            case varToUpdate.SOGIH: children[i].GetComponent<convexOut>().ShowOutline_GOs_InHierarchy_D = parent.GetComponent<convexOut>().ShowOutline_GOs_InHierarchy_D; break;
                            //Sprite Overlay
                            case varToUpdate.A_SO: children[i].GetComponent<convexOut>().Active_SO = parent.GetComponent<convexOut>().Active_SO; break;
                            case varToUpdate.OIL_SO: children[i].GetComponent<convexOut>().OrderInLayer_SO = parent.GetComponent<convexOut>().OrderInLayer_SO; break;
                            case varToUpdate.C_SO: children[i].GetComponent<convexOut>().Color_SO = parent.GetComponent<convexOut>().Color_SO; break;
                            //Clipping Mask
                            case varToUpdate.CC_CM: children[i].GetComponent<convexOut>().ClipCenter_CM = parent.GetComponent<convexOut>().ClipCenter_CM; break;
                            case varToUpdate.AC_CM: children[i].GetComponent<convexOut>().AlphaCutoff_CM = parent.GetComponent<convexOut>().AlphaCutoff_CM; break;
                            case varToUpdate.CR_CM: children[i].GetComponent<convexOut>().CustomRange_CM = parent.GetComponent<convexOut>().CustomRange_CM; break;
                            case varToUpdate.FL_CM: children[i].GetComponent<convexOut>().FrontLayer_CM = parent.GetComponent<convexOut>().FrontLayer_CM; break;
                            case varToUpdate.BL_CM: children[i].GetComponent<convexOut>().BackLayer_CM = parent.GetComponent<convexOut>().BackLayer_CM; break;
                            //Sprite Outline
                            case varToUpdate.A_O: children[i].GetComponent<convexOut>().Active_O = parent.GetComponent<convexOut>().Active_O; break;
                            case varToUpdate.C_O: children[i].GetComponent<convexOut>().Color_O = parent.GetComponent<convexOut>().Color_O; break;
                            case varToUpdate.OIL_O: children[i].GetComponent<convexOut>().OrderInLayer_O = parent.GetComponent<convexOut>().OrderInLayer_O; break;
                            case varToUpdate.S_O: children[i].GetComponent<convexOut>().Size_O = parent.GetComponent<convexOut>().Size_O; break;
                            case varToUpdate.SWPX_O: children[i].GetComponent<convexOut>().ScaleWithParentX_O = parent.GetComponent<convexOut>().ScaleWithParentX_O; break;
                            case varToUpdate.SWPY_O: children[i].GetComponent<convexOut>().ScaleWithParentY_O = parent.GetComponent<convexOut>().ScaleWithParentY_O; break;
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