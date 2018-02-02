using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using objOutlines;

public class crazySettings : MonoBehaviour {

    GameObject crazyOutlineObj;

    // Use this for initialization
    void Start () {

        crazyOutlineObj = gameObject;

        StartCoroutine("crazyTransforms");
    }

    /*IEnumerator crazyTransforms()
    {
        while (1 == 1)
        {
            //---TARGET

            //transform vars

            float targetPosX = Random.Range(-7.5f, 7.5f);
            float targetPosY = Random.Range(-7.5f, 7.5f);

            float targetScaleX = Random.Range(.1f, 2.5f);
            float targetScaleY = Random.Range(.1f, 2.5f);

            float targetRotZ = Random.Range(-180, 180);

            //outline vars

            Color targetSOcolor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
            Color targetOcolor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
            float targetOSize = Random.Range(0f, 2.5f);

            //---START

            //transform vars

            float startPosX = crazyOutlineObj.transform.position.x;
            float startPosY = crazyOutlineObj.transform.position.y;

            float startScaleX = crazyOutlineObj.transform.localScale.x;
            float startScaleY = crazyOutlineObj.transform.localScale.y;

            float startRotZ = crazyOutlineObj.transform.rotation.eulerAngles.z;

            //outline vars

            Color startSOcolor = crazyOutlineObj.GetComponent<concaveOutlineV4>().Color_SO;
            Color startOColor = crazyOutlineObj.GetComponent<concaveOutlineV4>().Color_O;
            float startOSize = crazyOutlineObj.GetComponent<concaveOutlineV4>().Size_O;

            //---TRAVEL FROM START -TO- TARGET

            //non lerping vars

            crazyOutlineObj.GetComponent<concaveOutlineV4>().Active_SO = !crazyOutlineObj.GetComponent<concaveOutlineV4>().Active_SO; //turn the sprite overlay (on or off)
            crazyOutlineObj.GetComponent<concaveOutlineV4>().ScaleWithParentX_O = !crazyOutlineObj.GetComponent<concaveOutlineV4>().ScaleWithParentX_O; //turn scale with parent x (on or off)
            crazyOutlineObj.GetComponent<concaveOutlineV4>().ScaleWithParentY_O = !crazyOutlineObj.GetComponent<concaveOutlineV4>().ScaleWithParentY_O; //turn scale with parent y (on or off)
            crazyOutlineObj.GetComponent<concaveOutlineV4>().RadialPush_OPR = !crazyOutlineObj.GetComponent<concaveOutlineV4>().RadialPush_OPR; //turn radial push (on or off)
            crazyOutlineObj.GetComponent<concaveOutlineV4>().ObjsMakingOutline_OPR = Random.Range(1, 12); //change the number of objects that make the outline

            //lerping vars

            float lerpValue = 0f;
            float incrementer = .01f;

            do
            {
                lerpValue += incrementer;
                lerpValue = Mathf.Clamp01(lerpValue);

                //transform lerps
                float posX = Mathf.Lerp(startPosX, targetPosX, lerpValue);
                float posY = Mathf.Lerp(startPosY, targetPosY, lerpValue);
                float posZ = 0;

                float scaleX = Mathf.Lerp(startScaleX, targetScaleX, lerpValue);
                float scaleY = Mathf.Lerp(startScaleY, targetScaleY, lerpValue);

                float rotZ = Mathf.Lerp(startRotZ, targetRotZ, lerpValue);

                //transform updates
                crazyOutlineObj.transform.position = new Vector3(posX, posY, posZ);
                crazyOutlineObj.transform.localScale = new Vector3(scaleX, scaleY, 1);
                crazyOutlineObj.transform.rotation = Quaternion.Euler(0, 0, rotZ);

                //variable lerps and updates
                crazyOutlineObj.GetComponent<concaveOutlineV4>().Color_O = Color.Lerp(startOColor, targetOcolor, lerpValue);
                crazyOutlineObj.GetComponent<concaveOutlineV4>().Color_SO = Color.Lerp(startSOcolor, targetSOcolor, lerpValue);
                crazyOutlineObj.GetComponent<concaveOutlineV4>().Size_O = Mathf.Lerp(startOSize, targetOSize, lerpValue);

                yield return new WaitForEndOfFrame();

            } while (lerpValue < 1);
        }
    }*/
}
