using UnityEngine;
using UnityEngine.UI;

public class Answer : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UiManager uiManager;

    [Header("UI")]
    [SerializeField] private Image background;
    
    [Header("Answer Properties")]
    [SerializeField] private int answerQuestion;

    private void OnEnable() {
        gameManager.OnRoundEnd += OnRoundEnd;
    }

    private void OnDisable() {
        gameManager.OnRoundEnd -= OnRoundEnd;
    }

    private void OnRoundEnd() {
        background.color = Color.gray;
    }

    public void OnClick() {
        background.color = answerQuestion == uiManager.CurQuestionCorrectAnswer ? Color.green : Color.red;
    }
}