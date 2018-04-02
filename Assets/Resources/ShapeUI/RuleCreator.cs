using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;
using SGGeometry;
using Rules;

public class RuleCreator
{
    public string UIPrefix = "Bt_Crt_";
    public Type[] ruleTypes;
    public Grammar grammar;

    public RuleCreator()
    {
        ruleTypes = GetRuleTypes();
        AssociateUI();
    }

    //UserStats will initialize an instance of RuleCreator
    public void CreateToolBar()
    {
        //it generates the tool bar
    }
    public void AssociateUI()
    {
        //if the UI is already created, associate each creation buttons
        foreach (Type t in ruleTypes)
        {
            string name = UIPrefix + t.Name;
            Button button = GameObject.Find(name).GetComponent<Button>();
            button.onClick.AddListener
                (delegate
                {
                    Debug.Log("type=" + t.ToString());
                    Rule r = Activator.CreateInstance(t) as Rule;
                    Debug.Log("r.name=" + r.name);
                    UserStats.SelectedGrammar.AddRule(r,true);
                    UserStats.UI_RuleList.AddItem(r.description);
                }
                );
        }
    }
    public void AssociateUI2()
    {
        //if the UI is already created, associate each creation buttons
        foreach(Type t in ruleTypes)
        {
            string name = UIPrefix + t.Name;
            Button button = GameObject.Find(name).GetComponent<Button>();
            button.onClick.AddListener
                (delegate 
                    {
                        Debug.Log("type=" + t.ToString());
                        Rule r = Activator.CreateInstance(t) as Rule;
                        Debug.Log("r=" + r.ToString());
                        grammar.AddRule(r);
                    }
                );
        }
    }
    public Type[] GetRuleTypes()
    {
        List<Type> rules = new List<Type>();
        rules.Add(typeof(Rules.Bisect)); //name the UI: Bt_Crt_Bisect
        rules.Add(typeof(Rules.Scale));//name the UI: Bt_Crt_Scale
        return rules.ToArray();
    }

}
   

