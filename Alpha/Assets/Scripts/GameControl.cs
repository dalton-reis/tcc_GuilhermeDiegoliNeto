using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class GameControl : MonoBehaviour {

    public TerrainControl terrainControl;
    public Text fileText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SaveToFile()
    {
        BinaryWriter saveFile = new BinaryWriter(File.Open(fileText.text, FileMode.Create));

        int x = terrainControl.xResolution;
        int z = terrainControl.zResolution;
        float[,] heights = terrainControl.heights;

        // Estrutura do arquivo: 2 ints para resolução x e z do mapa + sequência de floats (alturas)
        saveFile.Write(x);
        saveFile.Write(z);

        foreach (float item in heights)
        {
            saveFile.Write(item);
        }

        saveFile.Close();
    }

    public void LoadFromFile()
    {
        BinaryReader loadFile = new BinaryReader(File.Open(fileText.text, FileMode.Open));

        int x = loadFile.ReadInt32();
        int z = loadFile.ReadInt32();
        float[,] heights = new float[x,z];

        float sum = 0;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                heights[i, j] = loadFile.ReadSingle();
                sum += heights[i, j];
            }
        }

        loadFile.Close();
        terrainControl.LoadHeights(x,z,heights);
    }
}
