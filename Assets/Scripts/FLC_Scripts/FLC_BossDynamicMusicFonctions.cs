using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FLC_BossDynamicMusicFonctions : MonoBehaviour
{
    public AudioSource[] audioPlayer;
    [SerializeField] float fadeOutFactor;
    [SerializeField] float fadeOutSpeed;
    [SerializeField] float transitionFadeFactor;
    [SerializeField] float transitionFadeSpeed;

    int e = 1;
    int lastPartPlayed = 0;
    [SerializeField] int partPlayed = 0;

    public void MusicsStart()
    {
        for(int i =0; i < audioPlayer.Length; i++)
        {
            audioPlayer[i].Play();
            audioPlayer[i].volume = 0;
        }

        audioPlayer[1].volume = 1;
        partPlayed = 1;
    }

    public void SwitchMusicPart(int musicPartNbr)
    {     
        if(partPlayed < audioPlayer.Length)
        {
            lastPartPlayed = partPlayed;
            partPlayed = musicPartNbr;
            MusicFadeIn();
            MusicFadeOut();
        }
        else
        {            
            MusicLastFadeOut();
        }
    }

    public void StopBossMusic()
    {
        Invoke("MusicLastFadeOut",0);
    }

    public void MusicFadeIn()
    {
        if (audioPlayer[partPlayed].volume <= 1)
        {
            audioPlayer[partPlayed].volume += transitionFadeFactor;
            Invoke("MusicFadeIn", transitionFadeSpeed);
        }
    }

    public void MusicFadeOut()
    {
        if(audioPlayer[lastPartPlayed].volume >= 0)
        {
            audioPlayer[lastPartPlayed].volume -= transitionFadeFactor;
            Invoke("MusicFadeOut", transitionFadeSpeed);
        }
    }

    public void MusicLastFadeOut()
    {
        if (audioPlayer[partPlayed].volume >= 0)
        {
            audioPlayer[partPlayed].volume -= fadeOutFactor;
            Invoke("MusicLastFadeOut", fadeOutSpeed);
        }
    }
}