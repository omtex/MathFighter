using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum DifficultyGame
{
    None,
    Easy,
    Normal,
    Hard,
    HardPlus
}
[System.Serializable]
public class GameMode
{
    public int HighScore;
    public DifficultyGame GameDifficulty;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Canvas Canv_s;
    public List<GameMode> Modes;
    public TMP_Text ValueTxt;
    public TMP_Text PointText;
    [SerializeField] TMP_Text HighPointText;
    public TMP_Text HealthText;
    public TMP_Text GoldText;

    public int value_number;
    private int Value1, Value2;

    public bool IsGameRun = false;
    public bool IsGamePaused;
    public Coroutine SpawnBaloonCoroutine;

    public Button PrefabBalonn;
    public GameObject ParentBalon;
    public GameObject[] SpawnPoints;
    private int CorrectCount = 0;
    public GameObject DestroyPoint;

    [SerializeField] List<GameObject> SpawnedBaloons = new List<GameObject>();

    public AudioClip NegativeScoreSound;
    public AudioClip PozitiveScoreSound;
    public AudioClip ChangeNumberSound;
    public AudioClip LoseGameSound;
    public AudioClip ComboSound;
    public AudioClip ButtonClickSound;
    public AudioClip CoinWinSound;
    public AudioClip TimerSound;
    public AudioClip GameStartSound;
    public AudioClip HighScoredSound;


    private AudioSource Audio_Source;
    private int nextTreshold = 100;

    private int MaxNumber = 10;
    private int MinNumber = 2;

    public float DefaultBalloonSpeed;
    public float CurrentBaloonSpeed;
    public float BaloonSpeedIncrease;
    private float currentBaloonSpeedIncrease;
    public float BalloonTime;
    public float currentBaloonTime;

    [SerializeField] float StartingDuration;

    public int Health;
    private int curren_health;
    public int gamePoints;
    public int highPoints;
    public bool IsSetHighPoint;
    public int Gold;
    [SerializeField] int getGoldCount;
    private int goldMultiplier;

    public GameObject LoseGamePanel;
    public GameObject ComboPanel;

    [SerializeField] TMP_Text InfoText; // genel anlarda ekrana çýkan yazý
    [SerializeField] GameObject InfoTxtSpawnPoint;
    [SerializeField] TMP_Text LoseText;
    [SerializeField] TMP_Text StartTimeText;

    private int combo_count = 0;
    [SerializeField] int minComboCount;
    [SerializeField] int maxCorrectCount;

    public GameObject StartingPanel;
    public GameObject DifficultPanel;

    public DifficultyGame GameDiff;

