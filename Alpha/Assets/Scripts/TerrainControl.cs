using System.Collections;
using TerrainAlgorithm;
using UnityEngine;
using UnityEngine.UI;

public class TerrainControl : MonoBehaviour
{
    /*
     * Classe geral de controle e acesso ao terreno
     */

    public Terrain myTerrain;

    public int xResolution { get; private set; }
    public int zResolution { get; private set; }
    public float[,] heights;

    public TransformSet transformSet { get; private set; }

    // Parâmetros UI
    public Text textMass = null;

    void Start()
    {
        xResolution = myTerrain.terrainData.heightmapWidth;
        zResolution = myTerrain.terrainData.heightmapHeight;
        heights = myTerrain.terrainData.GetHeights(0, 0, xResolution, zResolution);

        transformSet = new TransformSet();
    }

    public void ToggleTransform(int index)
    {
        TerrainTransform transform = transformSet.transformSet[index];
        transform.active = !transform.active;
    }

    public void LoadHeightmap(string filename)
    {
        // http://damienclassen.blogspot.com.br/2014/02/loading-terrain-heightmap-data-via-c.html

        // Load heightmap.
        Texture2D heightmap = (Texture2D)Resources.Load("HeightMaps/" + filename);

        // Acquire an array of colour values.
        Color[] values = heightmap.GetPixels();

        // Run through array and read height values.
        int index = 0;
        for (int z = 0; z < zResolution && z < heightmap.height; z++)
        {
            for (int x = 0; x < xResolution && x < heightmap.width; x++)
            {
                heights[z, x] = values[index].r;
                index++;
            }
        }

        // Now set terrain heights.
        myTerrain.terrainData.SetHeights(0, 0, heights);
    }

    public void LoadHeights(int newX, int newZ, float [,] newHeights)
    {
        xResolution = newX;
        zResolution = newZ;
        heights = newHeights;
        myTerrain.terrainData.SetHeights(0, 0, newHeights);
    }

    void Update()
    {
        RunAllTransforms();
    }

    void FixedUpdate()
    {
        myTerrain.ApplyDelayedHeightmapModification();
    }

    private void RunAllTransforms()
    {
        foreach (TerrainTransform transform in transformSet.transformSet)
        {
            if (transform.active)
            {
                transform.ApplyTransform(ref heights);
            }
        }

        myTerrain.terrainData.SetHeightsDelayLOD(0, 0, heights);

        UpdateMass();
    }

    private void UpdateMass()
    {
        if (textMass != null)
        {
            float sumHeights = 0.0f;
            foreach (float height in heights)
            {
                sumHeights += height;
            }
            textMass.text = sumHeights.ToString();
        }
    }

    private void MouseEditTerrain()
    {
        // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                RaiseTerrain(hit.point);
            }
        }
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                LowerTerrain(hit.point);
            }
        }
    }

    private void RaiseTerrain(Vector3 point)
    {
        // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

        int terX = (int)((point.x / myTerrain.terrainData.size.x) * xResolution);
        int terZ = (int)((point.z / myTerrain.terrainData.size.z) * zResolution);
        float y = heights[terX, terZ];
        y += 0.001f;
        float[,] height = new float[1, 1];
        height[0, 0] = y;
        heights[terX, terZ] = y;
        myTerrain.terrainData.SetHeights(terX, terZ, height);
    }

    private void LowerTerrain(Vector3 point)
    {
        // Método para referência https://forum.unity3d.com/threads/edit-terrain-in-real-time.98410/

        int terX = (int)((point.x / myTerrain.terrainData.size.x) * xResolution);
        int terZ = (int)((point.z / myTerrain.terrainData.size.z) * zResolution);
        float y = heights[terX, terZ];
        y -= 0.001f;
        float[,] height = new float[1, 1];
        height[0, 0] = y;
        heights[terX, terZ] = y;
        myTerrain.terrainData.SetHeights(terX, terZ, height);
    }
}
