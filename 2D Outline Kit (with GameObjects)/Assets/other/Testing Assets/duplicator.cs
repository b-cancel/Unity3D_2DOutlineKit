using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class duplicator : MonoBehaviour {

    public GameObject objToDuplicate;
    int duplicateQuantity;

	// Use this for initialization
	void Awake () {
        duplicateQuantity = 0;
	}

    void Update()
    {
        if(objToDuplicate != null)
        {
            float fps = 1.0f / Time.deltaTime;
            if (fps < 5)
                print("fps: " + fps + "----------!");
            else
                print("fps: " + fps);

            if (Input.GetKey(KeyCode.Space) == false)
            {
                print(duplicateQuantity);

                Instantiate(objToDuplicate);

                duplicateQuantity++;
            }
        }
    }
}