    private void Awake()
    {
        // Singleton ayarý
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        StartPanelGame(); // Main Menu(Ýleride farklý sahneye aktarýlabilir)
        GetHighScore();
        Audio_Source = GetComponent<AudioSource>();   
    }
    public void StartGame() // Oyun baþlatma
    {
        StartCoroutine(StartingTimer());
    }
    private IEnumerator StartingTimer() // Oyun baþlamadan önce süre sayýlýr
    {
        int duration = Convert.ToInt32(StartingDuration);
        StartTimeText.gameObject.SetActive(true);
        for (int i = 0; i <= Convert.ToInt32(StartingDuration); i++)
        {
            StartTimeText.text = duration.ToString();
            Audio_Source.PlayOneShot(TimerSound);
            yield return new WaitForSeconds(1f);
            duration--;
            if (duration <= 0)
            {
                StartTimeText.gameObject.SetActive(false);
                IsGameRun = true;
                curren_health = Health;

                PointText.text = "Point : " + gamePoints.ToString();
                HealthText.text = "Health : " + curren_health.ToString();
                GoldText.text = "Gold : " + Gold.ToString();
                Audio_Source.PlayOneShot(GameStartSound);
                SetValue();
                StartBaloonSpawn();
                break;
            }
        }
    }
    private void StartPanelGame()
    {
        if (IsGameRun == false)
        {
            StartingPanel.SetActive(true);
        }
    }
    public void StartBaloonSpawn()
    {
        if (SpawnBaloonCoroutine == null)
        {
            SpawnBaloonCoroutine = StartCoroutine(BaloonSpawn());
        }
    }
    public void StopBaloonSpawn()
    {
        if (SpawnBaloonCoroutine != null)
        {
            StopCoroutine(SpawnBaloonCoroutine);
            SpawnBaloonCoroutine = null;
        }
    }
    public void EndGame(bool IsRestartScene) // Oyun durdurulur. Restart ise sahne yenilenir. Deðil ise oyun resetlenir
    {
        if (IsRestartScene)
        {
            HighScoreControl();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
        else
        {
            IsGameRun = false;
            HighScoreControl();
            ResetGame(true);
            StartPanelGame();
            if (IsSetHighPoint) IsSetHighPoint = false;
        }
    }
    public void PauseGame()
    {
        StopBaloonSpawn();

    }
    public void ResumeGame()
    {
        StartBaloonSpawn();
    }
    #region Starting
    public void StartButton()
    {
        Audio_Source.PlayOneShot(ButtonClickSound);
        DifficultPanel.SetActive(true);
    }
    public void SetEasy()
    {
        SetDifficulty(DifficultyGame.Easy);
    }

    public void SetNormal()
    {
        SetDifficulty(DifficultyGame.Normal);
    }

    public void SetHard()
    {
        SetDifficulty(DifficultyGame.Hard);
    }

    public void SetHardPlus()
    {
        SetDifficulty(DifficultyGame.HardPlus);
    }
    public void SetDifficulty(DifficultyGame gameDiff) // UI da zorluk seçerken ki backend
    {
        currentBaloonTime = BalloonTime;
        currentBaloonSpeedIncrease = BaloonSpeedIncrease;
        CurrentBaloonSpeed = DefaultBalloonSpeed;
        switch(gameDiff)
        {
            case DifficultyGame.Easy:
                MinNumber = 2;
                MaxNumber = 51;
                CurrentBaloonSpeed -= 100f;
                goldMultiplier = 1;
                break;
            case DifficultyGame.Normal:
                MinNumber = 50;
                MaxNumber = 121;
                CurrentBaloonSpeed -= 50f;
                currentBaloonSpeedIncrease += 0.2f;
                currentBaloonTime += 0.5f;
                goldMultiplier = 2;
                break;
            case DifficultyGame.Hard:
                MinNumber = 100;
                MaxNumber = 1001;
                CurrentBaloonSpeed -= 150f;
                currentBaloonSpeedIncrease += 0.5f;
                currentBaloonTime += 1.5f;
                goldMultiplier = 3;
                break;
            case DifficultyGame.HardPlus:
                MinNumber = 1000;
                MaxNumber = 10001;
                CurrentBaloonSpeed -= 200f;
                currentBaloonSpeedIncrease += 0.8f;
                currentBaloonTime += 2.5f;
                goldMultiplier = 4;
                break;
        }
        if (gameDiff != DifficultyGame.None)
        {
            GameDiff = gameDiff;
        }
        StartingPanel.SetActive(false);
        DifficultPanel.SetActive(false);
        StartGame();
    }
    public void ResetGame(bool isToMenu) // Belli durumlarda oyundaki sistemleri sýfýrlar durdurur.
    {
        IsGamePaused  = false;
        currentBaloonSpeedIncrease = BaloonSpeedIncrease;
        currentBaloonTime = BalloonTime;
        curren_health = Health;
        gamePoints = 0;
        value_number = 0;
        ValueTxt.text = string.Empty;
        PointText.text = "Point : " + gamePoints.ToString();
        HealthText.text = "Health : " + curren_health.ToString();
        GoldText.text = "Gold : " + Gold.ToString();

        if (isToMenu)
        {
            CurrentBaloonSpeed = DefaultBalloonSpeed;
        }
        StopBaloonSpawn();
        ClearBaloons();
       
    }

    #endregion
    private IEnumerator BaloonSpawn() // Balonlar spawn olmaya baþlar. 
    {
        yield return new WaitForSecondsRealtime(0.3f);

        while (IsGameRun)
        {
            int spawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Length);

            Button newBaloon = Instantiate(PrefabBalonn, SpawnPoints[spawnIndex].transform.position,Quaternion.identity); // balonu oluþtur
            newBaloon.transform.SetParent(SpawnPoints[spawnIndex].transform);

            BaloonButton btn = newBaloon.GetComponent<BaloonButton>();

            btn.TrueValue = value_number;
            int rndResult = UnityEngine.Random.Range(0, 2); // doðru cevap yada yanlýþ cevabý entegre et
            int result = 0;
            btn.TrueValue = value_number; 
            if (rndResult == 0) // doðru deðeri ata
            {
                int a = UnityEngine.Random.Range(1, value_number + 1);
                int b = value_number - a;
                result = a + b;
                btn.BtnTxt.text = a.ToString() + " + " + b.ToString();
            }
            else if(rndResult == 1) // yanlýþ deðer ata
            {
                int x = UnityEngine.Random.Range(1, value_number);
                int y = UnityEngine.Random.Range(1, value_number);
                result = x + y;
                btn.BtnTxt.text = x.ToString() + " + " + y.ToString();
            }
            btn.Value = result;
            SpawnedBaloons.Add(newBaloon.gameObject);
            yield return new WaitForSecondsRealtime(BalloonTime);
        }
    }
    public void ClearBaloons() // Tüm balonlar silinir
    {
        foreach (GameObject x in SpawnedBaloons)
        {
            if (x != null)
            {
                Destroy(x);
            }
        }
        SpawnedBaloons.Clear();
    }
  
