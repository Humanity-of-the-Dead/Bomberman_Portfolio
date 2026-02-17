using UnityEngine;

public class SelectUI : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] float uiDistance = 0.9f;
    [SerializeField] bool isChild;
    [SerializeField] float cursorAnimeTime = 1f;
    [SerializeField] float animationMoveDistance = 10f;

    [Header("プレハブ")]
    [SerializeField] GameObject uiCursor;
    [SerializeField] GameObject[] uiObjects;

    [Header("ボタン効果処理スクリプト")]
    [SerializeField] MyButtons buttonsScript;

    RectTransform curserTransform;
    RectTransform[] uisRect;
    Vector2 settingPos;
    Vector2 movementPos;
    public int selectUI = 0;
    public int lastestSelect = 0;
    bool canSelected = true;
    float timeCount = 0;

    // 初期化処理
    void Start()
    {
        selectUI = 0;
        lastestSelect = 0;
        timeCount = 0;
        curserTransform = uiCursor.GetComponent<RectTransform>();
        
        uisRect = new RectTransform[uiObjects.Length];
        for(int i = 0; i < uiObjects.Length; i++)
        {
            uisRect[i] = uiObjects[i].GetComponent<RectTransform>();
        }
        buttonsScript.Init();

        if (isChild)
        {
            this.enabled = false;
            return;
        }

        CursorSet();

        SceneLoader.Ins.SceneChangeEnd += ChangePlayable;
        canSelected = SceneLoader.Ins.CanControl;
    }

    void Update()
    {
        // Debug.Log("SelectUI Update");
        // Debugger.Log(joyCon1P.StickDirection);

        // if (joyCon1P != null)
        if (!canSelected) return;
        if (MyJoyConManager.Ins.joycon1 != null)
        {
            // Debug.Log("62");
            // if (joyCon1P.GetButtonsDown(Joycon.Button.DPAD_RIGHT))
            if (MyJoyConManager.Ins.joycon1.J.GetButtonDown(Joycon.Button.DPAD_UP))
            {
                AudioManager.Ins.PlayOneShotSE(3);
                lastestSelect = selectUI;
                buttonsScript.ButtonSelect(selectUI % uiObjects.Length);
            }

            // Debug.Log("69");
            // Debug.Log(joyCon1P.StickDirection);
            // Debug.Log("Manager"+MyJoyConManager.Ins.joycon1.StickDirection);
            switch ( MyJoyConManager.Ins.joycon1.StickDirection)
            {
                case MyJoyConManager.StickDirection.Up:
                case MyJoyConManager.StickDirection.Down:
                    ChangeSelectedUI( MyJoyConManager.Ins.joycon1.StickDirectionTilt);
                    // ChangeSelectedUI( MyJoyConManager.Ins.joycon1.StickDirection);
                    //Debug.Log("StickDirection Update");
                    break;
                case MyJoyConManager.StickDirection.Left:
                    AudioManager.Ins.PlayOneShotSE(4);
                    buttonsScript.SideChange(selectUI % uiObjects.Length, 0);
                    break;
                case MyJoyConManager.StickDirection.Right:
                    AudioManager.Ins.PlayOneShotSE(4);
                    buttonsScript.SideChange(selectUI % uiObjects.Length, 1);
                    break;
                default:
                    break;
            }
            
            //Debug.Log("カーソル移動");
        }
        else
        {
            //Debug.Log("キーボード");
            // テスト用
            if (Input.GetKeyDown(KeyCode.W))
            {
                ChangeSelectedUI(0);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                ChangeSelectedUI(1);
            }

            if (Input.GetKey(KeyCode.A))
            {
                AudioManager.Ins.PlayOneShotSE(4);
                buttonsScript.SideChange(selectUI % uiObjects.Length, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                AudioManager.Ins.PlayOneShotSE(4);
                buttonsScript.SideChange(selectUI % uiObjects.Length, 1);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                AudioManager.Ins.PlayOneShotSE(3);
                lastestSelect = selectUI;
                buttonsScript.ButtonSelect(selectUI % uiObjects.Length);
            }
        }

        CurosorAnimation();
    }

    void CurosorAnimation()
    {
        timeCount += Time.deltaTime;
        if (timeCount < cursorAnimeTime * 0.5f)
        {
            curserTransform.position = movementPos;
        }
        else if(timeCount < cursorAnimeTime)
        {
            curserTransform.position = settingPos;
        }
        else timeCount = 0;
    }

    void ChangePlayable() { canSelected = true; }

    /// <summary>
    /// UI用カーソルを最後に選択したUIにセット
    /// </summary>
    public void CursorSet()
    {
        settingPos = uisRect[lastestSelect % uiObjects.Length].position;
        settingPos.x -= uisRect[lastestSelect % uiObjects.Length].rect.width - uisRect[lastestSelect % uiObjects.Length].rect.width * uiDistance;
        curserTransform.position = settingPos;
        movementPos = settingPos;
        movementPos.x -= animationMoveDistance;
    }

    /// <summary>
    /// 与えられた数値により選択中のUIを切り替える
    /// 0 = 前のUI, 1 = 次のUI
    /// </summary>
    /// <param name="_dir"></param>
    void ChangeSelectedUI(int _dir)
    {
        switch (_dir)
        {
            case 0:
                selectUI--;
                if (selectUI < 0) selectUI = uiObjects.Length - 1;
                settingPos = uisRect[selectUI % uiObjects.Length].position;
                settingPos.x -= uisRect[selectUI % uiObjects.Length].rect.width - uisRect[selectUI % uiObjects.Length].rect.width * uiDistance;
                curserTransform.position = settingPos;
                movementPos = settingPos;
                movementPos.x -= animationMoveDistance;

                AudioManager.Ins.PlayOneShotSE(4);
                break;
            case 1:
                selectUI++;
                settingPos = uisRect[selectUI % uiObjects.Length].position;
                settingPos.x -= uisRect[selectUI % uiObjects.Length].rect.width - uisRect[selectUI % uiObjects.Length].rect.width * uiDistance;
                curserTransform.position = settingPos;
                movementPos = settingPos;
                movementPos.x -= animationMoveDistance;

                AudioManager.Ins.PlayOneShotSE(4);
                break;
            default:
                Debug.Log("有効でない数字を受け取りました");
                break;
        }
    }

    /// <summary>
    /// 与えられた引数により選択中のUIを切り替える
    /// Stick.Down = 前のUI, Stick.Down = 次のUI
    /// </summary>
    /// <param name="_stickDirection"></param>
    void ChangeSelectedUI(MyJoyConManager.StickDirection _stickDirection){
        switch (_stickDirection){
            case MyJoyConManager.StickDirection.Up:
                selectUI--;
                if (selectUI < 0) selectUI = uiObjects.Length - 1;
                settingPos = uisRect[selectUI % uiObjects.Length].position;
                settingPos.x -= uisRect[selectUI % uiObjects.Length].rect.width - uisRect[selectUI % uiObjects.Length].rect.width * uiDistance;
                curserTransform.position = settingPos;
                movementPos = settingPos;
                movementPos.x -= animationMoveDistance;

                AudioManager.Ins.PlayOneShotSE(4);
                
                break;
            case MyJoyConManager.StickDirection.Down:
                selectUI++;
                settingPos = uisRect[selectUI % uiObjects.Length].position;
                settingPos.x -= uisRect[selectUI % uiObjects.Length].rect.width - uisRect[selectUI % uiObjects.Length].rect.width * uiDistance;
                curserTransform.position = settingPos;
                movementPos = settingPos;
                movementPos.x -= animationMoveDistance;

                AudioManager.Ins.PlayOneShotSE(4);
                break;
            default:
                Debug.Log("有効でない引数を受け取りました");
                break;
        }
    }
}
