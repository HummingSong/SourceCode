using UnityEngine;
using Assets.Scripts.Cores;
using Assets.Scripts.Utility;
using Assets.Scripts.InGame.Mode;

public class ComboUI : MonoBehaviour {

    public UISprite uiTimeGage;
    public UISprite[] uiComboCount;
    //public UILabel uiComboMsg;

    public float fActiveTime = 5.0f;

    public float fCheckTime = 0.0f;
    // 콤보에 쓸 임시 변수들 ( 후에 인게임 데이터로 받아온다 )
    private int iComboCnt = 0;


    public int ComboCount { get { return iComboCnt; } }


    void Awake()
    {
        fCheckTime = fActiveTime;
    }

    void Update()
    {
        fCheckTime -= Time.deltaTime;
        if (fCheckTime < 0)
            Deactive();

        uiTimeGage.fillAmount = fCheckTime / fActiveTime;
    }

    public void Activate()
    {
        iComboCnt += 1;
        fCheckTime = fActiveTime;

        var sfmg = Core.Presenter.Get<SFManager>();

        sfmg.SetImageFont(uiComboCount, iComboCnt, EImageFontSize.Combo);
    }

    public void Deactive()
    {
		// [ypqp35 2016/04/19] 플레이점수
		Util.SetScore( null, GameScore.ScoreType.Combo, iComboCnt );

		// 초기화
		iComboCnt = 0;
        gameObject.SetActive(false);
    }

    public string CheckComboMsg(int combo)
    {
        switch (combo)
        {
            case 6:
                return "Good";
            case 9:
                return "Nice";
            case 11:
                return "Cool";
            case 14:
                return "Great";
        }

        if (combo > 16)
            return "God";

        return "";
    }

}
