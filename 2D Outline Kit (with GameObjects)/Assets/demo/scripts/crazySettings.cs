using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using objOutlines;

public class crazySettings : MonoBehaviour {

    GameObject anim;

    // Use this for initialization
    void Start () {

        anim = gameObject;

        StartCoroutine("crazyTransforms");
    }

    IEnumerator crazyTransforms()
    {
        while (1 == 1)
        {
            float lerpValue = .01f;
            float incrementer = .01f;

            //---TARGET

            //transforms

            float targetPosX = Random.Range(-7.5f, 7.5f);
            float targetPosY = Random.Range(-7.5f, 7.5f);
            float targetPosZ = Random.Range(-7.5f, 7.5f);

            float targetScaleX = Random.Range(.1f, 2.5f);
            float targetScaleY = Random.Range(.1f, 2.5f);

            float targetRotZ = Random.Range(-180, 180);

            Color targetSOcolor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
            Color targetOcolor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
            float targetSize = Random.Range(0f, 2.5f);

            //---START

            //transform

            float startPosX = anim.transform.position.x;
            float startPosY = anim.transform.position.y;
            float startPosZ = anim.transform.position.z;

            float startScaleX = anim.transform.localScale.x;
            float startScaleY = anim.transform.localScale.y;

            float startRotZ = anim.transform.rotation.eulerAngles.z;

            Color startSOcolor = anim.GetComponent<concaveOutline>().Color_SO;
            Color startOColor = anim.GetComponent<concaveOutline>().Color_O;
            float startSize = anim.GetComponent<concaveOutline>().Size_O; //0 -> 2

            //non lerping vars
            anim.GetComponent<concaveOutline>().Active_SO = !anim.GetComponent<concaveOutline>().Active_SO;
            anim.GetComponent<concaveOutline>().ScaleWithParentX_O = !anim.GetComponent<concaveOutline>().ScaleWithParentX_O;
            anim.GetComponent<concaveOutline>().ScaleWithParentY_O = !anim.GetComponent<concaveOutline>().ScaleWithParentY_O;
            anim.GetComponent<concaveOutline>().RadialPush_OPR = !anim.GetComponent<concaveOutline>().RadialPush_OPR;
            anim.GetComponent<concaveOutline>().ObjsMakingOutline_OPR = Random.Range(1, 12);

            //---Start to Target Lerp

            do
            {
                float posX = Mathf.Lerp(startPosX, targetPosX, Mathf.Clamp01(lerpValue));
                float posY = Mathf.Lerp(startPosY, targetPosY, Mathf.Clamp01(lerpValue));
                float posZ = Mathf.Lerp(startPosZ, targetPosZ, Mathf.Clamp01(lerpValue));

                float scaleX = Mathf.Lerp(startScaleX, targetScaleX, Mathf.Clamp01(lerpValue));
                float scaleY = Mathf.Lerp(startScaleY, targetScaleY, Mathf.Clamp01(lerpValue));

                float rotZ = Mathf.Lerp(startRotZ, targetRotZ, Mathf.Clamp01(lerpValue));

                Vector3 newPos = new Vector3(posX, posY, posZ);
                Vector3 newScale = new Vector3(scaleX, scaleY, 1);
                Quaternion newRot = Quaternion.Euler(0, 0, rotZ);

                //transform updates
                anim.transform.position = newPos;
                anim.transform.localScale = newScale;
                anim.transform.rotation = newRot;

                //lerping vars
                anim.GetComponent<concaveOutline>().Color_O = Color.Lerp(startOColor, targetOcolor, lerpValue);
                anim.GetComponent<concaveOutline>().Color_SO = Color.Lerp(startSOcolor, targetSOcolor, lerpValue);
                anim.GetComponent<concaveOutline>().Size_O = Mathf.Lerp(startSize, targetSize, lerpValue);

                yield return new WaitForEndOfFrame();

                lerpValue += incrementer;

            } while (lerpValue < 1);
        }
    }
}
