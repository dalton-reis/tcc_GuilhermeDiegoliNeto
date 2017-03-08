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
            smoothConfigs.Active = smooth.active;
            smoothConfigs.Range = smooth.range;
            smoothConfigs.Factor = smooth.factor;

            OneSideSimConfigs oneSideConfigs = new OneSideSimConfigs();
            OneSideDigTransform oneSide = TerrainControl.Instance.transformSet.transformSet[1] as OneSideDigTransform;
            oneSideConfigs.Active = oneSide.active;
            oneSideConfigs.Range = oneSide.range;
            oneSideConfigs.Factor = oneSide.factor;

            SimulationConfigs.UIControl.SmoothConfigs = smoothConfigs;
            SimulationConfigs.UIControl.OneSideConfigs = oneSideConfigs;

            GameControl.Instance.SetBackgroundMode(true);
            SceneManager.LoadScene("SimulationConfigs", LoadSceneMode.Additive);
        }
    }
}