using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MenuManager : MonoBehaviour
{
    public string playerName;
    public float difficulty;

    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] Slider volumeSlider;
    public static MenuManager Instance;
    [SerializeField] private TextMeshProUGUI playerNameInput;
    [SerializeField] private Slider difficultyLevel;
    [SerializeField] private Button startButton;

    void Awake()
    {
        startButton.onClick.AddListener(() => StartTheGame());
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        backgroundMusic.volume = volumeSlider.value;
    }

    void StartTheGame()
    {
        MenuManager.Instance.playerName = playerNameInput.text;
        MenuManager.Instance.difficulty = difficultyLevel.value;
        SceneManager.LoadScene("GameScene");
    }
}