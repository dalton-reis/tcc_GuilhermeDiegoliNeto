using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.TerrainAlgorithm;

namespace SimulationConfigsScreen
{
    public class UIControl : MonoBehaviour
    {
        // Singleton
        public static UIControl Instance { get; private set; }

        // Test Configs
        public static SmoothSimConfigs SmoothConfigs { get; set; }
        public static WindDecaySimConfigs WindDecayConfigs { get; set; }

        public SmoothConfigsControl smoothControl;
        public WindDecayConfigsControl windDecayControl;

        // Sim Configs
        public static DryErosionSimConfigs DryErosionConfigs { get; set; }
        public static HydroErosionSimConfigs HydroErosionConfigs { get; set; }

        public DryErosionConfigsControl dryErosionControl;
        public HydroErosionConfigsControl hydroErosionControl;

        // Use this for initialization
        void Start()
        {
            Instance = this;

            LoadData();
            UpdateAllCounters();
        }

        public void LoadData()
        {
            if (smoothControl != null) smoothControl.LoadData(SmoothConfigs);
            if (windDecayControl != null) windDecayControl.LoadData(WindDecayConfigs);

            if (dryErosionControl != null) dryErosionControl.LoadData(DryErosionConfigs);
            if (hydroErosionControl != null) hydroErosionControl.LoadData(HydroErosionConfigs);
        }

        public void UpdateAllCounters()
        {
            if (smoothControl != null) smoothControl.UpdateCounters();
            if (windDecayControl != null) windDecayControl.UpdateCounters();

            if (dryErosionControl != null) dryErosionControl.UpdateCounters();
            if (hydroErosionControl != null) hydroErosionControl.UpdateCounters();
        }

        public void UpdateAllData()
        {
            if (smoothControl != null) smoothControl.UpdateData(SmoothConfigs);
            if (windDecayControl != null) windDecayControl.UpdateData(WindDecayConfigs);

            if (dryErosionControl != null) dryErosionControl.UpdateData(DryErosionConfigs);
            if (hydroErosionControl != null) hydroErosionControl.UpdateData(HydroErosionConfigs);
        }
    }
}
