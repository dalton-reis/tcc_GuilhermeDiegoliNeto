using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    /// <summary>
    /// Algoritmo para simular efeitos da erosão hidráulica.
    /// Mais detalhadamente, este algoritmo simula o acúmulo de água da chuva, recolhendo e depositando material conforme a corrente.
    /// Referência: http://web.mit.edu/cesium/Public/terrain.pdf (p. 8-11)
    /// </summary>
    public class HydroErosionTransform : TerrainTransform
    {
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

        public override void ApplyTransform(float[,] rockHeights, float[,] soilHeights)
        {
            // Criar matriz de água se não existir
            if (waterHeights == null)
            {
                waterHeights = new float[soilHeights.GetLength(0), soilHeights.GetLength(1)];
            }

            // Distribuir água da chuva
            if (DistributeWater())
            {
                GroundToSediment(soilHeights, rockHeights);
            }

            DoTransform(soilHeights);

            EvaporateWater(soilHeights);
        }

        public override void ApplyTransform(float[,] heights)
        {
            // Criar matriz de água se não existir
            if (waterHeights == null)
            {
                waterHeights = new float[heights.GetLength(0), heights.GetLength(1)];
            }

            // Distribuir água da chuva
            if (DistributeWater())
            {
                GroundToSediment(heights);
            }

            DoTransform(heights);

            EvaporateWater(heights);
        }

        public float[,] GetWaterMatrix(float[,] heights)
        {
            float[,] matrix = heights.Clone() as float[,];

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

        private void DoTransform(float[,] heights)
        {
            // Loop geral do mapa
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    // Altura do terreno + altura da água
                    float localSurfaceHeight = heights[x, y] + waterHeights[x, y];

                    // Média das alturas
                    float avgSurfaceHeight = 0;
                    int countHeights = 0;
                    
                    // Soma das diferenças de altura positivas
                    float totalDifference = 0;

                    // Loop horizontal
                    VonNeumannTransform(x, y, heights,
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
                    VonNeumannTransform(x, y, heights,
                        (ref float localHeight, ref float nearbyHeight, int nearbyX, int nearbyY) =>
                        {
                            float nearbySurfaceHeight = (nearbyHeight + waterHeights[nearbyX, nearbyY]);

                            if (nearbySurfaceHeight >= localSurfaceHeight) return;

                            float difference = localSurfaceHeight - nearbySurfaceHeight;
                            float deltaWater = Math.Min(waterHeights[x, y], deltaSurfaceHeight) * (difference / totalDifference);

                            waterHeights[nearbyX, nearbyY] += deltaWater;
                            totalDeltaWater += deltaWater;
                        }
                    );

                    waterHeights[x, y] -= totalDeltaWater;
                }
            }
        }

        private bool DistributeWater()
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
                        waterHeights[x, y] += Configs.RainIntensity;
                    }
                }

                return true;
            }

            return false;
        }

        private void GroundToSediment(float[,] heights)
        {
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    heights[x, y] -= Configs.TerrainSolubility * waterHeights[x, y];
                }
            }
        }

        private void GroundToSediment(float[,] soilHeights, float[,] rockHeights)
        {
            if (Configs.TerrainSolubility == 0)
                return;

            for (int x = 0; x < soilHeights.GetLength(0); x++)
            {
                for (int y = 0; y < soilHeights.GetLength(1); y++)
                {
                    float amountToRemove = Configs.TerrainSolubility * waterHeights[x, y];
                    amountToRemove = Math.Min(amountToRemove, soilHeights[x,y] - rockHeights[x,y]);
                    soilHeights[x, y] -= amountToRemove;

                    // TODO: Talvez a água pudesse converter a camada de rocha para sedimento caso não haja solo suficiente para atingir a saturação geral
                }
            }
        }

        private void EvaporateWater(float[,] heights)
        {
            if (Configs.EvaporationFactor == 0)
                return;

            float evaporationPercent = (1 - Configs.EvaporationFactor);

            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    float diff = waterHeights[x, y] - (waterHeights[x, y] * evaporationPercent);
                    waterHeights[x, y] -= diff;
                    heights[x, y] += Configs.TerrainSolubility * diff;
                }
            }
        }
    }
}
