using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

namespace Rules
{
    //creation
    public class CreateBox : Rule
    {
        public CreateBox() : base()
        {
            outputs.names.Add("A");

        }
        public CreateBox(string outName, Vector3 pos, Vector3 size, Vector3 rotation) : base()
        {
            outputs.names.Add("A");
            for (int i = 0; i < 3; i++)
            {
                ((ParameterGroup)paramGroups["Position"]).parameters[i].Value = pos[i];
                ((ParameterGroup)paramGroups["Position"]).parameters[i].min = pos[i] / 5;
                ((ParameterGroup)paramGroups["Position"]).parameters[i].max = pos[i] * 5;

                ((ParameterGroup)paramGroups["Size"]).parameters[i].Value = size[i];
                ((ParameterGroup)paramGroups["Size"]).parameters[i].min = pos[i] / 5;
                ((ParameterGroup)paramGroups["Size"]).parameters[i].max = pos[i] * 5;

                ((ParameterGroup)paramGroups["Rotation"]).parameters[i].Value = rotation[i];
            }
        }
        public override List<Meshable> ExecuteShape(ShapeObject so)
        {
            float[] pos = new float[3];
            float[] size = new float[3];
            for (int i = 0; i < 3; i++)
            {
                pos[i] = ((ParameterGroup)paramGroups["Position"]).parameters[i].Value;
                size[i] = ((ParameterGroup)paramGroups["Size"]).parameters[i].Value;
            }
            Vector3 vpos = new Vector3(pos[0], pos[1], pos[2]);
            Vector3 vsize = new Vector3(size[0], size[1], size[2]);

            Vector3[] pts = new Vector3[4];
            Vector3 vx = new Vector3(size[0], 0, 0);
            Vector3 vy = new Vector3(0, size[1], 0);
            Vector3 vz = new Vector3(0, 0, size[2]);


            pts[0] = vpos;
            pts[1] = vpos + vx;
            pts[2] = pts[1] + vz;
            pts[3] = pts[0] + vz;
            Polygon pg = new Polygon(pts);
            Form f = pg.ExtrudeToForm(vy);

            List<Meshable> outs = new List<Meshable>();
            outs.Add(f);
            AssignNames(outs);
            return outs;

        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();
            ParameterGroup pg3 = new ParameterGroup();

            for (int i = 0; i < 3; i++)
            {
                pg1.Add(new Parameter(0, 0, 100, 3f));
                pg2.Add(new Parameter(10, 0, 100, 3f));
                pg3.Add(new Parameter(0, 0, 90, 1));
            }
            dict.Add("Position", pg1);
            dict.Add("Size", pg2);
            dict.Add("Rotation", pg3);

            return dict;
        }

    }
    public class CreateOperableBox : Rule
    {
        public CreateOperableBox() : base()
        {
            outputs.names.Add("A");

        }
        public CreateOperableBox(string outName, Vector3 size) : base()
        {
            outputs.names.Add("A");
            for (int i = 0; i < 3; i++)
            {
                ((ParameterGroup)paramGroups["Size"]).parameters[i].Value = size[i];
                ((ParameterGroup)paramGroups["Rotation"]).parameters[i].Value = 0;
            }
        }

