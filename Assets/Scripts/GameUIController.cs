using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] GameObject joystickParent = null;
    [SerializeField] GameObject buttonParent = null;
    [SerializeField] GameObject healthUI = null;

    [Header("HealthBar Colors")]
    [SerializeField] Color healthyColor = Color.green;
    [SerializeField] Color hurtColor = new Color(1, .647f, 0);
    [SerializeField] Color dyingColor = Color.red;

    Button skillButton1 = null;
    Button skillButton2 = null;

    Slider healthBarUI = null;
    Image fillImage = null;

    Image skillCooldownImg1 = null;
    Image skillCooldownImg2 = null;

    PlayerCombat playerInfo = null;

    Coroutine updatingHealthBar = null;

    // Start is called before the first frame update
    void Start()
    {
        skillButton1 = buttonParent.transform.GetChild(1).GetComponent<Button>();
        skillButton2 = buttonParent.transform.GetChild(2).GetComponent<Button>();

        skillCooldownImg1 = skillButton1.transform.GetChild(0).GetComponent<Image>();
        skillCooldownImg2 = skillButton2.transform.GetChild(0).GetComponent<Image>();

        skillCooldownImg1.fillAmount = 0f;
        skillCooldownImg2.fillAmount = 0f;

        healthBarUI = healthUI.GetComponent<Slider>();
        fillImage = healthUI.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();

        playerInfo = GameObject.Find("Player").GetComponent<PlayerCombat>();

        playerInfo.onDamageTakenCallback += UpdateHealthBarUI;
        playerInfo.onCooldownCallback += SetCooldownImg;

        healthBarUI.maxValue = playerInfo.totalHealth;
        healthBarUI.value = healthBarUI.maxValue;
        healthBarUI.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateHealthBarUI(int newValue)
    {
        if (newValue == healthBarUI.value)
        {
            return;
        }

        if (updatingHealthBar != null)
        {
            StopCoroutine(updatingHealthBar);
        }
        updatingHealthBar = StartCoroutine(TransitionHealthValue(newValue));
    }

    void SetCooldownImg(int skillIndex, float fillAmt)
    {
        switch(skillIndex)
        {
            case 0:
                skillCooldownImg1.fillAmount = fillAmt;
                return;
            case 1:
                skillCooldownImg2.fillAmount = fillAmt;
                return;
            default:
                break;
        }
        Debug.LogWarning("Incorrect Skill Index");
    }

    IEnumerator TransitionHealthValue(int newValue)
    {
        float timer = 0f;
        float oldValue = healthBarUI.value;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            healthBarUI.value = Mathf.Lerp(oldValue, newValue, timer);
            fillImage.color = ColorLerpBetween3(dyingColor, hurtColor, healthyColor, playerInfo.totalHealth, healthBarUI.value);
            yield return null;
        }
        healthBarUI.value = newValue;
        fillImage.color = ColorLerpBetween3(dyingColor, hurtColor, healthyColor, playerInfo.totalHealth, healthBarUI.value);

        updatingHealthBar = null;
    }

    // Lerp Between 3 Colors: a @ current = 0f, b @ current = 0.5f, c @ current = 1f
    Color ColorLerpBetween3(Color a, Color b, Color c, int maxValue, float current)
    {
        float halfValue = maxValue / 2f;
        float interpolationValue = (current < halfValue) ? (current / halfValue) : (current - halfValue) / halfValue;
        return (current < halfValue) ? Color.Lerp(a, b, interpolationValue) : Color.Lerp(b, c, interpolationValue);
    }
}
