using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility.TerrainAlgorithm;
using Utility;

namespace SimulationConfigsScreen
{
    public class WindDecayConfigsControl : MonoBehaviour
    {
        public Toggle toggleActive;

        public Slider sliderRange;
        public Slider sliderFactor;

        public Text textRangeValue;
        public Text textFactorValue;

        public Dropdown dropDirection;

        public Toggle toggleMoore;

        // Use this for initialization
        void Start()
        {

        }

        public void LoadData(WindDecaySimConfigs data)
        {
            toggleActive.isOn = data.Active;
            sliderRange.value = data.Range;
            sliderFactor.value = data.Factor;

            SelectDropDownFromDirection(data.WindDirection);

            toggleMoore.isOn = data.UseMoore;
        }

        public void UpdateCounters()
        {
            textRangeValue.text = sliderRange.value.ToString();
            textFactorValue.text = sliderFactor.value.ToString();
        }

        public void UpdateData(WindDecaySimConfigs data)
        {
            data.Active = toggleActive.isOn;
            data.Range = (int)sliderRange.value;
            data.Factor = sliderFactor.value;
            data.WindDirection = GetDirectionFromDropdown();
            data.UseMoore = toggleMoore.isOn;
        }

        private Directions GetDirectionFromDropdown()
        {
            return (Directions)dropDirection.value;
        }

        private void SelectDropDownFromDirection(Directions direction)
        {
            dropDirection.value = (int)direction;
        }
    }
}
