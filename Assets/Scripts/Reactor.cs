using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class Reactor : MonoBehaviour
{

    public static Reactor Instance { get; private set; }

    public enum CellType
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple
    }

    //[SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject[] dropPoints; // 4 elements, 1 for each cell drop point
    [SerializeField] private CellType[] acceptedCells;

    [SerializeField] private float dropDistance = 1.5f;

    private GameObject heldCell;

    //private event EventHandler OnDoorsActivated;
    //private event EventHandler OnConsumedCell;
    //private event EventHandler OnGameOver;

    private bool consuming = false;
    private bool consumed = false;
    private float consumingTime = 2f;
    private bool crossedIn;

    [SerializeField] private bool gameStarted = false;
    private bool gameOver = false;
    private bool paused = false;
    [SerializeField] private float totalTime;
    [SerializeField] private float bonusTime;
    private float gameTime = 0;
    private float currentTime;


    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI timerDisplay;
    [SerializeField] private Image timerBar;

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private GameObject pauseScreen;

    private void Awake()
    {
        Instance = this;
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneData.nextScene = "MainMenuScene";
            SceneManager.LoadScene("LoadingScene");
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        acceptedCells = new CellType[4];

        SetDropPoint(0);
        SetDropPoint(1);
        SetDropPoint(2);
        SetDropPoint(3);

        PlayerActions.Instance.OnInteractPerformed += Instance_OnInteractPerformed;
        PlayerActions.Instance.OnPausePerformed += Instance_OnPausePerformed;

        gameOverText.enabled = false;
        currentTime = totalTime;
        gameStarted = true;
    }

    private void Instance_OnInteractPerformed(object sender, System.EventArgs e)
    {
        for (int i = 0; i < dropPoints.Length; i++)
        {
            if (PlayerActions.Instance.heldCell != null &&
                heldCell == null &&
                !consuming &&
                Vector3.Distance(PlayerActions.Instance.heldCell.transform.position, dropPoints[i].transform.position) < dropDistance &&
                acceptedCells[i] == PlayerActions.Instance.heldCell.GetComponentInChildren<EnergyCell>().cellType)
            {
                PlayerActions.Instance.heldCell.transform.SetParent(dropPoints[i].transform);
                PlayerActions.Instance.heldCell.transform.localPosition = Vector3.zero;
                heldCell = PlayerActions.Instance.heldCell;
                PlayerActions.Instance.heldCell = null;
                consuming = true;
                SetDropPoint(i);
            }
        }
    }

    private void Instance_OnPausePerformed(object sender, System.EventArgs e)
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        pauseScreen.SetActive(paused);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted && !gameOver && !paused)
            UpdateTimer();

        //Debug.Log("Current Time: "+currentTime.ToString());

        if (gameOver)
        {
            Time.timeScale = 0f;

            if (gameStarted)
            {
                Time.timeScale = 0f;
                gameOverText.enabled = true;
                mainMenuButton.gameObject.SetActive(true);
                gameOverText.text = "Game Over\nFinal Time:\n"+ ((int)(gameTime / 60f)).ToString() + ":" + (gameTime % 60).ToString("00") + "." + gameTime.ToString("0.00").Split(",")[1];
                timerDisplay.enabled = false;
            }
            gameStarted = false;
        }
        else
        {
            timerBar.fillAmount = currentTime / totalTime;
            //timerDisplay.text = "Time: " + gameTime.ToString("F2") + "s";
            timerDisplay.enabled = true;
            timerDisplay.text = "Time: " + ((int)(gameTime / 60f)).ToString() + ":" + (gameTime % 60).ToString("00") + "." + gameTime.ToString("0.00").Split(",")[1];
        }
    }

    private void UpdateTimer()
    {
        gameTime += Time.deltaTime; // keet track of the time passed from the start of the game
        currentTime -= Time.deltaTime; // time left to the player before game over

        if (currentTime <= 0)
        {
            currentTime = 0; // good practive is to st to 0 stuff that shouldn't be below 0
            gameOver = true;
        }
        if (currentTime > totalTime)
        {
            currentTime = totalTime;
        }
        if (consuming)
        {
            consumingTime -= Time.deltaTime;

            if (consumingTime < 0)
            {
                consumingTime = 2f;
                consumed = false;
                consuming = false;
            }
            else if (consumingTime < 1 && !consumed)
            {
                consumed = true;
                Destroy(heldCell);
                heldCell = null;
                currentTime += bonusTime; // give the reward to the player
            }
        }
    }

    private void SetDropPoint(int id)
    {
        int rndTypeID = UnityEngine.Random.Range(0, 5);
        Color color;
        switch (rndTypeID)
        {
            case 0:
                acceptedCells[id] = CellType.Red;
                color = Color.red;
                break;
            case 1:
                acceptedCells[id] = CellType.Green;
                color = Color.green;
                break;
            case 2:
                acceptedCells[id] = CellType.Blue;
                color = Color.blue;
                break;
            case 3:
                acceptedCells[id] = CellType.Yellow;
                color = Color.yellow;
                break;
            case 4:
                acceptedCells[id] = CellType.Purple;
                color = Color.magenta;
                break;
            default:
                color = Color.white;
                break;
        }

        dropPoints[id].GetComponentInChildren<SpriteRenderer>().color = color;



    }

    private void OnDestroy()
    {
        PlayerActions.Instance.OnInteractPerformed -= Instance_OnInteractPerformed;
        PlayerActions.Instance.OnPausePerformed -= Instance_OnPausePerformed;
    }
}
