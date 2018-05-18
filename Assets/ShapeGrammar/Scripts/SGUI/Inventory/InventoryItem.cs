using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGGeometry;
using SGCore;


public class InventoryItem
{
    public GameObject buttonObject;
    public Button button;
    public string name;

    public InventoryItem(string name)
    {
        buttonObject = DefaultControls.CreateButton(SceneManager.uiResources);
        button = buttonObject.GetComponent<Button>();
        button.name = "Button";
        this.name = name;
        button.onClick.AddListener(delegate { callback(); });

    }
    public virtual void callback()
    {
        //override
    }
    public void Destroy()
    {
        GameObject.Destroy(buttonObject);
    }
}
public class GrammarItem : InventoryItem
{
    private Type nodeType;
    private SGBuilding building;
    //private Grammar tobeReplaced;

    public GrammarItem(string name, Type nodeType, SGBuilding building) : base(name)
    {
        this.nodeType = nodeType;
        this.building = building;

    }
    public override void callback()
    {
        Grammar g = Activator.CreateInstance(nodeType) as Grammar;
        building.SetGrammar(g);
    }

}

