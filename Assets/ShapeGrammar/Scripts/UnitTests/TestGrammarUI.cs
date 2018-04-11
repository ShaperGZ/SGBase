using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;
using SGGUI;
using System.Linq;
using UnityEngine.UI;


public class TestGrammarUI : MonoBehaviour {
    Grammar g1;
    Text stageIOText;
    RuleParamEditor ruleParamEditor;
    // Use this for initialization

    public static Vector3[] initShape1()
    {
        Vector3[] pts = new Vector3[5];
        pts[0] = new Vector3(-80, 0, 46);
        pts[1] = new Vector3(-80, 0, -11);
        pts[2] = new Vector3(-10, 0, -54);
        pts[3] = new Vector3(103, 0, 25);
        pts[4] = new Vector3(96, 0, 70);
        return pts;
    }
    public static Vector3[] initShape2()
    {
        Vector3[] pts = new Vector3[4];
        pts[0] = new Vector3(-20, 0, 15);
        pts[1] = new Vector3(-20, 0, -15);
        pts[2] = new Vector3(20, 0, -15);
        pts[3] = new Vector3(20, 0, 15);
        return pts;
    }


    public static void Rule1(Grammar g1)
    {
        g1.AddRule(new Rules.CreateBox("A", new Vector3(), new Vector3(60, 30, 18), new Vector3()), false);
        g1.AddRule(new Rules.Scale("A", "A", 2f, 0), false);
        g1.AddRule(new Rules.DivideTo("A", "A", 30f, 0), false);
        g1.AddRule(new Rules.Bisect("A", new string[] { "B", "C" }, 0.5f, 0), false);
        g1.AddRule(new Rules.Bisect("C", new string[] { "D", "C" }, 0.5f, 2), false);
        g1.AddRule(new Rules.Bisect("C", new string[] { "D", "C" }, 0.6f, 0), false);
        g1.AddRule(new Rules.Scale("C", "D", 1.2f, 1), false);
        g1.AddRule(new Rules.Scale("D", "D", 1.5f, 1), false);
    }
    public static void Rule2(Grammar g1)
    {
        g1.AddRule(new Rules.Divide("A", new string[] { "A" ,"B"}, new float[] { 0.2f, 0.3f, 0.5f }, 0), false);
        g1.AddRule(new Rules.Bisect("A", new string[] { "B", "C" }, 0.5f, 0), false);
        g1.AddRule(new Rules.Bisect("C", new string[] { "D", "C" }, 0.5f, 2), false);
        g1.AddRule(new Rules.Bisect("C", new string[] { "D", "C" }, 0.6f, 0), false);
        g1.AddRule(new Rules.Scale("C", "D", 1.2f, 1), false);
        g1.AddRule(new Rules.Scale("D", "D", 1.5f, 1), false);
    }

    public static void Rule3(Grammar g1)
    {
        g1.AddRule(new Rules.BisectMirror("A", new string[] { "B", "B" }, 0.5f, 0), false);
        g1.AddRule(new Rules.Bisect("B", new string[] { "D", "C" }, 0.2f, 0), false);
        //g1.AddRule(new Rules.Scale("C", "D", 1.2f, 1), false);
    }
    public static void Rule4(Grammar g1)
    {
        g1.AddRule(new Rules.Bisect("A", new string[] { "A", "A" }, 0.6f, 0), false);
        g1.AddRule(new Rules.Bisect("A", new string[] { "D", "A" }, 0.6f, 0), false);
        g1.AddRule(new Rules.Bisect("A", new string[] { "A", "A" }, 0.6f, 0), false);
        g1.AddRule(new Rules.Bisect("A", new string[] { "D", "A" }, 0.6f, 0), false);
        //g1.AddRule(new Rules.PivotMirror("C", "A", 0), false);
        //g1.AddRule(new Rules.Bisect("A", new string[] { "A", "C" }, 0.6f, 0), false);
        //g1.AddRule(new Rules.BisectMirror("A", new string[] { "A", "A" }, 0.5f, 0), false);
        //g1.AddRule(new Rules.Bisect("A", new string[] { "D", "C" }, 0.2f, 2), false);
        //g1.AddRule(new Rules.Bisect("C", new string[] { "D", "C" }, 0.6f, 0), false);

    }

    void Start () {
        Vector3[] pts = initShape1();
        Polyline pl = new Polyline(pts);
        Polygon pg = new Polygon(pts);
        Form f = pg.Extrude(new Vector3(0, 20, 0));

        
        f.direction = PointsBase.LongestDirection(pts).Value;
        pg.direction = f.direction;
        //ShapeObject init = ShapeObject.CreateMeshable(f,f.direction);
        ShapeObject init = ShapeObject.CreateMeshable(f,f.direction);
        init.name = "init";


        g1 = new Grammar();
        g1.name = "g1";
        g1.assignedObjects.Add(init);
        g1.AddRule(new Rules.Bisect("A", new string[] { "D", "C" }, 0.2f, 0), false);
        //g1.AddRule(new Rules.PivotMirror("A", "A", 0), false);
        //Rule4(g1);
        //g1.Execute();
        g1.Execute();

        //relate UI elements
        stageIOText = GameObject.Find("StagedIOText").GetComponent<Text>();
        //GrammarInspector gi = GameObject.Find("GrammarInspector").GetComponent<GrammarInspector>();
        //gi.Load();
        //gi.SetGrammar(g1);

        ruleParamEditor = GameObject.Find("RuleParamEditor").GetComponent<RuleParamEditor>();
        
        //ruleParamEditor.GenerateUI(g1.rules[1]);

        UserStats.SelectedGrammar = g1;
  
    }
    
    void Update () {
		
	}
}
