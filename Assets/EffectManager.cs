using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;
    [SerializeField] Image flashingImage;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void FlashingEffect(Color color,float effectDuration,float effectDelay)
    {
        if (flashingImage != null)
        {
            StartCoroutine(FlashingStart(effectDuration,effectDelay,color));
        }
    }
    private IEnumerator FlashingStart(float duration,float delay,Color clr)
    {
        flashingImage.gameObject.SetActive(true);
        Color imageClr = clr;
        imageClr.a = 0f;
        float alpha = imageClr.a;
        float t = 0f;
        float defaultA = flashingImage.color.a;
        while(t <duration)
        {
            t += 1f;
            alpha += 0.6f;
            imageClr.a = alpha;
            yield return new WaitForSeconds(delay);
            alpha -= 0.2f;
            imageClr.a = alpha;
            if (t == duration)
            {
                flashingImage.gameObject.SetActive(false);
                flashingImage.color = new Color(0f, 0f, 0f, 0f);
            }
        }

    }
}
