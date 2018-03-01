using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----Draw Mode Issues

//NOTE: "DRAW_MODE_SLICED" WON'T WORK (IF you require a sprite mask)
//this is because the current unity sprite mask does not support the 9 slice system

//NOTE: "DRAW_MODE_TILED" WONT'T WORK 
//this is because the current unity sprite mask does not support tiled draw mode

//BOTH of the features above MIGHT be added when I create the outlineKit with shaders instead of gameobjects
//NOTE: that if you really wanted to you could force it to support tiled by having multiple sprite masks and multiple outlines for each tile

//-----Sprite Mask Details

//NOTE: here we are not allowing users to change "backSortingLayerID" -OR- "frontSortingLayerID"

//-----Inspector Tool Details (editor only)

//NOTE: Unity Reset will break things and SHOULD NOT be used

//-----Disabling Details

//use GameObject.activeInHierarchy
//OR GameObject.activeSelf

//NOTE: disabling the script and disabling the whole gameobject calls the same function[OnDisable] (Awake, and Update wont run)
//CHECK IF INDEED THESE WONT RUN
//IF I disable the parent of a gameobject it will also be disabled (disable is recursive through gameobjects [disabling a script will not disable all scripts in its children])
//OnDestroy will still run if the object is disabled

//NOTE: when disabling a gameobject that data is still accessible but monobehavior scripts wont run
//since some of our variables require the update function to update properly... its best if we shut off the modification of those variables when we are not enabled
//since we are shutting off modification its also best to shut off the inspector

namespace object2DOutlines
{
    //--------------------------------------------------EXTRA "DICTIONARY" CLASS--------------------------------------------------

    [System.Serializable, ExecuteInEditMode]
    public class GO_to_Vector2
    {
        [SerializeField, HideInInspector]
        public GameObject go;

        [SerializeField, HideInInspector]
        public Vector2 v2;

        public GO_to_Vector2(GameObject G, Vector2 V)
        {
            go = G;
            v2 = V;
        }
    }

    //--------------------------------------------------ENUMERABLES--------------------------------------------------

    //---This Enum makes it easier for us to pass our variables to our children (helps keep code clean)
    public enum varToUpdate
    {
        //optimization
        US, //updateSprite

        //debugging
        SOGIH, //showOutline_GOs_InHierarchy

        //overlay
        A_SO, //active_SO
        OIL_SO, //orderInLayer_SO
        C_SO, //color_SO

        //clipping Mask
        CC_CM, //clipCenter_CM
        AC_CM, //alphaCutoff_CM
        CR_CM, //customRange_CM
        FL_CM, //frontLayer_CM
        BL_CM, //backLayer_CM

        //outline
        A_O, //active_O
        C_O, //color_O
        OIL_O, //orderInLayer_O
        SWPX_O, //scaleWithParentX_O
        SWPY_O, //scaleWithParentY_O

        ST_O, //spriteType_O

        S_O, //size_O

        //-----conCAVE

        PT_O_CAVE, //pushType_O_CAVE

        //ONLY regular
        EC_O_CAVE_R, //edgeCount_O_CAVE_R

        //BOTH
        SS_O_CAVE, //stdSize_O_CAVE
        R_O_CAVE //rotation_O_CAVE
    };

    public enum spriteUpdateSetting { EveryFrame, AfterEveryChange, Manually }

    public enum spriteType { conCAVE, conVEX }; //"SpriteType_O" and the first value of the "spriteType" enum MUST NOT MATCH for things to work properly

    //ONLY for conCAVE outline
    public enum pushPattern { radial, custom };

    //--------------------------------------------------PARENT CLASS--------------------------------------------------

    //NOTE: hack inits dont really need to be serialized by we do so to simplify code

    [System.Serializable, ExecuteInEditMode]
    public class outline : MonoBehaviour
    {
        //-----Optimization Variable-----

        [Header("OPTIMIZATION VARIABLES-----")]
        [SerializeField, HideInInspector]
        internal spriteUpdateSetting updateSprite;
        public spriteUpdateSetting UpdateSprite
        {
            get { return updateSprite; }
            set { updateSprite = value; }
        }

        //-----Debugging Variables-----

        [SerializeField, HideInInspector]
        internal GameObject outlineGameObjectsFolder; //contains all the gameobjects used by the script

