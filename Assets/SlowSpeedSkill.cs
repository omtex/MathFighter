using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TMPro;
public class SlowSpeedSkill : MonoBehaviour
{
    public float SlowSpeedValue;
    public float SlowTime;
    public int SlowSkillCost;

    public bool SkillActive = false;
    public AudioSource aus;
    public AudioClip slowedSound;
    public AudioClip slowUpSound;

    public Slider SlowTimeSlider;
    public TMP_Text TimerText;
    public GameObject SlowedPanel;

    public Button SlowButton;
    private void Start()
    {
        aus = GetComponent<AudioSource>();
    }
    public void StartSkill()
    {
        if (!SkillActive && GameManager.Instance.Gold >= SlowSkillCost)
        {
            aus.PlayOneShot(slowedSound);
            SkillActive = true;
            SlowButton.interactable = !SkillActive;
            GameManager.Instance.SetGold(-SlowSkillCost);
            StartCoroutine(SlowSkill());
            
        }
    }
    private IEnumerator SlowSkill()
    {
        int crnt_time = Convert.ToInt32(SlowTime);
        float oldSpeed = GameManager.Instance.CurrentBaloonSpeed;
        GameManager.Instance.CurrentBaloonSpeed = SlowSpeedValue;
        TimerText.gameObject.SetActive(true);
        GameManager.Instance.currentBaloonTime += 0.5f;

        TimerText.text = "Time : " + crnt_time.ToString();

        for (int i = 0; i <= Convert.ToInt32(SlowTime); i++)
        {
            yield return new WaitForSeconds(1f);
            crnt_time--;
            TimerText.text = "Time : " + crnt_time.ToString();
            if (crnt_time <= 0)
            {
                TimerText.gameObject.SetActive(false);

                GameManager.Instance.CurrentBaloonSpeed = oldSpeed;
                //SlowedPanel.SetActive(false);
                SkillActive = false;
                SlowButton.interactable = !SkillActive;
                GameManager.Instance.currentBaloonTime -= 0.5f;
                aus.PlayOneShot(slowUpSound);
                break;
            }

        }



    }
}
