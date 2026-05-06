using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;

     void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 1);
            Load();
        }
        else
        {
            
            Load();
        } 
    }
    public void ChangeVolume()
    {
        AudioListener.volume = musicSlider.value;
        Save();
    }
    private void Load()
    {
        musicSlider.value = PlayerPrefs.GetFloat("Volume");
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("Volume", musicSlider.value);
    }
}
