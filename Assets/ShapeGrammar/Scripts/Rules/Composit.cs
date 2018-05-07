using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

namespace Rules
{
    public class BisectMirror : Grammar
    {
        public BisectMirror() : base()
        {
            SetRules();
            name = "BisectMirror";
        }
        public BisectMirror(string inName, string[] outNames, float d, int axis) : base(inName, outNames)
        {
            AddParam("Axis", axis, 0, 2, 1);
            AddParam("Position", d, 0, 1, 0.1f);
            name = "BisectMirror";
            SetRules();
        }
        void SetRules()
        {
            string on1 = outputs.names[0];
            string onM = outputs.names[1] + "toBeMirrored";
            string on2 = outputs.names[1];

            float d = GetParamVal("Position", 0);
            int axis = (int)GetParamVal("Axis", 0);
            Rule rule1 = new Bisect(inputs.names[0], new string[] { on1, onM }, d, axis);
            Rule rule2 = new PivotMirror(onM, on2, axis);
            AddRule(rule1, false);
            AddRule(rule2, false);

            rule1.paramGroups["Position"] = paramGroups["Position"];
            rule2.paramGroups["Axis"] = paramGroups["Axis"];
        }
    }
    public class SingleLoaded : Grammar
    {
        public SingleLoaded() : base()
        {
            name = "SingleLoaded";
            SetRules();
        }
        public SingleLoaded(string inName, string outName) : base(inName, new string[] { outName })
        {
            name = "SingleLoaded";
            SetRules();
        }
        public void SetRules()
        {
            string inName = inputs.names[0];
            string nameCD ="CD" ;
            string nameAPT = outputs.names[0];

            Rule r1 = new Rules.PivotTurn(inName, inName, 2);
            Rule r2 = new Rules.BisectLength(inName, new string[] { nameCD, nameAPT }, 2, 2);
            Rule r3 = new Rules.CreateStair(nameCD, "STA");

            AddRule(r1, false);
            AddRule(r2, false);
            AddRule(r3, false);
            
        }
    }
    public class DoubleLoaded : Grammar
    {
        public DoubleLoaded() : base()
        {
            name = "SingleLoaded";
            SetRules();
        }
        public DoubleLoaded(string inName, string outName) : base(inName, new string[] { outName })
        {
            name = "SingleLoaded";
            SetRules();
        }
        public void SetRules()
        {
            string inName = inputs.names[0];
            string nameCD = "CD";
            string nameAPT = outputs.names[0];
            string nameAPT2 = nameAPT + "Mirror";
            string nameTMP = nameAPT + "TMP";
            
            Rule r1 = new Rules.BisectLength(inName,  new string[] { nameAPT, nameTMP }, 2, 2);
            Rule r2 = new Rules.BisectLength(nameTMP, new string[] {"CD",nameAPT2}, 2, 2);
            Rule r3 = new Rules.PivotMirror(nameAPT2, nameAPT2, 2);
            Rule r4 = new Rules.PivotMirror(nameAPT2, nameAPT, 0);
            //Rule r3 = new Rules.PivotTurn(nameAPT2, nameAPT, 2);
            Rule r5 = new Rules.PivotTurn(nameCD, nameCD, 2);
            Rule r6 = new Rules.CreateStair(nameCD, "STA");

            Parameter pm = r1.GetParam("Position", 0);
            pm.getSORefValueCallback = Callback;
                
            AddRule(r1, false);
            AddRule(r2, false);
            AddRule(r3, false);
            AddRule(r4, false);
            AddRule(r5, false);
            AddRule(r6, false);
        }
        public float Callback(ShapeObject so)
        {
            return (so.Size[2] - 2) / 2;
        }
    }
    public class CentralVoid : Grammar
    {
        public CentralVoid() : base()
        {
            name = "CenteralVoid";
            SetRules();
        }
        public CentralVoid(string inName, string outMain, string outSub):
            base(inName, new string[] { outMain, outSub })
        {
            name = "CentralVoid";
            SetRules();
        }
        public void SetRules()
        {
            string inName = inputs.names[0];
            string nameCD = "CD";
            string nameAPT = outputs.names[0];
            string nameAPTN = nameAPT + "Mirror";
            string nameAPT2 = outputs.names[1];
            string nameTMP = nameAPT + "TMP";
            float depth=12;

            AddRule(new BisectLength(inName, new string[] { nameAPT, nameTMP }, depth, 2),false);
            AddRule(new PivotMirror(nameTMP, nameTMP, 2),false);
            AddRule(new BisectLength(nameTMP, new string[] { nameAPTN, nameTMP }, depth, 2),false);
            AddRule(new PivotTurn(nameAPTN, nameAPT, 2), false);

            AddRule(new PivotMirror(nameTMP, nameTMP, 2), false);
            AddRule(new BisectLength(nameTMP, new string[] { nameAPT2, nameTMP }, depth, 0),false);
            AddRule(new PivotMirror(nameTMP, nameTMP, 0),false);
            AddRule(new BisectLength(nameTMP, new string[] { nameTMP, nameAPT2 }, -depth, 0), false);

            //AddRule(new BisectLength(inName, new string[] { nameAPT2, nameTMP }, depth, 1),false);
            AddRule(new Rules.Scale3D(nameTMP, "DEL", new Vector3(1, 0.01f, 1)),false);
        }
    }
   
}