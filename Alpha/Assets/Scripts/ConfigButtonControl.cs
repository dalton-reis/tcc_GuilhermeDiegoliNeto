using UnityEngine;
using System.Collections;

public class ConfigButtonControl : MonoBehaviour {

    public TerrainControl terrainControl = null;
    public ConfigPanel configPanel = null;

    
    public void TogglePanel()
    {
        if (configPanel != null)
        {
            configPanel.ToggleConfigPanel(terrainControl);
        }
    }
}
