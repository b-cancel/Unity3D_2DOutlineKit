using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class duplicator : MonoBehaviour {

    public GameObject objToDuplicate;
    int duplicateQuantity;

    int counter;

	// Use this for initialization
	void Awake () {
        counter = 0;
        duplicateQuantity = 0;
	}

    
    void Update()
    {
        if(objToDuplicate != null)
        {
            if (counter < 15)
            {
                float fps = 1.0f / Time.deltaTime;
                if (fps < 15)
                {
                    print("fps: " + fps + "----------!");
                    counter++;
                }
                else
                {
                    print("fps: " + fps);
                    counter = 0;
                }

                if (Input.GetKey(KeyCode.Space) == false)
                {
                    print(duplicateQuantity);

                    Instantiate(objToDuplicate);

                    duplicateQuantity++;
                }
            }
            else
            {
                print("done with: " + duplicateQuantity);
                Time.timeScale = 0;
            }
        }
    }
}
