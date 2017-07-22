using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ultimate_2D_script : MonoBehaviour {

    public Material prefabMaterial;

    private SpriteRenderer spriteRenderer;

    /*
     * alpha cut off var between 0 and 1
     * in pass "AlphaTest Greater [_Cutoff]"
     */

    [Header("Overides to Shader Tags")] //does so by overide the render order on the material

    [Tooltip("Overlay(4,000) | Transparent(3,000) | AlphaTest(2,450) | Geometry(2,000) | Background(1,000)")]
    [Range(0, 5000)]
    public int renderQueue; //Queue Tag

    [Header("Overides to Shader Properties")]

    [Tooltip("Effective Outline Size Range [0, Infinity)")]
    public int outlineSize = 1;
    public Color color = Color.white;

    void OnAwake()
    {
        //this creates the material we need on runtime if we dont have it... AND... allows us to have different render queue in every copy of our source material
        gameObject.GetComponent<SpriteRenderer>().sharedMaterial.CopyPropertiesFromMaterial(prefabMaterial);
    }

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateOutline(true);
    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void Update()
    {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline)
    {
        //--- Override Shader tags by changing the material

        gameObject.GetComponent<SpriteRenderer>().sharedMaterial.renderQueue = renderQueue;

        //--- Update Variables in Material Property Block

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
