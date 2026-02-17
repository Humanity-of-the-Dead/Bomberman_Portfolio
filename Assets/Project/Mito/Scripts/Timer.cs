using System;
using System.Text;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("タイマー用テキスト")]
    [SerializeField] TextMeshProUGUI timerText;
    [Header("テスト用 時間減少速度")]
    [SerializeField] float timeSpeed = 1f;

    public Action PressureTime;

    float gameTime = 0;
    int timeMinute = 0;
    int timeSecond = 0;
    bool timerStop = false;

    const float MINUTE = 60;
    
    StringBuilder timeTextArray = new StringBuilder();

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="_setTime"></param>
    public void Init(float _setTime)
    {
        gameTime = _setTime + 1;
    }

    /// <summary>
    /// タイマーを止める
    /// </summary>
    public void TimeStop()
    {
        timerStop = true;
    }

    /// <summary>
    /// タイマーのUpdate
    /// </summary>
    public void TimeUpdate()
    {
        if(timerStop) return;
        if (gameTime <= 0)
        {
            GameManager.Instance.GameSet(-1);
            timerStop = true;
            return;
        }
        //if ((int)gameTime == (int)pressureTime)
        //{
        //    if (!placePressureBlock)
        //    {
        //        PressureTime.Invoke();
        //        placePressureBlock = true;
        //    }
        //}

        timeMinute = (int)(gameTime / MINUTE);
        timeSecond = (int)(gameTime % MINUTE);

        timeTextArray.Clear();
        timeTextArray.Append($"{timeMinute}:");
        timeTextArray.AppendFormat("{0:00}", timeSecond);

        gameTime -= Time.deltaTime * timeSpeed;

        timerText.text = timeTextArray.ToString();
    }
}
