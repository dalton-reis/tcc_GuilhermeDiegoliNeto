using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoadSaveScreen
{
    public class EventManager : MonoBehaviour
    {
        public void OnClose()
        {
            TerrainView.GameControl.Instance.SetBackgroundMode(false);
            SceneManager.UnloadSceneAsync("LoadSaveScreen");
        }

        public void OnSaveFile()
        {
            TerrainView.GameControl.Instance.SaveToFile(UIControl.Instance.saveFileInput.text);
            OnClose();
        }

        public void OnLoadFile()
        {
            TerrainView.GameControl.Instance.LoadFromFile(UIControl.Instance.loadFileInput.text);
            OnClose();
        }

        public void OnImportHeightMap()
        {
            TerrainView.GameControl.Instance.LoadFromHeightMap(UIControl.Instance.heightMapInput.text);
            OnClose();
        }
    }
}