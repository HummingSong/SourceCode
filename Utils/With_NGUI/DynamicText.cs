/// <summary>
/// DynamicText
/// 
/// 작성자 : 배정욱
/// 작성일 : 2015.2.
/// 
/// 가지고 있는 UI Text 연출을 관리하는 스크립트
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class DynamicText : MonoBehaviour
{
    public enum DamageType : int
    {
        Nomal,
        Critical,
        WeakPoint,

        None,
    }

    // 철진
    [System.Serializable]
    public struct DynamicTextSet
    {
        public DamageType dType;
        public Color cColor;
        public Color cOutlineColor;
        public int iSize;

        public bool bOutline;
    }

    public DynamicTextSet[] TextType;

    public float fOffsetHeight = 10.0f;
    public float fDestroyTime = 1.0f;
    public float fVariable = 10.0f;
    private float fGravity = 10.0f;

    private float fElapsedTime = 0.0f;
    private float fAddTime = 0.0f;
    private float fAngle = 0.0f;
    private float fVelocity = 0.0f;
    private float fTargetHeight = 0.0f;
    private float fOffset = 0.0f;

    private Vector3 vTargetPosition = Vector3.zero;
    private Vector3 vOriginPosition = Vector3.zero;

    private bool bReady = false;
    private bool bCritical = false;

    public UILabel uiText;

    void Awake()
    {
        if (!bReady)
        {
            if (gameObject.GetComponent<UILabel>() != null)
            {
                gameObject.GetComponent<UILabel>().enabled = false;
            }
        }

        fTargetHeight = Screen.height * 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bReady)
            return;

        fElapsedTime = Time.deltaTime;

        CoordinateUpdate();

        MoveUpdate();

        fDestroyTime -= fElapsedTime;

        if (fDestroyTime < 0)
            Destroy(gameObject);
    }

    public void SetInfoForReady( Vector3 pos, float damage, bool critical, bool weakPoint, int cnt, bool bMyDamage )
    {
        vTargetPosition = pos;
        vOriginPosition = vTargetPosition;
        DamageType damageType = critical ? DamageType.Critical : weakPoint ? DamageType.WeakPoint : DamageType.Nomal;

        if (Camera.main == null)
            return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(vTargetPosition);
        screenPos = DynamicUI.instance.ConvertPosToUI(screenPos);

        screenPos.y += fOffsetHeight;

        transform.localPosition = screenPos;

        switch(damageType)
        {
            case DamageType.Critical :
                fAngle = 90.0f;
                break;

            case DamageType.Nomal :
            case DamageType.WeakPoint :
                {
                    if (cnt % 2 == 0)
                        fAngle = Random.Range(70, 85);
                    else
                        fAngle = Random.Range(95, 110);
                }
                break;
        }

        bReady = true;

		var uiDamageText = gameObject.GetComponent<UILabel>();

		if( uiDamageText != null )
		{
			uiDamageText.fontSize = TextType[(int)damageType].iSize;
			uiDamageText.enabled = true;
			uiDamageText.text = ((int)damage).ToString();
			uiDamageText.color = TextType[(int)damageType].cColor;

			if( TextType[(int)damageType].bOutline )
			{
				uiDamageText.effectStyle = UILabel.Effect.Outline;
				uiDamageText.effectColor = TextType[(int)damageType].cOutlineColor;
			}
			else
			{
				uiDamageText.effectStyle = UILabel.Effect.None;
			}
        }
    }

    // WORLD -> SCREEN -> NGUI 좌표로 변환
    public void CoordinateUpdate()
    {
        if (Camera.main == null)
            return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(vTargetPosition);

        screenPos = DynamicUI.instance.ConvertPosToUI(screenPos);

        screenPos.y += fOffsetHeight;

        transform.localPosition = screenPos;
    }

    public void MoveUpdate()
    {
        if (Camera.main == null)
            return;

        fAddTime += fElapsedTime;

        Vector3 movePos = Vector3.zero;

        if (!bCritical)
        {
            movePos.x = 400.0f * Mathf.Cos(fAngle * Mathf.Deg2Rad) * fAddTime;
            movePos.y = 400.0f * Mathf.Sin(fAngle * Mathf.Deg2Rad) * fAddTime - 0.5f * fGravity * fAddTime * fAddTime * fVariable;

            transform.localPosition += movePos;
        }
        else
        {
            fVelocity = (fTargetHeight - fOffset) / fDestroyTime;
            fOffset += fVelocity * fAddTime;
            movePos.y = fOffset;
            transform.localPosition += movePos;
        }
    }
}