   public void SetValue() // yeni sayý belirlenir
    {
        int new_value = UnityEngine.Random.Range(MinNumber, MaxNumber);
        value_number = new_value;
        ValueTxt.text = value_number.ToString();
        Audio_Source.PlayOneShot(ChangeNumberSound);
    }
    public void SelectControl(int value, Button btn) // Topa(Balona) týklandýðýnda kontrol gerçekleþir
    {
        Button baloonBtn = btn;
        if (value == value_number)
        {
            SetPoint(10);
            CorrectCount++;
            combo_count++;
            StartCoroutine(InfoTextCreate("Combo " + combo_count.ToString(),Color.blue,0.3f));
            if (CurrentBaloonSpeed < 750f)
            {
                CurrentBaloonSpeed += 0.5f;
            }
            Audio_Source.PlayOneShot(PozitiveScoreSound);
            baloonBtn.image.color = Color.blue;
            if (CorrectCount == maxCorrectCount)
            {
                StartCoroutine(ChangeValueDelayed());
            }
            if (combo_count == minComboCount)
            {
                StartCoroutine(ComboEffect());
                SetGold(goldMultiplier * getGoldCount);
            }
            if (combo_count == minComboCount + 3)
            {
                StartCoroutine(ComboEffect());
                SetGold(goldMultiplier * getGoldCount + 25 * goldMultiplier);
                combo_count = 0;
            }
        }
        else
        {
            SetHealth(-1);
            Audio_Source.PlayOneShot(NegativeScoreSound);
            baloonBtn.image.color = Color.red;
            combo_count = 0;
            StartCoroutine(InfoTextCreate("Wrong!", Color.softRed, 0.6f));
        }
    }
    private IEnumerator ComboEffect() // Combo gerçekleþtiðinde efekt yapýlýr.
    {
        Audio_Source.PlayOneShot(ComboSound);
        ComboPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        SetPoint(50);
        ComboPanel.SetActive(false);

    }
    private IEnumerator InfoTextCreate(string text,Color clr,float delay) // Ekrana yazý gösterir
    {
        yield return new WaitForSecondsRealtime(delay);
        GameObject point_txt = Instantiate(InfoText.gameObject, InfoTxtSpawnPoint.transform.position, Quaternion.identity);
        point_txt.transform.SetParent(Canv_s.transform);
        TMP_Text txt_p = point_txt.GetComponent<TMP_Text>();
        txt_p.color = clr;
        txt_p.text = text;

        yield return new WaitForSecondsRealtime(1.1f);

        Destroy(point_txt);
    }
    private IEnumerator GetPointText(int value) // silinecek
    {
        GameObject point_txt = Instantiate(InfoText.gameObject, InfoTxtSpawnPoint.transform.position, Quaternion.identity);
        point_txt.transform.SetParent(Canv_s.transform);
        TMP_Text txt_p = point_txt.GetComponent<TMP_Text>();
        txt_p.text =  "+" + value.ToString();
        yield return new WaitForSecondsRealtime(1.3f);
        Destroy(point_txt);
    }
    private IEnumerator ChangeValueDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        CorrectCount = 0;
        SetValue();
    }
    public void SetPoint(int value)
    {
        if (gamePoints >= 0)
        {
            gamePoints += value;   
            StartCoroutine(GetPointText(value));
            HighScoreControl();
        }
        PointText.text = "Point : " + gamePoints.ToString();
        //PointControl();

    }
    private void GetHighScore()
    {
        highPoints = PlayerPrefs.GetInt("highsc");
        HighPointText.text = "Highscore : " + highPoints.ToString();
    }
    private void HighScoreControl()
    {
        if (highPoints <= gamePoints)
        {
            highPoints = gamePoints;
            PlayerPrefs.SetInt("highsc",highPoints);
            HighPointText.text = "Highscore : " + highPoints.ToString();
            if (!IsSetHighPoint)
            {
                IsSetHighPoint = true;
                StartCoroutine(HighScoreEffective());
            }
        }
    }
    private IEnumerator HighScoreEffective()
    {
        PauseGame();
        yield return new WaitForSeconds(0.3f);
        EffectManager.Instance.FlashingEffect(Color.darkBlue, 3f, 0.2f);
        Audio_Source.PlayOneShot(HighScoredSound);
        StartCoroutine(InfoTextCreate("New Highscore : " + highPoints.ToString(), Color.green, 1.2f));
        yield return new WaitForSeconds(1f);
        ResumeGame();
       
    }
    public void SetGold(int value)
    {
        if(Gold >= 0)
        {
            Gold += value;
            Audio_Source.PlayOneShot(CoinWinSound);
            GoldText.text = "Gold : " + Gold.ToString();
            if (value > 0)
            {
                StartCoroutine(InfoTextCreate("+" + value.ToString() + " Gold",Color.orange,0.6f));

            }
            if (value < 0)
            {
                StartCoroutine(InfoTextCreate("-" + value.ToString() + " Gold", Color.red,0.6f));
            }
            if (Gold < 0)
            {
                Gold = 0;
            }
        }
    }
    public void SetHealth(int value)
    {
        if(curren_health > 0)
        {
            curren_health += value;
        }
        if(curren_health == 0)
        {
           GameOver();
        }
        HealthText.text = "Health : " + curren_health.ToString();
    }
    private void GameOver()
    {
        IsGameRun = false;
        Audio_Source.PlayOneShot(LoseGameSound);
        LoseGamePanel.gameObject.SetActive(true);
        LoseText.text = "You came close to a record! Score : " + gamePoints.ToString();
        ResetGame(false);


    }
    public void RestartTheGame() // Game Over Ekranýnda
    {
        LoseGamePanel.SetActive(false);
        ResetGame(false);
        StartGame();
    }
    public void ToMenuLose()
    {
        LoseGamePanel.SetActive(false);
        EndGame(false);
    }
    //private void PointControl()
    //{

    //    if (GamePoints == nextTreshold)
    //    {
    //        nextTreshold += 100;
    //        BalloonSpeed += 10f;
    //    }
    //}
    public void DestroyPointControl(bool isGood , Button btn)
    {
        Button button = btn;
        if (!isGood)
        {
            button.image.color = Color.red;
            Audio_Source.PlayOneShot(NegativeScoreSound);
            SetHealth(-1);
        }
        else button.image.color = Color.white;
    }
}
