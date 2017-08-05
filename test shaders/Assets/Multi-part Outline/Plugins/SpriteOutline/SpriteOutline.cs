using UnityEngine;

namespace multiPartOutline
{
    [ExecuteInEditMode]
    public class SpriteOutline : MonoBehaviour
    {
        private static Material baseMaterial;

        [HideInInspector]
        [SerializeField]
        private SpriteRenderer outline;

        [SerializeField]
        private SpriteRenderer parent;

        [SerializeField]
        private float lineSize = 1f;
        [SerializeField]
        private Color lineColor = Color.white;
        [SerializeField]
        private bool scaleLine = true;

        private MaterialPropertyBlock materialBlock;
        private Vector2 currentSize = new Vector2(1f, 1f);
        private Vector2 objectSizeVector = new Vector2(1f, 1f);
        private float lineDepth = 99f;
        private Color cacheColor;

        private void Generate()
        {
            #region method
            materialBlock = new MaterialPropertyBlock();

            if (baseMaterial == null)
            {
                baseMaterial = new Material(Shader.Find("Sprites/SpriteOutline"));
            }
            if (parent == null)
            {
                parent = gameObject.GetComponent<SpriteRenderer>();
                if (parent == null)
                {
                    enabled = false;
                    Debug.LogWarning("outline has no parent.");
                }
            }
            if (outline == null)
            {
                outline = new GameObject().AddComponent<SpriteRenderer>();
                outline.material = baseMaterial;
                outline.gameObject.name = "outline";
                outline.gameObject.transform.position = new Vector3(0f, 0f, lineDepth);
                outline.gameObject.transform.SetParent(parent.transform, false);
                outline.color = lineColor;

            }
            ScaleObjectSize(parent.transform.localScale);
            outline.sprite = parent.sprite;
            outline.sortingLayerID = parent.sortingLayerID;
            outline.sortingOrder = parent.sortingOrder;

            #endregion
        }

        void Awake()
        {
            #region method
            Generate();
            #endregion
        }

        void OnValidate()
        {
            #region method
            if (outline != null)
            {
                if (materialBlock == null)
                {
                    materialBlock = new MaterialPropertyBlock();
                }
                SetLineSize(lineSize);
                SetColor(lineColor);
                if (scaleLine)
                {
                    objectSizeVector = new Vector4(1f, 1f);

                    outline.GetPropertyBlock(materialBlock);
                    materialBlock.SetVector("_ObjectSize", objectSizeVector);
                    outline.SetPropertyBlock(materialBlock);
                }
                else
                {
                    ScaleObjectSize(parent.transform.localScale);
                }
            }
            #endregion
        }

        void LateUpdate()
        {
            #region method
            if (parent == null)
            {
                return;
            }
            if (outline == null)
            {
                Generate();
            }
            if (!scaleLine)
            {
                ScaleObjectSize(parent.transform.localScale);
            }
            SetParentAlpha();
            outline.sprite = parent.sprite;
            outline.sortingLayerID = parent.sortingLayerID;
            outline.sortingOrder = parent.sortingOrder;
            #endregion
        }

        public void SetLineSize(float newLineSize)
        {
            #region method
            outline.GetPropertyBlock(materialBlock);
            materialBlock.SetFloat("_Distance", newLineSize);
            outline.SetPropertyBlock(materialBlock);
            #endregion
        }
        public void SetColor(Color newColor)
        {
            #region method
            lineColor = newColor;
            newColor.a *= parent.color.a;
            outline.color = newColor;
            #endregion
        }

        public void SetLineActive(bool status)
        {
            #region method
            outline.enabled = status;
            #endregion
        }
        private void SetParentAlpha()
        {
            #region method
            cacheColor = lineColor;
            cacheColor.a *= parent.color.a;
            outline.color = cacheColor;
            #endregion
        }
        private void ScaleObjectSize(Vector2 objectSize)
        {
            #region method
            if (scaleLine == true)
            {
                return;
            }
            if (objectSize.x == 0 || objectSize.y == 0 || currentSize == objectSize)
            {
                return;
            }

            currentSize = objectSize;
            objectSizeVector = new Vector4((float)1f / objectSize.x, (float)1f / objectSize.y, objectSize.x, objectSize.y);

            outline.GetPropertyBlock(materialBlock);
            materialBlock.SetVector("_ObjectSize", objectSizeVector);
            outline.SetPropertyBlock(materialBlock);

            #endregion
        }
    }
}