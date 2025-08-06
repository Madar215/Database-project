using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

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
    
    // Player
    public int playerId = -1;
    
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

    public void OnAnswer(bool isCorrect) {
        // TODO: Cache Player's Answer and Time Accumulated
    }
    
    private IEnumerator PostPlayerData(int id, int score, float time) {
        string url = "http://localhost:5195/update-player";

        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("score", score);
        form.AddField("time", time.ToString("F2"));

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.LogError("Error updating player: " + www.error);
        } else {
            Debug.Log("Player updated: " + www.downloadHandler.text);
        }
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

    public void AddNewPlayer(string playerName, bool isActive = true) {
        PlayerData data = new PlayerData(playerName, isActive);
        string json = JsonUtility.ToJson(data);

        StartCoroutine(SendPlayerData(json));
    }

    private IEnumerator SendPlayerData(string json) {
        string url = "http://localhost:5195/api/Trivia";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) {
            Debug.Log("Add player failed: " + request.error);
        } else {
            // Parse the returned player ID
            string responseText = request.downloadHandler.text;
            
            if (int.TryParse(responseText, out int id)) {
                playerId = id;
                Debug.Log($"Player added! ID = {playerId}");
            } else {
                Debug.Log("Failed to parse player ID: " + responseText);
            }
        }
    }
}
