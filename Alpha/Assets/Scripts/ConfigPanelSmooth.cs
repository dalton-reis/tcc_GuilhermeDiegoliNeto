using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConfigPanelSmooth : ConfigPanel
{
    public Slider sliderRange = null;
    public Slider sliderFactor = null;

    public Text textRange = null;
    public Text textFactor = null;


    public override void UpdateValues()
    {
        textRange.text = sliderRange.value.ToString();
        textFactor.text = sliderFactor.value.ToString();
    }

    protected override void LoadConfigs(TerrainControl terrainControl)
    {
        sliderRange.value = terrainControl.smoothRange;
        sliderFactor.value = terrainControl.smoothFactor;

        UpdateValues();
    }

    protected override void SaveConfigs(TerrainControl terrainControl)
    {
        terrainControl.smoothRange = (int)sliderRange.value;
        terrainControl.smoothFactor = sliderFactor.value;

        terrainControl.UpdateConfigs();
    }
}
