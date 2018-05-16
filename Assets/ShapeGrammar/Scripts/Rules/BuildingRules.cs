﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

namespace Rules
{
    public class CspFlrToUnitsAbstract:Rule
    {
        public CspFlrToUnitsAbstract() : base() { name = "DcpFlrToUnits"; }
        public CspFlrToUnitsAbstract(string[] inNames):base()
        {
            name = "DcpFlrToUnits";
            inputs.names.Clear();
            inputs.names.AddRange(inNames);
        }
        public override void Execute()
        {

            Debug.Log("executing "+name);
            Dictionary<SGBuilding, List<Meshable>> container = new Dictionary<SGBuilding, List<Meshable>>();
            
            foreach(ShapeObject o in inputs.shapes)
            {
                if (o.parentRule.grammar == null || o.parentRule.grammar.building == null) continue;
                SGBuilding b = o.parentRule.grammar.sgbuilding;
                //Debug.Log(b.mode.ToString());
                //Debug.LogFormat("b.mode==unit{0}", b.mode == Building.DisplayMode.UNIT);
                if (b.mode != SGBuilding.DisplayMode.PROGRAM) continue;

                Debug.Log("getting units");
                if (!container.ContainsKey(b))
                {
                    container[b] = new List<Meshable>();
                }
                Meshable[] units=SGUtility.DivideFormToLength(o.meshable,3,0);
                container[b].AddRange(units);
            }

            foreach(KeyValuePair<SGBuilding,List<Meshable>> kv in container)
            {
                SGBuilding b = kv.Key;
                b.units = new List<List<Meshable>>();
                b.units.Add(kv.Value);
            }

            outputs.shapes = inputs.shapes;
        }
    }
    public class DcpFlrToUnitsReal : Rule
    {
        public DcpFlrToUnitsReal() : base() { name = "DcpFlrToUnits"; }
        public DcpFlrToUnitsReal(string[] inNames, string outName) : base()
        {
            name = "DcpFlrToUnits";
            inputs.names.Clear();
            inputs.names.AddRange(inNames);
            outputs.names.Clear();
            outputs.names.Add(outName);
        }
        public override void Execute()
        {
            //Debug.Log("Executing unit, input count="+inputs.shapes.Count);
            //if(sgbuilding!=null && sgbuilding.mode==SGBuilding.DisplayMode.PROGRAM)
            
            string namePrefix = outputs.names[0];
            outMeshables.Clear();

            foreach (ShapeObject o in inputs.shapes)
            {

                Meshable[] units = SGUtility.DivideFormToLength(o.meshable, 3, 0);
                outMeshables.AddRange(units);
            }

            int dif = outputs.shapes.Count - outMeshables.Count;
            SGUtility.RemoveExtraShapeObjects(ref outputs.shapes, dif);
            //Debug.Log("outMeshable count=" + outMeshables.Count);
            for (int i = 0; i < outMeshables.Count; i++)
            {
                if (i >= outputs.shapes.Count)
                {
                    outputs.shapes.Add(ShapeObject.CreateBasic());
                }
                Meshable m = outMeshables[i];
                m.Scale(new Vector3(0.8f, 0.8f, 1f), m.bbox.vects, m.bbox.GetOriginFromAlignment(Alignment.Center), false);
                //Debug.Log("meshable=" + m.ToString());
                outputs.shapes[i].SetMeshable(m);
                outputs.shapes[i].name = namePrefix + "_A";
                outputs.shapes[i].parentRule = this;
            }
            
            
        }
    }

}