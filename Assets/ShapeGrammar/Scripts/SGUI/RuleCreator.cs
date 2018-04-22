using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;
using SGGeometry;
using Rules;

namespace SGGUI
{
    public class RuleCreator: IGrammarOperator
    {
        public string UIPrefix = "Bt_Crt_";
        public Type[] ruleTypes;
        Grammar grammar;
        
        public RuleCreator()
        {
            ruleTypes = GetRuleTypes();
            AssociateUI();
        }
        public void SetGrammar(Grammar g)
        {
            if (g == null) UnSetGrammar();
            grammar = g;
            AssociateUI();
        }
        public void UnSetGrammar()
        {
            foreach (Type t in ruleTypes)
            {
                string name = UIPrefix + t.Name;
                try
                {
                    Button button = GameObject.Find(name).GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                }
                catch { };

            }
        }

        //SceneManager will initialize an instance of RuleCreator
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
                try
                {
                    Button button = GameObject.Find(name).GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener
                        (delegate
                        {
                            Rule r = Activator.CreateInstance(t) as Rule;
                            //Debug.Log(string.Format("create {0} on click in RuleCreator", r.name));
                            grammar.AddRule(r, true);
                            SceneManager.ruleNavigator.AddRuleListItem(r.description);
                        }
                        );
                   
                }
                catch { };

            }
        }

        public Type[] GetRuleTypes()
        {
            List<Type> rules = new List<Type>();
            rules.Add(typeof(Rules.Bisect)); //name the UI: Bt_Crt_Bisect
            rules.Add(typeof(Rules.Scale));//name the UI: Bt_Crt_Scale
            rules.Add(typeof(Rules.DivideTo));
            rules.Add(typeof(Rules.CreateBox));
            return rules.ToArray();
        }

    }
}


   

