using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DisplayManager:MonoBehaviour
{
    public Button btNormal;
    public Button btRule;
    public Button btVisual;
    public Button btNames;

    public int currMode = DisplayMode.NORMAL;
    public int lastMode = DisplayMode.NORMAL;

    private void Start()
    {
        btNormal.onClick.AddListener(delegate { setNormalMode(); });
        btRule.onClick.AddListener(delegate { setRuleMode(); });
        setNormalMode();  
    }

    public void SetMode(int mode)
    {
        currMode = mode;
    }

    public void setNormalMode()
    {
        foreach (ShapeObject o in SceneManager.existingShapes.Values)
        {
            o.SetMaterial(DisplayMode.NORMAL);
        }
    }
    public void setRuleMode()
    {

        if (SceneManager.SelectedGrammar == null)
        {
            setNormalMode();
            return;
        }
        if (SceneManager.existingShapes == null) return;
        foreach(ShapeObject o in SceneManager.existingShapes.Values)
        {
            if (o.parentRule == null) continue;
            if (o.parentRule.grammar == null) continue;
            if (o.parentRule.grammar == SceneManager.SelectedGrammar)
            {
                o.SetMaterial(DisplayMode.RULE);
            }
            else
            {
                o.SetMaterial(DisplayMode.NORMAL);
            }
        }
    }
    public void setNameMode()
    {

    }

}