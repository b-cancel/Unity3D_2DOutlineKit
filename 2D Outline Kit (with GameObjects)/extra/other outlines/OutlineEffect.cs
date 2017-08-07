using UnityEngine;
using System.Collections;

public enum OutlineColor {
	Color0,
	Color1,
	Color2,
	Color3
}

public class OutlineEffect : MonoBehaviour {
    public GameObject TargetMesh;
	public OutlineColor OutlineColor = OutlineColor.Color0;
	public bool Active { get; set; }
}
