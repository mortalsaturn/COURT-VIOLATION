using UnityEngine;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ChangeGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }
}
