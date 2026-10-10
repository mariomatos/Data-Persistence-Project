using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; // MIGRATED: New Input System namespace
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class MainManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SyncPersistentData();
#endif

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    // MIGRATED: InputAction replaces Input.GetKeyDown(KeyCode.Space)
    private InputAction m_LaunchAction;
    private InputAction m_ReturnToMenuAction;

    // My variables
    public static string PlayerName { get; set; }
    public static string BestPlayerName { get; private set; } = "Name";
    public static int BestScore { get; private set; }
    public static event System.Action BestScoreChanged;

    [System.Serializable]
    private class SaveData
    {
        public string PlayerName;
        public int Score;
    }

    

    // MIGRATED: bind the Space key as a button action
    void Awake()
    {
        LoadBestScore();
        m_LaunchAction = new InputAction("Launch", InputActionType.Button, "<Keyboard>/space");
        m_ReturnToMenuAction = new InputAction("ReturnToMenu", InputActionType.Button, "<Keyboard>/escape");
    }

    // MIGRATED: enable the action while the component is active
    void OnEnable()
    {
        m_LaunchAction?.Enable();
        m_ReturnToMenuAction?.Enable();
    }

    // MIGRATED: disable the action when the component is inactive
    void OnDisable()
    {
        m_LaunchAction?.Disable();
        m_ReturnToMenuAction?.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (m_LaunchAction.WasPressedThisFrame()) // MIGRATED: was Input.GetKeyDown(KeyCode.Space)
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (m_ReturnToMenuAction.WasPressedThisFrame())
            {
                SceneManager.LoadScene("StartMenu");
            }
            else if (m_LaunchAction.WasPressedThisFrame()) // MIGRATED: was Input.GetKeyDown(KeyCode.Space)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        UpdateBestScore();
    }

    private void UpdateBestScore()
    {
        if (m_Points <= BestScore)
        {
            return;
        }

        BestScore = m_Points;
        BestPlayerName = string.IsNullOrWhiteSpace(PlayerName) ? "Name" : PlayerName;
        SaveBestScore();
        BestScoreChanged?.Invoke();
    }

    private static string SavePath => Path.Combine(Application.persistentDataPath, "bestScore.json");

    public static void LoadBestScore()
    {
        if (!File.Exists(SavePath))
        {
            return;
        }

        try
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            if (saveData == null)
            {
                Debug.LogError("Best score data could not be read.");
                return;
            }

            BestPlayerName = string.IsNullOrWhiteSpace(saveData.PlayerName) ? "Name" : saveData.PlayerName;
            BestScore = saveData.Score;
        }
        catch (IOException exception)
        {
            Debug.LogError($"Could not load best score data: {exception.Message}");
        }
        catch (System.UnauthorizedAccessException exception)
        {
            Debug.LogError($"Could not load best score data: {exception.Message}");
        }
        catch (System.ArgumentException exception)
        {
            Debug.LogError($"Could not load best score data: {exception.Message}");
        }
    }

    private static void SaveBestScore()
    {
        SaveData saveData = new SaveData
        {
            PlayerName = BestPlayerName,
            Score = BestScore
        };

        try
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(saveData));
#if UNITY_WEBGL && !UNITY_EDITOR
            SyncPersistentData();
#endif
        }
        catch (IOException exception)
        {
            Debug.LogError($"Could not save best score data: {exception.Message}");
        }
        catch (System.UnauthorizedAccessException exception)
        {
            Debug.LogError($"Could not save best score data: {exception.Message}");
        }
    }
}
