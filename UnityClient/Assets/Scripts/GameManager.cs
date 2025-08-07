using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private UiManager uiManager;

    [Header("Timer")] 
    [SerializeField] private float timerDuration = 10f;

    [Header("Join Parameters")] 
    [SerializeField] private float activePlayersPoll = 2f;

    [Header("Game Parameters")] 
    [SerializeField] private int correctAnswerScoreReward = 10;
    
    // Events
    public event UnityAction OnGameStart;
    public event UnityAction<string, int, float> OnGameOver;
    public event UnityAction<Question> OnRoundStart;
    public event UnityAction OnRoundEnd;
    
    // Timer
    public CountdownTimer RoundTimer { get; private set; }
    
    // Player
    private int _playerId = -1;
    private int _score;
    private float _timeAccumulated;
    
    // Questions
    private Question[] _questions;
    private int _curQuestionIndex;
    private int _totalQuestions;
    
    // Server Client
    private readonly ServerClient _serverClient = new();
    
    // Game
    private bool _gameStarted;

    #region MonoBehaviour
    
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
        StartCoroutine(WaitForPlayers());
    }

    private void Update() {
        RoundTimer.Tick(Time.deltaTime);
    }
    
    #endregion

    #region Game

    private void StartGame() {
        RoundTimer.Start();
        OnGameStart?.Invoke();
    }
    
    private void StartRound() {
        // If there are questions from the database AND we are no at the last question -> get the next one
        if (_questions.Length > 0 && _curQuestionIndex < _questions.Length)
            OnRoundStart?.Invoke(_questions[_curQuestionIndex]);
    }

    private void EndRound() {
        var isLastQuestion = _curQuestionIndex + 1 == _totalQuestions;
        
        if (isLastQuestion)
            GameOver();
        else {
            OnRoundEnd?.Invoke();
            ++_curQuestionIndex;
            RoundTimer.Start();
        }
    }

    private void GameOver()
    {
        StartCoroutine(_serverClient.GetTopPlayerId(
            winnerId =>
            {
                if (_playerId == winnerId)
                {
                    OnGameOver?.Invoke("You Won", _score, _timeAccumulated);
                }
                else
                {
                    OnGameOver?.Invoke("You Lose", _score, _timeAccumulated);
                }
            },
            error => Debug.LogError("Failed to get winner ID: " + error)
        ));
    }


    public void OnAnswer(bool isCorrect) {
        if (isCorrect) {
            _score += correctAnswerScoreReward;
            _timeAccumulated += timerDuration - RoundTimer.Time;
        }
        else {
            _timeAccumulated += timerDuration;
        }

        StartCoroutine(PostPlayerData(_playerId, _score, _timeAccumulated));
    }
    
    #endregion

    #region Server
    
    private IEnumerator PostPlayerData(int id, int score, float time) {
        yield return _serverClient.UpdatePlayer(id, score, time,
            () => Debug.Log("Player updated."),
            error => Debug.LogError("Update failed: " + error)
        );
    }
    
    private IEnumerator GetQuestions() {
        yield return _serverClient.GetQuestions(
            questions => {
                _questions = questions;
                _totalQuestions = questions.Length;
            },
            error => Debug.LogError("Get questions failed: " + error)
        );
    }


    public void AddNewPlayer(string playerName, bool isActive = true) {
        StartCoroutine(_serverClient.AddNewPlayer(playerName, isActive, 
            id => {
                _playerId = id;
                Debug.Log("Player joined, ID: " + _playerId);
                // Optionally start waiting for players here
            },
            error => Debug.LogError("Add player failed: " + error)
        ));
    }

    private IEnumerator WaitForPlayers() {
        while (!_gameStarted) {
            yield return _serverClient.GetActivePlayers(
                count => {
                    Debug.Log("Active players: " + count);
                    if (count >= 2) {
                        _gameStarted = true;
                        StartGame();
                    }
                },
                error => Debug.LogError("Active count failed: " + error)
            );

            yield return new WaitForSeconds(activePlayersPoll);
        }
    }
    
    #endregion
}
