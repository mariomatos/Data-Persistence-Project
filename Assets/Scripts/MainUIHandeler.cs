using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainUIHandeler : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_PlayerNameInput;

    private void Awake()
    {
        if (m_PlayerNameInput == null)
        {
            m_PlayerNameInput = GetComponentInChildren<TMP_InputField>();
        }
    }

    public void NewColorSelected(string name)
    {
        SetPlayerName(name);
    }

    public void SetPlayerName(string name)
    {
        MainManager.PlayerName = name;
    }

    // My method
    public void StartGame()
    {
        if (m_PlayerNameInput == null)
        {
            Debug.LogError("No player-name input field was found.", this);
            return;
        }

        SetPlayerName(m_PlayerNameInput.text);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }



}
