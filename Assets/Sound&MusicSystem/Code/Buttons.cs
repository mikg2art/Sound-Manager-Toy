using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Buttons : MonoBehaviour
{
    #region Variables

    [Header("Play-Pause Buttons Highlight")]
    public Transform playButton;
    public Transform pauseButton;
    public Transform highlightPPB;

    [Header("Slider Value Texts")]
    public TextMeshProUGUI masterVolumeTMP;
    public TextMeshProUGUI sfxVolumeTMP;
    public TextMeshProUGUI musicVolumeTMP;

    public TextMeshProUGUI sfxPitchTMP;
    public TextMeshProUGUI musicPitchTMP;

    [Header("Music Buttons Highlight")]
    public List<Transform> musicButtons = new List<Transform>();
    public Transform highlightMB;
    public TextMeshProUGUI musicTitleText;
    private bool deathInTheEnd;
    private bool pauseButtonPressed;

    #endregion

    private void Start() => pauseButtonPressed = false;

    //Sound Effects
    public void PlaySoundEffect(int num)
    {
        if (!pauseButtonPressed)
        {
            switch (num)
            {
                case 1:
                    SoundManager.i.CreateSoundEvent(SFX.BigOni.TalkTwo);
                    break;
                case 2:
                    SoundManager.i.CreateSoundEvent(SFX.ActiveRage.FirstAttack);
                    break;
                case 3:
                    SoundManager.i.CreateSoundEvent(SFX.Universal.ReceivingDamage);
                    break;
                case 4:
                    SoundManager.i.CreateSoundEvent(SFX.ActiveRage.RageActivate);
                    break;
                case 5:
                    SoundManager.i.CreateSoundEvent(SFX.Orb.ReceivingDamage);
                    break;

                default:
                    break;
            }
        }
    }

    #region Control Buttons
    public void Play()  
    {
        pauseButtonPressed = false;
        SoundManager.i.PlayAllSFX(); 
        SoundManager.i.MusicPlay(); 
        highlightPPB.position = playButton.position; 
        Time.timeScale = 1; 
    }
    public void Pause() 
    {
        pauseButtonPressed = true;
        SoundManager.i.PauseAllSFX(); 
        SoundManager.i.MusicPause(); 
        highlightPPB.position = pauseButton.position; 
        Time.timeScale = 0; 
    }
    public void Stop() 
    { 
        SoundManager.i.StopAllSFX(); 
        SoundManager.i.MusicStop(); 
    }
    private void Update()
    {
        if (SoundManager.i.musicState == SoundManager.musicStates.level5)
        {
            musicTitleText.text = "Game Over - Level 5";
            highlightMB.position = musicButtons[8].position;
        }
    }
    #endregion

    #region Volume & Pitch Change
    public void masterVolumeChange(float value)
    {
        SoundManager.i.SetMasterVolume(value);
        masterVolumeTMP.text = ((int)(value * 100f)).ToString();
    }
    public void sfxVolumeChange(float value)
    {
        SoundManager.i.SetSFXVolume(value);
        sfxVolumeTMP.text = ((int)(value * 100f)).ToString();
    }
    public void musicVolumeChange(float value)
    {
        SoundManager.i.SetMusicVolume(value);
        musicVolumeTMP.text = ((int)(value * 100f)).ToString();
    }

    //PitchChange
    public void sfxPitchChange(float value)
    {
        SoundManager.i.SetSFXPitch(value/10);
        sfxPitchTMP.text = ((int)(value)).ToString();
    }
    public void musicPitchChange(float value)
    {
        SoundManager.i.SetMusicPitch(value/10);
        musicPitchTMP.text = ((int)(value)).ToString();
    }
    #endregion

    public void PlayMusic(int num)
    {
        if (!pauseButtonPressed)
        {
            if (musicButtons.Count > 0 && num < 10 && num > 0)
            {
                highlightMB.position = musicButtons[num - 1].position;
                deathInTheEnd = num < 6 ? false : true;
            }

            switch (num)
            {
                case 1:
                    SoundManager.i.MusicStart();
                    musicTitleText.text = "Music Start - Level 0";
                    break;
                case 2:
                    SoundManager.i.MusicChangeLevel(SoundManager.musicStates.level1);
                    musicTitleText.text = "Exploring - Level 1";
                    break;
                case 3:
                    SoundManager.i.MusicChangeLevel(SoundManager.musicStates.level2);
                    musicTitleText.text = "Battle - Level 2";
                    break;
                case 4:
                    SoundManager.i.MusicChangeLevel(SoundManager.musicStates.level2End);
                    musicTitleText.text = "First Boss Defeated - Level 2";
                    break;
                case 5:
                    SoundManager.i.MusicChangeLevel(SoundManager.musicStates.level3);
                    musicTitleText.text = "Going To Fin. Boss - Level 3";
                    break;
                case 6:
                    SoundManager.i.MusicChangeLevel(SoundManager.musicStates.level4s1);
                    musicTitleText.text = "F. Boss First Stage - Level 4 part 1";
                    break;
                case 7:
                    SoundManager.i.MusicChangeLevel(SoundManager.musicStates.level4s2);
                    musicTitleText.text = "F. Boss Second Stage - Level 4 part 2";
                    break;
                case 8:
                    SoundManager.i.MusicChangeLevel(SoundManager.musicStates.level4End);
                    musicTitleText.text = "F. Boss Defeated - Level 4 part 3";
                    break;
                case 9:
                    SoundManager.i.MusicChangeLevel(SoundManager.musicStates.level5);
                    musicTitleText.text = "Game Over - Level 5";
                    break;

                default:
                    break;
            }
        }
    }

    public void MusicPlayerIsDead()
    {
        if (!pauseButtonPressed)
        {
            if (musicButtons.Count > 0)
            {
                if (deathInTheEnd)
                {
                    highlightMB.position = musicButtons[4].position;
                    musicTitleText.text = "Player Dead - Level 5";
                }
                else
                {
                    highlightMB.position = musicButtons[1].position;
                    musicTitleText.text = "Player Dead - Level 2";
                }
            }
            SoundManager.i.CreateSoundEvent(SFX.Universal.Death);
            SoundManager.i.MusicPlayerIsDead();
        }
    }
}
