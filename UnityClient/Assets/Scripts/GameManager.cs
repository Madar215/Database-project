using System.Collections;
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
}
