using UnityEngine;
using UnityEngine.Audio;

public class AudioManager:PersistentSingleton<AudioManager>{
    [System.Serializable]struct AudioInfo{
        public AudioClip audioClip;
        public float volume;
    }
    [SerializeField]AudioInfo[] _bgmAudioInfo;
    [SerializeField]AudioInfo[] _seAudioInfo;
    
    [SerializeField]AudioSource _bgmAudioSource;
    [SerializeField]AudioSource _seAudioSource;
    
    [Header("BGM")]
    public int titleBGMIndex=0;
    public int gameBGMIndex=1;

    [SerializeField] AudioMixer AudioMixer;

    //[Header("SE")]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start(){
    //    // if(MySceneManager.ins.currentSceneType==MySceneManager.SceneType.TITLE)PlayBGM(titleBGMIndex);
    //    // else if(MySceneManager.ins.currentSceneType == MySceneManager.SceneType.GAME)PlayBGM(gameBGMIndex);
    //    // else PlayBGM(0);
    //}

    public void PlayBGM(int i){
        _bgmAudioSource.clip=_bgmAudioInfo[i].audioClip;
        _bgmAudioSource.volume=_bgmAudioInfo[i].volume;
        _bgmAudioSource.Play();
    }

    public void PlayOneShotSE(int i){
        if (!_seAudioSource.isPlaying)
        {
            _seAudioSource.PlayOneShot(_seAudioInfo[i].audioClip, _seAudioInfo[i].volume);
        }
    }

    public void SetBGMMixer(float _value)
    {
        // 0 - 1の値が来るので、-80 - 20の値に変換
        _value = -80 + 100 * _value;
        AudioMixer.SetFloat("BGM", _value);
    }

    public float GetBGMMixer()
    {
        float _value = 0;
        AudioMixer.GetFloat("BGM", out _value);
        // -80 - 20の値を取得するので、0 - 1の値に変換
        return (_value + 80) / 100;
    }

    public void SetSEMixer(float _value)
    {
        // 0 - 1の値が来るので、-80 - 20の値に変換
        _value = -80 + 100 * _value;
        AudioMixer.SetFloat("SE", _value);
    }

    public float GetSEMixer()
    {
        float _value = 0;
        AudioMixer.GetFloat("SE", out _value);
        // -80 - 20の値を取得するので、0 - 1の値に変換
        return (_value + 80) / 100;
    }
}