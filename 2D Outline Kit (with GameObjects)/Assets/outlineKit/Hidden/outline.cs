using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: "DRAW_MODE_SLICED" WON'T WORK (IF you require a sprite mask)
//this is because the current unity sprite mask does not support the 9 slice system

//NOTE: "DRAW_MODE_TILED" WONT'T WORK 
//this is because the current unity sprite mask does not support tiled draw mode
//and

//BOTH of the features above MIGHT be added when I create the outlineKit with shaders instead of gameobjects
//NOTE: that if you really wanted to you could force it to support tiled by having multiple sprite masks and multiple outlines for each tile

/*
 * ---Other Resources
 * https://answers.unity.com/questions/235595/custom-editor-losing-settings-on-play.html
 * 
 * ---Other Docs
 * https://docs.unity3d.com/Manual/script-Serialization.html
 * https://docs.unity3d.com/ScriptReference/EditorUtility.SetDirty.html
 * 
 * ---Unity Documentation
 * UnityEditor.Build
 * UnityEditor.Callbacks
 * UnityEditor.EventSystems
 * 
 * UnityEditor.IOS
 * 
 * CHECK ALL CLASSES INBETWEEN FOR ANTHING USEFUL
 * 
 * Classes
 * Enumerations
 * Attributes
 */

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
        S_O, //Size_O (ONLY convex)
        SWPX_O, //ScaleWithParentX_O
        SWPY_O, //ScaleWithParentY_O

        //----------Variables For ONLY (Concave/Push) Outline Type
        PT_OP, //PushType_OP
        SS_OP, //stdSize_OP
        S_OP, //size_OP
        OMO_OPR, //ObjsMakingOutline_OPR
        SA_OPR, //StartAngle_OPR
        PP_OPR, //PushPattern_OPR
        RS_OPRS, //RectSize_OPRS 
        RW_OPRS, //RectWidth_OPRS 
        RH_OPRS //RectHeight_OPRS 
    };

    public enum spriteUpdateSetting { EveryFrame, AfterEveryChange, Manually }

    [System.Serializable, ExecuteInEditMode]
    public class outline : MonoBehaviour
    {
        //--- Optimization

        [Header("OPTIMIZATION VARIABLES-----")]
        [SerializeField, HideInInspector]
        internal spriteUpdateSetting updateSprite;
        public spriteUpdateSetting UpdateSprite
        {
            get { return updateSprite; }
            set { updateSprite = value; }
        }

        //-----Debugging Variables-----

        //IMPORTANT NOTE: currently only one outline is supported
        [SerializeField, HideInInspector]
        internal GameObject outlineGameObjectsFolder; //contains all the outlines

        [Space(10)]
        [Header("DEBUGGING VARIABLES-----")]
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

        [SerializeField, HideInInspector]
        internal GameObject spriteOverlay;

        [Space(10)]
        [Header("SPRITE OVERLAY VARIABLES-----")]
        [SerializeField, HideInInspector]
        internal bool active_SO;
        public bool Active_SO
        {
            get { return active_SO; }
            set
            {
                active_SO = value; //update local value

                newActiveSO = true;
            }
        }
        [SerializeField, HideInInspector]
        private bool newActiveSO;

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

        [SerializeField, HideInInspector]
        internal GameObject clippingMask; //gameobject with sprite mask

        [SerializeField, HideInInspector]
        internal float alphaCutoff_CM;
        public float AlphaCutoff_CM
        {
            get { return alphaCutoff_CM; }
            set
            {
                alphaCutoff_CM = Mathf.Clamp01(value); //update local value

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
            UpdateSprite = spriteUpdateSetting.AfterEveryChange;

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

            //---Hack Inits
            newActiveSO = false;
        }

        public void Update()
        {
            //required hacks because of warnings
            if (newActiveSO)
            {
                spriteOverlay.GetComponent<SpriteRenderer>().enabled = active_SO;
                newActiveSO = false;
            }
        }

        [SerializeField, HideInInspector]
        Sprite prevSprite;
        [SerializeField, HideInInspector]
        bool prevFlipX;
        [SerializeField, HideInInspector]
        bool prevFlipY;
        [SerializeField, HideInInspector]
        SpriteDrawMode prevDrawMode;
        [SerializeField, HideInInspector]
        Vector2 prevSize;

        //GIVEN THE NOTES BELOW: we only check if (1) sprite (2) flip X and flip Y changes (3) DrawMode (4) Size [for draw mode = tiled]
        public bool spriteChanged(SpriteRenderer SR)
        {
            if (prevSprite != SR.sprite)
                return updateAllPrevs(SR, true);
            else if (prevFlipX != SR.flipX)
                return updateAllPrevs(SR, true);
            else if (prevFlipY != SR.flipY)
                return updateAllPrevs(SR, true);
            else if(prevDrawMode != SR.drawMode)
                return updateAllPrevs(SR, true);
            else if(prevSize != SR.size)
                return updateAllPrevs(SR, true);
            else
                return updateAllPrevs(SR, false);
        }

        bool updateAllPrevs(SpriteRenderer SR, bool spriteChanged)
        {
            prevSprite = SR.sprite;
            prevFlipX = SR.flipX;
            prevFlipY = SR.flipY;
            prevDrawMode = SR.drawMode;
            prevSize = SR.size;

            return spriteChanged;
        }

        //--------------------------------------------------STATIC FUNCTIONS--------------------------------------------------

        //-------------------------Used By All Outlines-------------------------

        //-------------------------Inits

        public static Material initPart1(GameObject main, ref GameObject thisOutline, ref GameObject folder)
        {
            //-----Outline Folder [MUST BE FIRST]
            folder = new GameObject("Outline Folder");
            copyTransform(main, folder);
            folder.transform.parent = main.transform;

            //-----Outline GameObject
            copyTransform(main, thisOutline);
            thisOutline.transform.parent = folder.transform;
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

        //-------------------------Other

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

        //NOTE: ONLY TESTED changing... sprite, FlipX and FlipY, DrawMode(Single), DrawMode(Sliced)
        public static void copySpriteRendererData(SpriteRenderer from, SpriteRenderer to)
        {
            to.sprite = from.sprite;
            to.flipX = from.flipX;
            to.flipY = from.flipY;
            to.drawMode = from.drawMode;

            //---only if draw mode SLICED -or- TILED
            to.size = from.size;

            //---|---only if draw mode TILED
            to.tileMode = from.tileMode;
            
            //---|---|only if TILE_MODE is Adaptive
            to.adaptiveModeThreshold = from.adaptiveModeThreshold;

            //(1) Color (2) Mask Interaction -> are set elsewhere
        }

        public static void copySpriteRendererDataToClipMask(GameObject SR, GameObject CM)
        {
            //update sprite
            CM.GetComponent<SpriteMask>().sprite = SR.GetComponent<SpriteRenderer>().sprite;

            //update the flip
            if (SR.GetComponent<SpriteRenderer>().flipX == true)
                CM.transform.rotation = Quaternion.Euler(CM.transform.rotation.eulerAngles.x, 180, 0);
            else
                CM.transform.rotation = Quaternion.Euler(CM.transform.rotation.eulerAngles.x, 0, 0);

            if (SR.GetComponent<SpriteRenderer>().flipY == true)
                CM.transform.rotation = Quaternion.Euler(180, CM.transform.rotation.eulerAngles.y, 0);
            else
                CM.transform.rotation = Quaternion.Euler(0, CM.transform.rotation.eulerAngles.y, 0);
        }

        //-------------------------Used by Families-------------------------

        //NOTE: according to microsoft delegates are x8 time slower than methods... so eventually make sure that the switch isnt converting the code and using it as a sort of delegate
        //TODO... I.O.W. make sure the switch case isnt become a dictionary from Enums -> Delegates when compiled... cuz that would be really slow
        public static void passToChildren(varToUpdate varEnum, GameObject parent, List<GameObject> children)
        {
            if(parent.GetComponent<convexOut>() != null)
            {
                for (int childID = 0; childID < children.Count; childID++)
                {
                    if (children[childID] != null)
                    {
                        convexOut[] convexOutlines = children[childID].GetComponents<convexOut>();
                        concaveOut[] concaveOutlines = children[childID].GetComponents<concaveOut>();

                        if (convexOutlines.Length != 0)
                            for(int i=0; i<convexOutlines.Length; i++)
                                convexSwitch(varEnum, parent.GetComponent<convexOut>(), convexOutlines[i]); //convex, convex
                            
                        if (concaveOutlines.Length != 0)
                            for(int i=0; i<concaveOutlines.Length; i++)
                                convexParent_concaveChild_Switch(varEnum, parent.GetComponent<convexOut>(), concaveOutlines[i]); //convex, concave
                            
                    }
                    else
                    {
                        children.RemoveAt(childID);
                        childID--;
                    }
                }

            }
            else if(parent.GetComponent<concaveOut>() != null)
            {

                for (int childID = 0; childID < children.Count; childID++)
                {
                    if (children[childID] != null)
                    {
                        convexOut[] convexOutlines = children[childID].GetComponents<convexOut>();
                        concaveOut[] concaveOutlines = children[childID].GetComponents<concaveOut>();

                        if (convexOutlines.Length != 0)
                            for (int i = 0; i < convexOutlines.Length; i++)
                                concaveParent_convexChild_Switch(varEnum, parent.GetComponent<concaveOut>(), convexOutlines[i]); //concave, convex

                        if (concaveOutlines.Length != 0)
                            for (int i = 0; i < concaveOutlines.Length; i++)
                                concaveSwitch(varEnum, parent.GetComponent<concaveOut>(), concaveOutlines[i]); //concave, concave
                    }
                    else
                    {
                        children.RemoveAt(childID);
                        childID--;
                    } 
                }
            }
            //ELSE... parent has no data to pass
        }

        //-------------------------Switch Cases For Every Combination Of Parent And Children

        public static void convexSwitch(varToUpdate varEnum, convexOut par, convexOut child)
        {
            switch (varEnum)
            {
                //Optimization
                case varToUpdate.USEF: child.UpdateSprite = par.UpdateSprite; break;
                //Debugging
                case varToUpdate.SOGIH: child.ShowOutline_GOs_InHierarchy_D = par.ShowOutline_GOs_InHierarchy_D; break;
                //Sprite Overlay
                case varToUpdate.A_SO: child.Active_SO = par.Active_SO; break;
                case varToUpdate.OIL_SO: child.OrderInLayer_SO = par.OrderInLayer_SO; break;
                case varToUpdate.C_SO: child.Color_SO = par.Color_SO; break;
                //Clipping Mask
                case varToUpdate.CC_CM: child.ClipCenter_CM = par.ClipCenter_CM; break;
                case varToUpdate.AC_CM: child.AlphaCutoff_CM = par.AlphaCutoff_CM; break;
                case varToUpdate.CR_CM: child.CustomRange_CM = par.CustomRange_CM; break;
                case varToUpdate.FL_CM: child.FrontLayer_CM = par.FrontLayer_CM; break;
                case varToUpdate.BL_CM: child.BackLayer_CM = par.BackLayer_CM; break;
                //Sprite Outline
                case varToUpdate.A_O: child.Active_O = par.Active_O; break;
                case varToUpdate.C_O: child.Color_O = par.Color_O; break;
                case varToUpdate.OIL_O: child.OrderInLayer_O = par.OrderInLayer_O; break;
                case varToUpdate.S_O: child.Size_O = par.Size_O; break;
                case varToUpdate.SWPX_O: child.ScaleWithParentX_O = par.ScaleWithParentX_O; break;
                case varToUpdate.SWPY_O: child.ScaleWithParentY_O = par.ScaleWithParentY_O; break;
            };
        }

        public static void concaveSwitch(varToUpdate varEnum, concaveOut par, concaveOut child)
        {
            switch (varEnum)
            {
                //Optimization
                case varToUpdate.USEF: child.UpdateSprite = par.UpdateSprite; break;
                //Debugging
                case varToUpdate.SOGIH: child.ShowOutline_GOs_InHierarchy_D = par.ShowOutline_GOs_InHierarchy_D; break;
                //Sprite Overlay
                case varToUpdate.A_SO: child.Active_SO = par.Active_SO; break;
                case varToUpdate.OIL_SO: child.OrderInLayer_SO = par.OrderInLayer_SO; break;
                case varToUpdate.C_SO: child.Color_SO = par.Color_SO; break;
                //Clipping Mask
                case varToUpdate.CC_CM: child.ClipCenter_CM = par.ClipCenter_CM; break;
                case varToUpdate.AC_CM: child.AlphaCutoff_CM = par.AlphaCutoff_CM; break;
                case varToUpdate.CR_CM: child.CustomRange_CM = par.CustomRange_CM; break;
                case varToUpdate.FL_CM: child.FrontLayer_CM = par.FrontLayer_CM; break;
                case varToUpdate.BL_CM: child.BackLayer_CM = par.BackLayer_CM; break;
                //Sprite Outline
                case varToUpdate.A_O: child.Active_O = par.Active_O; break;
                case varToUpdate.C_O: child.Color_O = par.Color_O; break;
                case varToUpdate.OIL_O: child.OrderInLayer_O = par.OrderInLayer_O; break;
                case varToUpdate.SWPX_O: child.ScaleWithParentX_O = par.ScaleWithParentX_O; break;
                case varToUpdate.SWPY_O: child.ScaleWithParentY_O = par.ScaleWithParentY_O; break;

                //---ONLY push type
                case varToUpdate.PT_OP: child.PushType_OP = par.PushType_OP; break;
                case varToUpdate.SS_OP: child.StdSize_OP = par.StdSize_OP; break;
                case varToUpdate.S_OP: child.Size_OP = par.Size_OP; break;
                case varToUpdate.OMO_OPR: child.EdgeCount_OPR = par.EdgeCount_OPR; break;
                case varToUpdate.SA_OPR: child.StartAngle_OPR = par.StartAngle_OPR; break;
                case varToUpdate.PP_OPR: child.PushPattern_OPR = par.PushPattern_OPR; break;

                case varToUpdate.RS_OPRS: child.RectSize_OPRS = par.RectSize_OPRS; break;
                case varToUpdate.RW_OPRS: child.RectWidth_OPRS = par.RectWidth_OPRS; break;
                case varToUpdate.RH_OPRS: child.RectHeight_OPRS = par.RectHeight_OPRS; break;
            };
        }

        public static void concaveParent_convexChild_Switch(varToUpdate varEnum, concaveOut par, convexOut child)
        {
            switch (varEnum)
            {
                //Optimization
                case varToUpdate.USEF: child.UpdateSprite = par.UpdateSprite; break;
                //Debugging
                case varToUpdate.SOGIH: child.ShowOutline_GOs_InHierarchy_D = par.ShowOutline_GOs_InHierarchy_D; break;
                //Sprite Overlay
                case varToUpdate.A_SO: child.Active_SO = par.Active_SO; break;
                case varToUpdate.OIL_SO: child.OrderInLayer_SO = par.OrderInLayer_SO; break;
                case varToUpdate.C_SO: child.Color_SO = par.Color_SO; break;
                //Clipping Mask
                case varToUpdate.CC_CM: child.ClipCenter_CM = par.ClipCenter_CM; break;
                case varToUpdate.AC_CM: child.AlphaCutoff_CM = par.AlphaCutoff_CM; break;
                case varToUpdate.CR_CM: child.CustomRange_CM = par.CustomRange_CM; break;
                case varToUpdate.FL_CM: child.FrontLayer_CM = par.FrontLayer_CM; break;
                case varToUpdate.BL_CM: child.BackLayer_CM = par.BackLayer_CM; break;
                //Sprite Outline
                case varToUpdate.A_O: child.Active_O = par.Active_O; break;
                case varToUpdate.C_O: child.Color_O = par.Color_O; break;
                case varToUpdate.OIL_O: child.OrderInLayer_O = par.OrderInLayer_O; break;
                //NOTE: sizes are too different from one type of script to the next
                case varToUpdate.SWPX_O: child.ScaleWithParentX_O = par.ScaleWithParentX_O; break;
                case varToUpdate.SWPY_O: child.ScaleWithParentY_O = par.ScaleWithParentY_O; break;
            };
        }

        public static void convexParent_concaveChild_Switch(varToUpdate varEnum, convexOut par, concaveOut child)
        {
            switch (varEnum)
            {
                //Optimization
                case varToUpdate.USEF: child.UpdateSprite = par.UpdateSprite; break;
                //Debugging
                case varToUpdate.SOGIH: child.ShowOutline_GOs_InHierarchy_D = par.ShowOutline_GOs_InHierarchy_D; break;
                //Sprite Overlay
                case varToUpdate.A_SO: child.Active_SO = par.Active_SO; break;
                case varToUpdate.OIL_SO: child.OrderInLayer_SO = par.OrderInLayer_SO; break;
                case varToUpdate.C_SO: child.Color_SO = par.Color_SO; break;
                //Clipping Mask
                case varToUpdate.CC_CM: child.ClipCenter_CM = par.ClipCenter_CM; break;
                case varToUpdate.AC_CM: child.AlphaCutoff_CM = par.AlphaCutoff_CM; break;
                case varToUpdate.CR_CM: child.CustomRange_CM = par.CustomRange_CM; break;
                case varToUpdate.FL_CM: child.FrontLayer_CM = par.FrontLayer_CM; break;
                case varToUpdate.BL_CM: child.BackLayer_CM = par.BackLayer_CM; break;
                //Sprite Outline
                case varToUpdate.A_O: child.Active_O = par.Active_O; break;
                case varToUpdate.C_O: child.Color_O = par.Color_O; break;
                case varToUpdate.OIL_O: child.OrderInLayer_O = par.OrderInLayer_O; break;
                //NOTE: sizes are too different from one type of script to the next
                case varToUpdate.SWPX_O: child.ScaleWithParentX_O = par.ScaleWithParentX_O; break;
                case varToUpdate.SWPY_O: child.ScaleWithParentY_O = par.ScaleWithParentY_O; break;
            };
        }
    }
}