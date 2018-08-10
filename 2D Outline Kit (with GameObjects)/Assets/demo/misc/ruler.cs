using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ruler : MonoBehaviour {

    public GameObject knob1;
    public GameObject knob2;

    public Vector2 distances;
	
	// Update is called once per frame
	void Update () {
        Vector2 newDist = knob1.transform.position - knob2.transform.position;
        newDist.x = Mathf.Abs(newDist.x);
        newDist.y = Mathf.Abs(newDist.y);
        distances = newDist;
	}
}
