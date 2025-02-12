using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrarySO", menuName = "ScriptableObjects/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public int test;
    public class ProtoAudioClip
    {
        [HideInInspector] public int type;
        public AudioClip audioClip;
        public Setup setups;

        [System.Serializable]
        public class Setup
        {
            [Range(0, 256)] public int priority;
            [Range(0, 100)] public int volume;
            [Range(-3, 3)] public float pitch;
            public float startTime;
            public float endTime;

            public S3DSettings s3DSettings;
            [System.Serializable]
            public class S3DSettings
            {
                public float minDistance;
                public float maxDistance;
            }
        }
        public void SetType(int _type) { type = _type; }
    }

    [Header("Background SFX")]
    public BackgroundAudioClip[] BackgroundAudioClips;

    [System.Serializable]
    public class BackgroundAudioClip : ProtoAudioClip
    {
        public SFX.Background soundType;
    }

    [Header("Environment SFX")]
    public EnvironmentAudioClip[] EnvironmentAudioClips;

    [System.Serializable]
    public class EnvironmentAudioClip : ProtoAudioClip
    {
        public SFX.Environment soundType;
    }


    [Header("\nPlayer SFX: ")]
    [Header("Universal")]
    public UniversalAudioClip[] UniversalAudioClips;

    [System.Serializable]
    public class UniversalAudioClip : ProtoAudioClip
    {
        public SFX.Universal soundType;
    }

    [Header("NoRage")]
    public NoRageAudioClip[] NoRageAudioClips;

    [System.Serializable]
    public class NoRageAudioClip : ProtoAudioClip
    {
        //public NoRageAudioClip() { type = (int)soundType; }
        public SFX.NoRage soundType;
    }

    [Header("PassiveRage")]
    public PassiveRageAudioClip[] PassiveRageAudioClips;

    [System.Serializable]
    public class PassiveRageAudioClip : ProtoAudioClip
    {
        public SFX.PassiveRage soundType;
    }

    [Header("ActiveRage")]
    public ActiveRageAudioClip[] ActiveRageAudioClips;

    [System.Serializable]
    public class ActiveRageAudioClip : ProtoAudioClip
    {
        public SFX.ActiveRage soundType;
    }

    [Header("\nEnemies SFX: ")]
    [Header("Oni")]
    public OniAudioClip[] OniAudioClips;

    [System.Serializable]
    public class OniAudioClip : ProtoAudioClip
    {
        public SFX.Oni soundType;
    }

    [Header("BigOni")]
    public BigOniAudioClip[] BigOniAudioClips;

    [System.Serializable]
    public class BigOniAudioClip : ProtoAudioClip
    {
        public SFX.BigOni soundType;
    }

    [Header("Yurei")]
    public YureiAudioClip[] YureiAudioClips;

    [System.Serializable]
    public class YureiAudioClip : ProtoAudioClip
    {
        public SFX.Yurei soundType;
    }

    [Header("Orb")]
    public OrbAudioClip[] OrbAudioClips;

    [System.Serializable]
    public class OrbAudioClip : ProtoAudioClip
    {
        public SFX.Orb soundType;
    }

    [Header("FinalBoss")]
    public FinalBossAudioClip[] FinalBossAudioClips;

    [System.Serializable]
    public class FinalBossAudioClip : ProtoAudioClip
    {
        public SFX.FinalBoss soundType;
    }
}