        public override List<Meshable> ExecuteShape(ShapeObject inSo)
        {
            float[] size = new float[3];
            for (int i = 0; i < 3; i++)
            {
                size[i] = ((ParameterGroup)paramGroups["Size"]).parameters[i].Value;
            }
            //Vector3 vpos = Vector3.zero;
            Vector3 vpos = inSo.Position;
            //Vector3 vsize = new Vector3(size[0], size[1], size[2]);

            Vector3[] pts = new Vector3[4];
            Vector3 vx = new Vector3(size[0], 0, 0);
            Vector3 vy = new Vector3(0, size[1], 0);
            Vector3 vz = new Vector3(0, 0, size[2]);


            pts[0] = vpos;
            pts[1] = vpos + vx;
            pts[2] = pts[1] + vz;
            pts[3] = pts[0] + vz;
            Polygon pg = new Polygon(pts);
            Meshable f = pg.Extrude(vy);

            List<Meshable> outs = new List<Meshable>();
            outs.Add(f);
            AssignNames(outs);
            return outs;

        }
        public override OrderedDictionary DefaultParam()
        {
            OrderedDictionary dict = new OrderedDictionary();
            ParameterGroup pg1 = new ParameterGroup();
            ParameterGroup pg2 = new ParameterGroup();

            for (int i = 0; i < 3; i++)
            {
                pg1.Add(new Parameter(10, 0, 100, 3f));
                pg2.Add(new Parameter(0, 0, 90, 1));
            }
            dict.Add("Size", pg1);
            dict.Add("Rotation", pg2);

            return dict;
        }

    }
    public class CreateStair : Rule
    {
        public float stairW = 4;
        public float stairH = 10;
        public CreateStair() : base() { }
        public CreateStair(string inName, string outName) : base(inName, outName) { }
        public override List<Meshable> ExecuteShape(ShapeObject inSo)
        {

            inSo.PivotTurn(2);
            float w = inSo.Size[0];
            List<Vector3> stairLocs = new List<Vector3>();
            if (w <= 30)
            {
                stairLocs.Add(inSo.Position);
                Vector3 v = inSo.Vects[0];
                v *= (w - stairW);
                stairLocs.Add(inSo.Position + v);
            }
            else if (w < 45)
            {
                stairLocs.Add(inSo.Position);
                Vector3 v = inSo.Vects[0];
                v *= 30 - stairW;
                stairLocs.Add(inSo.Position + v);
            }
            else if (w < 60)
            {
                float d1 = w / 2 - 15;
                float d2 = w - d1;
                Vector3 v1 = inSo.Vects[0] * d1;
                Vector3 v2 = inSo.Vects[0] * d2;
                stairLocs.Add(inSo.Position + v1);
                stairLocs.Add(inSo.Position + v2);
            }
            else
            {
                stairLocs.Add(inSo.Position);
                Vector3 v1 = inSo.Vects[0] * (30 - stairW);
                Vector3 v2 = inSo.Vects[0] * (60 - stairW);
                stairLocs.Add(inSo.Position + v1);
                stairLocs.Add(inSo.Position + v2);
            }

            List<Meshable> outs = new List<Meshable>();
            float coreH = inSo.Size[1] + 3;
            foreach (Vector3 v in stairLocs)
            {
                outs.Add(CreateBox(v, new Vector3(stairW, coreH, stairH), inSo.Vects));
            }

            AssignNames(outs);
            inSo.meshable.name = inputs.names[0];
            outs.Add(inSo.meshable);
            return outs;

        }
        protected Meshable CreateBox(Vector3 pos, Vector3 size, Vector3[] vects)
        {
            Vector3[] pts = new Vector3[4];
            Vector3 mv1 = vects[0] * size[0];
            Vector3 mv2 = vects[1] * size[1];
            Vector3 mv3 = vects[2] * size[2];
            pts[0] = pos;
            pts[1] = pts[0] + mv1;
            pts[2] = pts[1] + mv3;
            pts[3] = pts[0] + mv3;
            Extrusion ext = new Extrusion(pts, size[1]);
            return ext;
        }
    }

    public class CreateBuilding : Rule
    {
        public DesignContext site;
        public SGParticleSystem psys;
        PlaningMatrix3 matrix;
        public List<SGBuilding> buildings;
        public CreateBuilding() : base() { }
        public CreateBuilding(string inName, PlaningMatrix3 matrix=null, SGParticleSystem psys=null) : base(inName, new string[] { })
        {
            this.matrix = matrix;
            this.psys = psys;
            Debug.Log("assigned matrix=" + matrix);
        }
        public override void Execute()
        {
            if (matrix == null) return;
            PlaningScheme scheme = matrix.recommendedScheme;
            //Debug.Log("scheme="+scheme);
            if (scheme == null) return;
            //Debug.Log("has scheme");
            int total = 0;
            for (int i = 0; i < scheme.counts.Count; i++)
            {
                total += scheme.counts[i];
            }
            total = Mathf.Clamp(total, 0,inputs.shapes.Count);

            if (buildings == null) buildings = new List<SGBuilding>();
            else
            {
                //先删除多出来的building
                int dif = buildings.Count - total;
                for (int i = 0; i < dif; i++)
                {
                    int index = buildings.Count - 1;
                    buildings[index].ClearAllAssociated();
                    buildings.RemoveAt(index);
                }//for
            }


            int typeIndex = 0;
            int nextLevel = scheme.counts[0];
            for (int i = 0; i < total; i++)
            {
                if (i >= nextLevel)
                {
                    //Debug.LogFormat("i={0}, typeIndex={1}, total={2}", i, typeIndex,total);
                    typeIndex++;
                    nextLevel = nextLevel + scheme.counts[typeIndex];
                }
                     
                ShapeObject so = inputs.shapes[i];
                //补足不够的building
                if (i >= buildings.Count)
                {
                    SOPoint sop = SOPoint.CreatePoint(new Vector3(i * 40, 0, 0));
                    SGBuilding building = SGBuilding.CreateApt(sop, new Vector3(30, 60, 15));
                    building.Execute();
                    buildings.Add(building);
                }

                
                BuildingType bt = scheme.buildingTypes[typeIndex];
                
                Vector3 size=new Vector3(bt.width,bt.height,bt.depth);
                SGBuilding b = buildings[i];
                b.gPlaning.inputs.shapes[0].Position = so.Position;
                b.SetSize(size);
                b.Execute();

            }//for


            //update particle system
            if (psys != null)
            {
                psys.particles.Clear() ;
                for (int i = 0; i < buildings.Count; i++)
                {
                    GraphNode g = buildings[i].gPlaning;
                    ShapeObject sop = g.inputs.shapes[0];
                    psys.particles.Add(sop);
                }
            }
            
        }
    }

    
}