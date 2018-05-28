using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class MassingGrammars : MonoBehaviour {

    public static Grammar GA(bool addToScene = true)
    {
        Grammar g = new Grammar(addToScene);
        g.category = GraphNode.Category.Bd_Massing;
        g.inputs.names.Add("APT");
        g.inputs.names.Add("APT2");
        g.inputs.names.Add("STA");
        g.inputs.names.Add("CD");
        g.inputs.names.Add("DHUS");
        g.name = "AptFormA";
        g.AddRule(new Rules.Bisect("APT", new string[] { "APT", "C" }, 0.4f, 0), false);
        g.AddRule(new Rules.Bisect("C", new string[] { "C", "D" }, 0.7f, 1), false);
        g.AddRule(new Rules.Scale3D("C", "APT", new Vector3(1.3f, 1f, 1.2f), null, Alignment.NE), false);
        g.AddRule(new Rules.Scale3D("D", "APT", new Vector3(1f, 1f, 0.8f), null, Alignment.NE), false);
        g.AddRule(new Rules.ResitHouse("DHUS", "DHUS"),false);
        g.AddRule(new Rules.Scale3D("DHUS", "DHUS", new Vector3(1,1,1),null,Alignment.NE), false);
        return g;
    }
    public static Grammar GB(bool addToScene = true)
    {
        Grammar g = new Grammar(addToScene);
        g.category = GraphNode.Category.Bd_Massing;
        g.inputs.names.Add("APT");
        g.inputs.names.Add("DHUS");
        g.name = "AptFormB";
        g.AddRule(new Rules.DivideTo("APT", "APT", 30, 0));
        g.AddRule(new Rules.Divide("APT", new string[] { "APT", "B" }, new float[] { 0.2f, 0.6f, 0.2f }, 0), false);
        //g2.AddRule(new Rules.DivideTo("APT", "APT_B" , 8,0));

        //g2.AddRule(new Rules.PivotMirror("APT_B", "APT_B", 2));
        g.AddRule(new Rules.Scale3D("B", "B", new Vector3(1f, 1.1f, 1.2f), null, Alignment.N), false);
        g.AddRule(new Rules.Bisect("B", new string[] { "B", "APT" }, 0.4f, 1), false);
        g.AddRule(new Rules.Scale3D("B", "APT", new Vector3(1.2f, 1f, 1.2f), null, Alignment.N), false);
        g.AddRule(new Rules.ResitHouse("DHUS", "DHUS"), false);
        g.AddRule(new Rules.Scale3D("DHUS", "DHUS", new Vector3(1, 1, 1), null, Alignment.NE), false);
        return g;
    }
    public static Grammar GC(bool addToScene = true)
    {
        Grammar g = new Grammar(addToScene);
        g.category = GraphNode.Category.Bd_Massing;
        g.inputs.names.Add("APT");
        g.inputs.names.Add("DHUS");
        g.name = "AptFormC";
        //g2.AddRule(new Rules.Divide("APT", new string[] { "APT_A", "APT_B" }, new float[] { 0.2f, 0.6f, 0.2f }, 0), false);
        //g2.AddRule(new Rules.DivideTo("APT", "APT_B" , 8,0));
        g.AddRule(new Rules.Bisect("APT", new string[] { "APT", "C" }, 0.6f, 1), false);
        g.AddRule(new Rules.Bisect("C", new string[] { "C", "APT" }, 0.35f, 0), false);
        g.AddRule(new Rules.Scale3D("C", "C", new Vector3(1.05f, 1, 1.05f), null, Alignment.NE), false);
        g.AddRule(new Rules.BisectLength("C", new string[] { "C", "APT" }, 4, 1), false);
        g.AddRule(new Rules.Scale3D("C", "APT", new Vector3(0.8f, 1, 0.8f), null, Alignment.NE), false);
        g.AddRule(new Rules.ResitHouse("DHUS", "DHUS"), false);
        g.AddRule(new Rules.Scale3D("DHUS", "DHUS", new Vector3(1, 1, 1), null, Alignment.NE), false);
        return g;
    }
}
