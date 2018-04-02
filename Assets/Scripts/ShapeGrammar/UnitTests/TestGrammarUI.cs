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
        ShapeObject init = ShapeObject.CreateMeshable(f);

        g1 = new Grammar();
        g1.assignedObjects.Add(init);

        g1.AddRule(new Rules.Bisect("A", new string[] { "B", "C" }, 0.7f, 0), false);
        g1.AddRule(new Rules.Bisect("B", new string[] { "B", "C" }, 0.7f, 2), false);
        //g1.AddRule(new Rules.Scale("C", "C", 1.5f, 1), false);
        g1.Execute();

        //relate UI elements
        stageIOText = GameObject.Find("StagedIOText").GetComponent<Text>();
        GrammarInspector gi = GameObject.Find("GrammarInspector").GetComponent<GrammarInspector>();
        gi.Load();
        gi.SetGrammar(g1);

        ruleParamEditor = GameObject.Find("RuleParamEditor").GetComponent<RuleParamEditor>();
        ruleParamEditor.GenerateUI(g1.rules[1]);

        UserStats.SelectedGrammar = g1;
        UserStats.UI_RuleList = GameObject.Find("ExpandableListPanel").GetComponent<ExpandableList>();
        UserStats.UI_RuleList.onAddClick += delegate {
            Rule r = new Rules.Bisect("C", new string[] { "B", "C" }, 0.4f, 0);
            g1.AddRule(r, true);
            printStructure();
            //g.AddRule(Rules.Scale.Create("B", new string[] { "B" }, 1.5f, 1),true);
            UserStats.UI_RuleList.AddItem(r.description);

        };
        UserStats.UI_RuleList.onSelectCallbacks += SelectRule;
        UserStats.UI_RuleList.Clear();

        foreach (Rule r in g1.rules)
        {
            UserStats.UI_RuleList.AddItem(r.description);
        }
    }
    void printStructure()
    {
        //display debug texts
        string txt = "";
        txt += "Grammar.currentStep=" + g1.currentStep.ToString();
        for (int i = 0; i < g1.stagedOutputs.Count; i++)
        {
            txt += "\nRule #" + i.ToString();
            txt += "\n   Input: ";
            foreach (ShapeObject o in g1.rules[i].inputs.shapes)
            {
                txt += o.Format() + ",";
            }
            txt += "\n   Output: ";
            foreach (ShapeObject o in g1.rules[i].outputs.shapes)
            {
                txt += o.Format() + ",";
            }


            txt += "\n   Staged output: ";
            SGIO io = g1.stagedOutputs[i];
            foreach (ShapeObject o in io.shapes)
            {
                txt += o.Format() + ",";
            }
            txt += "\n";
        }
        stageIOText.text = txt;
    }
    void SelectRule(int index)
    {
        g1.currentStep = index;
        g1.displayStep = index;
        bool stage;
        SGIO sgio;
        //TODO: add this to grammar
        for (int i = 0; i < g1.stagedOutputs.Count; i++)
        {
            sgio = g1.stagedOutputs[i];
            List<ShapeObject> tobeRemoved = new List<ShapeObject>();
            foreach (ShapeObject o in sgio.shapes)
            {
                try
                {
                    o.gameObject.SetActive(false);
                }
                catch
                {
                    tobeRemoved.Add(o);
                }
                
            }
            foreach(ShapeObject o in tobeRemoved)
            {
                sgio.shapes.Remove(o);
            }
            

        }
        sgio = g1.stagedOutputs[g1.displayStep];
        foreach (ShapeObject o in sgio.shapes)
        {
            o.gameObject.SetActive(true);
        }
        ruleParamEditor.GenerateUI(g1.rules[index]);
        printStructure();

    }
    // Update is called once per frame
    void Update () {
		
	}
}
