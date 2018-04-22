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
    public static Vector3[] initShape3()
    {
        Vector3[] pts = initShape2();
        Vector3 offset = new Vector3(60, 0, 0);
        for (int i = 0; i < pts.Length; i++)
        {
            pts[i] += offset;
        }
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
        g1.AddRule(new Rules.Scale("C", "D", 0.8f, 1), false);
        g1.AddRule(new Rules.Scale("D", "D", 0.5f, 1), false);
    }

    public static void Rule3(Grammar g1)
    {
        g1.AddRule(new Rules.BisectMirror("A", new string[] { "B", "B" }, 0.5f, 0), false);
        g1.AddRule(new Rules.Bisect("B", new string[] { "D", "C" }, 0.2f, 0), false);
        g1.AddRule(new Rules.Scale("C", "D", 0.8f, 1), false);
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
        Vector3[] pts1 = initShape2();
        Vector3[] pts2 = initShape3();
        
        Polygon pg1 = new Polygon(pts1);
        Polygon pg2 = new Polygon(pts2);
        Form f1 = pg1.ExtrudeToForm(new Vector3(0, 40, 0));
        Form f2 = pg2.ExtrudeToForm(new Vector3(0, 30, 0));

        
        f1.direction = PointsBase.LongestDirection(pts1).Value;
        f2.direction = f1.direction;
        //ShapeObject init = ShapeObject.CreateMeshable(f,f.direction);
        ShapeObject init1 = ShapeObject.CreateMeshable(f1,f1.direction);
        init1.name = "init1";
        ShapeObject init2 = ShapeObject.CreateMeshable(f2, f2.direction);
        init2.name = "init2";

        //ShapeObject rinit = ShapeObject.CreateMeshable(rpg, f.direction);



        //g1 = new Grammar();
        //g1.name = "g1";
        //g1.assignedObjects.Add(init1);
        //Rule3(g1);
        //g1.Execute();

        Grammar g2 = new Grammar();
        g2.name = "g2";
        g2.assignedObjects.Add(init2);
        Rule4(g2);
        g2.Execute();

        //relate UI elements
        stageIOText = GameObject.Find("StagedIOText").GetComponent<Text>();
        //GrammarInspector gi = GameObject.Find("GrammarInspector").GetComponent<GrammarInspector>();
        //gi.Load();
        //gi.SetGrammar(g1);

        ruleParamEditor = GameObject.Find("RuleParamEditor").GetComponent<RuleParamEditor>();
        
        //ruleParamEditor.GenerateUI(g1.rules[1]);

        //SceneManager.SelectedGrammar = g1;
  
    }
    
    void Update () {
		
	}
}
