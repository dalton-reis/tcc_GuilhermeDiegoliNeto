using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.HeatAlgorithm
{
    public class HeatCalculator
    {
        public float[,] WaterMap { get; set; }
        public float[,] SoilMap { get; set; }
        public float[,] RockMap { get; set; }
        public int[,] SurfaceMap { get; set; }
        public float[,] HumidityMap { get; set; }

        public float[,] HeatMap { get; private set; }

        public HeatTypes Type { get; set; }

        public void CalculateHeat()
        {
            if (HeatMap == null || HeatMap.GetLength(0) != SoilMap.GetLength(0) || HeatMap.GetLength(1) != SoilMap.GetLength(1))
            {
                HeatMap = new float[SoilMap.GetLength(0), SoilMap.GetLength(1)];
            }

            switch (Type)
            {
                case HeatTypes.SoilDepth:
                    CalculateSoilDepth();
                    break;
                case HeatTypes.WaterDepth:
                    CalculateWaterDepth();
                    break;
                case HeatTypes.SoilHumidity:
                    CalculateSoilHumidity();
                    break;
                case HeatTypes.Inclination:
                    CalculateInclination();
                    break;
            }
        }

        private void CalculateSoilDepth()
        {
            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
                {
                    float value = (SoilMap[x, y] - RockMap[x, y]) * 10;
                    if (value < 0) value = 0;
                    else if (value > 1.0f) value = 1.0f;

                    HeatMap[x, y] = value;
                 }
            }
        }

        private void CalculateWaterDepth()
        {

        }

        private void CalculateSoilHumidity()
        {

        }

        private void CalculateInclination()
        {
            for (int x = 0; x < SoilMap.GetLength(0); x++)
            {
                for (int y = 0; y < SoilMap.GetLength(1); y++)
                {
                    float value = GetHighestIncline(x, y) * 50;
                    if (value < 0) value = 0;
                    else if (value > 1.0f) value = 1.0f;

                    HeatMap[x, y] = value;
                }
            }
        }

        private float GetHighestIncline(int x, int y)
        {
            float highest = 0;

            if (x != 0)
            {
                float incline = SoilMap[x, y] - SoilMap[x - 1, y];
                if (incline > highest) highest = incline;
            }
            if (y != 0)
            {
                float incline = SoilMap[x, y] - SoilMap[x, y - 1];
                if (incline > highest) highest = incline;
            }
            if (x != SoilMap.GetLength(0) - 1)
            {
                float incline = SoilMap[x, y] - SoilMap[x + 1, y];
                if (incline > highest) highest = incline;
            }
            if (y != SoilMap.GetLength(1) - 1)
            {
                float incline = SoilMap[x, y] - SoilMap[x, y + 1];
                if (incline > highest) highest = incline;
            }

            return highest;
        }
    }
}
