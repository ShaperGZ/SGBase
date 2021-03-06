﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public delegate void OnPositionChanged(Vector3 newPos);

public class SOPoint : ShapeObject{
    
    public OnPositionChanged onPositionChanged;
    //public SGBuilding sgbuilding;
    public float _radius = 1;
    public bool sizable = true;
    public float Radius
    {
        get { return _radius; }
        set
        {
            _radius = value;
            float d = value * 2;
            transform.localScale = new Vector3(d,d,d);
        }
    }
    public bool drawOffset = false;
    public bool allowDrag = false;
    private bool startDragging = false;
    public Material colorMaterial;
    public Color Color
    {
        get {
            if (colorMaterial != null)
            {
                return colorMaterial.color;
            }
            return Color.white;
        }
        set
        {
            if (colorMaterial != null)
            {
                colorMaterial.color = value;
            }
        }
    }

    public Vector3 _posOrg;
    public Vector3 _posOffset;
    public override Vector3 Position
    {
        get
        {
            return _posOrg;
        }
        set
        {
            
            _posOrg = value;
            Vector3 newPos= _posOrg + _posOffset;
            if(newPos != transform.position)
                stale = true;

            Vector3 offset = newPos - transform.position;
            transform.position = newPos;
            if(onPositionChanged!=null)
                onPositionChanged(newPos);
            
            //if(grammar!=null && grammar.building!=null)
            //{
            //    grammar.building.transform.position = newPos;
            //    //grammar.UpdateToBuildingPosition();
            //}
                
            Invalidate();
        }
    }
    public Vector3 PositionOffset
    {
        get
        {
            return _posOffset;
        }
        set
        {
            _posOffset = value;
            transform.position = _posOrg + _posOffset;
        }
    }
    Plane plane = new Plane(Vector3.up, Vector3.zero);
    // Update is called once per frame
    void Update () {
        
	}

    public override ShapeObject Clone(bool geometryOnly = true)
    {
        SOPoint sop=SOPoint.CreatePoint(Position); 
        if (geometryOnly)
        {
            return sop;
        }
        sop.parentRule = parentRule;
        sop.name = name;
        

        return sop;
    }

    public static SOPoint CreatePoint(Vector3? location=null)
    {
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        
        
        SOPoint sop=o.AddComponent<SOPoint>();
        if (location == null) location = new Vector3(0, 0, 0);
        {
            sop.Position = location.Value;
            sop.PositionOffset = new Vector3(0, 0, 0);
        }
        sop.Radius = 1;
        sop.alwaysActive = true;
        BoxCollider bc = o.AddComponent<BoxCollider>();
        bc.center = o.transform.position;
        return sop;
    }
    
    private void OnRenderObject()
    {
        if (PositionOffset.magnitude > 0.1f && drawOffset)
        {
            SGGeometry.GLRender.Lines(new Vector3[] { Position, transform.position }, Color.black);
        }
    }
    public override void SetDefaultMaterials()
    {
        //Debug.Log("SOP SetDefaultMat");
        colorMaterial = Instantiate(MaterialManager.GB.Default) as Material;
        materialsByMode[DisplayMode.NORMAL] = colorMaterial;
        materialsByMode[DisplayMode.NAMES] = colorMaterial;
        materialsByMode[DisplayMode.RULE] = colorMaterial;
        materialsByMode[DisplayMode.VISUAL] = colorMaterial;
        if (meshRenderer != null)
            meshRenderer.material = materialsByMode[DisplayMode.NORMAL];
    }

    private void OnMouseEnter()
    {
        //this.Color = Color.red;
        allowDrag = true;
        this.colorMaterial.color = Color.red;
        if(sizable)
            Radius *= 6;
        drawOffset = true;
    }

    private void OnMouseDown()
    {
        if(Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1))
        {
            PositionOffset = new Vector3(0, 0, 0);
            allowDrag = false;
        }
    }

    private void OnMouseExit()
    {
        allowDrag = false;
        this.colorMaterial.color = Color.white;
        if(sizable)
            Radius /=6f;
        drawOffset = false;
    }

    private void OnMouseDrag()
    {
        Drag();
    }
    private void Drag()
    {
        if (!allowDrag) return;
        if(Input.GetKey(KeyCode.LeftShift))
            PositionOffset = ScreenPointToWorld(Input.mousePosition, plane)-Position;
        else
            Position= ScreenPointToWorld(Input.mousePosition, plane)-PositionOffset;
    }

    private Vector3 ScreenPointToWorld(Vector3 sp, Plane workPlane)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(sp);
        Ray ray = Camera.main.ScreenPointToRay(sp);
        float d;
        workPlane.Raycast(ray, out d);
        Vector3 xp = ray.GetPoint(d);
        return xp;
    }
}
