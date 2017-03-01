using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainView;
using UnityEngine.SceneManagement;

namespace SimulationConfigs
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

            TerrainView.GameControl.Instance.LoadSmoothConfigs(UIControl.SmoothConfigs);
            TerrainView.GameControl.Instance.LoadOneSideConfigs(UIControl.OneSideConfigs);

            TerrainView.GameControl.Instance.SetBackgroundMode(false);
            SceneManager.UnloadSceneAsync("SimulationConfigs");
        }
    }
}
