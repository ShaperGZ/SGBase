using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;

//public class GrammarInspector : MonoBehaviour {

//    public Grammar grammar;
//    Text titleText;
//    Text stagedNameText;
//    Text stageIOText;

//    // Use this for initialization
//    void Start () {
//        Load();
//    }
//    public void Load()
//    {
//        GameObject GrammarInspectorObj = GameObject.Find("GrammarInspector");
//        GameObject textObj = GameObject.Find("GrammarInspector/Panel/TitleText");
//        GameObject nameTextObj = GameObject.Find("GrammarInspector/Panel/ScrollView/Viewport/Text");
//        titleText = textObj.GetComponent<Text>();
//        stagedNameText = nameTextObj.GetComponent<Text>();
//        stageIOText = GameObject.Find("StagedIOText").GetComponent<Text>();
//    }

//    // Update is called once per frame
//    void Update () {
//        //UpdateState();
        
//    }
//    public void SetGrammar(Grammar g)
//    {
//        grammar = g;
//        UpdateState();
//    }
//    public void UpdateState()
//    {
//        if (grammar == null) return;
//        string text = "";
//        text += grammar.name.ToString();
//        text += string.Format(" R{0}/{1} | N{2}O{3} -> N{4}O{5}",
//            grammar.currentStep,
//            grammar.subNodes.Count,
//            grammar.inputs.names.Count,
//            grammar.inputs.shapes.Count,
//            grammar.outputs.names.Count,
//            grammar.outputs.shapes.Count
//            );
//        titleText.text = text;


//        text = "";
//        for (int i = 0; i < grammar.subNodes.Count; i++)
//        {
//            text += grammar.subNodes[i].description;
//            text += "\n";
//        }


//        stagedNameText.text = text;

//    }

//}
