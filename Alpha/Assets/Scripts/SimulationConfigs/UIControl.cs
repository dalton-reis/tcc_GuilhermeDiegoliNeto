using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.TerrainAlgorithm;

namespace SimulationConfigs
{
    public class UIControl : MonoBehaviour
    {
        // Singleton
        public static UIControl Instance { get; private set; }
        public static SmoothSimConfigs SmoothConfigs { get; set; }
        public static OneSideSimConfigs OneSideConfigs { get; set; }

        public SmoothConfigsControl smoothControl;
        public OneSideConfigsControl oneSideControl;

        // Use this for initialization
        void Start()
        {
            Instance = this;

            LoadData();
            UpdateAllCounters();
        }

        public void LoadData()
        {
            smoothControl.LoadData(SmoothConfigs);
            oneSideControl.LoadData(OneSideConfigs);
        }

        public void UpdateAllCounters()
        {
            smoothControl.UpdateCounters();
            oneSideControl.UpdateCounters();
        }

        public void UpdateAllData()
        {
            smoothControl.UpdateData(SmoothConfigs);
            oneSideControl.UpdateData(OneSideConfigs);
        }
    }
}
