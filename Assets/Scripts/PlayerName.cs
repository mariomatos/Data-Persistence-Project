using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    private TextMeshProUGUI m_TmpPlayerNameText;
    private Text m_PlayerNameText;

    private void Awake()
    {
        m_TmpPlayerNameText = GetComponent<TextMeshProUGUI>();
        m_PlayerNameText = GetComponent<Text>();
        if (m_TmpPlayerNameText == null && m_PlayerNameText == null)
        {
            Debug.LogError("PlayerName requires a TextMeshProUGUI or Text component.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        MainManager.LoadBestScore();
        UpdateBestScoreText();
    }

    private void OnEnable()
    {
        MainManager.BestScoreChanged += UpdateBestScoreText;
    }

    private void OnDisable()
    {
        MainManager.BestScoreChanged -= UpdateBestScoreText;
    }

    private void UpdateBestScoreText()
    {
        string bestScoreText = $"Best Score : {MainManager.BestPlayerName} : {MainManager.BestScore}";
        if (m_TmpPlayerNameText != null)
        {
            m_TmpPlayerNameText.text = bestScoreText;
        }
        else
        {
            m_PlayerNameText.text = bestScoreText;
        }
    }
}
