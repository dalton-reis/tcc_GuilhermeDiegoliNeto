using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class ConfigPanel : MonoBehaviour {

    public virtual void ToggleConfigPanel(TerrainControl terrainControl)
    {
        bool enable = !gameObject.activeInHierarchy;

        if (enable)
        {
            LoadConfigs(terrainControl);
        }
        else
        {
            SaveConfigs(terrainControl);
        }

        gameObject.SetActive(enable);
    }

    public abstract void UpdateValues();

    protected abstract void LoadConfigs(TerrainControl terrainControl);

    protected abstract void SaveConfigs(TerrainControl terrainControl);
}
