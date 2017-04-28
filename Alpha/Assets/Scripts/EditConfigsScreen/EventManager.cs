using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace EditConfigsScreen
{
    class EventManager : MonoBehaviour
    {
        public void OnClose()
        {
            TerrainView.GameControl.Instance.SetEditConfigs(UIControl.Instance.GetConfigs());

            TerrainView.GameControl.Instance.SetBackgroundMode(false);
            SceneManager.UnloadSceneAsync("EditConfigs");
        }
        
    }
}
