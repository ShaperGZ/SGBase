using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGParticle {

    private Vector3 _position;

    public ShapeObject shapeObject;
    public float radius = 10;

    public SGParticle(ShapeObject so)
    {
        shapeObject = so;
    }
    public Vector3 Position
    {
        get { return _position; }
        set
        {
            _position = value;
            if(shapeObject != null)
            {
                shapeObject.Position = value;
            }
        }
    }
}
