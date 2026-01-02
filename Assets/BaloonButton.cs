using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaloonButton : MonoBehaviour
{
    public float Speed;
    public int Value;
    public int TrueValue;
    public TMP_Text BtnTxt;
    private RectTransform rect;
    private bool IsFallen;
    private AudioSource audioSource;
    public Button buttonBaloon;
    private void Start()
    {
       // gameObject.GetComponent<Button>().onClick.AddListener(BaloonSelect);
        rect = GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();
        buttonBaloon = GetComponent<Button>();
       
    }
    private void Update()
    {
        Speed = GameManager.Instance.CurrentBaloonSpeed;
        rect.anchoredPosition += Vector2.down * Speed * Time.deltaTime;
        if (!IsFallen && this.transform.position.y <= GameManager.Instance.DestroyPoint.transform.position.y - 60f)
        {
            IsFallen = true;
            if (Value == GameManager.Instance.value_number)
            {
                GameManager.Instance.DestroyPointControl(false, buttonBaloon);
            }
            else
            {
                GameManager.Instance.DestroyPointControl(true, buttonBaloon);
            }
            Destroy(this.gameObject, 0.3f);
        }
    }
    public void BaloonSelect()
    {
        GameManager.Instance.SelectControl(Value,buttonBaloon);
        Destroy(this.gameObject,0.05f);
    }
}
