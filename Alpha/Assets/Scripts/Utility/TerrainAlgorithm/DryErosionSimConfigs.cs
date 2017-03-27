using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    /// <summary>
    /// Configurações para simulação de erosão térmica.
    /// </summary>
    public class DryErosionSimConfigs
    {
        /// <summary>
        /// Estado da simulação.
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Inclinação máximo de estabilidade. A erosão acontecerá em inclinações superiores a este valor.
        /// </summary>
        public double MaxInclination { get; set; }
        /// <summary>
        /// Fator de ajuste de distribuição do material movido durante a erosão.
        /// </summary>
        public double DistributionFactor { get; set; }
    }
}
