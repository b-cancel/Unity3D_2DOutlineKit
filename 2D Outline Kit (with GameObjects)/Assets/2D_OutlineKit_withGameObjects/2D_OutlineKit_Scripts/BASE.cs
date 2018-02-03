using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace object2DOutlines
{
    //---This Enum makes it easier for us to pass our variables to our children (helps keep code clean)
    enum varToUpdate
    {
        //----------Variables For All Outline Types

        //-----Optimization
        USEF, //UpdateSpriteEveryFrame
        //-----Debugging
        SOGIH, //ShowOutline_GOs_InHierarchy_D
        //-----Sprite Overlay
        A_SO, //Active_SO
        OIL_SO, //OrderInLayer_SO
        C_SO, //Color_SO
        //-----Clipping Mask
        CC_CM, //ClipCenter_CM
        AC_CM, //AlphaCutoff_CM
        CR_CM, //CustomRange_CM
        FL_CM, //FrontLayer_CM
        BL_CM, //BackLayer_CM
        //-----Sprite Outline
        A_O, //Active_O
        C_O, //Color_O
        OIL_O, //OrderInLayer_O
        S_O, //Size_O
        SWPX_O, //ScaleWithParentX_O
        SWPY_O //ScaleWithParentY_O

        //----------Variables For ONLY (Concave/Push) Outline Type

        //NONE
    };

    public class BASE
    {
        //-------------------------parent child reltionship code[PCRC]-------------------------

        //NOTE: according to microsoft delegates are x8 time slower than methods... so eventually make sure that the switch isnt converting the code and using it as a sort of delegate
        //I.O.W. make sure the switch case isnt become a dictionary from Enums -> Delegates when compiled... cuz that would be really slow
        void passToChildren(varToUpdate varEnum)
        {
            for (int i = 0; i < children.Count; i++) //loop through all of our children
            {
                if (children[i] != null) //if this child still exists
                {
                    //TODO... iterate through different script with these same variables "convexC" and "concaveC" and possible others
                    //replace if statemetn for "for loop"

                    if (children[i].GetComponent<convexC>() != null)
                    {
                        switch (varEnum)
                        {
                            //Optimization
                            case varToUpdate.USEF: children[i].GetComponent<convexC>().UpdateSpriteEveryFrame = UpdateSpriteEveryFrame; break;
                            //Debugging
                            case varToUpdate.SOGIH: children[i].GetComponent<convexC>().ShowOutline_GOs_InHierarchy_D = ShowOutline_GOs_InHierarchy_D; break;
                            //Sprite Overlay
                            case varToUpdate.A_SO: children[i].GetComponent<convexC>().Active_SO = Active_SO; break;
                            case varToUpdate.OIL_SO: children[i].GetComponent<convexC>().OrderInLayer_SO = OrderInLayer_SO; break;
                            case varToUpdate.C_SO: children[i].GetComponent<convexC>().Color_SO = Color_SO; break;
                            //Clipping Mask
                            case varToUpdate.CC_CM: children[i].GetComponent<convexC>().ClipCenter_CM = ClipCenter_CM; break;
                            case varToUpdate.AC_CM: children[i].GetComponent<convexC>().AlphaCutoff_CM = AlphaCutoff_CM; break;
                            case varToUpdate.CR_CM: children[i].GetComponent<convexC>().CustomRange_CM = CustomRange_CM; break;
                            case varToUpdate.FL_CM: children[i].GetComponent<convexC>().FrontLayer_CM = FrontLayer_CM; break;
                            case varToUpdate.BL_CM: children[i].GetComponent<convexC>().BackLayer_CM = BackLayer_CM; break;
                            //Sprite Outline
                            case varToUpdate.A_O: children[i].GetComponent<convexC>().Active_O = Active_O; break;
                            case varToUpdate.C_O: children[i].GetComponent<convexC>().Color_O = Color_O; break;
                            case varToUpdate.OIL_O: children[i].GetComponent<convexC>().OrderInLayer_O = OrderInLayer_O; break;
                            case varToUpdate.S_O: children[i].GetComponent<convexC>().Size_O = Size_O; break;
                            case varToUpdate.SWPX_O: children[i].GetComponent<convexC>().ScaleWithParentX_O = ScaleWithParentX_O; break;
                            case varToUpdate.SWPY_O: children[i].GetComponent<convexC>().ScaleWithParentY_O = ScaleWithParentY_O; break;
                        };
                    }
                }
                else
                {
                    children.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}