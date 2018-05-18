using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class ProgramGrammars : MonoBehaviour {

	public static Grammar APT1()
    {
        Grammar g = new Grammar();
        g.name = "APT1";
        g.inputs.names.Add("APT2");
        g.inputs.names.Add("APT");
        g.inputs.names.Add("STA");
        g.inputs.names.Add("CD");
        g.AddRule(new Rules.Hide(new string[] { "STA"}),false);
        g.AddRule(new Rules.Hide(new string[] { "CD"}),false);
        g.AddRule(new Rules.DivideToFTFH("APT", new string[] { "APTL"}, 4), false);
        g.AddRule(new Rules.DivideToFTFH("APT2", new string[] { "APTL2"}, 4), false);
        g.AddRule(new Rules.DcpFlrToUnitsReal(new string[] { "APTL","APTL2" },"UNIT"), false);
        g.AddRule(new Rules.UpdateBuildingParamDisplay("UNIT"),false);
        return g;
    }
}
