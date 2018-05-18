using System.Collections;
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
                if (b.displayMode != SGBuilding.DisplayMode.PROGRAM) continue;

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
            int counter = -1;
            List<string> names = new List<string>();
            Dictionary<string, Color> namedColors = new Dictionary<string, Color>();
            string namePrefix = outputs.names[0];
            outMeshables.Clear();
            List<Color> colors = new List<Color>();
            //Dictionary<float, List<Meshable>> sortedContainer = new Dictionary<float, List<Meshable>>();

            foreach (ShapeObject o in inputs.shapes)
            {
                if(o.name==inputs.names[0])
                {
                    Meshable[] units = SGUtility.DivideFormToLength(o.meshable, 3, 0);
                    float d = o.Size[2];
                    string mbname = namePrefix + d.ToString();
                    if (!namedColors.ContainsKey(mbname))
                    {
                        counter++;
                        int colorIndex = counter % SchemeColor.ColorSetDefault.Length;
                        Color c = SchemeColor.ColorSetDefault[colorIndex];
                        namedColors.Add(mbname, c);
                    }
                    foreach (Meshable mb in units)
                    {
                        mb.name = mbname;
                        mb.Scale(new Vector3(0.9f, 0.8f, 1f), mb.bbox.vects, mb.bbox.GetOriginFromAlignment(Alignment.Center), false);
                    }
                    outMeshables.AddRange(units);
                }
                if (inputs.names.Count>1 && o.name == inputs.names[1])
                {
                    Meshable[] units = SGUtility.DivideFormToLength(o.meshable, 3, 2);
                    float d = o.Size[2];
                    string mbname = namePrefix + d.ToString();
                    if (!namedColors.ContainsKey(mbname))
                    {
                        counter++;
                        Color c = SchemeColor.ColorSetDefault[counter];
                        namedColors.Add(mbname, c);
                    }
                    foreach (Meshable mb in units)
                    {
                        mb.name = mbname;
                        mb.Scale(new Vector3(1f, 0.8f, 0.9f), mb.bbox.vects, mb.bbox.GetOriginFromAlignment(Alignment.Center), false);
                    }
                    outMeshables.AddRange(units);
                }

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
                //Debug.Log("meshable=" + m.ToString());
                
                outputs.shapes[i].SetMeshable(m);
                outputs.shapes[i].name = m.name;
                outputs.shapes[i].parentRule = this;
                outputs.shapes[i].GetComponent<MeshRenderer>().material.color = namedColors[m.name];
            }
            
            
        }
    }
    public class UpdateBuildingParamDisplay : Rule
    {
        public UpdateBuildingParamDisplay() : base()
        {
            name = "UpdateBuildingParamDisplay";
        }
        public UpdateBuildingParamDisplay(string inName) : base(inName, "")
        {
            name = "UpdateBuildingParamDisplay";
        }
        public override void Execute()
        {
            base.Execute();
            List<SGBuilding> buildings = new List<SGBuilding>();
            foreach (ShapeObject o in inputs.shapes)
            {
                SGBuilding sgbuilding= o.parentRule.sgbuilding;
                if (!buildings.Contains(sgbuilding)) buildings.Add(sgbuilding);

            }

            foreach (SGBuilding sgbuilding in buildings)
            {
                sgbuilding.UpdateParamDisplay();
            }
        }
    }
    public class DisplayDimension : Rule
    {
        public DisplayDimension() : base()
        {
            name = "DisplayDimension";
        }
        public DisplayDimension(string inName) : base(inName, "")
        {
            name = "DisplayDimension";
        }
        public override void Execute()
        {
            base.Execute();
            List<SGBuilding> buildings = new List<SGBuilding>();
            foreach (ShapeObject o in inputs.shapes)
            {
                o.gameObject.AddComponent<DrawDimension>();

            }
            outputs.shapes = inputs.shapes;
        }
    }
}
