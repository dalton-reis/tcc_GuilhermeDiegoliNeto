using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TerrainAlgorithm;

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
        SmoothTransform transform = terrainControl.transformSet.transformSet[(int)TransformIndex.Smooth] as SmoothTransform;

        sliderRange.value = transform.range;
        sliderFactor.value = transform.factor;

        UpdateValues();
    }

    protected override void SaveConfigs(TerrainControl terrainControl)
    {
        SmoothTransform transform = terrainControl.transformSet.transformSet[(int)TransformIndex.Smooth] as SmoothTransform;

        transform.range = (int)sliderRange.value;
        transform.factor = sliderFactor.value;
    }
}
