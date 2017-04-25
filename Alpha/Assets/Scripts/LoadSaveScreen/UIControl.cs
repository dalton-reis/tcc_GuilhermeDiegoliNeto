using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoadSaveScreen
{
    public class UIControl : MonoBehaviour
    {

        public static UIControl Instance { get; private set; }

        public InputField saveFileInput;
        public InputField loadFileInput;
        public InputField heightMapInput;

        public Dropdown heightMapType;

        public Slider soilDepth;
        public Text soilDepthValue;

        // Use this for initialization
        void Start()
        {
            Instance = this;
        }

        public void UpdateValues()
        {
            soilDepthValue.text = soilDepth.value.ToString();
        }

        public float GetSoilDepth()
        {
            return soilDepth.value;
        }
    }
}
