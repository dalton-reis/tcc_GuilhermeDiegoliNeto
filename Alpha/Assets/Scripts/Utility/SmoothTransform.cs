using System;
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
            };
        }

        public override bool IsActive()
        {
            return Configs.Active;
        }

        public override void ApplyTransform(ref float[,] heights)
        {
            int topX = heights.GetLength(0);
            int topY = heights.GetLength(1);

            float[,] baseHeights = heights.Clone() as float[,];

            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    // Fazer a média da altura com base nas alturas vizinhas
                    float sumHeights = 0.0f;
                    int countHeights = 0;

                    for (int relX = -Configs.Range; relX <= Configs.Range; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            break;

                        for (int relY = -Configs.Range; relY <= Configs.Range; relY++)
                        {
                            int absY = y + relY;
                            if (absY < 0 || absY >= topY)
                                break;

                            sumHeights += baseHeights[absX, absY];
                            countHeights++;
                        }
                    }

                    if (countHeights > 0)
                    {
                        float diff = (sumHeights / countHeights) - heights[x, y];
                        heights[x, y] += diff * Configs.Factor;
                    }
                }
            }
        }
    }
}
