using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.TerrainAlgorithm
{
    public enum TransformIndex
    {
        Smooth = 0,
        WindDecayDig,
        Deposit,
        DryErosion,
        HydroErosion,

        IndexCount,
    }

    public class TransformSet
    {
        /*
         * Contém todas as transformações padrão do programa
         * Novo algoritmos deverão ser adicionados à esta coleção para funcionarem
         */

        public TerrainTransform[] transformSet { get; private set; }

        public TransformSet()
        {
            transformSet = new TerrainTransform[(int)TransformIndex.IndexCount];

            transformSet[(int)TransformIndex.Smooth]       = new SmoothTransform();
            transformSet[(int)TransformIndex.WindDecayDig] = new WindDecayDigTransform();
            transformSet[(int)TransformIndex.Deposit]      = new DepositTransform();
            transformSet[(int)TransformIndex.DryErosion]   = new DryErosionTransform();
            transformSet[(int)TransformIndex.HydroErosion] = new HydroErosionTransform();
        }

        public TerrainTransform this[TransformIndex index]
        {
            get
            {
                return transformSet[(int)index];
            }
            private set
            {
                transformSet[(int)index] = value;
            }
        }
    }
}
