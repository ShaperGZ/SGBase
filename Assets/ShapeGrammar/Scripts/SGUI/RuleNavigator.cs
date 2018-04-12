using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;

namespace SGGUI
{
    public delegate void OnSelect(int i);
    public delegate void OnClick();

    public class RuleNavigator : MonoBehaviour, IGrammarOperator
    {

        // Use this for initialization
        // Transform btTransform;
        RectTransform pnContent;
        List<GameObject> items = new List<GameObject>();
        Transform btHolder;
        Text selectedIndexText;
        Text stageIOText;
        Text grammarTitleText;
        public int selectedIndex = -1;
        public OnSelect onSelectCallbacks;
        public OnClick onAddClick;
        public OnClick onSubClick;
        public OnClick onSaveClick;
        public OnClick onLoadClick;
        public OnClick OnGrammarTitleClick;

        string recentPath = "\\";

        RuleParamEditor ruleParamEditor;
        Grammar grammar;

        private void Awake()
        {
            Load();
        }

        void Start()
        {
            //Load();

        }
        public void Load()
        {
            btHolder = transform.Find("BtHolder") as Transform;
            pnContent = transform.Find("ContentPanel") as RectTransform;
            grammarTitleText = GameObject.Find("GrammarInspector/Panel/Bt_GrammarTitle/TitleText").GetComponent<Text>();

            GameObject.Find("BtHolder/BtSaveRuleSet").GetComponent<Button>().onClick.AddListener(delegate { onSaveClick(); });
            GameObject.Find("BtHolder/BtLoadRuleSet").GetComponent<Button>().onClick.AddListener(delegate { onLoadClick(); });
            GameObject.Find("BtHolder/BtClearGrammar").GetComponent<Button>().onClick.AddListener(
                delegate {
                    UserStats.SelectedGrammar.Clear();
                    this.Clear();
                });
            GameObject.Find("Bt_GrammarTitle").GetComponent<Button>().onClick.AddListener(delegate { OnGrammarTitleClick(); });
            selectedIndexText = transform.Find("BtHolder/SelectIndex").GetComponent<Text>() as Text;
            stageIOText = GameObject.Find("StagedIOText").GetComponent<Text>();
            ruleParamEditor = GameObject.Find("RuleParamEditor").GetComponent<RuleParamEditor>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void UpdateGrammarTitleDescription()
        {
            if (grammar == null) return;
            string text = "";
            text += grammar.name.ToString();
            text += string.Format(" R{0}/{1} | N{2}O{3} -> N{4}O{5}",
                grammar.currentStep,
                grammar.subNodes.Count,
                grammar.inputs.names.Count,
                grammar.inputs.shapes.Count,
                grammar.outputs.names.Count,
                grammar.outputs.shapes.Count
                );
            grammarTitleText.text = text;

        }
        public void SetGrammar(Grammar g)
        {
            if (g == null && grammar != null)
                UnSetGrammar();

            grammar = g;
            onSaveClick += delegate {
                recentPath = Crosstales.FB.FileBrowser.SaveFile("save grammar", recentPath, "UnNamedGrammar", "sgr");
                if (recentPath == "" || recentPath == null) return;
                grammar.Save(recentPath);
            };
            onLoadClick += delegate {
                recentPath = Crosstales.FB.FileBrowser.OpenSingleFile("load grammar", recentPath, "sgr");
                if (recentPath == "" || recentPath == null) return;
                grammar.Load(recentPath, true);
            };
            OnGrammarTitleClick += delegate
            {
                ruleParamEditor.GenerateUI(g);
                ruleParamEditor.ruleNavigator = this;
            };

            //add buttons
            Clear();
            foreach (GraphNode r in grammar.subNodes)
            {
                UserStats.ruleNavigator.AddItem(r.GetDescription());
            }
        }
        public void UnSetGrammar()
        {
            Clear();
            if (grammar != null)
            {
                //TODO: remove onclick events
            }
        }
        public void AddItem(string name = null)
        {
            if (name == null) name = "item" + items.Count.ToString();
            /////////////////////////////
            //resize the parent container
            /////////////////////////////
            Vector2 offset = new Vector2(0, 20);
            Vector3 offset3 = new Vector3(0, 20, 0);
            pnContent.sizeDelta += offset;

            int i = items.Count;
            GameObject item = Instantiate(UICreator.GetDefaultButtonAsset(), pnContent);
            item.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(i); });
            float pos = items.Count * 20 + 5;
            RectTransform trans = item.transform as RectTransform;
            trans.anchoredPosition = new Vector2(0, -pos);
            Vector2 size = new Vector2(200, 20);
            trans.sizeDelta = size;
            trans.Find("Text").GetComponent<Text>().text = name;
            items.Add(item);
        }
        public void SubItem()
        {
            /////////////////////////////
            //resize the parent container
            /////////////////////////////
            Vector2 offset = new Vector2(0, 20);
            Vector3 offset3 = new Vector3(0, 20, 0);
            pnContent.sizeDelta -= offset;

            //TODO: implement
        }
        public void SelectItem(int i)
        {
            if (grammar == null) return;
            selectedIndex = i;
            selectedIndexText.text = selectedIndex.ToString();
            if (onSelectCallbacks != null)
                onSelectCallbacks(i);
            Grammar g = grammar;
            g.SelectStep(i);
            ruleParamEditor.GenerateUI(g.subNodes[i]);
            ruleParamEditor.ruleNavigator = this;
            printStructure();
            UpdateGrammarTitleDescription();
        }
        public void printStructure()
        {
            //display debug texts
            Grammar g = UserStats.SelectedGrammar;
            string txt = "";
            txt += "Grammar.currentStep=" + g.currentStep.ToString();
            for (int i = 0; i < g.stagedOutputs.Count; i++)
            {
                if (i < g.subNodes.Count)
                {
                    txt += "\nRule #" + i.ToString() + g.subNodes[i].description;
                }
                else
                {
                    txt += "\nRule #" + i.ToString() + ">=" + g.subNodes.Count;
                }
                txt += "\n   Input: ";
                foreach (ShapeObject o in g.subNodes[i].inputs.shapes)
                {
                    if (o != null)
                        txt += o.Format() + ",";
                    else
                        txt += "null,";
                }
                txt += "\n   Output: ";
                foreach (ShapeObject o in g.subNodes[i].outputs.shapes)
                {
                    if (o != null)
                        txt += o.Format() + ",";
                    else
                        txt += "null,";
                }


                txt += "\n   Staged output: ";
                SGIO io = g.stagedOutputs[i];
                foreach (ShapeObject o in io.shapes)
                {
                    if (o != null)
                        txt += o.Format() + ",";
                }
                txt += "\n";
            }
            stageIOText.text = txt;
        }
        public void Clear()
        {
            foreach (GameObject o in items)
            {
                GameObject.Destroy(o);
            }
            items.Clear();
            selectedIndex = -1;
        }
        public void UpdateButtonDescriptions()
        {
            if (grammar == null) return;
            Grammar g = grammar;
            for (int i = 0; i < g.subNodes.Count; i++)
            {
                if (i < items.Count)
                    items[i].transform.Find("Text").GetComponent<Text>().text = g.subNodes[i].GetDescription();
                else
                    AddItem(g.subNodes[i].GetDescription());
            }
        }
    }

}

