using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;

namespace SGGUI
{
    public delegate void OnSelect(int i);
    public delegate void OnClick();

    public class Navigator : MonoBehaviour, IGrammarOperator
    {

        // Use this for initialization
        // Transform btTransform;
        
        //Transform btHolder;
        public Text selectedIndexText;
        public Text stageIOText;
        public Text grammarTitleText;
        public int selectedIndex = -1;

        public Button saveButton;
        public Button loadButton;
        public Button clearButton;
        public Button titleButton;
        public Button deactivateButton;

        public Button btGrammarList;
        public GameObject grammarList;
        public Button btRuleList;
        public GameObject grammarInspector;



        public RectTransform rectPanelContent;
        public RectTransform grammarListPanelContent;
        List<GameObject> ruleListItems = new List<GameObject>();
        List<Button> grammarListButtons = new List<Button>();

        Dictionary<string,Button> buttons;
        Grammar grammar;

        string recentPath = "\\";

        RuleParamEditor ruleParamEditor;
       

        private void Awake()
        {
            //Load();
            Debug.LogWarning("Navigator.Awake()");

        }

        void Start()
        {
            Load();
            Debug.LogWarning("Navigator.Start()");
            ListAllRules();
            
        }
        public void Load()
        {
            saveButton.onClick.AddListener(OnSaveButtonClicked);
            loadButton.onClick.AddListener(OnLoadButtonClicked);
            clearButton.onClick.AddListener(OnClearButtonClicked);
            titleButton.onClick.AddListener(OnGrammarTitleClicked);
            deactivateButton.onClick.AddListener(OnDeactivateButtonClicked);

            btGrammarList.onClick.AddListener(OnGrammarListClicked);
            btRuleList.onClick.AddListener(OnRuleListClicked);

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
            {
                UnSetGrammar();
                return;
            }
            else if (g==null && grammar ==null)
            {
                return;
            }

            grammar = g;
            //add buttons
            Clear();
            foreach (GraphNode r in grammar.subNodes)
            {
                AddRuleListItem(r.GetDescription());
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
        public void AddRuleListItem(string name = null)
        {
            if (name == null) name = "item" + ruleListItems.Count.ToString();
            /////////////////////////////
            //resize the parent container
            /////////////////////////////
            Vector2 offset = new Vector2(0, 20);
            Vector3 offset3 = new Vector3(0, 20, 0);
            Debug.LogWarningFormat("rectPanelContent={0}", rectPanelContent);
            Debug.LogWarningFormat("rectPanelContent.sizeDelta={0}", rectPanelContent.sizeDelta);
            rectPanelContent.sizeDelta += offset;

            int i = ruleListItems.Count;
            GameObject item = Instantiate(UICreator.GetDefaultButtonAsset(), rectPanelContent);
            item.GetComponent<Button>().onClick.AddListener(delegate { SelectRuleListItem(i); });
            float pos = ruleListItems.Count * 20 + 5;
            RectTransform trans = item.transform as RectTransform;
            trans.anchoredPosition = new Vector2(0, -pos);
            Vector2 size = new Vector2(200, 20);
            trans.sizeDelta = size;
            trans.Find("Text").GetComponent<Text>().text = name;
            ruleListItems.Add(item);
        }
        public void SubItem()
        {
            /////////////////////////////
            //resize the parent container
            /////////////////////////////
            Vector2 offset = new Vector2(0, 20);
            Vector3 offset3 = new Vector3(0, 20, 0);
            rectPanelContent.sizeDelta -= offset;

            //TODO: implement
        }
        public void SelectRuleListItem(int i)
        {
            if (grammar == null) return;
            selectedIndex = i;
            selectedIndexText.text = selectedIndex.ToString();
            //if (onSelectCallbacks != null)
            //    onSelectCallbacks(i);
            Grammar g = grammar;
            g.SelectStep(i);
            ruleParamEditor.GenerateUI(g.subNodes[i]);
            ruleParamEditor.ruleNavigator = this;
            printStructure();
            UpdateGrammarTitleDescription();
        }

        public void SetGrammarPath(string path)
        {
            
        }
        public void LoadGrammarList()
        {
            foreach(Button b in grammarListButtons)
            {
                GameObject.Destroy(b.gameObject);
            }
            grammarListButtons.Clear();

            string path = UserStats.directoryRules;
            string[] files = System.IO.Directory.GetFiles(path, "*.sgr");
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i];
                Button bt = Instantiate(UICreator.GetDefaultButtonAsset(),grammarListPanelContent).GetComponent<Button>();
                bt.transform.Find("Text").GetComponent<Text>().text = fileName;
                bt.onClick.AddListener(delegate { OnSelectGrammarClicked(fileName); });
                grammarListButtons.Add(bt);
                float posY = i * 20 + 5;
                ((RectTransform)bt.transform).anchoredPosition=new Vector2(5,-posY);
            }
        }
        public void OnSelectGrammarClicked(string grammarName)
        {
            Debug.Log("onSelect GrammarClicked");
            if (UserStats.selectedShape == null) return;
            ShapeObject so = UserStats.selectedShape;
            Grammar g = new Grammar();
            //g.assignedObjects.Add(so);
            //so.grammar = g;
            g.Load(grammarName, false);
            //notworking
            so.SetGrammar(g,true);
        }
        public void OnSaveButtonClicked()
        {
            if (grammar == null) return;
            recentPath = Crosstales.FB.FileBrowser.SaveFile("save grammar", recentPath, "UnNamedGrammar", "sgr");
            if (recentPath == "" || recentPath == null) return;
            grammar.Save(recentPath);
            //onSaveClick();
        }
        public void OnLoadButtonClicked()
        {
            if (grammar == null) return;
            recentPath = Crosstales.FB.FileBrowser.OpenSingleFile("load grammar", recentPath, "sgr");
            if (recentPath == "" || recentPath == null) return;
            grammar.Load(recentPath, true);
            //onLoadClick();
        }
        public void OnClearButtonClicked()
        {
            if (grammar == null) return;
            grammar.Clear();
        }
        public void OnDeactivateButtonClicked()
        {
            if (grammar == null) return;
            grammar.SelectStep(-1);
        }
        public void OnGrammarTitleClicked()
        {
            ruleParamEditor.GenerateUI(grammar);
            ruleParamEditor.ruleNavigator = this;
            //if (grammar == null) return;
        }
        public void OnGrammarListClicked()
        {
            grammarList.SetActive(true);
            grammarInspector.SetActive(false);
            LoadGrammarList();

        }
        public void OnRuleListClicked()
        {
            grammarList.SetActive(false);
            grammarInspector.SetActive(true);
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
            foreach (GameObject o in ruleListItems)
            {
                GameObject.Destroy(o);
            }
            ruleListItems.Clear();
            selectedIndex = -1;
        }
        public void UpdateButtonDescriptions()
        {
            if (grammar == null) return;
            Grammar g = grammar;
            for (int i = 0; i < g.subNodes.Count; i++)
            {
                if (i < ruleListItems.Count)
                    ruleListItems[i].transform.Find("Text").GetComponent<Text>().text = g.subNodes[i].GetDescription();
                else
                    AddRuleListItem(g.subNodes[i].GetDescription());
            }
        }

        public void ListAllRules()
        {
            string[] files= System.IO.Directory.GetFiles(UserStats.directoryRules, "*.sgr");
            foreach(string s in files)
            {
                Debug.Log(s);
            }
        }
    }

}

