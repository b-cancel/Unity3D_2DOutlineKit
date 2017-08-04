using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace objOutlines
{
    [ExecuteInEditMode]
    public class outline1 : MonoBehaviour
    {
        GameObject Outline;

        [Range(1,2.5f)]
        public float size;

        public bool followParent;

        void Awake()
        {
            size = 1;
            followParent = true;

            //--- Cover Duplication Problem

            Transform psblOF_T = this.transform.Find("Our Outline");
            if (psblOF_T != null) //transform found
            {
                GameObject psblOF_GO = psblOF_T.gameObject;
                if (psblOF_GO.transform.parent.gameObject == gameObject) //this gameobject ours
                    DestroyImmediate(psblOF_GO);
            }

            Outline = new GameObject("Our Outline");
            Outline.AddComponent<SpriteRenderer>();
            copyOriginalGO_Transforms(Outline);
            //assign our parent
            Outline.transform.parent = this.transform;
            //use text shader so that we only conserve the silhouette of our sprite
            var tempMaterial = new Material(Outline.GetComponent<SpriteRenderer>().sharedMaterial);
            tempMaterial.shader = Shader.Find("GUI/Text Shader");
            Outline.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;
            //set color
            Outline.GetComponent<SpriteRenderer>().color = Color.white;
            //set sorting layer
            Outline.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
            //set sprite renderer data
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), Outline.GetComponent<SpriteRenderer>());
        }

        void Update()
        {
            float localSize;
            if (followParent)
                localSize = size;
            else
                localSize = size / this.transform.localScale.x;

            Outline.transform.localScale = new Vector3(localSize, localSize, 1);
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

            print("x: " + to.size.x + " y: " + to.size.y);

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