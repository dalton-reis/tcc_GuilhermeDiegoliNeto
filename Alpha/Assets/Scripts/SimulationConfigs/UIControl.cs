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
        public static WindDecaySimConfigs WindDecayConfigs { get; set; }

        public SmoothConfigsControl smoothControl;
        public WindDecayConfigsControl WindDecayControl;

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
            WindDecayControl.LoadData(WindDecayConfigs);
        }

        public void UpdateAllCounters()
        {
            smoothControl.UpdateCounters();
            WindDecayControl.UpdateCounters();
        }

        public void UpdateAllData()
        {
            smoothControl.UpdateData(SmoothConfigs);
            WindDecayControl.UpdateData(WindDecayConfigs);
        }
    }
}
