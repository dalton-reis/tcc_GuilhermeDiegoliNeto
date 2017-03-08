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

        // Use this for initialization
        void Start()
        {
            Instance = this;
        }

    }
}
