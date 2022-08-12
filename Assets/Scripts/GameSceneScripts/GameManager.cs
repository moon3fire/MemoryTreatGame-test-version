using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    //UI elements variables start here
    [SerializeField] private Image gameOverPanel;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Image startPanel;
    [SerializeField] private Image movesTextPanel;
    [SerializeField] private Image timerPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button restartButton;
    [SerializeField] ParticleSystem explosion;
    public List<Button> btns = new List<Button>();


    MenuManager Instance;

    //variables about sprites start here
    public Sprite[] cards;
    public List<Sprite> gameCards = new List<Sprite>();
    [SerializeField] private Sprite bgImage;
    public float playerWaitCount = 5.0f;



    //Game Logic staff goes here
    private bool isGameFinished, isTimeExpired, firstGuess, secondGuess;
    private int countGuesses, countCorrectGuesses, gameGuesses, secondsRemaining = 5, timerSecondsCount = 60;
    private int firstGuessIndex, secondGuessIndex;
    bool timerTick = false;
    private string firstGuessCard, secondGuessCard;
    [SerializeField] private float difficultyLevel = 1;


    void Awake()
    {
        cards = Resources.LoadAll<Sprite>("Sprites/CardImages");
    }
    
    void Start()
    {
        difficultyLevel = MenuManager.Instance.difficulty;
        explosion.gameObject.SetActive(false);
        GetButtons();
        AddListeners();
        AddGameCards();
        RandomizeCards(gameCards);
        PrepareGameStart();
        CalculateTime();
        StartCoroutine(WaitForPlayerPrepare());
    }

    void Update()
    {
        if(timerTick == false && secondsRemaining > 0)
        {
            countdownText.text = "Game starts in " + secondsRemaining;
            StartCoroutine(StartTimerTick());
        }
        movesText.text = "Moves: " + countGuesses;
        if(!isGameFinished)
        {
            timerText.text = timerSecondsCount.ToString();
        }
        if(timerTick == false && timerSecondsCount > 0)
        {
            StartCoroutine(GameTimerTick());
        }
        if(isTimeExpired)
        {
            isTimeExpired = false;
            GameIsOver();
        }
    }

    void PrepareGameStart()
    {
        gameOverPanel.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        movesTextPanel.gameObject.SetActive(false);
        timerPanel.gameObject.SetActive(false);
        startPanel.color = new Color(0, 0, 0, 0);
        gameGuesses = gameCards.Count / 2;
    }

    void CalculateTime()
    {
        if(difficultyLevel > 7)
            timerSecondsCount = 40;
        else if(difficultyLevel > 5)
            timerSecondsCount = 60;
        else if(difficultyLevel > 3)
            timerSecondsCount = 80;
        else
            timerSecondsCount = 100;
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("CardButton");
        for(int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage;
        }
    }

    void AddListeners()
    {
        foreach(Button btn in btns)
        {
            btn.onClick.AddListener(() => SelectCard());
        }
    }

    void AddGameCards()
    {
        int multiplier = 2;
        if(difficultyLevel < 3)
            multiplier = 8;
        else if(difficultyLevel < 5)
            multiplier = 4;
        int looper = btns.Count;
        int index = 0;
        for(int i = 0; i < looper; i++)
        {
            if(index == looper/multiplier)
            {
                index = 0;
            }
            gameCards.Add(cards[index]);

            index++;
        }
    }

    public void SelectCard()
    {
        if(!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            btns[firstGuessIndex].image.sprite = gameCards[firstGuessIndex];
            firstGuessCard = gameCards[firstGuessIndex].name;

        }
        else if(!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            btns[secondGuessIndex].image.sprite = gameCards[secondGuessIndex];
            secondGuessCard = gameCards[secondGuessIndex].name;
            countGuesses++;
            StartCoroutine(CheckIfCardsMatch());
        }
    }

    IEnumerator CheckIfCardsMatch()
    {
        yield return new WaitForSeconds(.5f);
        if(firstGuessCard == secondGuessCard && firstGuessIndex != secondGuessIndex)
        {
            yield return new WaitForSeconds(.5f);
            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;
            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);
            CheckIfGameFinished();
            firstGuess = secondGuess = false;
        }
        else if(firstGuessIndex == secondGuessIndex)
        {
            secondGuess = false;
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            btns[firstGuessIndex].image.sprite = bgImage;
            btns[secondGuessIndex].image.sprite = bgImage;
            firstGuess = secondGuess = false;
        }

        yield return new WaitForSeconds(.5f);

    }

    IEnumerator WaitForPlayerPrepare()
    {
        for(int i = 0; i < btns.Count; i++)
        {
            btns[i].image.sprite = gameCards[i];
        }
        yield return new WaitForSeconds(playerWaitCount);
        for(int i = 0; i < btns.Count; i++)
        {
            btns[i].image.sprite = bgImage;
        }
        startPanel.gameObject.SetActive(false);
        movesTextPanel.gameObject.SetActive(true);
        timerPanel.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        restartButton.onClick.AddListener(() => RestartTheGame());
        StartCoroutine(StartGameCountdown());
    }

    IEnumerator StartGameCountdown()
    {
        yield return new WaitForSeconds(timerSecondsCount);
        isTimeExpired = true;
    }

    IEnumerator StartTimerTick()
    {
        timerTick = true;
        yield return new WaitForSeconds(1);
        secondsRemaining -= 1;
        timerTick = false;
    }

    IEnumerator GameTimerTick()
    {
        timerTick = true;
        yield return new WaitForSeconds(1);
        timerSecondsCount -= 1;
        timerTick = false;
    }

    void CheckIfGameFinished()
    {
        countCorrectGuesses++;
        if(countCorrectGuesses == gameGuesses && !isTimeExpired)
        {
            isGameFinished = true;
            StopCoroutine(StartGameCountdown());
            isTimeExpired = false;
            explosion.gameObject.SetActive(true);
            winText.gameObject.SetActive(true);
        }
    }

    void RandomizeCards(List<Sprite> cards)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            Sprite tempCard = cards[i];
            int randomIndex = Random.Range(0, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = tempCard;
        }
    }

    void GameIsOver()
    {
        foreach(Button btn in btns)
            btn.interactable = false;
        gameOverPanel.gameObject.SetActive(true);
    }

    void RestartTheGame()
    {
        SceneManager.LoadScene("GameScene");
    }

}
