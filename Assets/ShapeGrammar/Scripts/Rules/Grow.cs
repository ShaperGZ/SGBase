using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

namespace Rules
{
    public class Grow:Rule
    {
        List<ShapeObject> grewObjects;
        GameObject[] prefab;
        public override void Execute()
        {
            outMeshables.Clear();
            int dif = grewObjects.Count - outMeshables.Count;
            removeExtraObjects(grewObjects, dif);

        }
        public void removeExtraObjects(List<ShapeObject> sos, int dif)
        {
            
            if (dif > 0)
            {
                for (int i = 0; i < dif; i++)
                {
                    int index = sos.Count - 1;
                    try
                    {
                        GameObject.Destroy(sos[index].gameObject);

                    }
                    catch { }
                    sos.RemoveAt(index);
                }
            }
        }
    }

}
