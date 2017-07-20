using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class duplicator : MonoBehaviour {

    public GameObject objToDuplicate;

	// Use this for initialization
	void Start () {
        StartCoroutine("testing");
	}

    IEnumerator testing()
    {
        int duplicateQuantity = 0;

        while( 1 == 1)
        {
            Instantiate(objToDuplicate);

            yield return new WaitForEndOfFrame();
            duplicateQuantity++;

            print("count: " + duplicateQuantity);
        }
    }
}
