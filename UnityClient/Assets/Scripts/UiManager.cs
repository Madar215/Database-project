using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour {
    [Header("Questions")] 
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI answer1Text;
    [SerializeField] private TextMeshProUGUI answer2Text;
    [SerializeField] private TextMeshProUGUI answer3Text;
    [SerializeField] private TextMeshProUGUI answer4Text;
    
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
