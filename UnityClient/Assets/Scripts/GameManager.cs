using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Question
{
    public string questionText;
    public string answer1Text;
    public string answer2Text;
    public string answer3Text;
    public string answer4Text;
    public int correctAnswer;
}
public class GameManager : MonoBehaviour
{
    public int numberOfQuestion;
    public TMP_Text questionText;
    public TMP_Text answer1Text;
    public TMP_Text answer2Text;
    public TMP_Text answer3Text;
    public TMP_Text answer4Text;

    private void Start()
    {
        StartCoroutine(GetText());
    }

    private IEnumerator GetText()
    {
        var www = UnityWebRequest.Get("https://localhost:7057/api/Trivia");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            //UpdateQuestionUI(www.downloadHandler.text);
        }
    }

    private void UpdateQuestionUI(string json)
    {
        var curQuestion = JsonUtility.FromJson<Question>(json);
        questionText.text = curQuestion.questionText;
        answer1Text.text = curQuestion.answer1Text;
        answer2Text.text = curQuestion.answer2Text;
        answer3Text.text = curQuestion.answer3Text;
        answer4Text.text = curQuestion.answer4Text;

    }
}
