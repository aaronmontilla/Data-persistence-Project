using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;
    public string playerName;

    public string maxScoreName;
    public int maxScore;


    // Start is called before the first frame update
    void Start()
    {
        playerName = MenuUIHandler.Instance.playerName;
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
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
        LoadMaxScore();
        
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score :  {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if(m_Points > maxScore)
        {
            Debug.Log($"Saving the score of {playerName} that is {m_Points}");
            SaveMaxScore();
            LoadMaxScore();
            Debug.Log($"Now best score is {maxScoreName} {maxScore}");
        }
    }

    public class SaveData{
        public string playerName;
        public int maxScore;
    }

    public void SaveMaxScore()
    {
        SaveData data = new SaveData();
        data.playerName = playerName;
        data.maxScore = m_Points;

        string json = JsonUtility.ToJson(data);
        Debug.Log(Application.persistentDataPath + "/savefile.json");
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadMaxScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            maxScoreName = data.playerName;
            maxScore = data.maxScore;
        }
        BestScoreText.text = $"Best Score: {maxScoreName} : {maxScore}";
    }
}
