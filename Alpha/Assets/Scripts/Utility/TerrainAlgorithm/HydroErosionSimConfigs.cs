using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    /// <summary>
    /// Configurações para simulação de erosão hidráulica.
    /// </summary>
    public class HydroErosionSimConfigs
    {
        /// <summary>
        /// Estado da simulação.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Intensidade da chuva. (acúmulo de água)
        /// </summary>
        public float RainIntensity { get; set; }

        /// <summary>
        /// Intervalos entre ocorrências de chuva. (acúmulo de água)
        /// </summary>
        public int RainInterval { get; set; }

        /// <summary>
        /// Fator de evaporação. (remoção da água)
        /// </summary>
        public float EvaporationFactor { get; set; }

        /// <summary>
        /// Fator de solubilidade do terreno. (quantia de material perdida para a água)
        /// </summary>
        public float TerrainSolubility { get; set; }
    }
}
