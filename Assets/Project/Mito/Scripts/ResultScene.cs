using UnityEngine;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour
{
    [Header("表示用 プレイヤー")]
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;

    [Header("結果を表示するImageとSprite画像")]
    [SerializeField] GameObject resultImageObject;
    [SerializeField] Sprite[] resultSprite;

    Animator p1Animator;
    Animator p2Animator;
    Image resultImage;
    
    // 結果画面のアニメーションネーム
    string pAnimeIdle = "player_idle";
    string pAnimeVictory = "player_victory";
    string pAnimeDeath = "player_death";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resultImage = resultImageObject.GetComponent<Image>();
        p1Animator = player1.GetComponent<Animator>();
        p2Animator = player2.GetComponent<Animator>();

        switch (ResultManager.Ins.GetWinner())
        {
            case -1:
                resultImage.sprite = resultSprite[0];
                p1Animator.Play(pAnimeIdle);
                p2Animator.Play(pAnimeIdle);

                if (AudioManager.Ins) AudioManager.Ins.PlayBGM(3);
                break;
            case 0:
                resultImage.sprite = resultSprite[1];
                p1Animator.Play(pAnimeVictory);
                p2Animator.Play(pAnimeDeath);

                if(AudioManager.Ins) AudioManager.Ins.PlayBGM(2);
                break;
            case 1:
                resultImage.sprite = resultSprite[2];
                p1Animator.Play(pAnimeDeath);
                p2Animator.Play(pAnimeVictory);

                if(AudioManager.Ins) AudioManager.Ins.PlayBGM(2);
                break;
        }
        
        SceneLoader.Ins.LoadSceneAsync("TitleScene");
    }
}
