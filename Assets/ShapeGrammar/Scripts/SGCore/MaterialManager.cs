using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MaterialManager:MonoBehaviour{

    public Material Default;
    public Material RuleSelect;
    public Material RuleSelectCommonName;
    public Material RuleEditing;
    public Material NameDifferentiate;

    //visualization
    public Material Wall0;
    public Material Roof0;
    public Material Grass0;
    public Material Glass0;

    public static MaterialManager _GB;
    public static MaterialManager GB
    {
        get {
            //if (_GB == null)
            //    _GB = new MaterialManager();
            return _GB;
        }
    }
    public void Awake()
    {
        _GB = this;
    }


}
