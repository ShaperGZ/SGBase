using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGCore;
using SGGeometry;

public class Floor{
    
    public List<ShapeObject> forms;
    public List<Meshable> floor;
    public List<Meshable> walls;
    public List<GameObject> components;

    public Floor()
    {
        forms = new List<ShapeObject>();
        floor = new List<Meshable>();
        walls = new List<Meshable>();
        components = new List<GameObject>();
    }
    public void SetMeshables(List<Meshable> mbs)
    {
        int diff = forms.Count- mbs.Count;
        for (int i = 0; i < diff; i++)
        {
            try
            {
                int index = forms.Count - 1;
                GameObject.Destroy(forms[index].gameObject);
                forms.RemoveAt(index);
            }
            catch { Debug.Log("Exception"); }
        }

        for (int i = 0; i < mbs.Count; i++)
        {
            if (i >= forms.Count) forms.Add(ShapeObject.CreateMeshable(mbs[i]));
            else forms[i].SetMeshable(mbs[i]);
            forms[i].name = "floor";
        }

    }
    public void SetVisible(bool flag)
    {
        foreach(ShapeObject so in forms)
        {
            so.SetVisible(flag);
        }
    }
    public void SetActive(bool flag)
    {
        foreach (ShapeObject so in forms)
        {
            so.gameObject.SetActive(flag);
        }
    }
    public void Destroy()
    {
        try
        {
            foreach (ShapeObject so in forms)
            {
                GameObject.Destroy(so.gameObject);
            }
        }
        catch{ }
    }

}
public class Floor2
{

    public Dictionary<ShapeObject, ShapeObject> forms;
    public List<Meshable> floor;
    public List<Meshable> walls;
    public List<GameObject> components;

    public Floor2()
    {
        forms = new Dictionary<ShapeObject, ShapeObject>();
        floor = new List<Meshable>();
        walls = new List<Meshable>();
        components = new List<GameObject>();
    }
    public void SetForm(Meshable mb, ShapeObject parent)
    {
        if (!forms.ContainsKey(parent)) forms[parent] = ShapeObject.CreateMeshable(mb);
        else forms[parent].SetMeshable(mb);
        forms[parent].name = "floor_form";
    }
    public void SetVisible(bool flag)
    {
        foreach (KeyValuePair<ShapeObject, ShapeObject> kv in forms)
        {
            kv.Value.SetVisible(flag);
        }
    }
    public void SetActive(bool flag)
    {
        foreach (KeyValuePair<ShapeObject, ShapeObject> kv in forms)
        {
            kv.Value.gameObject.SetActive(flag);
        }
    }
    public void Destroy()
    {
        try
        {
            foreach (KeyValuePair<ShapeObject, ShapeObject> kv in forms)
            {
                GameObject.Destroy(kv.Value.gameObject);
            }
        }
        catch { }
    }

}
