using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility.TerrainAlgorithm;
using Utility;

namespace SimulationConfigs
{
    public class DryErosionConfigsControl : MonoBehaviour
    {
        public Toggle toggleActive;

        public Slider sliderIncline;
        public Slider sliderFactor;

        public Text textInclineValue;
        public Text textFactorValue;

        // Use this for initialization
        void Start()
        {

        }

        public void LoadData(DryErosionSimConfigs data)
        {
            toggleActive.isOn = data.Active;
            sliderIncline.value = (float)data.MaxInclination;
            sliderFactor.value = (float)data.DistributionFactor;
        }

        public void UpdateCounters()
        {
            textInclineValue.text = sliderIncline.value.ToString();
            textFactorValue.text = sliderFactor.value.ToString();
        }

        public void UpdateData(DryErosionSimConfigs data)
        {
            data.Active = toggleActive.isOn;
            data.MaxInclination = sliderIncline.value;
            data.DistributionFactor = sliderFactor.value;
        }
    }
}
