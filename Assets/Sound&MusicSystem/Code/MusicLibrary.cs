using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicLibrarySO", menuName = "ScriptableObjects/MusicLibrary")]
public class MusicLibrary : ScriptableObject
{
    public class ProtoMusicClip
    {
        [HideInInspector] public int type;
        public AudioClip audioClip;
        public Setup setup;

        [System.Serializable]
        public class Setup
        {
            [Range(0, 256)] public int priority;
            [Range(0, 100)] public int volume;
            [Range(-3, 3)] public float pitch;
            public float startTime;
            public float endTime;
        }
        public void SetType(int _type) { type = _type; }
    }

    [Header("MusicSamples")]
    public MusicSampleClip[] MusicSampleClips;

    [System.Serializable]
    public class MusicSampleClip : ProtoMusicClip
    {
        public Music.MusicSamples musicSampleType;
    }

    [Header("MusicEffects")]
    public MusicEffectClip[] MusicEffectClips;

    [System.Serializable]
    public class MusicEffectClip : ProtoMusicClip
    {
        public Music.Effects musicEffectType;
    }
}
