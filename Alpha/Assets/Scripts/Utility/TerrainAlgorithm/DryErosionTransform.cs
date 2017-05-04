using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrainView;
using Utility.TerrainData;

namespace Utility.TerrainAlgorithm
{
    /// <summary>
    /// Algoritmo para simular efeitos da erosão térmica.
    /// Mais detalhadamente, este algoritmo simula material caindo de encostas inclinadas e sendo depositado na sua base.
    /// Referência: http://web.mit.edu/cesium/Public/terrain.pdf (p. 5-6)
    /// </summary>
    public class DryErosionTransform : TerrainTransform
    {
        // Multiplicadores de inclinação { Solo, Grama, Floresta, Concreto }
        private static float[] SurfaceInclinationModifiers = { 1.0f, 1.1f, 1.2f, 2.0f };

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

        public override void ApplyTransform()
        {
            DoTransform();
        }

        public void DoTransform()
        {
            // Loop geral do mapa
            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
                {
                    // Concreto não pode sofrer erosão, mas pode ser destruído por queda de materiais.
                    if (SurfaceMap[x, y] == (int)SurfaceType.Concrete)
                        continue;

                    float localSoilMass = SoilMap[x,y] - RockMap[x,y];

                    // Obter a maior inclinação, a soma das inclinações superiores à configuração e a quantidade esperada de material movido
                    float maxInclination = 0.0f;
                    float sumInclinations = 0.0f;
                    float sumMovedMaterial = 0.0f;

                    float thresholdInclination = Configs.MaxInclination * SurfaceInclinationModifiers[SurfaceMap[x, y]];
                    // Inclinação máxima reduzida até a metade de acordo com a umidade do local
                    thresholdInclination -= (HumidityMap[x, y] * thresholdInclination) / 2;

                    VonNeumannTransform(x, y, SoilMap,
                        (ref float localHeight, ref float nearbyHeight) =>
                        {
                            float inclination = localHeight - nearbyHeight;
                            if (inclination > maxInclination)
                            {
                                maxInclination = inclination;
                            }
                            if (inclination > thresholdInclination)
                            {
                                sumInclinations += inclination;
                                sumMovedMaterial += Configs.DistributionFactor * (inclination - thresholdInclination);
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
                    float inclinationDifference = (maxInclination - thresholdInclination);

                    int soilIndex = (int)SurfaceType.Soil;

                    VonNeumannTransform(x, y, SoilMap,
                        (ref float localHeight, ref float nearbyHeight, int nearbyX, int nearbyY) =>
                        {
                            float inclination = localHeight - nearbyHeight;
                            if (inclination > thresholdInclination)
                            {
                                float movedMaterial = Configs.DistributionFactor * inclinationDifference * (inclination / sumInclinations);
                                movedMaterial *= movementLimitingFactor;
                                nearbyHeight += movedMaterial;
                                sumMovedMaterial += movedMaterial;

                                if (SurfaceMap[nearbyX, nearbyY] != soilIndex)
                                {
                                    // Locais que recebem queda de material têm sua superfície destruída
                                    SurfaceMap[nearbyX, nearbyY] = soilIndex;
                                    UpdateTextures = true;
                                }
                            }
                        }
                    );

                    if (sumMovedMaterial > 0)
                    {
                        SoilMap[x, y] -= sumMovedMaterial;
                        UpdateMeshes = true;
                        UpdateShades = true;

                        if (SurfaceMap[x, y] != soilIndex)
                        {
                            SurfaceMap[x, y] = soilIndex;
                            UpdateTextures = true;
                        }
                    }
                }
            }
        }
    }
}

