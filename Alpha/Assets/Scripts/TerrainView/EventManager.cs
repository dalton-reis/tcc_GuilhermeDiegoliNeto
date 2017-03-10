using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimulationConfigs;
using Utility.TerrainAlgorithm;
using UnityEngine.UI;

namespace TerrainView
{
    public class EventManager : MonoBehaviour
    {
        public void OnLoadSave()
        {
            GameControl.Instance.SetBackgroundMode(true);
            SceneManager.LoadScene("LoadSaveScreen", LoadSceneMode.Additive);
        }

        public void OnSimulationConfigs()
        {
            SmoothSimConfigs smoothConfigs = new SmoothSimConfigs();
            SmoothTransform smooth =  TerrainControl.Instance.transformSet.transformSet[0] as SmoothTransform;
            smoothConfigs = smooth.Configs;

            WindDecaySimConfigs WindDecayConfigs = new WindDecaySimConfigs();
            WindDecayDigTransform WindDecay = TerrainControl.Instance.transformSet.transformSet[1] as WindDecayDigTransform;
            WindDecayConfigs = WindDecay.Configs;

            SimulationConfigs.UIControl.SmoothConfigs = smoothConfigs;
            SimulationConfigs.UIControl.WindDecayConfigs = WindDecayConfigs;

            GameControl.Instance.SetBackgroundMode(true);
            SceneManager.LoadScene("SimulationConfigs", LoadSceneMode.Additive);
        }
    }
}