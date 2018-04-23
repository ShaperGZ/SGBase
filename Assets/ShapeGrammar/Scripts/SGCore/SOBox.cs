using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class SOBox : ShapeObject {
    BoundingBox boundingBox;

    
    public static SOBox Create(Vector3 pos, Vector3 size, Vector3 forward)
    {
        GameObject o = new GameObject();
        SOBox so = o.AddComponent<SOBox>();
        o.transform.LookAt(forward);
        o.transform.position = pos;
        so.Size = size;
        return so;
        
    }
   
    private void OnRenderObject()
    {
        //GLDrawScope(Color.black);
        if (true)
        {
            Color c = Color.black;

            GLDrawScope(c);
        }
    }
    public override ShapeObject Clone(bool geometryOnly = true)
    {
        GameObject o = new GameObject();

        SOBox so = o.AddComponent<SOBox>();
        so.Size = Size;
        so.boundingBox = boundingBox;
        return so;
    }
}
