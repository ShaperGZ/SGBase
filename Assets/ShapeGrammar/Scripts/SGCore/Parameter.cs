using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;

namespace SGCore
{
    public class Parameter
    {
        public float min=0;
        public float max=1;
        public float value=0.4f;
        public float step = 0.01f;
        public Parameter() { }
        public Parameter(float val, float imin=0, float imax=1, float istep=0.01f)
        {
            min = imin;
            max = imax;
            step = istep;
            value = val;
        }
        public void increase()
        {
            float nv = value + step;
            value= Mathf.Clamp(nv, min, max);
        }
        public void decrease()
        {
            float nv = value - step;
            value = Mathf.Clamp(nv, min, max);
        }

    }
    public class ParameterGroup
    {
        public string name;
        public string extractName;
        public List<Parameter> parameters;
        public GraphNode rule;
        public bool expandable = false;
        public GraphNode grammar { get { return rule.grammar; } }
        public ParameterGroup()
        {
            name = "";
            extractName = "";
            parameters = new List<Parameter>();
        }
        public void Add(Parameter p)
        {
            parameters.Add(p);
        }
    }
}