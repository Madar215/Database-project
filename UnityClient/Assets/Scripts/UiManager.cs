using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private GameManager gameManager;
    
    [Header("Questions")] 
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI answer1Text;
    [SerializeField] private TextMeshProUGUI answer2Text;
    [SerializeField] private TextMeshProUGUI answer3Text;
    [SerializeField] private TextMeshProUGUI answer4Text;

    [Header("Timer")] 
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI timerCounter;
    
    // Current Question
    private Question _curQuestion;
    public int CurQuestionCorrectAnswer { get; private set; }

    private void OnEnable() {
        gameManager.OnRoundStart += OnRoundStart;
    }

    private void OnDisable() {
        gameManager.OnRoundStart-= OnRoundStart;
    }

    private void Update() {
        timerSlider.value = gameManager.RoundTimer.Progress;
        timerCounter.text = Mathf.CeilToInt(gameManager.RoundTimer.Time).ToString();
    }

    private void OnRoundStart(Question question) {
        // Cache the current question
        _curQuestion = question;
        CurQuestionCorrectAnswer = question.correctAnswer;
        
        // Update UI
        UpdateQuestionUI(question);
    }

    private void UpdateQuestionUI(Question question) {
        questionText.text = question.questionText;
        answer1Text.text = question.answer1Text;
        answer2Text.text = question.answer2Text;
        answer3Text.text = question.answer3Text;
        answer4Text.text = question.answer4Text;
    }
}
