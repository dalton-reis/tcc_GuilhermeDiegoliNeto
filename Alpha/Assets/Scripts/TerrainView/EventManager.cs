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
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
            {
                LoadTestConfigs();
            }
            else
            {
                LoadSimulationConfigs();
            }
        }

        private void LoadSimulationConfigs()
        {
            DryErosionTransform dryErosion = TerrainControl.Instance.transformSet.transformSet[3] as DryErosionTransform;
            DryErosionSimConfigs dryErosionConfigs = dryErosion.Configs;

            SimulationConfigs.UIControl.DryErosionConfigs = dryErosionConfigs;

            GameControl.Instance.SetBackgroundMode(true);
            SceneManager.LoadScene("SimulationConfigs", LoadSceneMode.Additive);
        }

        private void LoadTestConfigs()
        {
            SmoothTransform smooth = TerrainControl.Instance.transformSet.transformSet[0] as SmoothTransform;
            SmoothSimConfigs smoothConfigs = smooth.Configs;

            WindDecayDigTransform windDecay = TerrainControl.Instance.transformSet.transformSet[1] as WindDecayDigTransform;
            WindDecaySimConfigs windDecayConfigs = windDecay.Configs;

            SimulationConfigs.UIControl.SmoothConfigs = smoothConfigs;
            SimulationConfigs.UIControl.WindDecayConfigs = windDecayConfigs;

            GameControl.Instance.SetBackgroundMode(true);
            SceneManager.LoadScene("TestConfigs", LoadSceneMode.Additive);
        }
    }
}