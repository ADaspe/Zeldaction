using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class FLC_VolumeChange : MonoBehaviour
{
    private Slider slider;
    public AudioMixer mixer;
    public string volumeMixerName;

    private void Start()
    {
        slider = GetComponent<Slider>();

        float mixerValue;
        mixer.GetFloat(volumeMixerName, out mixerValue);
        slider.value = mixerValue;
    }

    public void ActualizeVolume(float value)
    {
        mixer.SetFloat(volumeMixerName, value);
    }
}