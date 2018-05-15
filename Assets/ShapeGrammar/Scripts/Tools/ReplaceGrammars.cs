using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;
using SGGeometry;

public class ReplaceGrammars : MonoBehaviour {
    
    Button btReplace;

	// Use this for initialization
	void Start () {
       
        //gameObject.SetActive(false);
        btReplace = GameObject.Find("BtReplaceRule").GetComponent<Button>();
        btReplace.onClick.AddListener(delegate { ShowPanel(); });
    }
	


    void ShowPanel()
    {
        Debug.Log("show panel");
        gameObject.SetActive(true);
    }
    public Grammar GA(bool addToScene=true)
    {
        Grammar g = new Grammar(addToScene);
        g.category = "massingForm";
        g.inputs.names.Add("APT");
        g.name = "AptFormA";
        g.AddRule(new Rules.Bisect("APT", new string[] { "APT", "C" }, 0.4f, 0), false);
        g.AddRule(new Rules.Bisect("C", new string[] { "C", "APT" }, 0.25f, 2), false);
        g.AddRule(new Rules.Scale3D("C", "APT", new Vector3(1.3f, 0.7f, 1.6f), null, Alignment.NE), false);
        return g;
    }
    public Grammar GB(bool addToScene = true)
    {
        Grammar g = new Grammar(addToScene);
        g.category = "massingForm";
        g.inputs.names.Add("APT");
        g.name = "AptFormB";
        g.AddRule(new Rules.DivideTo("APT", "APT", 30, 0));
        g.AddRule(new Rules.Divide("APT", new string[] { "APT", "B" }, new float[] { 0.2f, 0.6f, 0.2f }, 0), false);
        //g2.AddRule(new Rules.DivideTo("APT", "APT_B" , 8,0));

        //g2.AddRule(new Rules.PivotMirror("APT_B", "APT_B", 2));
        g.AddRule(new Rules.Scale3D("B", "B", new Vector3(1f, 1.1f, 1.2f), null, Alignment.N), false);
        g.AddRule(new Rules.Bisect("B", new string[] { "B", "APT" }, 0.4f, 1), false);
        g.AddRule(new Rules.Scale3D("B", "APT", new Vector3(1.2f, 1f, 1.2f), null, Alignment.N), false);
        return g;
    }
    public Grammar GC(bool addToScene = true)
    {
        Grammar g = new Grammar(addToScene);
        g.category = "massingForm";
        g.inputs.names.Add("APT");
        g.name = "AptFormC";
        //g2.AddRule(new Rules.Divide("APT", new string[] { "APT_A", "APT_B" }, new float[] { 0.2f, 0.6f, 0.2f }, 0), false);
        //g2.AddRule(new Rules.DivideTo("APT", "APT_B" , 8,0));
        g.AddRule(new Rules.Bisect("APT", new string[] { "APT", "C" }, 0.6f, 1), false);
        g.AddRule(new Rules.Bisect("C", new string[] { "C", "APT" }, 0.35f, 0), false);
        g.AddRule(new Rules.Scale3D("C", "C", new Vector3(1.05f, 1, 1.05f), null, Alignment.NE), false);
        g.AddRule(new Rules.BisectLength("C", new string[] { "C", "APT" }, 4, 1), false);
        g.AddRule(new Rules.Scale3D("C", "APT", new Vector3(0.8f, 1, 0.8f), null, Alignment.NE), false);
        return g;
    }
    public void ReplaceAptFormA()
    {
        if(SceneManager.SelectedGrammar != null &&SceneManager.SelectedGrammar.category == "massingForm")
        {
            Grammar g2 = GA();
            Grammar g = SceneManager.SelectedGrammar;
            g2.CloneTo(g);
            g.Execute();
        }
        //gameObject.SetActive(false);
    }
    public void ReplaceAptFormB()
    {
        if (SceneManager.SelectedGrammar != null)
        {
            //Grammar g = SceneManager.SelectedGrammar;


            Grammar g2 = GB();
            //g2.AddRule(new Rules.DivideToFTFH("APT2", "APTL", 4), false);
            //g2.AddRule(new Rules.DivideToFTFH("APT", "APTL", 4), false);

            Grammar g = SceneManager.SelectedGrammar;
            g2.CloneTo(g);
            g.Execute();
        }
        //gameObject.SetActive(false);
    }
    public void ReplaceAptFormC()
    {
        if (SceneManager.SelectedGrammar != null)
        {
            Grammar g2 = GC();
            Grammar g = SceneManager.SelectedGrammar;
            g2.CloneTo(g);
            g.Execute();
        }
        //gameObject.SetActive(false);
    }
    public void BatchReplace(Grammar newGrammar)
    {
        if (SceneManager.existingGrammar.Count < 1) return;
        List<Grammar> gs = new List<Grammar>();
        foreach (Grammar g in SceneManager.existingGrammar.Values)
        {
            gs.Add(g);
        }


        for (int i = 0; i < gs.Count; i++)
        {
            Grammar g = gs[i];
            Grammar gg = null;
            for (int j = 0; j < g.subNodes.Count; j++)
            {
                GraphNode n = g.subNodes[j];
                if (n.category == "massingForm")
                {
                    gg = (Grammar)n;
                    break;
                    
                }
            }
            if (gg!=null)
            {
                newGrammar.CloneTo(gg);
                g.Execute();
            }
            
        }
    }
    public void BatchReplaceA()
    {
        BatchReplace(GA(false));
    }
    public void BatchReplaceB()
    {
        BatchReplace(GB(false));
    }
    public void BatchReplaceC()
    {
        BatchReplace(GC(false));
    }
}
