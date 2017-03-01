using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility.TerrainAlgorithm;

namespace SimulationConfigs
{
    public class SmoothConfigsControl : MonoBehaviour
    {
        public Toggle toggleActive;

        public Slider sliderRange;
        public Slider sliderFactor;

        public Text textRangeValue;
        public Text textFactorValue;

        // Use this for initialization
        void Start()
        {

        }

        public void LoadData(SmoothSimConfigs data)
        {
            toggleActive.isOn = data.Active;
            sliderRange.value = data.Range;
            sliderFactor.value = data.Factor;
        }

        public void UpdateCounters()
        {
            textRangeValue.text = sliderRange.value.ToString();
            textFactorValue.text = sliderFactor.value.ToString();
        }

        public void UpdateData(SmoothSimConfigs data)
        {
            data.Active = toggleActive.isOn;
            data.Range = (int)sliderRange.value;
            data.Factor = sliderFactor.value;
        }
    }
}