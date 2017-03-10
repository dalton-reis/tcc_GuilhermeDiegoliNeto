using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using SimulationConfigs;
using Utility.TerrainAlgorithm;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TerrainView
{
    public class GameControl : MonoBehaviour
    {
        // Singleton
        public static GameControl Instance { get; private set; }

        public Canvas UIControl;

        public bool BackgroundMode { get; private set; }

        // Use this for initialization
        void Start()
        {
            BackgroundMode = false;
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SaveToFile(string filename)
        {
            Stream saveStream = new FileStream(filename, FileMode.Create);
            IFormatter formatter = new BinaryFormatter();

            formatter.Serialize(saveStream, TerrainControl.Instance.heights);
            saveStream.Close();
        }

        public void LoadFromFile(string filename)
        {
            Stream loadStream = new FileStream(filename, FileMode.Open);
            IFormatter formatter = new BinaryFormatter();

            float[,] heights = formatter.Deserialize(loadStream) as float[,];

            TerrainControl.Instance.LoadHeights(heights);
        }

        public void LoadFromHeightMap(string filename)
        {
            TerrainControl.Instance.LoadHeightmap(filename);
        }

        public void LoadSmoothConfigs(SmoothSimConfigs configs)
        {
            (TerrainControl.Instance.transformSet.transformSet[0] as SmoothTransform).Configs = configs;
        }

        public void LoadWindDecayConfigs(WindDecaySimConfigs configs)
        {
            (TerrainControl.Instance.transformSet.transformSet[1] as WindDecayDigTransform).Configs = configs;
        }

        public void SetBackgroundMode(bool mode)
        {
            // True para esconder a UI e exibir apenas o terreno (para background ao navegar em telas)

            BackgroundMode = mode;

            UIControl.gameObject.SetActive(!mode);
        }
    }
}