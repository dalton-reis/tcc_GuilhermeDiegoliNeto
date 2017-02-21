using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ConfigPanelOneSide : ConfigPanel {

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
        sliderRange.value = terrainControl.oneSideDigRange;
        sliderFactor.value = terrainControl.oneSideDigFactor;

        UpdateValues();
    }

    protected override void SaveConfigs(TerrainControl terrainControl)
    {
        terrainControl.oneSideDigRange = (int)sliderRange.value;
        terrainControl.oneSideDigFactor = sliderFactor.value;

        terrainControl.UpdateConfigs();
    }
}
