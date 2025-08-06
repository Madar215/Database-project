using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[System.Serializable]
public class Question {
    public string questionText;
    public string answer1Text;
    public string answer2Text;
    public string answer3Text;
    public string answer4Text;
    public int correctAnswer;
}

public class GameManager : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private UiManager uiManager;

    [Header("Timer")] 
    [SerializeField] private float timerDuration = 10f;
    
    // Events
    public event UnityAction<Question> OnRoundStart;
    public event UnityAction OnRoundEnd;
    
    // Timer
    public CountdownTimer RoundTimer { get; private set; }
    
    // Questions
    private Question[] _questions;
    private int _curQuestionIndex;
    private int _totalQuestions;

    private void Awake() {
        RoundTimer = new CountdownTimer(timerDuration);
    }

    private void OnEnable() {
        RoundTimer.OnTimerStart += StartRound;
        RoundTimer.OnTimerStop += EndRound;
    }

    private void OnDisable() {
        RoundTimer.OnTimerStart -= StartRound;
        RoundTimer.OnTimerStop -= EndRound;
    }

    private void Start() {
        StartCoroutine(GetQuestions());
    }

    private void Update() {
        RoundTimer.Tick(Time.deltaTime);
    }

    private void StartRound() {
        // If there are questions from the database AND we are no at the last question -> get the next one
        if (_questions.Length > 0 && _curQuestionIndex < _questions.Length)
            OnRoundStart?.Invoke(_questions[_curQuestionIndex]);
    }

    private void EndRound() {
        // TODO: Implement Round End
        if (_curQuestionIndex + 1 == _totalQuestions)
            GameOver();
        else {
            OnRoundEnd?.Invoke();
            ++_curQuestionIndex;
            RoundTimer.Start();
        }
    }

    private void GameOver() {
        // TODO: Implement Game Over
        Debug.Log("GAME OVER");
    }

    private IEnumerator GetQuestions() {
        var www = UnityWebRequest.Get("http://localhost:5195/api/Trivia");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log(www.error);
        } else {
            var json = www.downloadHandler.text;

            // Store it for later
            _questions = JsonHelper.FromJson<Question>(json);
            _totalQuestions = _questions.Length;
            
            // Start the round timer
            RoundTimer.Start();
        }
    }
}
