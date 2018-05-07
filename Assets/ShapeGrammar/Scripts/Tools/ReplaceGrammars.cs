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

    public void ReplaceAptFormA()
    {
        if(SceneManager.SelectedGrammar != null)
        {
            //Grammar g = SceneManager.SelectedGrammar;

            Grammar g2 = new Grammar();
            g2.name = "AptFormA";
            g2.AddRule(new Rules.Bisect("APT", new string[] { "B", "C" }, 0.4f, 0),false);
            g2.AddRule(new Rules.Bisect("C", new string[] { "C", "B" }, 0.25f, 2), false);
            g2.AddRule(new Rules.PivotMirror("C", "C", 2), false);
            g2.AddRule(new Rules.PivotMirror("C", "C", 0), false);
            g2.AddRule(new Rules.Scale3D("C", "C", new Vector3(1.3f, 0.7f, 1.6f), null, Alignment.N), false);
            g2.inputs.names.Add("APT");

            Grammar g = SceneManager.SelectedGrammar;
            g2.CloneTo(g);
            

            
        }
        //gameObject.SetActive(false);
    }
    public void ReplaceAptFormB()
    {
        if (SceneManager.SelectedGrammar != null)
        {
            //Grammar g = SceneManager.SelectedGrammar;

            Grammar g2 = new Grammar();
            g2.name = "AptFormB";
            g2.AddRule(new Rules.Bisect("APT", new string[] { "B", "C" }, 0.1f, 0));
            g2.AddRule(new Rules.Bisect("C", new string[] { "C", "B" }, 0.8f, 2));
            g2.AddRule(new Rules.PivotMirror("C", "C", 2));
            g2.AddRule(new Rules.Scale3D("C", "C", new Vector3(1f, 1f, 1.6f), null, Alignment.N));
            g2.inputs.names.Add("APT");

            Grammar g = SceneManager.SelectedGrammar;
            g2.CloneTo(g);
        }
        //gameObject.SetActive(false);
    }
}
