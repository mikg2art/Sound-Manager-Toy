using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSoundEvent : MonoBehaviour
{
    [SerializeField] public SFX.Environment SoundType;
    private AudioSource mySound;

    void Start()
    {
        mySound = this.gameObject.GetComponent<AudioSource>();
        if (mySound != null)
        {
            if (SoundType != SFX.Environment.None)
            {
                List<SoundLibrary.ProtoAudioClip> ranPAC = new List<SoundLibrary.ProtoAudioClip>();
                SoundLibrary.ProtoAudioClip help2 = new SoundLibrary.ProtoAudioClip();

                foreach (SoundLibrary.EnvironmentAudioClip uAudioClip in SoundManager.i.soundLibrary.EnvironmentAudioClips)
                {
                    if (uAudioClip.soundType == SoundType)
                    {
                        ranPAC.Add(uAudioClip);
                    }
                }
                if (ranPAC.Count > 0)
                {
                    if (ranPAC.Count > 1)
                    {
                        int randomNum = Random.Range(0, ranPAC.Count);
                        help2 = ranPAC[randomNum];
                    }
                    else
                    {
                        help2 = ranPAC[0];
                    }
                }

                mySound.clip = help2.audioClip;
                mySound.priority = help2.setups.priority;
                mySound.volume = (float)help2.setups.volume/100;
                mySound.pitch = help2.setups.pitch;

                SoundManager.i.AddCustomFunctionToList(mySound);
                mySound.Play();
            }
        }
    }

    public void SoundPlay()
    {
        if (mySound != null)
            mySound.Play();
    }

    public void SoundPause()
    {
        if (mySound != null)
            mySound.Pause();
    }

    public void SoundStop()
    {
        if(mySound!=null)
            mySound.Stop();
    }
}
