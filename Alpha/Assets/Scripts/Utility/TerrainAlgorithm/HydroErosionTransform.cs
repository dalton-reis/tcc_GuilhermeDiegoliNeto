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

        private float[,] waterHeights = null;
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

        public override void Reset()
        {
            waterHeights = null;
        }

        public override void ApplyTransform()
        {
            // Criar matriz de água se não existir
            if (waterHeights == null)
            {
                waterHeights = new float[SoilMap.GetLength(0), SoilMap.GetLength(1)];
            }

            // Distribuir água da chuva
            if (PourWater())
            {
                GroundToSediment();
            }

            DoWaterFlow();

            DrainWater();
        }

        public float[,] GetWaterMatrix()
        {
            float[,] matrix = SoilMap.Clone() as float[,];

            if (waterHeights != null)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    for (int y = 0; y < matrix.GetLength(1); y++)
                    {
                        matrix[x, y] += waterHeights[x, y];
                    }
                }
            }

            return matrix;
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
                    float localSurfaceHeight = SoilMap[x, y] + waterHeights[x, y];

                    // Média das alturas
                    float avgSurfaceHeight = 0;
                    int countHeights = 0;
                    
                    // Soma das diferenças de altura positivas
                    float totalDifference = 0;

                    // Loop horizontal
                    VonNeumannTransform(x, y, SoilMap,
                        (ref float localHeight, ref float nearbyHeight, int nearbyX, int nearbyY) =>
                        {
                            float nearbySurfaceHeight = (nearbyHeight + waterHeights[nearbyX, nearbyY]);
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
                            float nearbySurfaceHeight = (nearbyHeight + waterHeights[nearbyX, nearbyY]);

                            if (nearbySurfaceHeight >= localSurfaceHeight) return;

                            float difference = localSurfaceHeight - nearbySurfaceHeight;
                            float deltaWater = Math.Min(waterHeights[x, y], deltaSurfaceHeight) * (difference / totalDifference);

                            waterHeights[nearbyX, nearbyY] += deltaWater;
                            totalDeltaWater += deltaWater;

                            // Quando o nível da água passar de um certo ponto, destruir a superfície local
                            if (waterHeights[nearbyX, nearbyY] > 0.25f)
                                SurfaceMap[nearbyX, nearbyY] = soilType;
                        }
                    );

                    if (totalDeltaWater > 0)
                    {
                        waterHeights[x, y] -= totalDeltaWater;
                        if (waterHeights[x, y] < 0) waterHeights[x, y] = 0;

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
                for (int x = 0; x < waterHeights.GetLength(0); x++)
                {
                    for (int y = 0; y < waterHeights.GetLength(1); y++)
                    {
                        waterHeights[x, y] += Configs.RainIntensity * SurfacePourModifiers[SurfaceMap[x, y]];
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
                    float amountToRemove = Configs.TerrainSolubility * waterHeights[x, y];
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
                    float diff = waterHeights[x, y] - (waterHeights[x, y] * evaporationPercent);
                    diff *= SurfaceDrainModifiers[SurfaceMap[x, y]];
                    waterHeights[x, y] -= diff;
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
