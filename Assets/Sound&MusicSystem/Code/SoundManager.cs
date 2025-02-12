using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager i;
    public class ControlableSoundEvent
    {
        public SFX.Background type;
        public AudioSource controlableEvent;
        public bool isPaused;
        public float pausedTime;
        public float volume;
    }

    public class CMusicEvent
    {
        public int type;
        public AudioSource mEvent;
    }

    [Header("Referenses")]
    [SerializeField] public SoundLibrary soundLibrary;
    [SerializeField] public MusicLibrary musicLibrary;
    [SerializeField] public AudioSource soundEvent;
    [SerializeField] public AudioSource musicEvent;
    [SerializeField] public AudioMixer audioMixer;

    #region Variables
    //SoundSystem
    private List<AudioSource> soundEvents;
    private List<ControlableSoundEvent> controlableSoundEvents;

    private SoundLibrary.ProtoAudioClip currentPAC; //PAC - Proto Audio Clip

    private SoundLibrary.ProtoAudioClip[] CurrentAudioClips;

    private bool pauseAll;


    //MusicSystem
    private List<CMusicEvent> musicEvents;

    private MusicLibrary.ProtoMusicClip currentPMC; //PMC - Proto Music Clip
    //private MusicLibrary.ProtoMusicClip prevPMC;
    [HideInInspector] public AudioSource currentAS; //AS - Audio Source
    [HideInInspector] public AudioSource prevAS;

    [HideInInspector] public SimpleTimer startTransitionTimer;
    //[HideInInspector] public SimpleTimer endTransitionTimer;

    [HideInInspector] public float endTransiTime;
    [HideInInspector] public bool l1endCase;
    [HideInInspector] public bool volumeChange;
    [HideInInspector] public bool musicIsPlaying;
    [HideInInspector] public bool playerIsDead;
    [HideInInspector] public bool playTransitionEffect;
    [HideInInspector] public bool zeroTransition;
    [HideInInspector] public bool theEnd;
    //[HideInInspector] public bool musicReset;

    public bool oldLibrarySolution;

    public enum battleLevels { L1, L2, L3 }
    [HideInInspector] public battleLevels battleLevel;

    public enum battleStates { playerRevived, battleOver, battleStarted }
    [HideInInspector] public battleStates battleState;

    [Header("Time to music change after battle ends:")]
    [SerializeField] public float battleOverTime = 10;

    [HideInInspector] public SimpleTimer deathTimer;
    [HideInInspector] public SimpleTimer battleTimer;
    [HideInInspector] public SimpleTimer effectTimer;
    [HideInInspector] public float percentage1;
    [HideInInspector] public float percentage2;
    [HideInInspector] public float currentVolume;
    [HideInInspector] public float prevVolume;
    public enum musicStates { none, menu, started, level1, level1End, level2, level2End, level3Start, level2l2, level3, level3End, level4s1, level4s2, level4End, level5 }//new
    [HideInInspector] public musicStates musicState;

    #endregion

    #region Settings
    [Header("Settings:")]
    [Header("Volume")]
    [SerializeField] [Range(0.0001f, 1f)] public float masterVolume = 1;
    [SerializeField] [Range(0.0001f, 1f)] public float SFXVolume = 1;
    [SerializeField] [Range(0.0001f, 1f)] public float musicVolume = 1;

    [Header("Pitch")]
    [SerializeField] [Range(0f, 10f)] public float musicPitch = 1;
    [SerializeField] [Range(0f, 10f)] public float SFXPitch = 1;
    [HideInInspector] [Range(0f, 10f)] public float masterPitch = 1;

    #endregion

    private void Awake()
    {
        //Instance
        if (i == null)
        {
            i = this;
        }

        //SoundSystem
        soundEvents = new List<AudioSource>();
        controlableSoundEvents = new List<ControlableSoundEvent>();

        pauseAll = false;

        //MusicSystem
        soundEvents = new List<AudioSource>();
        musicEvents = new List<CMusicEvent>();

        deathTimer = new SimpleTimer();
        battleTimer = new SimpleTimer();
        effectTimer = new SimpleTimer();

        percentage1 = 0;
        percentage2 = 0;
        musicState = musicStates.none;
        battleState = battleStates.playerRevived;
        volumeChange = false;
        musicIsPlaying = false;
        playerIsDead = false;
        playTransitionEffect = false;
        zeroTransition = false;
        theEnd = false;
        //musicReset = false;
        battleTimer.SetMaxTime(battleOverTime);

        l1endCase = true;


        //------------------------------------------------
        SoundTypesAssignment();

        //SoundSystem
        currentPAC = new SoundLibrary.ProtoAudioClip();

        //MusicSystem
        currentPMC = new MusicLibrary.ProtoMusicClip();
        //prevPMC = new MusicLibrary.ProtoMusicClip();
        currentAS = new AudioSource();
        prevAS = new AudioSource();
    }

    private void Start()
    {
        //Settings
        SetMasterVolumeInMixer(masterVolume);
        SetMusicVolumeInMixer(masterVolume);
        SetSFXVolumeInMixer(masterVolume);

        SetMusicPitchInMixer(musicPitch);
        SetSFXPitchInMixer(SFXPitch);
    }

    private void Update()
    {
        //clipTimeStartFrame = currentAS.clip.length - currentAS.time;

        #region MusicSystemsUpdates
        if (musicIsPlaying)
        {
            MusicCycleUpdateSample();

            if (battleTimer.IsCompleted())
            {
                if (!theEnd)
                {
                    if (musicState == musicStates.level2 || musicState == musicStates.level3 || musicState == musicStates.level1End)
                    {
                        //Debug.Log((int)battleState);
                        //Debug.Log("MusicChangeLevel(musicStates.level1)");
                        //MusicChangeLevel(musicStates.level1);
                        if (battleLevel == battleLevels.L2)
                        {
                            MusicChangeLevel(musicStates.level1End); //Debug.Log((int)battleState);
                        }

                        if (battleLevel == battleLevels.L3)
                        {
                            MusicChangeLevel(musicStates.level2l2);
                        }

                        if (battleLevel == battleLevels.L1)
                        {
                            MusicChangeLevel(musicStates.level1); //Debug.Log((int)battleState);
                        }
                    }
                }
                battleTimer.TimerReset();
                battleState = battleStates.playerRevived;
            }

            if (battleState == battleStates.battleOver)
                battleTimer.Tick(Time.deltaTime);

            //Debug.Log(musicIsPlaying);

            MusicCycleNextSample();

            if (playerIsDead)
            {
                if (deathTimer.IsCompleted())
                {
                    playerIsDead = false;
                    deathTimer.TimerReset();
                }
            }

            deathTimer.Tick(Time.deltaTime);

            if (playTransitionEffect)
            {
                if (effectTimer.IsCompleted())
                {
                    //Debug.Log("Effect");
                    CreateMusicEvent((int)(Music.Effects.tention01), musicLibrary.MusicEffectClips, effect: true);
                    if (musicState == musicStates.level2)
                        effectTimer.TimerReset();
                    else
                        playTransitionEffect = false;
                }
                if (musicState == musicStates.level2)
                {
                    effectTimer.Tick(Time.deltaTime);
                }
                else
                {
                    playTransitionEffect = false;
                    effectTimer.TimerReset();
                }
            }
        }
        #endregion

        #region Check SoundEvents Array
        //SFX
        if (soundEvents.Count > 0)
        {
            for (int i = 0; i < soundEvents.Count; i++)
            {
                if (soundEvents[i] == null)
                {
                    soundEvents.RemoveAt(i);
                }
            }
        }

        if (controlableSoundEvents.Count > 0)
        {
            for (int i = 0; i < controlableSoundEvents.Count; i++)
            {
                if (controlableSoundEvents[i].controlableEvent == null)
                {
                    controlableSoundEvents.RemoveAt(i);
                }
            }
        }

        //Music
        if (musicEvents.Count > 0)
        {
            for (int i = 0; i < musicEvents.Count; i++)
            {
                if (musicEvents[i].mEvent == null)
                {
                    musicEvents.RemoveAt(i);
                }
            }
        }

        #endregion
    }

    #region SoundSystem


    public void CreateSoundEvent(object type, int priority = 128, float pitch = -1, int volume = -1,
                                 float startTime = 0, float endTime = 0, float minDist = 0, float maxDist = 0, Vector3 position = default(Vector3), bool loop = false)
    {
        bool typeWasFound = true;
        bool itIsBackround = false;
        if (type is SFX.ActiveRage)
        {
            CurrentAudioClips = soundLibrary.ActiveRageAudioClips;
        }
        else if (type is SFX.Background)
        {
            itIsBackround = true;
            CurrentAudioClips = soundLibrary.BackgroundAudioClips;
        }
        else if (type is SFX.BigOni)
        {
            CurrentAudioClips = soundLibrary.BigOniAudioClips;
        }
        else if (type is SFX.Environment)
        {
            CurrentAudioClips = soundLibrary.EnvironmentAudioClips;
        }
        else if (type is SFX.FinalBoss)
        {
            CurrentAudioClips = soundLibrary.FinalBossAudioClips;
        }
        else if (type is SFX.NoRage)
        {
            CurrentAudioClips = soundLibrary.NoRageAudioClips;
        }
        else if (type is SFX.Oni)
        {
            CurrentAudioClips = soundLibrary.OniAudioClips;
        }
        else if (type is SFX.Orb)
        {
            CurrentAudioClips = soundLibrary.OrbAudioClips;
        }
        else if (type is SFX.PassiveRage)
        {
            CurrentAudioClips = soundLibrary.PassiveRageAudioClips;
        }
        else if (type is SFX.Universal)
        {
            CurrentAudioClips = soundLibrary.UniversalAudioClips;
        }
        else if (type is SFX.Yurei)
        {
            CurrentAudioClips = soundLibrary.YureiAudioClips;
        }
        else
        {
            typeWasFound = false;
        }

        if (typeWasFound)
        {
            if (itIsBackround)
            {
                CreateSoundEventActual((int)type, soundLibrary.BackgroundAudioClips, volume, priority, pitch, startTime, endTime, minDist, maxDist, position, true, (SFX.Background)type, loop);
            }

            CreateSoundEventActual((int)type, CurrentAudioClips, volume, priority, pitch, startTime, endTime, minDist, maxDist, position, false);
        }
    }

    #region ActualCreate
    private void CreateSoundEventActual(int type, SoundLibrary.ProtoAudioClip[] ProtoAudioClips, float _volume, int _priority, float _pitch, float _startTime, float _endTime, float _minDist, float _maxDist, Vector3 _position, bool _controllable, SFX.Background _type = SFX.Background.None, bool _loop = false)
    {
        if (ProtoAudioClips.Length >= 0)
        {//Если в массиве что-то вобще есть
            currentPAC = null;
            List<SoundLibrary.ProtoAudioClip> ranPAC = new List<SoundLibrary.ProtoAudioClip>();

            foreach (SoundLibrary.ProtoAudioClip uAudioClip in ProtoAudioClips)
            {//То найти подходящий звук из массива.
                if (uAudioClip.type == type)
                {
                    ranPAC.Add(uAudioClip);
                    //ranClipVolume.Add(uAudioClip.volume);
                }
            }
            if (ranPAC.Count > 0)
            {//Если звук нашелся.
                if (ranPAC.Count > 1)
                {//Если больше одного звука на тип то выбираем рандомный.
                    int randomNum = Random.Range(0, ranPAC.Count);
                    currentPAC = ranPAC[randomNum];
                }
                else
                {//Если нет то первый.
                    currentPAC = ranPAC[0];
                }

                if (currentPAC.audioClip != null)
                {
                    AudioSource audioSource = new AudioSource();

                    audioSource = Instantiate(soundEvent, _position, Quaternion.identity);

                    audioSource.clip = currentPAC.audioClip;

                    if (_volume >= 0 && _volume <= 100) //Если указана громкость то ставим ее.
                        audioSource.volume = _volume;
                    else audioSource.volume = ((float)currentPAC.setups.volume / 100);

                    //if (_priority > 128 && _priority <= 256) //Если указана громкость то ставим ее.
                    //audioSource.priority = _priority;
                    //else
                    audioSource.priority = currentPAC.setups.priority;

                    if (_pitch >= 0 && _pitch <= 3)
                        audioSource.pitch = _pitch;
                    else audioSource.pitch = currentPAC.setups.pitch;

                    /*if (currentPAC.setups.volume > 0) //Если указана громкость то ставим ее.
                        audioSource.volume = ((float)currentPAC.setups.volume / 100);
                    else if(_volume >= 0 && _volume <= 100) 
                        audioSource.volume = _volume;*/

                    //3d sound check
                    if (_minDist >= 0 && _maxDist > 0)
                    {
                        audioSource.spatialBlend = 1f;
                        if (_minDist < _maxDist)
                        {
                            audioSource.minDistance = _minDist;
                            audioSource.maxDistance = _maxDist;
                        }
                        else
                        {
                            audioSource.minDistance = _maxDist - 1f;
                            audioSource.maxDistance = _maxDist;
                        }
                    }
                    else if (currentPAC.setups.s3DSettings.minDistance >= 0 && currentPAC.setups.s3DSettings.maxDistance > 0)
                    {
                        audioSource.spatialBlend = 1f;
                        if (currentPAC.setups.s3DSettings.minDistance < currentPAC.setups.s3DSettings.maxDistance)
                        {
                            audioSource.minDistance = currentPAC.setups.s3DSettings.minDistance;
                            audioSource.maxDistance = currentPAC.setups.s3DSettings.maxDistance;
                        }
                        else
                        {
                            audioSource.minDistance = currentPAC.setups.s3DSettings.maxDistance - 1f;
                            audioSource.maxDistance = currentPAC.setups.s3DSettings.minDistance;
                        }
                    }

                    //Background sound check
                    if (_controllable)
                    {
                        ControlableSoundEvent controlableSoundEvent = new ControlableSoundEvent();
                        controlableSoundEvent.controlableEvent = audioSource;
                        controlableSoundEvent.type = _type;
                        controlableSoundEvent.volume = audioSource.volume;
                        bool help = true;
                        if (controlableSoundEvents.Count > 0)
                        {
                            for (int i = 0; i < controlableSoundEvents.Count; i++)
                            {
                                if (controlableSoundEvents[i].type == controlableSoundEvent.type)
                                {
                                    GameObject help1 = controlableSoundEvents[i].controlableEvent.gameObject;
                                    controlableSoundEvents[i] = null; //а точно так надо?
                                    help1.GetComponent<AudioSource>().volume = 0f;
                                    Destroy(help1, 1f);
                                    controlableSoundEvents[i] = controlableSoundEvent;
                                    help = false;
                                    break;
                                }
                            }
                        }

                        if (help)
                            controlableSoundEvents.Add(controlableSoundEvent);

                        audioSource.Play();

                        if (_loop)
                            audioSource.loop = true;
                    }
                    else
                    {
                        soundEvents.Add(audioSource);

                        float clipLenght = audioSource.clip.length;
                        if (_endTime > 0)
                        {
                            if (_endTime <= clipLenght)
                                clipLenght = _endTime;
                        }

                        if (_startTime > 0 && _startTime < clipLenght)
                        {
                            audioSource.time = _startTime;
                        }
                        else if (currentPAC.setups.startTime > 0 && currentPAC.setups.startTime < clipLenght)
                        {
                            audioSource.time = currentPAC.setups.startTime;
                        }

                        audioSource.Play();

                        Destroy(audioSource.gameObject, clipLenght);
                    }
                }
            }
            else Debug.Log("Error. This sound type does not assigned in the library.");
        }
        else Debug.Log("Error. No sounds in the library.");


    }
    #endregion


    public void AddCustomFunctionToList(AudioSource _audioSource)
    {
        soundEvents.Add(_audioSource);
    }

    #region SetFunctions

    public void SetSoundEventPitch(SFX.Background type, float pitchValue)
    {
        bool help1 = false;
        for (int i = 0; i < controlableSoundEvents.Count; i++)
        {
            if (controlableSoundEvents[i].type == type)
            {
                if (pitchValue >= 0 && pitchValue <= 3)
                    controlableSoundEvents[i].controlableEvent.pitch = pitchValue;
                else controlableSoundEvents[i].controlableEvent.pitch = 1f;
                help1 = true;
            }
        }
        if (!help1) Debug.Log("Error. No sound of this type found.");
    }

    public void PauseSoundEvent(SFX.Background type)
    {
        //Debug.Log("Pause");
        bool help1 = false;
        for (int i = 0; i < controlableSoundEvents.Count; i++)
        {
            if (controlableSoundEvents[i].type == type)
            {
                controlableSoundEvents[i].isPaused = true;
                controlableSoundEvents[i].pausedTime = controlableSoundEvents[i].controlableEvent.time;
                controlableSoundEvents[i].controlableEvent.volume = 0f;
                controlableSoundEvents[i].controlableEvent.Pause();
                help1 = true;
            }
        }
        if (!help1) Debug.Log("Error. No sound of this type found.");
    }

    public void UnpauseSoundEvent(SFX.Background type)
    {
        //Debug.Log("Unpause");
        bool help1 = false;
        for (int i = 0; i < controlableSoundEvents.Count; i++)
        {
            if (controlableSoundEvents[i].type == type)
            {
                if (controlableSoundEvents[i].isPaused == true)
                {
                    controlableSoundEvents[i].isPaused = false;
                    controlableSoundEvents[i].controlableEvent.volume = controlableSoundEvents[i].volume;
                    controlableSoundEvents[i].controlableEvent.Play(0);
                    controlableSoundEvents[i].pausedTime = 0;
                    help1 = true;
                }
            }
        }
        if (!help1) Debug.Log("Error. No sound of this type found.");
    }

    public void StopSoundEvent(SFX.Background type)
    {
        //Debug.Log("Stop");
        bool help1 = false;
        for (int i = 0; i < controlableSoundEvents.Count; i++)
        {
            if (controlableSoundEvents[i].type == type)
            {
                if (controlableSoundEvents[i].isPaused == true)
                {
                    AudioSource help = controlableSoundEvents[i].controlableEvent;
                    help.volume = 0f;
                    controlableSoundEvents.RemoveAt(i);
                    Destroy(help.gameObject);
                    help1 = true;
                }
            }
        }
        if (!help1) Debug.Log("Error. No sound of this type found.");
    }

    public void PauseAllSFX()
    {
        if (soundEvents.Count > 0)
        {
            pauseAll = true;
            for (int i = 0; i < soundEvents.Count; i++)
            {
                if (soundEvents[i] != null)
                    soundEvents[i].Pause();
            }
        }

        if (controlableSoundEvents.Count > 0)
        {
            pauseAll = true;
            for (int i = 0; i < controlableSoundEvents.Count; i++)
            {
                if (controlableSoundEvents[i] != null)
                    controlableSoundEvents[i].controlableEvent.Pause();
            }
        }
    }

    public void PlayAllSFX()
    {
        if (pauseAll)
        {
            if (soundEvents.Count > 0)
            {
                pauseAll = false;
                for (int i = 0; i < soundEvents.Count; i++)
                {
                    if (soundEvents[i] != null)
                        soundEvents[i].Play();
                }
            }

            if (controlableSoundEvents.Count > 0)
            {
                pauseAll = false;
                for (int i = 0; i < controlableSoundEvents.Count; i++)
                {
                    if (controlableSoundEvents[i] != null)
                        controlableSoundEvents[i].controlableEvent.Play();
                }
            }
        }
    }

    public void StopAllSFX()
    {
        if (soundEvents.Count > 0)
        {
            for (int i = 0; i < soundEvents.Count; i++)
            {
                if (soundEvents[i] != null)
                    soundEvents[i].Stop();
            }
        }

        if (controlableSoundEvents.Count > 0)
        {
            for (int i = 0; i < controlableSoundEvents.Count; i++)
            {
                if (controlableSoundEvents[i] != null)
                    controlableSoundEvents[i].controlableEvent.Stop();
            }
        }
    }


    public void SetMasterVolume(float value) 
    {
        masterVolume = Mathf.Clamp(value, 0.0001f, 1f);
        SetMasterVolumeInMixer(masterVolume);
    }
    public void SetMusicVolume(float value) 
    { 
        musicVolume = Mathf.Clamp(value, 0.0001f, 1f);
        SetMusicVolumeInMixer(musicVolume);
    }
    public void SetSFXVolume(float value) 
    {
        SFXVolume = Mathf.Clamp(value, 0.0001f, 1f);
        SetSFXVolumeInMixer(SFXVolume);
    }

    private void SetMasterVolumeInMixer(float value)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(value) * 20f);
    }
    private void SetMusicVolumeInMixer(float value)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(value) * 20f);
    }
    private void SetSFXVolumeInMixer(float value)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(value) * 20f);
    }

    private void SetMusicLowPass(float value)
    {
        audioMixer.SetFloat("musicLowPass", value);
    }


    public void SetMasterPitch(float value) 
    { //Do not use!
        masterPitch = Mathf.Clamp(value, 0f, 100f);
        SetMasterPitchInMixer(masterPitch);
    }
    public void SetMusicPitch(float value) 
    {
        musicPitch = Mathf.Clamp(value, 0f, 100f);
        SetMusicPitchInMixer(musicPitch);
    }
    public void SetSFXPitch(float value) 
    {
        SFXPitch = Mathf.Clamp(value, 0f, 100f);
        SetSFXPitchInMixer(SFXPitch);
    }


    private void SetMasterPitchInMixer(float value)
    {
        //audioMixer.SetFloat("masterPitch", value / 100f);
    }
    private void SetMusicPitchInMixer(float value)
    {
        audioMixer.SetFloat("musicPitch", value);
    }
    private void SetSFXPitchInMixer(float value)
    {
        audioMixer.SetFloat("sfxPitch", value);
    }

    #endregion
    #endregion

    #region MusicSystem

    public void L1endCase(bool b)
    {
        l1endCase = b;
    }


    public void MusicStart(bool again = false)
    {
        MusicStop();
        if (currentAS != null && currentAS.loop == true) { Destroy(currentAS.gameObject); }
        currentAS = null; prevAS = null;
        musicEvents.Clear();
        musicIsPlaying = true;

        /*
            For this showcase I desided to use old music solution.
            Feels wierd to share music that wasn't ever licensed.
            It just was created by our musitian for this project
            So I'm not sure if I can share it on my website.
        */

        if (oldLibrarySolution)
        {
            musicState = musicStates.started;
            CreateMusicEvent((int)(Music.MusicSamples.ML0Start), musicLibrary.MusicSampleClips);
        }
        else
        {
            musicState = musicStates.menu;
            if (!again)
                CreateMusicEvent((int)(Music.MusicSamples.Menu), musicLibrary.MusicSampleClips);
            else
                CreateMusicEvent((int)(Music.MusicSamples.Menu1), musicLibrary.MusicSampleClips);
        }
    }

    public void MusicRestart()
    {
        if (musicIsPlaying)
        {
            endTransiTime = currentPMC.setup.endTime / musicPitch;
            volumeChange = true;

            musicState = musicStates.started;
            if (currentPMC.type == (int)Music.MusicSamples.ML0p1)
            {
                CreateMusicEvent((int)(Music.MusicSamples.ML0p2), musicLibrary.MusicSampleClips, _volume: 0);
            }
            else CreateMusicEvent((int)(Music.MusicSamples.ML0p1), musicLibrary.MusicSampleClips, _volume: 0);
        }
    }

    private void MusicCycleNextSample()
    {
        if (musicIsPlaying)
        {
            if (!playerIsDead)
            {
                if (currentPMC != null)
                {
                    if (currentPMC.setup.endTime > 0)
                    {
                        if (currentAS != null)
                        {
                            if (currentAS.time >= currentAS.clip.length - currentPMC.setup.endTime)
                            {
                                endTransiTime = 0.1f / musicPitch;

                                volumeChange = true;
                                switch (musicState)
                                {
                                    case musicStates.menu:
                                        if (currentPMC.type == (int)Music.MusicSamples.Menu ||
                                            currentPMC.type == (int)Music.MusicSamples.ML5Fin ||
                                            currentPMC.type == (int)Music.MusicSamples.ML4End ||
                                            currentPMC.type == (int)Music.MusicSamples.Menu1)
                                        {
                                            endTransiTime = 1f / musicPitch;
                                            CreateMusicEvent((int)(Music.MusicSamples.Menu2), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.Menu2)
                                        {
                                            endTransiTime = 1f / musicPitch;
                                            CreateMusicEvent((int)(Music.MusicSamples.Menu1), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        break;

                                    case musicStates.started:
                                        if (currentPMC.type == (int)Music.MusicSamples.ML0Start ||
                                            currentPMC.type == (int)Music.MusicSamples.ML0p2)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML0p1), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.ML0p1)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML0p2), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        break;

                                    case musicStates.level1:
                                        if (currentPMC.type == (int)Music.MusicSamples.ML1p1 ||
                                            currentPMC.type == (int)Music.MusicSamples.ML5Fin)
                                        {
                                            endTransiTime = 1f / musicPitch;
                                            CreateMusicEvent((int)(Music.MusicSamples.ML1p2), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.ML1p2)
                                        {
                                            endTransiTime = 1f / musicPitch;
                                            CreateMusicEvent((int)(Music.MusicSamples.ML1p1), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        break;

                                    case musicStates.level1End:

                                        if (currentPMC.type == (int)Music.MusicSamples.ML1End_p1)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML1End_p2), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.ML1End_p2)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML1End_p1), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        break;

                                    case musicStates.level2:
                                        if (currentPMC.type == (int)Music.MusicSamples.ML2p1 ||
                                            currentPMC.type == (int)Music.MusicSamples.ML2Start ||
                                            currentPMC.type == (int)Music.MusicSamples.ML2Death1 ||
                                            currentPMC.type == (int)Music.MusicSamples.ML2Death2)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML2p2), musicLibrary.MusicSampleClips, _volume: 0);
                                            if (playTransitionEffect) effectTimer.SetMaxTime((currentAS.clip.length - 2.5f) / musicPitch);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.ML2p2)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML2p1), musicLibrary.MusicSampleClips, _volume: 0);
                                            if (playTransitionEffect) effectTimer.SetMaxTime((currentAS.clip.length - 2.5f) / musicPitch);
                                        }
                                        break;

                                    case musicStates.level2End:
                                        if (currentPMC.type == (int)Music.MusicSamples.ML2End)
                                        {
                                            MusicChangeLevel(musicStates.level3Start);
                                        }
                                        break;

                                    case musicStates.level3Start://for new library
                                        if (currentPMC.type == (int)Music.MusicSamples.ML3bS1)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML3bS2), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.ML3bS2)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML3bS1), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        break;

                                    case musicStates.level2l2://for new library
                                        if (currentPMC.type == (int)Music.MusicSamples.ML3e1)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML3e2), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.ML3e2)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML3e1), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        break;

                                    case musicStates.level3: //and new and old
                                        if (oldLibrarySolution)
                                        {
                                            if (currentPMC.type == (int)Music.MusicSamples.ML3Start ||
                                                currentPMC.type == (int)Music.MusicSamples.ML3p2 ||
                                                currentPMC.type == (int)Music.MusicSamples.ML3Death1 ||
                                                currentPMC.type == (int)Music.MusicSamples.ML3Death2)
                                            {
                                                CreateMusicEvent((int)(Music.MusicSamples.ML3p1), musicLibrary.MusicSampleClips, _volume: 0);
                                            }
                                            else if (currentPMC.type == (int)Music.MusicSamples.ML3p1)
                                            {
                                                CreateMusicEvent((int)(Music.MusicSamples.ML3p2), musicLibrary.MusicSampleClips, _volume: 0);
                                            }
                                        }
                                        else
                                        {
                                            if (currentPMC.type == (int)Music.MusicSamples.ML3b1 ||
                                                currentPMC.type == (int)Music.MusicSamples.ML3bS1 ||
                                                currentPMC.type == (int)Music.MusicSamples.ML3bS2 ||
                                                currentPMC.type == (int)Music.MusicSamples.ML3bD1 ||
                                                currentPMC.type == (int)Music.MusicSamples.ML3bD2)
                                            {
                                                CreateMusicEvent((int)(Music.MusicSamples.ML3b2), musicLibrary.MusicSampleClips, _volume: 0);
                                            }
                                            else if (currentPMC.type == (int)Music.MusicSamples.ML3b2)
                                            {
                                                CreateMusicEvent((int)(Music.MusicSamples.ML3b1), musicLibrary.MusicSampleClips, _volume: 0);
                                            }
                                        }
                                        break;

                                    case musicStates.level3End:
                                        if (currentPMC.type == (int)Music.MusicSamples.ML3Start ||
                                            currentPMC.type == (int)Music.MusicSamples.ML3Death1 ||
                                            currentPMC.type == (int)Music.MusicSamples.ML3Death2 ||
                                            currentPMC.type == (int)Music.MusicSamples.ML3p2)
                                        {
                                            endTransiTime = currentPMC.setup.endTime / musicPitch; zeroTransition = false;
                                            CreateMusicEvent((int)(Music.MusicSamples.ML3p1), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.ML3p1)
                                        {
                                            endTransiTime = currentPMC.setup.endTime / musicPitch; zeroTransition = false;
                                            CreateMusicEvent((int)(Music.MusicSamples.ML3p2), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        break;

                                    case musicStates.level4s1:
                                        if (currentPMC.type == (int)Music.MusicSamples.ML4Start ||
                                            currentPMC.type == (int)Music.MusicSamples.ML4p1_2)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML4p1_1), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.ML4p1_1)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML4p1_2), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        break;

                                    case musicStates.level4s2:
                                        if (currentPMC.type == (int)Music.MusicSamples.ML4p2_2)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML4p2_1), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        else if (currentPMC.type == (int)Music.MusicSamples.ML4p2_1)
                                        {
                                            CreateMusicEvent((int)(Music.MusicSamples.ML4p2_2), musicLibrary.MusicSampleClips, _volume: 0);
                                        }
                                        break;

                                    case musicStates.level4End:
                                        if (currentPMC.type == (int)Music.MusicSamples.ML4End)
                                        {
                                            if(oldLibrarySolution)
                                                MusicChangeLevel(musicStates.level5);
                                            else
                                                MusicChangeLevel(musicStates.menu);
                                        }
                                        break;

                                    case musicStates.level5:

                                        break;
                                }
                            }

                        }
                        else Debug.Log("curAC = null");
                    }
                }
            }
        }
    }

    private void MusicCycleUpdateSample()
    {
        if (musicIsPlaying)
        {
            if (volumeChange)
            {
                if (currentAS != null)
                {
                    if (prevAS != null)
                    {
                        if (!zeroTransition)
                        {
                            if (!playerIsDead)
                            {
                                if (currentAS.volume < currentVolume)
                                {
                                    //Debug.Log("VolumeC: " + currentAS.volume);
                                    currentAS.volume = Mathf.Lerp(0, currentVolume, percentage1);
                                    percentage1 += 0.0085f / endTransiTime;
                                }
                            }

                            if (prevAS.volume > 0)
                            {
                                //Debug.Log("VolumeP: " + prevAS.volume);
                                prevAS.volume = Mathf.Lerp(prevVolume, 0, percentage2);
                                percentage2 += 0.0085f / endTransiTime;
                            }

                            if (prevAS.volume <= 0 && currentAS.volume >= currentVolume)
                            {
                                //Debug.Log("true-true");
                                percentage1 = 0;
                                percentage2 = 0;
                                prevAS.volume = 0;
                                currentAS.volume = currentVolume;
                                volumeChange = false;
                            }
                        }
                        else
                        {
                            //Debug.Log("ZeroT");
                            percentage1 = 0;
                            percentage2 = 0;
                            currentAS.volume = currentVolume;
                            prevAS.volume = 0;
                            volumeChange = false;
                            zeroTransition = false;
                        }
                    }
                    else Debug.Log("prevAC = null");
                }
                else Debug.Log("curAC = null");
            }

        }
    }

    public void MusicChangeLevel(musicStates nextState)
    {
        if (musicIsPlaying)
        {
            if (musicState != nextState)
            {
                if (currentAS != null)
                {
                    //if (currentPMC.setup.endTime != 0)
                    endTransiTime = currentPMC.setup.endTime / musicPitch;
                    //else 
                    //endTransiTime = 0;

                    volumeChange = true;
                    switch (nextState)
                    {
                        case musicStates.menu:
                            CreateMusicEvent((int)(Music.MusicSamples.Menu), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.started:
                            CreateMusicEvent((int)(Music.MusicSamples.ML0p1), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level1:
                            if (musicState == musicStates.level2)
                                endTransiTime = currentPMC.setup.endTime + 2f / musicPitch;
                            CreateMusicEvent((int)(Music.MusicSamples.ML1p1), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level1End:
                            l1endCase = false;
                            CreateMusicEvent((int)(Music.MusicSamples.ML1End_p1), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level2:
                            CreateMusicEvent((int)(Music.MusicSamples.ML2Start), musicLibrary.MusicSampleClips, _volume: 0);
                            playTransitionEffect = true;
                            effectTimer.SetMaxTime((currentAS.clip.length - 1.9f) / musicPitch);
                            break;
                        case musicStates.level2End:
                            CreateMusicEvent((int)(Music.MusicSamples.ML2End), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level3Start:
                            if (oldLibrarySolution)
                                CreateMusicEvent((int)(Music.MusicSamples.ML3Start), musicLibrary.MusicSampleClips, _volume: 0);
                            else
                                CreateMusicEvent((int)(Music.MusicSamples.ML3bS1), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level2l2:
                            if (musicState == musicStates.level3)
                                endTransiTime = currentPMC.setup.endTime + 2f / musicPitch;
                            CreateMusicEvent((int)(Music.MusicSamples.ML3e1), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level3:
                            if (oldLibrarySolution) 
                                CreateMusicEvent((int)(Music.MusicSamples.ML3Start), musicLibrary.MusicSampleClips, _volume: 0);
                            else
                                CreateMusicEvent((int)(Music.MusicSamples.ML3bS1), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level3End:
                            CreateMusicEvent((int)(Music.MusicSamples.ML3Start), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level4s1:
                            endTransiTime = 0.1f;
                            CreateMusicEvent((int)(Music.MusicSamples.ML4Start), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level4s2:
                            CreateMusicEvent((int)(Music.MusicSamples.ML4p2_1), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level4End:
                            CreateMusicEvent((int)(Music.MusicSamples.ML4End), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                        case musicStates.level5:
                            CreateMusicEvent((int)(Music.MusicSamples.ML5Fin), musicLibrary.MusicSampleClips, _volume: 0);
                            break;
                    }
                    musicState = nextState;
                }
                else Debug.Log("curAC = null");
            }
            //else Debug.Log("StateIsTheSame");
        }
    }

    public void SetMusicBattleState(battleStates nextBattleState)
    {
        if (musicIsPlaying)
        {
            switch (nextBattleState)
            {
                case battleStates.battleOver:
                    if (battleLevel == battleLevels.L2)
                        battleTimer.SetMaxTime((battleOverTime + 60f) / musicPitch);
                    else
                        battleTimer.SetMaxTime(battleOverTime / musicPitch);
                    break;

                case battleStates.battleStarted:
                    battleTimer.TimerReset();
                    if (battleLevel == battleLevels.L1)
                        MusicChangeLevel(musicStates.level1End);
                    if (battleLevel == battleLevels.L2)
                        MusicChangeLevel(musicStates.level2);
                    if (battleLevel == battleLevels.L3)
                        MusicChangeLevel(musicStates.level3);
                    break;

                case battleStates.playerRevived:
                    battleTimer.TimerReset();
                    break;
            }
            battleState = nextBattleState;
        }
    }

    public void MusicPlayerIsDead(bool closeToArena = false, bool loadCheck = false)
    {
        if (musicIsPlaying)
        {
            playerIsDead = true;
            SetMusicBattleState(battleStates.playerRevived);
            if (battleTimer != null)
                battleTimer.TimerReset();
            if (!loadCheck)
                deathTimer.SetMaxTime(4.0f / musicPitch);
            else
                deathTimer.SetMaxTime(0.5f / musicPitch);
            endTransiTime = 2f / musicPitch;
            volumeChange = true;
            if (closeToArena)
            {
                if (battleLevel == battleLevels.L1)
                {
                    musicState = musicStates.started;
                    if (currentPMC.type == (int)Music.MusicSamples.ML1Death2)
                        CreateMusicEvent((int)(Music.MusicSamples.ML2Death1), musicLibrary.MusicSampleClips, _volume: 0);
                    else if (currentPMC.type == (int)Music.MusicSamples.ML2Death1)
                        CreateMusicEvent((int)(Music.MusicSamples.ML1Death2), musicLibrary.MusicSampleClips, _volume: 0);
                    else
                        CreateMusicEvent((int)(Music.MusicSamples.ML1Death1), musicLibrary.MusicSampleClips, _volume: 0);
                }

                if (battleLevel == battleLevels.L2)
                {
                    musicState = musicStates.level2;
                    if (currentPMC.type == (int)Music.MusicSamples.ML2Death2)
                        CreateMusicEvent((int)(Music.MusicSamples.ML2Death1), musicLibrary.MusicSampleClips, _volume: 0);
                    else if (currentPMC.type == (int)Music.MusicSamples.ML2Death1)
                        CreateMusicEvent((int)(Music.MusicSamples.ML2Death2), musicLibrary.MusicSampleClips, _volume: 0);
                    else
                        CreateMusicEvent((int)(Music.MusicSamples.ML2Death1), musicLibrary.MusicSampleClips, _volume: 0);
                }

                if (battleLevel == battleLevels.L3)
                {
                    musicState = musicStates.level3;
                    if (currentPMC.type == (int)Music.MusicSamples.ML3bD1)
                        CreateMusicEvent((int)(Music.MusicSamples.ML3bD2), musicLibrary.MusicSampleClips, _volume: 0);
                    else if (currentPMC.type == (int)Music.MusicSamples.ML3bD1)
                        CreateMusicEvent((int)(Music.MusicSamples.ML3bD2), musicLibrary.MusicSampleClips, _volume: 0);
                    else
                        CreateMusicEvent((int)(Music.MusicSamples.ML3bD1), musicLibrary.MusicSampleClips, _volume: 0);

                }

                playTransitionEffect = true;
                effectTimer.SetMaxTime((currentAS.clip.length - 1.9f) / musicPitch);
            }
            else
            {
                if (musicState == musicStates.level3End || musicState == musicStates.level4s1 ||
                    musicState == musicStates.level4s2 || musicState == musicStates.level4End)
                {
                    musicState = musicStates.level3End;
                    if (currentPMC.type == (int)Music.MusicSamples.ML3Death2)
                        CreateMusicEvent((int)(Music.MusicSamples.ML3Death1), musicLibrary.MusicSampleClips, _volume: 0);
                    else if (currentPMC.type == (int)Music.MusicSamples.ML3Death1)
                        CreateMusicEvent((int)(Music.MusicSamples.ML3Death2), musicLibrary.MusicSampleClips, _volume: 0);
                    else
                        CreateMusicEvent((int)(Music.MusicSamples.ML3Death1), musicLibrary.MusicSampleClips, _volume: 0);
                }
                else
                {
                    if (battleLevel == battleLevels.L1)
                    {
                        musicState = musicStates.started;
                        if (currentPMC.type == (int)Music.MusicSamples.ML0p2)
                            CreateMusicEvent((int)(Music.MusicSamples.ML0p1), musicLibrary.MusicSampleClips, _volume: 0);
                        else if (currentPMC.type == (int)Music.MusicSamples.ML0p1)
                            CreateMusicEvent((int)(Music.MusicSamples.ML0p2), musicLibrary.MusicSampleClips, _volume: 0);
                        else
                            CreateMusicEvent((int)(Music.MusicSamples.ML0p1), musicLibrary.MusicSampleClips, _volume: 0);
                    }
                    if (battleLevel == battleLevels.L2)
                    {
                        musicState = musicStates.level1;
                        if (currentPMC.type == (int)Music.MusicSamples.ML1p2)
                            CreateMusicEvent((int)(Music.MusicSamples.ML1p1), musicLibrary.MusicSampleClips, _volume: 0);
                        else if (currentPMC.type == (int)Music.MusicSamples.ML1p1)
                            CreateMusicEvent((int)(Music.MusicSamples.ML1p2), musicLibrary.MusicSampleClips, _volume: 0);
                        else
                            CreateMusicEvent((int)(Music.MusicSamples.ML1p1), musicLibrary.MusicSampleClips, _volume: 0);
                    }
                    if (battleLevel == battleLevels.L3)
                    {
                        musicState = musicStates.level2l2;
                        if (currentPMC.type == (int)Music.MusicSamples.ML3e2)
                            CreateMusicEvent((int)(Music.MusicSamples.ML3e1), musicLibrary.MusicSampleClips, _volume: 0);
                        else if (currentPMC.type == (int)Music.MusicSamples.ML3e1)
                            CreateMusicEvent((int)(Music.MusicSamples.ML3e2), musicLibrary.MusicSampleClips, _volume: 0);
                        else
                            CreateMusicEvent((int)(Music.MusicSamples.ML3e1), musicLibrary.MusicSampleClips, _volume: 0);
                    }
                }
            }
        }
    }

    #region Play/Pause/Stop
    public void MusicPause(bool lp = false)
    {
        if (lp)
            SetMusicLowPass(4000f);
        else
        {
            if (musicEvents.Count > 0)
            {
                musicIsPlaying = false;
                for (int i = 0; i < musicEvents.Count; i++)
                {
                    if (musicEvents[i].mEvent != null)
                        musicEvents[i].mEvent.Pause();
                }
            }
        }
    }

    public void MusicPlay(bool lp = false)
    {
        if (lp)
            SetMusicLowPass(22000f);
        else
        {
            musicIsPlaying = true;
            if (musicEvents.Count > 0)
            {
                for (int i = 0; i < musicEvents.Count; i++)
                {
                    if (musicEvents[i].mEvent != null)
                        musicEvents[i].mEvent.Play();
                }
            }
        }
    }

    public void MusicStop()
    {
        musicIsPlaying = false;
        if (musicEvents.Count > 0)
        {
            for (int i = 0; i < musicEvents.Count; i++)
            {
                if (musicEvents[i].mEvent != null)
                    musicEvents[i].mEvent.Stop();
            }
        }
    }
    #endregion

    private void CreateMusicEvent(int _type, MusicLibrary.ProtoMusicClip[] arr, float _volume = 1, float _startTime = 0, float _endTime = 0, bool effect = false, bool loop = false)
    {
        if (effect)
        {
            if (arr.Length >= 0)
            {
                List<MusicLibrary.ProtoMusicClip> seqPMC = new List<MusicLibrary.ProtoMusicClip>();
                MusicLibrary.ProtoMusicClip pmcHelp = new MusicLibrary.ProtoMusicClip();
                foreach (MusicLibrary.ProtoMusicClip musicClip in arr)
                {
                    if (musicClip.type == _type)
                    {
                        seqPMC.Add(musicClip);
                    }
                }
                if (seqPMC.Count > 0)
                {
                    if (seqPMC.Count > 1)
                    {
                        int randomNum = Random.Range(0, seqPMC.Count);
                        pmcHelp = seqPMC[randomNum];
                    }
                    else
                    {
                        pmcHelp = seqPMC[0];
                    }
                    if (pmcHelp.audioClip != null)
                    {
                        AudioSource audioSource = new AudioSource();

                        audioSource = Instantiate(musicEvent, Vector3.zero, Quaternion.identity);

                        audioSource.clip = pmcHelp.audioClip;

                        float curVol = (float)pmcHelp.setup.volume / 100;
                        audioSource.volume = curVol;

                        audioSource.priority = pmcHelp.setup.priority;

                        audioSource.pitch = pmcHelp.setup.pitch;

                        CMusicEvent cMusicEvent = new CMusicEvent();
                        cMusicEvent.mEvent = audioSource;
                        cMusicEvent.type = _type;

                        bool help = true;
                        if (musicEvents.Count > 0)
                        {
                            for (int i = 0; i < musicEvents.Count; i++)
                            {
                                if (musicEvents[i].type == cMusicEvent.type)
                                {
                                    if (musicEvents[i].mEvent != null)
                                    {
                                        GameObject help1 = musicEvents[i].mEvent.gameObject;
                                        musicEvents[i] = null; //а точно так надо?
                                        Destroy(help1);
                                    }

                                    musicEvents[i] = cMusicEvent;
                                    help = false;
                                }
                            }
                        }

                        if (help)
                            musicEvents.Add(cMusicEvent);

                        float clipLenght = audioSource.clip.length / musicPitch;

                        audioSource.Play();

                        Destroy(audioSource.gameObject, clipLenght);
                    }
                }
                else Debug.Log("Error. This effect type does not assigned in the library.");
            }
            else Debug.Log("Error. No effects in the library.");
        }
        else
        {
            if (arr.Length >= 0)
            {//Если в массиве что-то вобще есть
                //prevPMC = currentPMC;
                currentPMC = null;
                //currentAudioClip = null;
                List<MusicLibrary.ProtoMusicClip> seqPMC = new List<MusicLibrary.ProtoMusicClip>();

                foreach (MusicLibrary.ProtoMusicClip musicClip in arr)
                {//То найти подходящий звук из массива.
                 //Debug.Log("pAudioClip.type = " + uAudioClip.type + " , type = " + type);
                    if (musicClip.type == _type)
                    {
                        seqPMC.Add(musicClip);
                    }
                }
                if (seqPMC.Count > 0)
                {//Если звук нашелся.
                    if (seqPMC.Count > 1)
                    {//Если больше одного на тип то выбираем по порядку.
                     //пока не уверен как, можно в будущем дописать.
                        int randomNum = Random.Range(0, seqPMC.Count);
                        currentPMC = seqPMC[randomNum];
                    }
                    else
                    {//Если нет то первый.
                        currentPMC = seqPMC[0];
                    }

                    if (currentPMC.audioClip != null)
                    {
                        AudioSource audioSource = new AudioSource();

                        audioSource = Instantiate(musicEvent, Vector3.zero, Quaternion.identity);
                        prevAS = currentAS;
                        currentAS = audioSource;

                        audioSource.clip = currentPMC.audioClip;

                        prevVolume = currentVolume;
                        currentVolume = (float)currentPMC.setup.volume / 100;
                        audioSource.volume = currentVolume;
                        if (_volume == 0) audioSource.volume = 0;

                        audioSource.priority = currentPMC.setup.priority;

                        audioSource.pitch = currentPMC.setup.pitch;

                        audioSource.loop = loop;//?

                        CMusicEvent cMusicEvent = new CMusicEvent();
                        cMusicEvent.mEvent = audioSource;
                        cMusicEvent.type = _type;

                        bool help = true;
                        if (musicEvents.Count > 0)
                        {
                            for (int i = 0; i < musicEvents.Count; i++)
                            {
                                if (musicEvents[i].type == cMusicEvent.type)
                                {
                                    if (musicEvents[i].mEvent != null)
                                    {
                                        GameObject help1 = musicEvents[i].mEvent.gameObject;
                                        musicEvents[i] = null; //а точно так надо?
                                        Destroy(help1);
                                    }

                                    musicEvents[i] = cMusicEvent;
                                    help = false;
                                }
                            }
                        }

                        if (help)
                            musicEvents.Add(cMusicEvent);

                        float clipLenght = audioSource.clip.length / musicPitch;

                        if (currentPMC.setup.startTime > 0 && currentPMC.setup.startTime < clipLenght)
                        {
                            audioSource.time = currentPMC.setup.startTime;
                        }

                        audioSource.Play();

                        if (!loop)
                            Destroy(audioSource.gameObject, clipLenght + 5f);
                    }
                }
                else Debug.Log("Error. This music type does not assigned in the library.");
            }
            else Debug.Log("Error. No music in the library.");
        }
    }

    /*
    Методы муз системы:
    1. Начать музыку - уровень 0. +
    2. Зациклить на уровне 1 - игрок в меню. +
    3. Когда начнется игра перейти в уровень 2 - игрок навчал игру и возможно стоит тупит. +
    4. Когда игрок пошел в бой с какого-то момента перейти в динамику (уровень 2.5) и потом в бой - уровень 3 +
    5. Иметь возможность вернуться назад на уровень 2 и возможно 1. +
    6. Зациклить на уровне 3 на время и если игрок не сражается то вернуться на уровень 2 (но если бой близко то ненадо мб)
    7. При смерти все отключать и включать заного после секундной паузы.+
    8. При окончании первой локации завершать трек естественно на его вершине (уровень 3.5) и перейти на начало 2-ки. +
    9. При переключении между музыкой создавать вуу и жжж эффекты чтобы скрыть неидеальность (мозможно нескольких типов). +
    10. При боссе запускать особый трек перехода на уровень 4 и в конце запускать уровень 5 - конец + подумать еще над этим. +
    11. Иметь таймер после которого музыка уйдет на уровень вниз вне боя. +
    12. Добавить звук смерти игрока который мне понравился.
    13. Сделать чтобы музыка перед поссом и музыка босса колайдились или тпа того.
    */

    #endregion


    private void SoundTypesAssignment()
    {
        //SFX
        foreach (SoundLibrary.BackgroundAudioClip uac in soundLibrary.BackgroundAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.EnvironmentAudioClip uac in soundLibrary.EnvironmentAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.UniversalAudioClip uac in soundLibrary.UniversalAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.NoRageAudioClip uac in soundLibrary.NoRageAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.PassiveRageAudioClip uac in soundLibrary.PassiveRageAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.ActiveRageAudioClip uac in soundLibrary.ActiveRageAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.OniAudioClip uac in soundLibrary.OniAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.BigOniAudioClip uac in soundLibrary.BigOniAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.YureiAudioClip uac in soundLibrary.YureiAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.OrbAudioClip uac in soundLibrary.OrbAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }
        foreach (SoundLibrary.FinalBossAudioClip uac in soundLibrary.FinalBossAudioClips)
        {
            uac.SetType((int)uac.soundType);
        }

        //Music
        foreach (MusicLibrary.MusicSampleClip c in musicLibrary.MusicSampleClips)
        {
            c.SetType((int)c.musicSampleType);
        }
        foreach (MusicLibrary.MusicEffectClip c in musicLibrary.MusicEffectClips)
        {
            c.SetType((int)c.musicEffectType);
        }
    }
}