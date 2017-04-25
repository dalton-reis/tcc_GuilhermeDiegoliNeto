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
            float depth = UIControl.Instance.GetSoilDepth();

            switch (UIControl.Instance.heightMapType.value)
            {
                case 1:
                    TerrainView.GameControl.Instance.LoadSoilFromHeightMap(UIControl.Instance.heightMapInput.text, depth);
                    break;
                case 2:
                    TerrainView.GameControl.Instance.LoadRockFromHeightMap(UIControl.Instance.heightMapInput.text, depth);
                    break;
                case 3:
                    TerrainView.GameControl.Instance.LoadFromHeightMap(UIControl.Instance.heightMapInput.text, depth);
                    break;
                default:
                    return;
            }

            OnClose();
        }

        public void OnUpdateValues()
        {
            UIControl.Instance.UpdateValues();
        }
    }
}