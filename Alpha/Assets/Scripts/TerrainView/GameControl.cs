using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using SimulationConfigs;
using Utility.TerrainAlgorithm;

namespace TerrainView
{
    public class GameControl : MonoBehaviour
    {
        // Singleton
        public static GameControl Instance { get; private set; }

        public Canvas UIControl;
        public Text fileText;

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

        public void SaveToFile()
        {
            BinaryWriter saveFile = new BinaryWriter(File.Open(fileText.text, FileMode.Create));

            int x = TerrainControl.Instance.xResolution;
            int z = TerrainControl.Instance.zResolution;
            float[,] heights = TerrainControl.Instance.heights;

            // Estrutura do arquivo: 2 ints para resolução x e z do mapa + sequência de floats (alturas)
            saveFile.Write(x);
            saveFile.Write(z);

            foreach (float item in heights)
            {
                saveFile.Write(item);
            }

            saveFile.Close();
        }

        public void LoadFromFile()
        {
            BinaryReader loadFile = new BinaryReader(File.Open(fileText.text, FileMode.Open));

            int x = loadFile.ReadInt32();
            int z = loadFile.ReadInt32();
            float[,] heights = new float[x, z];

            float sum = 0;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    heights[i, j] = loadFile.ReadSingle();
                    sum += heights[i, j];
                }
            }

            loadFile.Close();
            TerrainControl.Instance.LoadHeights(x, z, heights);
        }

        public void LoadSmoothConfigs(SmoothSimConfigs configs)
        {
            (TerrainControl.Instance.transformSet.transformSet[0] as SmoothTransform).LoadConfigs(configs);
        }

        public void LoadOneSideConfigs(OneSideSimConfigs configs)
        {
            (TerrainControl.Instance.transformSet.transformSet[1] as OneSideDigTransform).LoadConfigs(configs);
        }

        public void SetBackgroundMode(bool mode)
        {
            // True para esconder a UI e exibir apenas o terreno (para background ao navegar em telas)

            BackgroundMode = mode;

            UIControl.gameObject.SetActive(!mode);
        }
    }
}