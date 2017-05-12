using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StatsScreen
{
    class UIControl : MonoBehaviour
    {
        // Singleton
        public static UIControl Instance { get; private set; }

        // Dados de entrada
        public static float[,] WaterMap { private get; set; }
        public static float[,] SoilMap { private get; set; }
        public static float[,] RockMap { private get; set; }
        public static int[,] SurfaceMap { private get; set; }
        public static float[,] HumidityMap { private get; set; }

        public Text[] textValues;

        void Start()
        {
            Instance = this;

            StatCollector collector = new StatCollector();
            collector.WaterMap      = WaterMap;
            collector.SoilMap       = SoilMap;
            collector.RockMap       = RockMap;
            collector.SurfaceMap    = SurfaceMap;
            collector.HumidityMap   = HumidityMap;

            collector.CollectStats();

            for (int i = 0; i < (int)Stats.Count; i++)
            {
                textValues[i].text = collector.Output[i];
            }
        }

        public void OnClose()
        {
            TerrainView.GameControl.Instance.SetBackgroundMode(false);
            SceneManager.UnloadSceneAsync("StatsScreen");
        }
    }
}
