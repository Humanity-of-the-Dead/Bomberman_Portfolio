public class ResultManager : PersistentSingleton<ResultManager>
{
    int winner = 3;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 勝者をセットする関数
    /// -1 = ドロー, 0 = 1P, 1 = 2P
    /// </summary>
    /// <param name="_winner"></param>
    public void WinnerDicade(int _winner)
    {
        winner = _winner;
    }

    public int GetWinner()
    {
        return winner;
    }
}
