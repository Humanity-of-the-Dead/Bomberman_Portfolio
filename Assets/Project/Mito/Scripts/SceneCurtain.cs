using UnityEngine;
using UnityEngine.UI;

public class SceneCurtain : MonoBehaviour 
{
    [Header("ˆÃ“]/–¾“]‚Ì•b”")]
    [SerializeField] float changeSpeed = 1;
    float changeAmount = 0;
    Image curtain;
    Color black = Color.black;
    Color changeColor = Color.clear;

    /// <summary>
    /// ‰Šú‰»ˆ—
    /// </summary>
    public void Init()
    {
        changeAmount = changeSpeed * 60;
        changeColor.a = black.a / changeAmount;
        curtain = this.gameObject.GetComponentInChildren<Image>();
    }

    /// <summary>
    /// ˆÃ“]ŠJn‚©‚çI—¹‚Ü‚Å
    /// </summary>
    /// <returns></returns>
    public async Awaitable CurtainClose()
    {
        while (true)
        {
            curtain.color += changeColor;
            await Awaitable.WaitForSecondsAsync(changeColor.a);
            if (curtain.color.a >= black.a) return;
        }
    }

    /// <summary>
    /// –¾“]ŠJn‚©‚çI—¹‚Ü‚Å
    /// </summary>
    /// <returns></returns>
    public async Awaitable CurtainOpen()
    {
        while (true)
        {
            curtain.color -= changeColor;
            await Awaitable.WaitForSecondsAsync(changeColor.a);
            if (curtain.color.a <= 0) return;
        }
    }
}
