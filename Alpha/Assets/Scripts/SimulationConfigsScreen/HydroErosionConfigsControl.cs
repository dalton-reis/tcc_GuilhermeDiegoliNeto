using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Utility.TerrainAlgorithm;

namespace SimulationConfigsScreen
{
    public class HydroErosionConfigsControl : MonoBehaviour
    {
        public Toggle toggleActive;

        public Slider sliderRainIntensity;
        public Slider sliderRainInterval;
        public Slider sliderEvaporation;
        public Slider sliderSolubility;

        public Text textRainIntensityValue;
        public Text textRainIntervalValue;
        public Text textEvaporationValue;
        public Text textSolubilityValue;

        void Start()
        {

        }

        public void LoadData(HydroErosionSimConfigs data)
        {
            toggleActive.isOn = data.Active;
            sliderRainIntensity.value = data.RainIntensity;
            sliderRainInterval.value = data.RainInterval;
            sliderEvaporation.value = data.EvaporationFactor;
            sliderSolubility.value = data.TerrainSolubility;
        }

        public void UpdateCounters()
        {
            textRainIntensityValue.text = sliderRainIntensity.value.ToString();
            textRainIntervalValue.text = sliderRainInterval.value.ToString();
            textEvaporationValue.text = sliderEvaporation.value.ToString();
            textSolubilityValue.text = sliderSolubility.value.ToString();
        }

        public void UpdateData(HydroErosionSimConfigs data)
        {
            data.Active = toggleActive.isOn;
            data.RainIntensity = sliderRainIntensity.value;
            data.RainInterval = (int)sliderRainInterval.value;
            data.EvaporationFactor = sliderEvaporation.value;
            data.TerrainSolubility = sliderSolubility.value;
        }
    }
}
