using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility.TerrainAlgorithm;
using Utility;

namespace SimulationConfigs
{
    public class WindDecayConfigsControl : MonoBehaviour
    {
        public Toggle toggleActive;

        public Slider sliderRange;
        public Slider sliderFactor;

        public Text textRangeValue;
        public Text textFactorValue;

        public Dropdown dropDirection;

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
        }

        private Directions GetDirectionFromDropdown()
        {
            switch (dropDirection.value)
            {
                case 0: return Directions.North;
                case 1: return Directions.East;
                case 2: return Directions.South;
                case 3: return Directions.West;
            }

            return Directions.North;
        }

        private void SelectDropDownFromDirection(Directions direction)
        {
            int index = 0;

            switch (direction)
            {
                case Directions.North:
                    index = 0;
                    break;
                case Directions.Northeast:
                    break;
                case Directions.East:
                    index = 1;
                    break;
                case Directions.Southeast:
                    break;
                case Directions.South:
                    index = 2;
                    break;
                case Directions.SouthWest:
                    break;
                case Directions.West:
                    index = 3;
                    break;
                case Directions.Northwest:
                    break;
                default:
                    break;
            }

            dropDirection.value = index;
        }
    }
}
