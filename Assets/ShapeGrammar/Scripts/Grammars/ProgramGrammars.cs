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
        g.inputs.names.Add("APT");
        g.AddRule(new Rules.DivideToFTFH("APT", new string[] { "APTL"}, 4), false);
        g.AddRule(new Rules.DcpFlrToUnitsReal(new string[] { "APTL" },"UNIT"), false);
        return g;
    }
}
