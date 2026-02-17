using System;
using UnityEngine;

/// <summary>
/// Joy-Conの入力の取得などをする関数
/// </summary>
public class MyJoyConManager:PersistentSingleton<MyJoyConManager>{
    [SerializeField]float _stickDeadZone=0.3f;

    class JoyCon:IJoyCon{
        public Joycon J{get;private set;}
        public float[] Stick{get;private set;}
        public StickDirection StickDirection{get;set;}
        public StickDirection StickDirectionTilt{get;set;}
        public StickDirection PreviousStickDirection{get;set;}

        public readonly bool[] isButtonsDown=new bool[13];

        //コンストラクタ
        public JoyCon(Joycon j){
            J=j;
            Stick=new float[2];
            StickDirection=StickDirection.Neutral;
            StickDirectionTilt=StickDirection.Neutral;
            PreviousStickDirection=StickDirection.Neutral;
        }
        
        public bool GetButtonsDown(Joycon.Button b){
            return isButtonsDown[(int)b];
        }
    }
    
    public interface IJoyCon{
        Joycon J{get;}
        float[] Stick{get;}//右:0,上:1
        StickDirection StickDirection{get;}//スティックの傾けてる方向
        StickDirection StickDirectionTilt{get;}//スティックを傾けた方向

        bool GetButtonsDown(Joycon.Button b);
    }

    public enum StickDirection{
        Neutral,
        Up,
        Down,
        Left,
        Right
    }

    //Joy-Conの1つ目と2つ目
    //インターフェース
    public IJoyCon joycon1;
    public IJoyCon joycon2;
    
    JoyCon _joycon1;
    JoyCon _joycon2;

    Joycon.Button[] _buttonCache;//ボタンのEnum値をキャッシュする配列

    // public void Init(){
    void Start(){
        var js=JoyconManager.Instance.j;//全てのJoy-Conを取得
        //Joy-Conの1つ目と2つ目を取得
        _joycon1=new JoyCon(js[0]);
        _joycon2=new JoyCon(js[1]);
        //公開用のプロパティに、内部用変数を設定
        joycon1=_joycon1;
        joycon2=_joycon2;

        //ボタンのEnum値をあらかじめキャッシュする
        _buttonCache=new Joycon.Button[13];
        int[] tempRemap=new int[]{ 1,3,0,2 };//横持ち用リマップ
        for (int i=0; i<4; i++)
            _buttonCache[i]=(Joycon.Button)tempRemap[i];
        for (int i=4; i<13; i++) _buttonCache[i]=(Joycon.Button)i;
    }

    void Update(){
        if (Ins._joycon1!=null){
            ConvertStickForSideways();
            UpdateStickDirection();
            UpdateStickDirectionTilt();
            UpdateButtonsDown();
        }
    }

    /// <summary>
    /// MyJoyConManagerが管理する、全てのJoy-Conのボタン押下状態を更新
    /// </summary>
    void UpdateButtonsDown(){
        _joycon1=CalculateButtonsDown(_joycon1);
        _joycon2=CalculateButtonsDown(_joycon2);
    }

    /// <summary>
    /// 指定されたJoyConオブジェクトの、 全ボタンの押下状態(を更新<br/>
    /// 方向ボタンは横持ち用にリマップする
    /// </summary>
    /// <param name="j">更新されたJoyConオブジェクト</param>
    /// <returns></returns>
    JoyCon CalculateButtonsDown(JoyCon j){
        for (int i=0; i<13; i++){
            j.isButtonsDown[i] = j.J.GetButtonDown(_buttonCache[i]);
        }
        return j;
    }

    /// <summary>
    /// スティックの倒した瞬間の方向
    /// </summary>
    void UpdateStickDirectionTilt(){
        //前のステックのニュートラルのとき今の方向を代入
        //ニュートラルでないときニュートラル
        _joycon1.StickDirectionTilt
            =_joycon1.PreviousStickDirection==StickDirection.Neutral?
                _joycon1.StickDirection:StickDirection.Neutral;
        
        //前の方向を更新
        _joycon1.PreviousStickDirection=_joycon1.StickDirection;
        
        _joycon2.StickDirectionTilt
            =_joycon2.PreviousStickDirection==StickDirection.Neutral?
                _joycon2.StickDirection:StickDirection.Neutral;
        
        //前の方向を更新
        _joycon2.PreviousStickDirection=_joycon2.StickDirection;
    }

    /// <summary>
    /// スティックの方向を更新
    /// </summary>
    void UpdateStickDirection(){
        _joycon1.StickDirection=CalculateStickDirection(joycon1.Stick);
        _joycon2.StickDirection=CalculateStickDirection(joycon2.Stick);
    }

    /// <summary>
    /// スティックの方向を判定
    /// </summary>
    /// <param name="stickValues"></param>
    /// <returns></returns>
    StickDirection CalculateStickDirection(float[] stickValues){
        // Debugger.Log("CalculateStickDirection");
        
        var v=stickValues[1];
        var h=stickValues[0];

        // 縦の入力が横の入力より大きいとき
        if(Mathf.Abs(v)>=Mathf.Abs(h)){
            if(Mathf.Abs(v)>_stickDeadZone){
                return (v>0)?StickDirection.Up:StickDirection.Down;
            }
        }
        else// 横の入力が縦の入力より大きいとき
        {
            if(Mathf.Abs(h)>_stickDeadZone){
                // stick[1]は左が正の値と仮定
                return (h>0)?StickDirection.Right:StickDirection.Left;
            }
        }
        return StickDirection.Neutral;
    }


    /// <summary>
    /// 取得したスティックの値を横持ち用に変換
    /// </summary>
    void ConvertStickForSideways(){
        // Debugger.Log ("ConvertStickForSideways");
        
        float[] rawStick1=joycon1.J.GetStick();
        float[] rawStick2=joycon2.J.GetStick();

        // Debugger.Log(rawStick1[0]+","+rawStick1[1]);

        if(joycon1.J.isLeft){
            joycon1.Stick[0]=-rawStick1[1];
            joycon1.Stick[1]=rawStick1[0];
        }
        else{
            joycon1.Stick[0]=rawStick1[1];
            joycon1.Stick[1]=-rawStick1[0];
        }

        if(joycon2.J.isLeft){
            joycon2.Stick[0]=-rawStick2[1];
            joycon2.Stick[1]=rawStick2[0];
        }
        else{
            joycon2.Stick[0]=rawStick2[1];
            joycon2.Stick[1]=-rawStick2[0];
        }

        // Debugger.Log("J1"+Joycon1.stick[0]+","+Joycon1.stick[1]);
        // Debugger.Log("J2"+Joycon2.stick[0]+","+Joycon2.stick[1]);

        UpdateStickDirection();
    }
}