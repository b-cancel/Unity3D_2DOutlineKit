using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace object2DOutlines
{
    [System.Serializable, ExecuteInEditMode]
    public class testOut : MonoBehaviour
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

        [SerializeField, HideInInspector]
        private bool awakeFinished_VEX;

        void OnValidate()
        {
            if (awakeFinished_VEX)
            {
                //Optimization
                UpdateSprite = updateSprite;

                //Debugging
                ShowOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

                //Sprite Overlay
                Active_O = active_O;
                Color_O = color_O;
            }
        }

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
                active_O = value;//update local value

                thisOutline.SetActive(active_O);
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

                thisOutline.GetComponent<SpriteRenderer>().color = color_O;
            }
        }

        void Awake()
        {
            awakeFinished_VEX = false;

            thisOutline = new GameObject("The Outline");
            Material tempMaterial = initPart1(gameObject, ref thisOutline, ref outlineGameObjectsFolder);
            thisOutline.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial; //DIFFERENT
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), thisOutline.GetComponent<SpriteRenderer>()); //DIFFERENT

            //----------Variable Inits

            //---Outline Vars
            Active_O = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size
            //Color_O = Color.red;

            //----------Variable Inits

            //--- Optimization
            //updateSprite = spriteUpdateSetting.AfterEveryChange;

            //----- Debugging
            ShowOutline_GOs_InHierarchy_D = false;

            awakeFinished_VEX = true;
        }

        //-------------------------UNIQUE CODE-------------------------

        void Update()
        {
            switch (UpdateSprite)
            {
                case spriteUpdateSetting.EveryFrame: updateSpriteData(); break;
                case spriteUpdateSetting.AfterEveryChange:
                    if (spriteChanged(this.GetComponent<SpriteRenderer>()))
                        updateSpriteData();
                    break;
            }
        }

        public void updateSpriteData()
        {
            //update outline
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), thisOutline.GetComponent<SpriteRenderer>());

            updateOutline();
        }

        void updateOutline()
        {
            thisOutline.transform.localScale = new Vector3(2, 2, 0);
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
            else if (prevDrawMode != SR.drawMode)
                return updateAllPrevs(SR, true);
            else if (prevSize != SR.size)
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
            copyTransform(main, thisOutline);
            thisOutline.transform.parent = folder.transform;
            thisOutline.AddComponent<SpriteRenderer>();
            var tempMaterial = new Material(thisOutline.GetComponent<SpriteRenderer>().sharedMaterial);
            tempMaterial.shader = Shader.Find("GUI/Text Shader");

            return tempMaterial;
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
    }
}