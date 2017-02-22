using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TerrainAlgorithm;


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
        OneSideDigTransform transform = terrainControl.transformSet.transformSet[(int)TransformIndex.OneSideDig] as OneSideDigTransform;

        sliderRange.value = transform.range;
        sliderFactor.value = transform.factor;

        UpdateValues();
    }

    protected override void SaveConfigs(TerrainControl terrainControl)
    {
        OneSideDigTransform transform = terrainControl.transformSet.transformSet[(int)TransformIndex.OneSideDig] as OneSideDigTransform;

        transform.range = (int)sliderRange.value;
        transform.factor = sliderFactor.value;
    }
}
