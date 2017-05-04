using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using SimulationConfigsScreen;
using Utility.TerrainAlgorithm;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Utility;

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

            formatter.Serialize(saveStream, TerrainControl.Instance.RockMap);
            formatter.Serialize(saveStream, TerrainControl.Instance.SoilMap);

            saveStream.Close();
        }

        public void LoadFromFile(string filename)
        {
            Stream loadStream = new FileStream(filename, FileMode.Open);
            IFormatter formatter = new BinaryFormatter();

            float[,] rockHeights = formatter.Deserialize(loadStream) as float[,];
            float[,] soilHeights = formatter.Deserialize(loadStream) as float[,];

            TerrainControl.Instance.LoadHeights(soilHeights, rockHeights);
        }

        public void LoadSoilFromHeightMap(string filename, float minDepth)
        {
            TerrainControl.Instance.LoadSoilHeightmap(filename, minDepth);
        }

        public void LoadRockFromHeightMap(string filename, float minDepth)
        {
            TerrainControl.Instance.LoadRockHeightmap(filename, minDepth);
        }

        public void LoadFromHeightMap(string filename, float minDepth)
        {
            TerrainControl.Instance.LoadHeightmap(filename, minDepth);
        }

        public void LoadSmoothConfigs(SmoothSimConfigs configs)
        {
            (TerrainControl.Instance.transformSet[TransformIndex.Smooth] as SmoothTransform).Configs = configs;
        }

        public void LoadWindDecayConfigs(WindDecaySimConfigs configs)
        {
            (TerrainControl.Instance.transformSet[TransformIndex.WindDecayDig] as WindDecayDigTransform).Configs = configs;
        }

        public void LoadDryErosionConfigs(DryErosionSimConfigs configs)
        {
            (TerrainControl.Instance.transformSet[TransformIndex.DryErosion] as DryErosionTransform).Configs = configs;
        }

        public void LoadHydroErosionConfigs(HydroErosionSimConfigs configs)
        {
            (TerrainControl.Instance.transformSet[TransformIndex.HydroErosion] as HydroErosionTransform).Configs = configs;
        }

        public void SetEditConfigs(EditConfigs configs)
        {
            TerrainControl.Instance.EditConfigs = configs;
        }

        public void SetBackgroundMode(bool mode)
        {
            // True para esconder a UI e exibir apenas o terreno (para background ao navegar em telas)

            BackgroundMode = mode;

            UIControl.gameObject.SetActive(!mode);
        }
    }
}