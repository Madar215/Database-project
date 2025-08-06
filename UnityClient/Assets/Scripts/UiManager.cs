using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private GameManager gameManager;
    
    [Header("Questions")] 
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI answer1Text;
    [SerializeField] private TextMeshProUGUI answer2Text;
    [SerializeField] private TextMeshProUGUI answer3Text;
    [SerializeField] private TextMeshProUGUI answer4Text;

    private void OnEnable() {
        gameManager.OnRoundStart += UpdateQuestionUI;
    }

    private void OnDisable() {
        gameManager.OnRoundStart-= UpdateQuestionUI;
    }

    private void UpdateQuestionUI(Question question) {
        questionText.text = question.questionText;
        answer1Text.text = question.answer1Text;
        answer2Text.text = question.answer2Text;
        answer3Text.text = question.answer3Text;
        answer4Text.text = question.answer4Text;
    }
}
