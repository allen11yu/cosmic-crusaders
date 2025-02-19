using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer myMixer; 
    public Slider musicSlider; 

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("master",Mathf.Log10(volume)*20);
    }

}
