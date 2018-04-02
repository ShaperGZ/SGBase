using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICreator : MonoBehaviour {

    public static GameObject defaultButton;

    public static GameObject GetDefaultButtonAsset()
    {
        if (defaultButton != null) return defaultButton;
        defaultButton = Resources.Load("ShapeUI/DefaultButton") as GameObject;
        return defaultButton;
    }


}
