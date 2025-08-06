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
    
    // Properties
    public Question[] Questions { get; set; }
    
    // Events
    public event UnityAction<Question> OnRoundStart;
    
    // Timer
    public CountdownTimer RoundTimer { get; set; }
    
    // Questions
    private int _curQuestionIndex;

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
        if (Questions.Length > 0 && _curQuestionIndex < Questions.Length)
            OnRoundStart?.Invoke(Questions[_curQuestionIndex]);
        else
            GameOver();
    }

    private void EndRound() {
        // TODO: Implement Round End
    }

    private void GameOver() {
        // TODO: Implement Game Over
    }

    private IEnumerator GetQuestions() {
        var www = UnityWebRequest.Get("http://localhost:5195/api/Trivia");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log(www.error);
        } else {
            var json = www.downloadHandler.text;

            // Store it for later
            Questions = JsonHelper.FromJson<Question>(json);
            RoundTimer.Start();
        }
    }
}
