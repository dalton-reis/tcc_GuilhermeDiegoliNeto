using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    /// <summary>
    /// Algoritmo para simular efeitos da erosão térmica.
    /// Mais detalhadamente, este algoritmo simula material caindo de encostas inclinadas e sendo depositado na sua base.
    /// Referência: http://web.mit.edu/cesium/Public/terrain.pdf (p. 5-6)
    /// </summary>
    public class DryErosionTransform : TerrainTransform
    {
        public DryErosionSimConfigs Configs { get; set; }

        public DryErosionTransform()
        {
            Configs = new DryErosionSimConfigs()
            {
                Active = false,
                MaxInclination = 4.0f / 256,
                DistributionFactor = 0.5f,
            };
        }

        public override bool IsActive()
        {
            return Configs.Active;
        }

        public override void ApplyTransform(float[,] rockHeights, float[,] soilHeights)
        {
            DoTransform(soilHeights, rockHeights);
        }

        public override void ApplyTransform(float[,] heights)
        {
            DoTransform(heights);
        }

        public void DoTransform(float[,] heights)
        {
            // Loop geral do mapa
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    // Obter a maior inclinação e a soma das inclinações superiores à configuração
                    float maxInclination = 0.0f;
                    float sumInclinations = 0.0f;

                    VonNeumannTransform(x, y, heights,
                        (ref float localHeight, ref float nearbyHeight) =>
                        {
                            float inclination = localHeight - nearbyHeight;
                            if (inclination > maxInclination)
                            {
                                maxInclination = inclination;
                            }
                            if (inclination > Configs.MaxInclination)
                            {
                                sumInclinations += inclination;
                            }
                        }
                    );

                    // Se não houver nada para alterar prosseguir imediatamente
                    if (sumInclinations == 0.0)
                        continue;

                    // Mover material para os vizinhos com inclinações superiores à configuração
                    float sumMovedMaterial = 0.0f;

                    // Constante para os próximos cálculos
                    float inclinationDifference = (maxInclination - Configs.MaxInclination);

                    VonNeumannTransform(x, y, heights,
                        (ref float localHeight, ref float nearbyHeight) =>
                        {
                            float inclination = localHeight - nearbyHeight;
                            if (inclination > Configs.MaxInclination)
                            {
                                float movedMaterial = (Configs.DistributionFactor * inclinationDifference * (inclination / sumInclinations));
                                nearbyHeight += movedMaterial;
                                sumMovedMaterial += movedMaterial;
                            }
                        }
                    );

                    heights[x, y] -= sumMovedMaterial;
                }
            }
        }

        public void DoTransform(float[,] soilHeights, float[,] rockHeights)
        {
            // Loop geral do mapa
            for (int x = 0; x < soilHeights.GetLength(0); x++)
            {
                for (int y = 0; y < soilHeights.GetLength(1); y++)
                {
                    float localSoilMass = soilHeights[x,y] - rockHeights[x,y];

                    // Obter a maior inclinação, a soma das inclinações superiores à configuração e a quantidade esperada de material movido
                    float maxInclination = 0.0f;
                    float sumInclinations = 0.0f;
                    float sumMovedMaterial = 0.0f;

                    VonNeumannTransform(x, y, soilHeights,
                        (ref float localHeight, ref float nearbyHeight) =>
                        {
                            float inclination = localHeight - nearbyHeight;
                            if (inclination > maxInclination)
                            {
                                maxInclination = inclination;
                            }
                            if (inclination > Configs.MaxInclination)
                            {
                                sumInclinations += inclination;
                                sumMovedMaterial += Configs.DistributionFactor * (inclination - Configs.MaxInclination);
                            }
                        }
                    );

                    // Se não houver nada para alterar prosseguir imediatamente
                    if (sumInclinations == 0.0)
                        continue;

                    // Limitar a quantidade de material movimentada se não houver solo o bastante
                    float movementLimitingFactor = 1.0f;
                    if (sumMovedMaterial > localSoilMass)
                    {
                        movementLimitingFactor = localSoilMass / sumMovedMaterial;
                    }

                    // Mover material para os vizinhos com inclinações superiores à configuração
                    sumMovedMaterial = 0.0f;

                    // Constante para os próximos cálculos
                    float inclinationDifference = (maxInclination - Configs.MaxInclination);

                    VonNeumannTransform(x, y, soilHeights,
                        (ref float localHeight, ref float nearbyHeight) =>
                        {
                            float inclination = localHeight - nearbyHeight;
                            if (inclination > Configs.MaxInclination)
                            {
                                float movedMaterial = Configs.DistributionFactor * inclinationDifference * (inclination / sumInclinations);
                                movedMaterial *= movementLimitingFactor;
                                nearbyHeight += movedMaterial;
                                sumMovedMaterial += movedMaterial;
                            }
                        }
                    );

                    soilHeights[x, y] -= sumMovedMaterial;
                }
            }
        }
    }
}

