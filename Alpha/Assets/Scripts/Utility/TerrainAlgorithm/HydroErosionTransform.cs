using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.TerrainData;

namespace Utility.TerrainAlgorithm
{
    /// <summary>
    /// Algoritmo para simular efeitos da erosão hidráulica.
    /// Mais detalhadamente, este algoritmo simula o acúmulo de água da chuva, recolhendo e depositando material conforme a corrente.
    /// Referência: http://web.mit.edu/cesium/Public/terrain.pdf (p. 8-11)
    /// </summary>
    public class HydroErosionTransform : TerrainTransform
    {
        // Modificadores de superfície { Solo, Grama, Floresta, Concreto }
        public float[] SurfaceDrainModifiers = { 1.0f, 0.8f, 0.8f, 0.0f };
        public float[] SurfacePourModifiers = { 1.0f, 0.8f, 0.4f, 1.0f };

        public HydroErosionSimConfigs Configs { get; set; }

        private int rainCounter = 0;

        public HydroErosionTransform()
        {
            Configs = new HydroErosionSimConfigs()
            {
                Active = false,
                RainIntensity = 0.0001f,
                RainInterval = 20,
                EvaporationFactor = 0.01f,
                TerrainSolubility = 0.01f
            };
        }

        public override bool IsActive()
        {
            return Configs.Active;
        }

        public override void ApplyTransform()
        {
            // Distribuir água da chuva
            if (PourWater())
            {
                GroundToSediment();
            }

            DoWaterFlow();

            DrainWater();
        }

        private void DoWaterFlow()
        {
            int soilType = (int)SurfaceType.Soil;

            // Loop geral do mapa
            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
                {
                    // Altura do terreno + altura da água
                    float localSurfaceHeight = WaterMap[x, y];
                    float localWaterVolume = localSurfaceHeight - SoilMap[x, y];

                    if (localWaterVolume <= 0)
                        continue;

                    // Média das alturas
                    float avgSurfaceHeight = 0;
                    int countHeights = 0;
                    
                    // Soma das diferenças de altura positivas
                    float totalDifference = 0;

                    // Loop horizontal
                    VonNeumannTransform(x, y, SoilMap,
                        (ref float localHeight, ref float nearbyHeight, int nearbyX, int nearbyY) =>
                        {
                            float nearbySurfaceHeight = WaterMap[nearbyX, nearbyY];
                            float difference = localSurfaceHeight - nearbySurfaceHeight;

                            if (difference < 0) return;

                            totalDifference += difference;
                            avgSurfaceHeight += nearbySurfaceHeight;
                            countHeights++;
                        }
                    );

                    // Se não houver diferenças positivas prosseguir
                    if (totalDifference == 0) continue;

                    avgSurfaceHeight /= countHeights;

                    float deltaSurfaceHeight = localSurfaceHeight - avgSurfaceHeight;

                    float totalDeltaWater = 0;

                    // Loop horizontal
                    VonNeumannTransform(x, y, SoilMap,
                        (ref float localHeight, ref float nearbyHeight, int nearbyX, int nearbyY) =>
                        {
                            float nearbySurfaceHeight = WaterMap[nearbyX, nearbyY];

                            if (nearbySurfaceHeight >= localSurfaceHeight) return;

                            float difference = localSurfaceHeight - nearbySurfaceHeight;
                            float deltaWater = Math.Min(localWaterVolume, deltaSurfaceHeight) * (difference / totalDifference);

                            WaterMap[nearbyX, nearbyY] += deltaWater;
                            totalDeltaWater += deltaWater;

                            // Quando o nível da água passar de um certo ponto, destruir a superfície local
                            if (WaterMap[nearbyX, nearbyY] - SoilMap[nearbyX, nearbyY] > 0.25f)
                                SurfaceMap[nearbyX, nearbyY] = soilType;
                        }
                    );

                    if (totalDeltaWater > 0)
                    {
                        WaterMap[x, y] -= totalDeltaWater;
                        if (WaterMap[x, y] < SoilMap[x, y]) WaterMap[x, y] = SoilMap[x, y];

                        UpdateMeshes = true;
                        UpdateShades = true;
                    }
                }
            }
        }

        private bool PourWater()
        {
            rainCounter++;
            if (rainCounter == Configs.RainInterval && Configs.RainIntensity != 0)
            {
                rainCounter = 0;

                // Loop geral do mapa
                for (int x = 0; x < WaterMap.GetLength(0); x++)
                {
                    for (int y = 0; y < WaterMap.GetLength(1); y++)
                    {
                        WaterMap[x, y] += Configs.RainIntensity * SurfacePourModifiers[SurfaceMap[x, y]];
                    }
                }

                return true;
            }

            return false;
        }

        private void GroundToSediment()
        {
            if (Configs.TerrainSolubility == 0)
                return;

            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
                {
                    float waterVolume = WaterMap[x, y] - SoilMap[x, y];
                    if (waterVolume <= 0) continue;

                    float amountToRemove = Configs.TerrainSolubility * waterVolume;
                    amountToRemove = Math.Min(amountToRemove, SoilMap[x, y] - RockMap[x, y]);
                    SoilMap[x, y] -= amountToRemove;

                    // TODO: Talvez a água pudesse converter a camada de rocha para sedimento caso não haja solo suficiente para atingir a saturação geral
                }
            }

            UpdateMeshes = true;
            UpdateShades = true;
        }

        private void DrainWater()
        {
            if (Configs.EvaporationFactor == 0)
                return;

            float evaporationPercent = (1 - Configs.EvaporationFactor);

            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
                {
                    float waterVolume = WaterMap[x, y] - SoilMap[x, y];
                    if (waterVolume <= 0) continue;

                    float diff = waterVolume - (waterVolume * evaporationPercent);
                    diff *= SurfaceDrainModifiers[SurfaceMap[x, y]];
                    WaterMap[x, y] -= diff;
                    SoilMap[x, y] += Configs.TerrainSolubility * diff;
                    HumidityMap[x, y] += diff;
                    if (HumidityMap[x, y] > 1.0f) HumidityMap[x, y] = 1.0f;
                }
            }

            UpdateMeshes = true;
            UpdateShades = true;
        }
    }
}
