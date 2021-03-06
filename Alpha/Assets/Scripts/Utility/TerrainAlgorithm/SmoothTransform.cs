﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Utility.TerrainAlgorithm
{
    public class SmoothTransform : TerrainTransform
    {
        public SmoothSimConfigs Configs { get; set; }

        public SmoothTransform()
        {
            Configs = new SmoothSimConfigs()
            {
                Active = false,
                Range = 1,
                Factor = 1.0f,
                UseMoore = false,
            };
        }

        public override bool IsActive()
        {
            return Configs.Active;
        }

        public override void ApplyTransform()
        {
            if (Configs.UseMoore)
            {
                TransformMoore();
            }
            else
            {
                TransformVonNeumann();
            }
        }

        private void TransformMoore()
        {
            // Transformação usando vizinhança Moore

            int topX = SoilMap.GetLength(0);
            int topY = SoilMap.GetLength(1);

            float[,] baseHeights = SoilMap.Clone() as float[,];

            // Loop geral do mapa
            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
                {
                    // Fazer a média da altura com base nos vizinhos
                    float sumHeights = 0.0f;
                    int countHeights = 0;

                    // Loop interno dos vizinhos
                    for (int relX = -Configs.Range; relX <= Configs.Range; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            continue;

                        for (int relY = -Configs.Range; relY <= Configs.Range; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                continue;

                            sumHeights += baseHeights[absX, absY];
                            countHeights++;
                        }
                    }

                    // Aplicar a média dos valores
                    if (countHeights > 0)
                    {
                        float diff = (sumHeights / countHeights) - SoilMap[x, y];
                        SoilMap[x, y] += diff * Configs.Factor;
                    }
                }
            }
        }

        private void TransformVonNeumann()
        {
            // Transformação usando vizinhança Von Neumann

            int topX = SoilMap.GetLength(0);
            int topY = SoilMap.GetLength(1);

            float[,] baseHeights = SoilMap.Clone() as float[,];

            // Loop geral do mapa
            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
                {
                    // Fazer a média da altura com base nos vizinhos

                    float sumHeights = 0.0f;
                    int countHeights = 0;

                    // Primeiro somar os vizinhos na horizontal
                    for (int relX = -Configs.Range; relX <= Configs.Range; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            continue;

                        sumHeights += baseHeights[absX, y];
                        countHeights++;
                    }

                    // Depois na vertical
                    for (int relY = -Configs.Range; relY <= Configs.Range; relY++)
                    {
                        int absY = y + relY;
                        if (absY < 0 || absY >= topY)
                            continue;

                        sumHeights += baseHeights[x, absY];
                        countHeights++;
                    }

                    // Subtrair o valor da célula central que foi somado duas vezes
                    sumHeights -= baseHeights[x, y];
                    countHeights--;

                    // Aplicar a média dos valores
                    if (countHeights > 0)
                    {
                        float diff = (sumHeights / countHeights) - SoilMap[x, y];
                        SoilMap[x, y] += diff * Configs.Factor;
                    }
                }
            }
        }
    }
}
