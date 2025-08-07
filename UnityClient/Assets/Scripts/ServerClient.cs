using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class ServerClient {
    private const string BaseUrl = "http://localhost:5195";

    public IEnumerator AddNewPlayer(string playerName, bool isActive, Action<int> onSuccess, Action<string> onError) {
        var data = new PlayerData(playerName, isActive);
        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest($"{BaseUrl}/api/Trivia", "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) {
            onError?.Invoke(request.error);
        } else if (int.TryParse(request.downloadHandler.text, out int id)) {
            onSuccess?.Invoke(id);
        } else {
            onError?.Invoke("Invalid ID response: " + request.downloadHandler.text);
        }
    }

    public IEnumerator GetQuestions(Action<Question[]> onSuccess, Action<string> onError) {
        UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}/api/Trivia");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) {
            onError?.Invoke(request.error);
        } else {
            var json = request.downloadHandler.text;
            var questions = JsonHelper.FromJson<Question>(json);
            onSuccess?.Invoke(questions);
        }
    }

    public IEnumerator UpdatePlayer(int id, int score, float time, Action onSuccess, Action<string> onError) {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("score", score);
        form.AddField("time", time.ToString("F2"));

        UnityWebRequest request = UnityWebRequest.Post($"{BaseUrl}/api/Trivia/update-player", form);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) {
            onError?.Invoke(request.error);
        } else {
            onSuccess?.Invoke();
        }
    }

    public IEnumerator GetActivePlayers(Action<int> onSuccess, Action<string> onError) {
        UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}/api/Trivia/active-count");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) {
            onError?.Invoke(request.error);
        } else if (int.TryParse(request.downloadHandler.text, out int count)) {
            onSuccess?.Invoke(count);
        } else {
            onError?.Invoke("Invalid count format");
        }
    }
    
    public IEnumerator GetTopPlayerId(Action<int> onSuccess, Action<string> onError)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}/api/Trivia/top-player");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            if (int.TryParse(json, out int winnerId))
            {
                onSuccess?.Invoke(winnerId);
            }
            else
            {
                onError?.Invoke("Failed to parse winner ID: " + json);
            }
        }
    }

}
