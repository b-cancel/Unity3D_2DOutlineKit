using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using objOutlines;

public class changeValues : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        StartCoroutine("test");
	}

    IEnumerator test()
    {
        while (1 == 1)
        {
            gameObject.GetComponent<outline3>().Size_O = .1f;
            yield return new WaitForSeconds(1);
            gameObject.GetComponent<outline3>().Size_O = 1f;
            yield return new WaitForSeconds(1);
        }


    }
}
