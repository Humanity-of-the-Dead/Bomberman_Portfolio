using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("爆弾ステータス")]
    [SerializeField, Tooltip("爆発までの時間(秒)")] float explodeTime = 3f;
    [SerializeField, Tooltip("爆発の継続時間(秒)")] float explodeDuration = 3f;
    [SerializeField, Tooltip("爆風の長さ")]         int blastRange = 3;

    [Header("プレハブ")]
    [SerializeField] GameObject blastRenderer;

    #region 変数群
    Transform myTransform;
    GameObject[] blastObject;
    SpriteRenderer[] blastRenderers;
    Transform[] blastTransform;
    Animator[] blastAnimator;
    Vector3 blastRotationNeutral = new Vector3 (0f, 0f, 0f);
    Vector3 blastRotationVertical = new Vector3(0f, 0f, 90f);
    
    float elapsedTime = 0f;
    int phase = 0;
    int blastEndPoint = 0;
    Vector2 blastSettingPos;
    int blastArrayNum = 1;

    // 爆発アニメ用
    string blastAnimState_mid = "blast_mid";
    string blastAnimState_direction = "blast_direction";
    string blastAnimState_end = "blast_end";
    #endregion

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        myTransform = transform;
        blastObject = new GameObject[blastRange * 4 + 1];
        blastRenderers = new SpriteRenderer[blastRange * 4 + 1];
        blastTransform = new Transform[blastRange * 4 + 1];
        blastAnimator = new Animator[blastRange * 4 + 1];

        // 中央 + 爆風の長さ * 四方向
        for (int i = 0; i < blastRange * 4 + 1; i++)
        {
            blastObject[i] = Instantiate(blastRenderer, myTransform.transform);
            blastRenderers[i] = blastObject[i].GetComponent<SpriteRenderer>();
            blastAnimator[i] = blastObject[i].GetComponent<Animator>();
            blastTransform[i] = blastObject[i].transform;
            blastRenderers[i].sprite = null;
            blastObject[i].SetActive(false);
        }
    }

    void OnEnable()
    {
        elapsedTime = 0f;
        phase = 0;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if(phase == 1 && elapsedTime > explodeTime + explodeDuration)
        {
            for(int i = 0;i < blastRange * 4 + 1;i++)
            {
                blastObject[i].SetActive(false);
                blastRenderers[i].sprite = null;
                blastTransform[i].transform.position = myTransform.position;
            }

            GameManager.Instance.StageObjectRemove(myTransform.position);
            this.gameObject.SetActive(false);
            return;
        }

        if (phase == 0 && elapsedTime > explodeTime)
        {
            if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(0);
            BlastSet();
            phase = 1;
        }
    }

    /// <summary>
    /// 爆風を配置
    /// </summary>
    void BlastSet()
    {
        // 中央
        blastObject[0].SetActive(true);
        blastTransform[0].transform.position = myTransform.position;
        blastAnimator[0].Play(blastAnimState_mid);

        blastArrayNum = 1;
        blastEndPoint = 0;

        // 右方向
        for (int i = 0; i < blastRange; i++)
        {
            blastSettingPos.x = myTransform.position.x + i;
            blastSettingPos.y = myTransform.position.y;

            if (!GameManager.Instance.GetStageObject(blastSettingPos, 1))
            {
                blastSettingPos.x += 1;
                blastObject[blastArrayNum].SetActive(true);
                blastTransform[blastArrayNum].transform.position = blastSettingPos;
                blastRenderers[blastArrayNum].flipX = false;
                blastTransform[blastArrayNum].eulerAngles = blastRotationNeutral;
                blastAnimator[blastArrayNum].Play(blastAnimState_direction);
                blastArrayNum++;
            }
            else
            {
                blastSettingPos.x += 1;
                if (blastArrayNum - 1 != 0 && blastEndPoint != blastArrayNum - 1)
                {
                    blastObject[blastArrayNum - 1].SetActive(true);
                    blastRenderers[blastArrayNum - 1].flipX = false;
                    blastTransform[blastArrayNum - 1].eulerAngles = blastRotationNeutral;
                    blastAnimator[blastArrayNum - 1].Play(blastAnimState_end);                    
                }
                blastEndPoint = blastArrayNum - 1;
                GameManager.Instance.StageObjectRemove(blastSettingPos);
                break;
            }
            if (i == blastRange - 1)
            {
                if (blastArrayNum - 1 != 0 && blastEndPoint != blastArrayNum - 1)
                {
                    blastObject[blastArrayNum - 1].SetActive(true);
                    blastRenderers[blastArrayNum - 1].flipX = false;
                    blastTransform[blastArrayNum - 1].eulerAngles = blastRotationNeutral;
                    blastAnimator[blastArrayNum - 1].Play(blastAnimState_end);
                    blastEndPoint = blastArrayNum - 1;
                }
            }
        }

        // 左方向
        for (int i = 0; i < blastRange; i++)
        {
            blastSettingPos.x = myTransform.position.x - i;
            blastSettingPos.y = myTransform.position.y;

            if (!GameManager.Instance.GetStageObject(blastSettingPos, 3))
            {
                blastSettingPos.x -= 1;
                blastObject[blastArrayNum].SetActive(true);
                blastTransform[blastArrayNum].transform.position = blastSettingPos;
                blastRenderers[blastArrayNum].flipX = true;
                blastTransform[blastArrayNum].eulerAngles = blastRotationNeutral;
                blastAnimator[blastArrayNum].Play(blastAnimState_direction);
                blastArrayNum++;
            }
            else
            {
                blastSettingPos.x -= 1;
                if (blastArrayNum - 1 != 0 && blastEndPoint != blastArrayNum - 1)
                {
                    blastObject[blastArrayNum - 1].SetActive(true);
                    blastRenderers[blastArrayNum - 1].flipX = true;
                    blastTransform[blastArrayNum - 1].eulerAngles = blastRotationNeutral;
                    blastAnimator[blastArrayNum - 1].Play(blastAnimState_end);
                }
                blastEndPoint = blastArrayNum - 1;
                GameManager.Instance.StageObjectRemove(blastSettingPos);
                break;
            }
            if (i == blastRange - 1)
            {
                if (blastArrayNum - 1 != 0 && blastEndPoint != blastArrayNum - 1)
                {
                    blastObject[blastArrayNum - 1].SetActive(true);
                    blastRenderers[blastArrayNum - 1].flipX = true;
                    blastTransform[blastArrayNum - 1].eulerAngles = blastRotationNeutral;
                    blastAnimator[blastArrayNum - 1].Play(blastAnimState_end);
                    blastEndPoint = blastArrayNum - 1;
                }
            }
        }

        // 上方向
        for (int i = 0; i < blastRange; i++)
        {
            blastSettingPos.x = myTransform.position.x;
            blastSettingPos.y = myTransform.position.y + i;

            if (!GameManager.Instance.GetStageObject(blastSettingPos, 0))
            { 
                blastSettingPos.y += 1;
                blastObject[blastArrayNum].SetActive(true);
                blastTransform[blastArrayNum].transform.position = blastSettingPos;
                blastRenderers[blastArrayNum].flipX = false;
                blastTransform[blastArrayNum].eulerAngles = blastRotationVertical;
                blastAnimator[blastArrayNum].Play(blastAnimState_direction);
                blastArrayNum++;
            }
            else
            {
                blastSettingPos.y += 1;
                if (blastArrayNum - 1 != 0 && blastEndPoint != blastArrayNum - 1)
                {
                    blastObject[blastArrayNum - 1].SetActive(true);
                    blastRenderers[blastArrayNum - 1].flipX = false;
                    blastTransform[blastArrayNum - 1].eulerAngles = blastRotationVertical;
                    blastAnimator[blastArrayNum - 1].Play(blastAnimState_end);
                }
                blastEndPoint = blastArrayNum - 1;
                GameManager.Instance.StageObjectRemove(blastSettingPos);
                break;
            }
            if (i == blastRange - 1)
            {
                if (blastArrayNum - 1 != 0 && blastEndPoint != blastArrayNum - 1)
                {
                    blastObject[blastArrayNum - 1].SetActive(true);
                    blastRenderers[blastArrayNum - 1].flipX = false;
                    blastTransform[blastArrayNum - 1].eulerAngles = blastRotationVertical;
                    blastAnimator[blastArrayNum - 1].Play(blastAnimState_end);
                    blastEndPoint = blastArrayNum - 1;
                }
            }
        }

        // 下方向
        for (int i = 0; i < blastRange; i++)
        {
            blastSettingPos.x = myTransform.position.x;
            blastSettingPos.y = myTransform.position.y - i;

            if (!GameManager.Instance.GetStageObject(blastSettingPos, 2))
            {
                blastSettingPos.y -= 1;
                blastObject[blastArrayNum].SetActive(true);
                blastTransform[blastArrayNum].transform.position = blastSettingPos;
                blastRenderers[blastArrayNum].flipX = true;
                blastTransform[blastArrayNum].eulerAngles = blastRotationVertical;
                blastAnimator[blastArrayNum].Play(blastAnimState_direction);
                blastArrayNum++;
            }
            else
            {
                blastSettingPos.y -= 1;
                if (blastArrayNum - 1 != 0 && blastEndPoint != blastArrayNum - 1)
                {
                    blastObject[blastArrayNum - 1].SetActive(true);
                    blastRenderers[blastArrayNum - 1].flipX = true;
                    blastTransform[blastArrayNum - 1].eulerAngles = blastRotationVertical;
                    blastAnimator[blastArrayNum - 1].Play(blastAnimState_end);
                }
                blastEndPoint = blastArrayNum - 1;
                GameManager.Instance.StageObjectRemove(blastSettingPos);
                break;
            }
            if(i == blastRange)
            {
                if (blastArrayNum - 1 != 0 && blastEndPoint != blastArrayNum - 1)
                {
                    blastObject[blastArrayNum - 1].SetActive(true);
                    blastRenderers[blastArrayNum - 1].flipX = true;
                    blastTransform[blastArrayNum - 1].eulerAngles = blastRotationVertical;
                    blastAnimator[blastArrayNum - 1].Play(blastAnimState_end);
                    blastEndPoint = blastArrayNum - 1;
                }
            }
        }
    }
}
