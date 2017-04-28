using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimulationConfigsScreen;
using Utility.TerrainAlgorithm;
using UnityEngine.UI;

namespace TerrainView
{
    public class EventManager : MonoBehaviour
    {
        public Slider sliderSimSpeed;

        /// <summary>
        /// Chamar ao clicar no botão "carregar/salvar".
        /// </summary>
        public void OnLoadSave()
        {
            GameControl.Instance.SetBackgroundMode(true);
            SceneManager.LoadScene("LoadSaveScreen", LoadSceneMode.Additive);
        }

        /// <summary>
        /// Chamar ao clicar no botão "configurar simulação".
        /// </summary>
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

        /// <summary>
        /// Chamar ao clicar no botão "editar".
        /// </summary>
        public void OnEditConfigs()
        {
            EditConfigsScreen.UIControl.EditConfigs = TerrainControl.Instance.EditConfigs;

            GameControl.Instance.SetBackgroundMode(true);
            SceneManager.LoadScene("EditConfigs", LoadSceneMode.Additive);
        }

        public void OnChangeSimulationSpeed()
        {
            TerrainControl.Instance.SimulationInterval = (int)(sliderSimSpeed.value * 10000);
        }

        private void LoadSimulationConfigs()
        {
            DryErosionTransform dryErosion = TerrainControl.Instance.transformSet[TransformIndex.DryErosion] as DryErosionTransform;
            DryErosionSimConfigs dryErosionConfigs = dryErosion.Configs;

            HydroErosionTransform hydroErosion = TerrainControl.Instance.transformSet[TransformIndex.HydroErosion] as HydroErosionTransform;
            HydroErosionSimConfigs hydroErosionConfigs = hydroErosion.Configs;

            SimulationConfigsScreen.UIControl.DryErosionConfigs = dryErosionConfigs;
            SimulationConfigsScreen.UIControl.HydroErosionConfigs = hydroErosionConfigs;

            GameControl.Instance.SetBackgroundMode(true);
            SceneManager.LoadScene("SimulationConfigs", LoadSceneMode.Additive);
        }

        private void LoadTestConfigs()
        {
            SmoothTransform smooth = TerrainControl.Instance.transformSet[TransformIndex.Smooth] as SmoothTransform;
            SmoothSimConfigs smoothConfigs = smooth.Configs;

            WindDecayDigTransform windDecay = TerrainControl.Instance.transformSet[TransformIndex.WindDecayDig] as WindDecayDigTransform;
            WindDecaySimConfigs windDecayConfigs = windDecay.Configs;

            SimulationConfigsScreen.UIControl.SmoothConfigs = smoothConfigs;
            SimulationConfigsScreen.UIControl.WindDecayConfigs = windDecayConfigs;

            GameControl.Instance.SetBackgroundMode(true);
            SceneManager.LoadScene("TestConfigs", LoadSceneMode.Additive);
        }
    }
}