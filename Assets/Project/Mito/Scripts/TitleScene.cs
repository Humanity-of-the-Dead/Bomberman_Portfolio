using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    
    // 初期化処理
    void Start()
    {
        SceneLoader.Ins.LoadSceneAsync("GameScene");

        if (AudioManager.Ins)
        {
            AudioManager.Ins.PlayBGM(0);

            // 音量設定用スライダーを初期化
            bgmSlider.onValueChanged.AddListener(AudioManager.Ins.SetBGMMixer);
            bgmSlider.value = AudioManager.Ins.GetBGMMixer();

            seSlider.onValueChanged.AddListener(AudioManager.Ins.SetSEMixer);
            seSlider.value = AudioManager.Ins.GetSEMixer();
        }
    }
}
