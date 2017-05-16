using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility.HeatAlgorithm;

namespace ViewScreen
{
    class UIControl : MonoBehaviour
    {
        // Singleton
        public static UIControl Instance { get; private set; }

        public static HeatTypes ViewMode { private get; set; }

        public Dropdown dropdownViewMode = null;

        void Start()
        {
            Instance = this;

            dropdownViewMode.value = ((int)ViewMode) + 1;
        }

        public void OnClose()
        {
            TerrainView.GameControl.Instance.SetHeatMode((HeatTypes)dropdownViewMode.value - 1);
            TerrainView.GameControl.Instance.SetBackgroundMode(false);
            SceneManager.UnloadSceneAsync("ViewScreen");
        }
    }
}
