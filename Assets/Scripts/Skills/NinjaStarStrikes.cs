using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowStars", menuName = "Skills/ThrowingStars")]
public class NinjaStarStrikes : Skill
{
    [SerializeField] GameObject targetUIPrefab;
    [SerializeField] int dmg = 1;
    GameObject canvasUI;

    public override void ActivateSkill()
    {
        if (canvasUI == null)
            canvasUI = GameObject.Find("Canvas");
        GameController.instance.SlowTime(_slowDuration, _slowPercent, true);
        CreateUITargets();
    }

    public override void DeactivateSkill()
    {
        
    }

    public void CreateUITargets()
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag("Enemy");

        if (candidates.Length <= 0)
        {
            Debug.Log("No Enemies");
            return;
        }

        for (int i = 0; i < candidates.Length; ++i)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(candidates[i].transform.position);
            Debug.Log($"ScreenPos: {screenPos}");
            if (screenPos.x > 0 && screenPos.x <= Screen.width && screenPos.y > 0 && screenPos.y <= Screen.height)
            {   
                TargettingButton targetUI = Instantiate(targetUIPrefab, canvasUI.transform).GetComponent<TargettingButton>();
                targetUI.gameObject.GetComponent<RectTransform>().position = screenPos;
                targetUI.InitializeUI(this, candidates[i].GetComponentInChildren<Enemy>(), _skillDuration, dmg);
            }
            else
            {
                Debug.Log("Not on screen");
            }
        }
    }
}
