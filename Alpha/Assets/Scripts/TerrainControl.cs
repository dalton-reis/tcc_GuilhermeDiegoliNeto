using System.Collections;
using TerrainAlgorithm;
using UnityEngine;
using UnityEngine.UI;

public class TerrainControl : MonoBehaviour
{
    public Terrain myTerrain;

    int xResolution;
    int zResolution;
    float[,] heights;

    TerrainTransform[] transformArray = null;

    // Parâmetros UI
    public Text textMass = null;

    // Parâmetros suavização
    public int smoothRange = 1;
    public float smoothFactor = 1.0f;

    // Parâmetros unilateral
    public int oneSideDigRange = 1;
    public float oneSideDigFactor = 1.0f;

    void Start()
    {
        xResolution = myTerrain.terrainData.heightmapWidth;
        zResolution = myTerrain.terrainData.heightmapHeight;
        heights = myTerrain.terrainData.GetHeights(0, 0, xResolution, zResolution);

        transformArray = new TerrainTransform[] { 
            new SmoothTransform{active = false, range = smoothRange, factor = smoothFactor}, 
            new OneSideDigTransform{active = false, range = oneSideDigRange, factor = oneSideDigFactor}, 
            new DepositTransform{active = false}
        };

    }

    public void ToggleTransform(int index)
    {
        transformArray[index].active = !transformArray[index].active;
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

    void Update()
    {
        RunAllTransforms();
    }

    void FixedUpdate()
    {
        myTerrain.ApplyDelayedHeightmapModification();
    }

    public void UpdateConfigs()
    {
        (transformArray[0] as SmoothTransform).range = smoothRange;
        (transformArray[0] as SmoothTransform).factor = smoothFactor;

        (transformArray[1] as OneSideDigTransform).range = oneSideDigRange;
        (transformArray[1] as OneSideDigTransform).factor = oneSideDigFactor;
    }

    private void RunAllTransforms()
    {
        for (int i = 0; i < transformArray.Length; i++)
        {
            if (transformArray[i].active)
                transformArray[i].ApplyTransform(ref heights);
        }

        if (textMass != null)
        {
            float sumHeights = 0.0f;
            foreach (float height in heights)
            {
                sumHeights += height;
            }
            textMass.text = sumHeights.ToString();
        }
        
        myTerrain.terrainData.SetHeightsDelayLOD(0, 0, heights);
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