        [Header("DEBUGGING VARIABLES-----")]
        [SerializeField, HideInInspector]
        internal bool showOutline_GOs_InHierarchy;
        public bool ShowOutline_GOs_InHierarchy
        {
            get { return showOutline_GOs_InHierarchy; }
            set
            {
                showOutline_GOs_InHierarchy = value; 

                if (showOutline_GOs_InHierarchy)
                    outlineGameObjectsFolder.hideFlags = HideFlags.None;
                else
                    outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        //-----Overlay Variables-----

        [SerializeField, HideInInspector]
        internal GameObject spriteOverlay; //the sprite overlay gameobject

        [Header("SPRITE OVERLAY VARIABLES-----")]
        [SerializeField, HideInInspector]
        internal bool active_SO;
        public bool Active_SO
        {
            get { return active_SO; }
            set
            {
                active_SO = value; 

                _newActiveSO = true;
            }
        }
        [SerializeField, HideInInspector]
        private bool _newActiveSO;

        [SerializeField, HideInInspector]
        internal int orderInLayer_SO;
        public int OrderInLayer_SO
        {
            get { return orderInLayer_SO; }
            set
            {
                orderInLayer_SO = value; 

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
                color_SO = value; 

                spriteOverlay.GetComponent<SpriteRenderer>().color = color_SO;
            }
        }

        //-----Clipping Mask Variables-----

        [SerializeField, HideInInspector]
        internal GameObject clippingMask; //the gameobject with sprite mask

        [SerializeField, HideInInspector]
        internal float alphaCutoff_CM;
        public float AlphaCutoff_CM
        {
            get { return alphaCutoff_CM; }
            set
            {
                alphaCutoff_CM = Mathf.Clamp01(value); 

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
                customRange_CM = value; 

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
                frontLayer_CM = value; 

                clippingMask.GetComponent<SpriteMask>().frontSortingOrder = frontLayer_CM;
            }
        }

        [SerializeField, HideInInspector]
        internal int backLayer_CM;
        public int BackLayer_CM
        {
            get { return backLayer_CM; }
            set
            {
                backLayer_CM = value; 

                clippingMask.GetComponent<SpriteMask>().backSortingOrder = backLayer_CM;
            }
        }

        //assign good values to inspector
        public void ManualReset()
        {
            //----------Hack Inits (simplifies code bit by serializing eventhough not needed)
            _newActiveSO = false;

            //----------Variable Inits

            //-----Optimization
            UpdateSprite = spriteUpdateSetting.AfterEveryChange;

            //-----Debugging
            ShowOutline_GOs_InHierarchy = false;

            //-----Sprite Overlay
            Active_SO = false;
            OrderInLayer_SO = this.GetComponent<SpriteRenderer>().sortingOrder + 1; //by default in front
            Color_SO = new Color(0, 0, 1, .5f);

            //-----Clipping Mask
            AlphaCutoff_CM = .25f;
            CustomRange_CM = false;
            FrontLayer_CM = 0; //by defaults maps to "default" layer
            BackLayer_CM = 0; //by defaults maps to "default" layer
        }

        public void Update()
        {
            //required hacks because of warnings
            if (_newActiveSO)
            {
                spriteOverlay.GetComponent<SpriteRenderer>().enabled = active_SO;
                _newActiveSO = false;
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
            for (int childID = 0; childID < children.Count; childID++)
                theSwitch(varEnum, parent.GetComponent<spriteOutline>(), children[childID].GetComponent<spriteOutline>());
        }

        //-------------------------Switch Case

        public static void theSwitch(varToUpdate varEnum, spriteOutline par, spriteOutline child)
        {
            switch (varEnum)
            {
                //Optimization
                case varToUpdate.US: child.UpdateSprite = par.UpdateSprite; break;

                //Debugging
                case varToUpdate.SOGIH: child.ShowOutline_GOs_InHierarchy = par.ShowOutline_GOs_InHierarchy; break;

                //Overlay
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

                case varToUpdate.ST_O: child.SpriteType_O = par.SpriteType_O; break;

                case varToUpdate.S_O: child.Size_O = par.Size_O; break;

                //-----conCAVE

                case varToUpdate.PT_O_CAVE: child.PatternType_O_CAVE = par.PatternType_O_CAVE; break;

                //ONLY radial
                case varToUpdate.EC_O_CAVE_R: child.EdgeCount_O_CAVE_R = par.EdgeCount_O_CAVE_R; break;

                //BOTH
                case varToUpdate.SS_O_CAVE: child.StdSize_O_CAVE = par.StdSize_O_CAVE; break;
                case varToUpdate.R_O_CAVE: child.Rotation_O_CAVE = par.Rotation_O_CAVE; break;
            }
        }
    }
}