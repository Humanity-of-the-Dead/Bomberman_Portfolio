using System;
using UnityEngine;
using UnityEngine.UI;

public enum UIType
{
    None = 0,
    SceneChange,
    UIOpen,
    UIClose,
    SliderValueChange,
}

[Serializable]
public struct UIInfo
{
    [SerializeField] UIType uiType;
    [SerializeField] string uiEffectData;
    [SerializeField] GameObject uiEffectTarget;

    public UIType GetUIType() { return uiType; }
    public string GetUIEffectData() { return uiEffectData; }
    public GameObject GetUIEffectTarget() { return uiEffectTarget; }
}

public class MyButtons : MonoBehaviour
{
    [SerializeField] UIInfo[] buttonsInfo;

    SelectUI mySelectUI;
    UIEffect[] buttons;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Init()
    {
        mySelectUI = this.gameObject.GetComponent<SelectUI>();
        buttons = new UIEffect[buttonsInfo.Length];

        for (int i = 0; i < buttonsInfo.Length; i++)
        {
            //Debug.Log(i);
            switch (buttonsInfo[i].GetUIType())
            {
                case UIType.SceneChange:
                    buttons[i] = new SceneChange(buttonsInfo[i].GetUIEffectData());
                    break;
                case UIType.UIOpen:
                    buttons[i] = new UIOpen(buttonsInfo[i].GetUIEffectTarget(), mySelectUI);
                    break;
                case UIType.UIClose:
                    buttons[i] = new UIClose(buttonsInfo[i].GetUIEffectTarget(), mySelectUI);
                    break;
                case UIType.SliderValueChange:
                    buttons[i] = new SliderValueChange(buttonsInfo[i].GetUIEffectTarget());
                    break;
                case UIType.None:
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// ボタン選択時のイベント
    /// </summary>
    /// <param name="_buttonNum"></param>
    public void ButtonSelect(int _buttonNum)
    {
        if (buttonsInfo[_buttonNum].GetUIType() != UIType.SliderValueChange)
            buttons[_buttonNum].OnSelect();
    }

    /// <summary>
    /// 左右ボタン選択時のイベント
    /// </summary>
    /// <param name="_buttonNum"></param>
    /// <param name="_dir"></param>
    public void SideChange(int _buttonNum, int _dir)
    {
        if (buttonsInfo[_buttonNum].GetUIType() == UIType.SliderValueChange)
        buttons[_buttonNum].OnSideChange(_dir);
    }
}

/// <summary>
/// ボタンイベントの基底クラス
/// </summary>
public abstract class UIEffect
{
    /// <summary>
    /// 決定ボタンを押されたとき
    /// </summary>
    public virtual void OnSelect() { }

    /// <summary>
    /// 左右ボタンを押されたとき
    /// </summary>
    /// <param name="_dir"></param>
    public virtual void OnSideChange(int _dir) { }
}

/// <summary>
/// ボタンエフェクト : スライダーの値変更
/// </summary>
public class SliderValueChange : UIEffect
{
    Slider mySlider;
    // スライダーの値を一度に変える量
    float sliderValueChangeAmount = 0.001f;

    public SliderValueChange(GameObject _targetObject)
    {
        mySlider = _targetObject.GetComponent<Slider>();
    }

    public override void OnSideChange(int _dir)
    {
        if(_dir == 0)
        {
            mySlider.value -= sliderValueChangeAmount;
        }
        else if(_dir == 1)
        {
            mySlider.value += sliderValueChangeAmount;
        }
    }
}

/// <summary>
/// ボタンエフェクト : UIオープン クラス
/// </summary>
public class UIOpen : UIEffect
{
    SelectUI parentScript;
    SelectUI targetScrpt;
    Transform targetObjectTransform;
    Vector3 summonUIPos = Vector3.zero;

    public UIOpen(GameObject _targetObject, SelectUI _parentScript)
    {
        targetObjectTransform = _targetObject.transform;
        targetScrpt = _targetObject.GetComponent<SelectUI>();
        parentScript = _parentScript;
    }

    public override void OnSelect()
    {
        parentScript.enabled = false;
        targetScrpt.enabled = true;
        targetObjectTransform.localPosition = summonUIPos;
        targetScrpt.CursorSet();
    }
}

/// <summary>
/// ボタンエフェクト : UIクローズ クラス
/// </summary>
public class UIClose : UIEffect
{
    SelectUI parentScript;
    SelectUI targetScrpt;
    Transform myObjectTransform;
    Vector3 targetOriginPos;

    public UIClose(GameObject _targetObject, SelectUI _parentScript)
    {
        myObjectTransform = _parentScript.gameObject.transform;
        targetOriginPos = myObjectTransform.localPosition;
        targetScrpt = _targetObject.GetComponent<SelectUI>();
        parentScript = _parentScript;
    }

    public override void OnSelect()
    {
        parentScript.enabled = false;
        targetScrpt.enabled = true;
        myObjectTransform.localPosition = targetOriginPos;
        targetScrpt.CursorSet();
    }
}

/// <summary>
/// ボタンイベント: シーンチェンジ クラス
/// </summary>
public class SceneChange : UIEffect
{
    string loadSceneName = string.Empty;

    /// <summary>
    /// コンストラクタ: チェンジ先のシーン
    /// </summary>
    /// <param name="_loadSceneName"></param>
    public SceneChange(string _loadSceneName)
    {
        this.loadSceneName = _loadSceneName;
    }

    /// <summary>
    /// イベント発火時のイベント
    /// </summary>
    public override void OnSelect()
    {
        // SceneManager.LoadScene(loadSceneName, LoadSceneMode.Single);
        SceneLoader.Ins.ActivateScene(loadSceneName);
    }
}