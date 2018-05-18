using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SGCore;
using SGGeometry;

public class Inventory:MonoBehaviour{

    public GameObject planingInventory;
    public GameObject massingInventory;
    public GameObject GraphicsInventory;
    public GameObject UnitsInventory;

    public void Start()
    {
        
    }

    public void planningMode()
    {
        if (planingInventory != null) planingInventory.SetActive(true);
        if (massingInventory != null) massingInventory.SetActive(false);
        if (GraphicsInventory != null) GraphicsInventory.SetActive(false);
        if (UnitsInventory != null) UnitsInventory.SetActive(false);
    }
    public void MassingMode()
    {
        if (planingInventory != null) planingInventory.SetActive(false);
        if (massingInventory != null) massingInventory.SetActive(true);
        if (GraphicsInventory != null) GraphicsInventory.SetActive(false);
        if (UnitsInventory != null) UnitsInventory.SetActive(false);
    }
    public void SetMassingA() { SceneManager.SelectedBuilding.SetMassing(MassingGrammars.GA(),true); }
    public void SetMassingB() { SceneManager.SelectedBuilding.SetMassing(MassingGrammars.GB(),true); }
    public void SetMassingC() { SceneManager.SelectedBuilding.SetMassing(MassingGrammars.GC(),true); }
   
}
