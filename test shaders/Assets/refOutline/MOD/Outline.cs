using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[DisallowMultipleComponent]

[RequireComponent(typeof(Renderer))]

public class Outline : MonoBehaviour
{
    //--- references

    public Renderer Renderer { get; private set; }

    //--- Vars for KNOWN outlineEffect use

    /* TODO (last step)... outlineIndex... index refers to a particular outline in the outlineEffects Scripts... 
     * every update we search through all those scripts and update with the values that have been changed
     * IF our outline index CHANGES (this doesnt run all the time...) then all our variables update to that of the outline index OR some default... If the outline index DNE
     * IF any of our variables change then only those variables are update in the outlineManager
     */

    //TODO (step 1)... include all the variables from outline effect...

    public int color;
    public bool eraseRenderer;

    //--- vars for UNKOWN outlineEffect use

    [HideInInspector]
    public int originalLayer; //this seems to be updated before rendering so but it updates every frame... chose more accurate name...
    [HideInInspector]
    public Material[] originalMaterials; //this seems to be updated before rendering so but it updates every frame... chose more accurate name...

    void Awake()
    {
        Renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        //--- Covering Exceptions

        //(0) -> Given that this Outline Script will not work unless a manager exists... make sure a manager exists
        if(findAllOutlineManagerScripts().Count<OutlineManager>() == 0)
            Camera.main.gameObject.AddComponent<OutlineManager>();
        
        //(1) -> Camera Deleted during runtime

        //(2) -> Manager Script Deleted during runtime

        //(3) -> Outline Deleted during runtime

        //(4) -> Outline Script deleted during runtime

        foreach (OutlineManager effect in findAllOutlineManagerScripts())
            ; // do stuff...
    }

    //add THIS class to ALL cameras with OutlineManager Script
    void OnEnable()
    {
        foreach (OutlineManager effect in findAllOutlineManagerScripts())
            effect.AddOutline(this);
    }

    //remove THIS class from ALL cameras with OutlineManager Script
    void OnDisable()
    {
        foreach (OutlineManager effect in findAllOutlineManagerScripts())
            effect.RemoveOutline(this);
    }

    IEnumerable<OutlineManager> findAllOutlineManagerScripts()
    {
        return Camera.allCameras.AsEnumerable().Select(camera => camera.GetComponent<OutlineManager>()).Where(effect => effect != null);
    }
}