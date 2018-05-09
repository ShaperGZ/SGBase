using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SGCore;

namespace SGCommand{
    public class Command
    {
        public virtual void run() { }
    }

    public class AddRuleCommand : Command
    {
        protected void AddRule(Type t)
        {
            if (SceneManager.SelectedGrammar == null)
            {
                Debug.Log("Please select a grammar first");
                return;
            }
            Grammar grammar = SceneManager.SelectedGrammar;
            Rule r = Activator.CreateInstance(t) as Rule;
            if(SceneManager.selectedShape!=null)
                r.inputs.names[0] = SceneManager.selectedShape.gameObject.name;
            //Debug.Log(string.Format("create {0} on click in RuleCreator", r.name));
            grammar.AddRule(r, true);
            SceneManager.ruleNavigator.AddRuleListItem(r.description);
        }
    }

    public class AddRuleBisect : AddRuleCommand
    {
        public override void run()
        {
            AddRule(typeof(Rules.Bisect));
        }
    }
    public class AddRuleScale : AddRuleCommand
    {
        public override void run()
        {
            AddRule(typeof(Rules.Scale));
        }
    }
    public class AddRuleDivideTo : AddRuleCommand
    {
        public override void run()
        {
            AddRule(typeof(Rules.DivideTo));
        }
    }
    public class AddRuleDcpA : AddRuleCommand
    {
        public override void run()
        {
            //AddRule(typeof(Rules.DcpA));
        }
    }
}

