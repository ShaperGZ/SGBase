using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;
using UnityEngine.UI;

public class TestGrammarUI : MonoBehaviour {
    Grammar g1;
    Text stageIOText;
    RuleParamEditor ruleParamEditor;
    // Use this for initialization
    void Start () {
        Vector3[] pts = new Vector3[5];
        pts[0] = new Vector3(-80, 0, 46);
        pts[1] = new Vector3(-80, 0, -11);
        pts[2] = new Vector3(-10, 0, -54);
        pts[3] = new Vector3(103, 0, 25);
        pts[4] = new Vector3(96, 0, 70);
        Polyline pl = new Polyline(pts);
        Polygon pg = new Polygon(pts);
        Form f = pg.Extrude(new Vector3(0, 5, 0));

        Vector3 nml = pts[4] - pts[0];
        nml.Normalize();
        //Plane pln = new Plane(nml, new Vector3());
        //Meshable[] forms = f.SplitByPlane(pln);
        //forms[0].direction= PointsBase.LongestDirection(pts).Value;
        //ShapeObject init = ShapeObject.CreateMeshable(forms[0]);

        f.direction = PointsBase.LongestDirection(pts).Value;
        ShapeObject init = ShapeObject.CreateMeshable(f);
        init.name = "init";

        g1 = new Grammar();
        g1.assignedObjects.Add(init);
        g1.Load(@"D:\FIrstRule.sgr", false);
        //g1.AddRule(new Rules.Bisect("A", new string[] { "C", "C" }, 0.7f, 0), false);
        //g1.AddRule(new Rules.Bisect("C", new string[] { "B", "C" }, 0.4f, 0), false);
        //g1.AddRule(new Rules.Bisect("C", new string[] { "D", "C" }, 0.5f, 2), false);
        //g1.AddRule(new Rules.Bisect("E", new string[] { "D", "C" }, 0.6f, 0), false);
        //g1.AddRule(new Rules.Scale("B",  "B" , 2, 1), false);
        //g1.AddRule(new Rules.Scale("D", "D", 3, 1), false);
        //g1.AddRule(new Rules.Bisect("D", new string[] { "B", "C" }, 0.6f, 0), false);
        //g1.AddRule(new Rules.Scale("B", "B", 3, 1), false);

        g1.Execute();

        //Rule ru = new Rules.Bisect("C", new string[] { "B", "C" }, 0.4f, 0);
        //g1.AddRule(ru, true);

        //relate UI elements
        stageIOText = GameObject.Find("StagedIOText").GetComponent<Text>();
        GrammarInspector gi = GameObject.Find("GrammarInspector").GetComponent<GrammarInspector>();
        gi.Load();
        gi.SetGrammar(g1);

        ruleParamEditor = GameObject.Find("RuleParamEditor").GetComponent<RuleParamEditor>();
        
        //ruleParamEditor.GenerateUI(g1.rules[1]);

        UserStats.SelectedGrammar = g1;
        UserStats.ruleNavigator = GameObject.Find("ExpandableListPanel").GetComponent<RuleNavigator>();

        //Assign rule navigator actions
        UserStats.ruleNavigator.onSaveClick += delegate {
            UserStats.SelectedGrammar.Save(@"D:\FIrstRule.sgr");

        };
        UserStats.ruleNavigator.onLoadClick += delegate {
            UserStats.SelectedGrammar.Clear();

            Grammar g = new Grammar();
            g.assignedObjects = UserStats.SelectedGrammar.assignedObjects;
            UserStats.SelectedGrammar = g;
            g.Load(@"D:\FIrstRule.sgr");
            g.Execute();

        };


        //UserStats.UI_RuleList.onSelectCallbacks += SelectRule;
        UserStats.ruleNavigator.Clear();

        foreach (Rule r in g1.rules)
        {
            UserStats.ruleNavigator.AddItem(r.description);
        }
    }
    
    void Update () {
		
	}
}
