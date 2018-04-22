using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class SOBox : ShapeObject {
    BoundingBox boundingBox;
    Vector3 _size;
    public override Vector3 Size
    {
        get
        {
            return _size;
        }
        set
        {
            _size = value;
        }
    }

    public SOBox():base()
    {
        boundingBox = new BoundingBox();
        _size = new Vector3(1, 1, 1);
    }
    public SOBox(Vector3 pos, Vector3 size, Vector3 forward):this()
    {
        transform.position = pos;
        transform.LookAt(pos + forward);
        Size = size;
    }
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
}
