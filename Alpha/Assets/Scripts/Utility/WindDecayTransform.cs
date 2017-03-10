using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Utility.TerrainAlgorithm
{
    public class WindDecayDigTransform : TerrainTransform
    {
        public WindDecaySimConfigs Configs { get; set; }

        public WindDecayDigTransform()
        {
            Configs = new WindDecaySimConfigs()
            {
                Active = false,
                Range = 1,
                Factor = 1.0f,
                WindDirection = Directions.North,
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

            int startX = GetStartingX();
            int startY = GetStartingY();
            int endX = GetEndingX();
            int endY = GetEndingY();

            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    // Fazer a média da altura com base nas alturas vizinhas no hemisfério Sul
                    // Apenas considerar alterações para baixo

                    float sumHeights = 0.0f;
                    int countHeights = 0;

                    for (int relX = startX; relX <= endX; relX++)
                    {
                        int absX = x + relX;
                        if (absX < 0 || absX >= topX)
                            break;

                        for (int relY = startY; relY <= endY; relY++)
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
                        float avg = sumHeights / countHeights;
                        if (avg < heights[x, y])
                        {
                            float diff = avg - heights[x, y];
                            heights[x, y] += diff * Configs.Factor;
                        }
                    }
                }
            }
        }

        private int GetStartingY()
        {
            //     N
            // [ ][+][ ]
            //W[-][0][+]E
            // [ ][-][ ]
            //     S

            switch (Configs.WindDirection)
            {
                case Directions.North: return -Configs.Range;
                case Directions.East: return -Configs.Range;
                case Directions.South: return -Configs.Range;
                case Directions.West: return 0;
            }

            return 0;
        }

        private int GetEndingY()
        {
            switch (Configs.WindDirection)
            {
                case Directions.North: return Configs.Range;
                case Directions.East: return 0;
                case Directions.South: return Configs.Range;
                case Directions.West: return Configs.Range;
            }

            return 0;
        }

        private int GetStartingX()
        {
            switch (Configs.WindDirection)
            {
                case Directions.North: return -Configs.Range;
                case Directions.East: return -Configs.Range;
                case Directions.South: return 0;
                case Directions.West: return -Configs.Range;
            }

            return 0;
        }

        private int GetEndingX()
        {
            switch (Configs.WindDirection)
            {
                case Directions.North: return 0;
                case Directions.East: return Configs.Range;
                case Directions.South: return Configs.Range;
                case Directions.West: return Configs.Range;
            }

            return 0;
        }
    }
}
