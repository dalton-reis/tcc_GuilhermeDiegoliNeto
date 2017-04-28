using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainView;
using UnityEngine.SceneManagement;

namespace SimulationConfigsScreen
{
    public class EventManager : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        public void OnValueChanged()
        {
            UIControl.Instance.UpdateAllCounters();
        }

        public void OnOK()
        {
            UIControl.Instance.UpdateAllData();

            TerrainView.GameControl.Instance.LoadDryErosionConfigs(UIControl.DryErosionConfigs);
            TerrainView.GameControl.Instance.LoadHydroErosionConfigs(UIControl.HydroErosionConfigs);
            
            TerrainView.GameControl.Instance.SetBackgroundMode(false);
            SceneManager.UnloadSceneAsync("SimulationConfigs");
        }

        public void OnOKTest()
        {
            UIControl.Instance.UpdateAllData();

            TerrainView.GameControl.Instance.LoadSmoothConfigs(UIControl.SmoothConfigs);
            TerrainView.GameControl.Instance.LoadWindDecayConfigs(UIControl.WindDecayConfigs);

            TerrainView.GameControl.Instance.SetBackgroundMode(false);
            SceneManager.UnloadSceneAsync("TestConfigs");
        }
    }
}
