using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutlineRenderer : LocatableByType<OutlineRenderer>
{
    public Shader outlineObjectShader;
    public Shader outlineBlurShader;
    public Shader outlineBlendShader;

	private Material outlineObjectMaterial;
    private Material outlineBlurMaterial;
    private Material outlineBlendMaterial;
	
	public Color color0;
	public Color color1;
	public Color color2;
	public Color color3;

	private RenderTexture outlineBuffer;
	private RenderTexture outlineBuffer2;

	// Use this for initialization
	void Start () {
        outlineObjectMaterial = new Material(outlineObjectShader);
        outlineBlurMaterial = new Material(outlineBlurShader);
        outlineBlendMaterial = new Material(outlineBlendShader);

		var camera = Camera.main;
		camera.depthTextureMode = DepthTextureMode.DepthNormals;
		
		outlineBuffer = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 16);
		outlineBuffer.Create();
		
		outlineBuffer2 = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 16);
		outlineBuffer2.Create();
	}

	void OnRenderImage (RenderTexture src, RenderTexture dest) {
        var outlineEffects = GameObject.FindObjectsOfType<OutlineEffect>();

		var lastActiveRenderTexture = RenderTexture.active;
		var camera = Camera.main;

		if(camera.pixelWidth != outlineBuffer.width || camera.pixelHeight != outlineBuffer.height)
		{
			outlineBuffer.Release();
			outlineBuffer.width = camera.pixelWidth;
			outlineBuffer.height = camera.pixelHeight;
			outlineBuffer.Create();
			
			outlineBuffer2.Release();
			outlineBuffer2.width = camera.pixelWidth;
			outlineBuffer2.height = camera.pixelHeight;
			outlineBuffer2.Create();
		}

		RenderTexture.active = outlineBuffer2;
		GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));

		RenderTexture.active = outlineBuffer;
		GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));

		foreach(var outlineEffect in outlineEffects)
		{
            if (!outlineEffect.Active)
                continue;

			switch(outlineEffect.OutlineColor)
			{
			case OutlineColor.Color0:
				outlineObjectMaterial.SetVector("weights", new Vector4(1.0f, 0f, 0f, 0f));
				break;
			case OutlineColor.Color1:
				outlineObjectMaterial.SetVector("weights", new Vector4(0f, 1.0f, 0f, 0f));
				break;
			case OutlineColor.Color2:
				outlineObjectMaterial.SetVector("weights", new Vector4(0f, 0f, 1.0f, 0f));
				break;
			case OutlineColor.Color3:
				outlineObjectMaterial.SetVector("weights", new Vector4(0f, 0f, 0f, 1.0f));
				break;
			}

			//needs to be set after all material properties have been set
			while(!outlineObjectMaterial.SetPass(0)) { Debug.Log("Could not render with material, repeating..."); }

			var go = outlineEffect.TargetMesh;
			var m = go.transform.localToWorldMatrix;
			var meshFilter = go.GetComponent<MeshFilter>();
			if(meshFilter != null) {
				Graphics.DrawMeshNow(meshFilter.sharedMesh, m);
			}else{
				var skinnedMeshRenderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
				if(skinnedMeshRenderer != null)
				{
					var mesh = new Mesh();
					skinnedMeshRenderer.BakeMesh(mesh);
					Graphics.DrawMeshNow(mesh, m);
				}
			}
		}

		Graphics.Blit(outlineBuffer, outlineBuffer2, outlineBlurMaterial, 0);
		Graphics.Blit(outlineBuffer2, outlineBuffer, outlineBlurMaterial, 1);

		outlineBlendMaterial.SetTexture("_OutlineBuffer", outlineBuffer);
		outlineBlendMaterial.SetColor("color0", color0);
		outlineBlendMaterial.SetColor("color1", color1);
		outlineBlendMaterial.SetColor("color2", color2);
		outlineBlendMaterial.SetColor("color3", color3);

        RenderTexture.active = lastActiveRenderTexture;
		Graphics.Blit(src, dest, outlineBlendMaterial);
	}
}
