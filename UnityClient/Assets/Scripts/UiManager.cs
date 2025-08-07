using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private GameManager gameManager;

    [Header("Panels")] 
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;

    [Header("Buttons")] 
    [SerializeField] private Button[] answersButtons;
    
    [Header("Questions")] 
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI answer1Text;
    [SerializeField] private TextMeshProUGUI answer2Text;
    [SerializeField] private TextMeshProUGUI answer3Text;
    [SerializeField] private TextMeshProUGUI answer4Text;
    
    [Header("Menu")] 
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Timer")] 
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI timerCounter;
    
    // Current Question
    public int CurQuestionCorrectAnswer { get; private set; }
    
    // Buttons
    private bool _buttonsActive;

    private void OnEnable() {
        gameManager.OnRoundStart += OnRoundStart;
        gameManager.OnGameStart += OnGameStart;
    }

    private void OnDisable() {
        gameManager.OnRoundStart -= OnRoundStart;
        gameManager.OnGameStart -= OnGameStart;
    }

    private void Start() {
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    private void Update() {
        timerSlider.value = gameManager.RoundTimer.Progress;
        timerCounter.text = Mathf.CeilToInt(gameManager.RoundTimer.Time).ToString();
    }

    private void OnGameStart() {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    private void OnRoundStart(Question question) {
        // Cache the current question answer
        CurQuestionCorrectAnswer = question.correctAnswer;
        
        // Update UI
        UpdateQuestionUI(question);
        
        // Activate answers buttons
        ToggleAnswersButtons();
    }

    private void UpdateQuestionUI(Question question) {
        questionText.text = question.questionText;
        answer1Text.text = question.answer1Text;
        answer2Text.text = question.answer2Text;
        answer3Text.text = question.answer3Text;
        answer4Text.text = question.answer4Text;
    }

    public void ToggleAnswersButtons() {
        // Toggle active state
        _buttonsActive = !_buttonsActive;
        
        // Active/Disable each button
        foreach (var button in answersButtons) {
            button.interactable = _buttonsActive;
        }
    }

    private void OnJoinClicked() {
        var playerName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName)) {
            statusText.text = "Please enter a name!";
            return;
        }
        
        gameManager.AddNewPlayer(playerName);
    }
}
