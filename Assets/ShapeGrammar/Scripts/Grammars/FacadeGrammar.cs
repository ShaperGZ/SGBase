using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class FacadeGrammars {
    public static Grammar CW01()
    {
        Grammar g = new Grammar();
        g.name = "CW01";
        g.category = GraphNode.Category.Bd_Graphics;
        g.inputs.names.Add("APT");
        g.AddRule(new Rules.ExtractFace(new string[] { "APT" }, "ROOF", "TOP"));
        g.AddRule(new Rules.Colorize(new string[] { "ROOF" }, new Color(130f/255f, 218f/255f, 80f/255f)));
        g.AddRule(new Rules.DivideToFTFH("APT", new string[] { "APTL", "APTLM" }, 4), false);
        g.AddRule(new Rules.PivotMirror("APTLM", "APTL", 0), false);
        g.AddRule(new Rules.DcpFace5("APTL", new string[] { "F", "DELETE", "S", "DELETE", "BT" }), false);
        g.AddRule(new Rules.Extrude("BT", "BT", -0.1f), false);
        g.AddRule(new Rules.AggCW01("F", "F", 3, 4), false);
        g.AddRule(new Rules.AggBasicSimp("S", "S", 3, 4), false);
        return g;
    }
    public static Grammar CW02()
    {
        Grammar g = new Grammar();
        g.name = "CW01";
        g.category = GraphNode.Category.Bd_Graphics;
        g.inputs.names.Add("APT");
        g.AddRule(new Rules.ExtractFace(new string[] { "APT" }, "ROOF", "TOP"));
        g.AddRule(new Rules.Colorize(new string[] { "ROOF" }, new Color(130f / 255f, 218f / 255f, 80f / 255f)));
        g.AddRule(new Rules.DivideToFTFH("APT", new string[] { "APTL", "APTLM" }, 4), false);
        g.AddRule(new Rules.PivotMirror("APTLM", "APTL", 0), false);
        g.AddRule(new Rules.DcpFace5("APTL", new string[] { "F", "DELETE", "S", "DELETE", "BT" }), false);
        g.AddRule(new Rules.Extrude("BT", "BT", -0.1f), false);
        g.AddRule(new Rules.AggCW02("F", "F", 6, 4), false);
        g.AddRule(new Rules.AggCW02("S", "S", 6, 4), false);
        //g.AddRule(new Rules.AggBasicSimp("S", "S", 3, 4), false);
        return g;
    }
}
