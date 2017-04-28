using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility.TerrainAlgorithm;

namespace SimulationConfigsScreen
{
    public class SmoothConfigsControl : MonoBehaviour
    {
        public Toggle toggleActive;

        public Slider sliderRange;
        public Slider sliderFactor;

        public Text textRangeValue;
        public Text textFactorValue;

        public Toggle toggleMoore;

        // Use this for initialization
        void Start()
        {

        }

        public void LoadData(SmoothSimConfigs data)
        {
            toggleActive.isOn = data.Active;
            sliderRange.value = data.Range;
            sliderFactor.value = data.Factor;
            toggleMoore.isOn = data.UseMoore;
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
            data.UseMoore = toggleMoore.isOn;
        }
    }
}