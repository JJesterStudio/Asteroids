using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Text scoreText;
    public GameObject loseScreen;

    private int score = 0;

    void Start()
    {
        Instance = this;
    }

    public void IncreaseScore(int value)
    {
        score += value;
        scoreText.text = $"Score: {score}";
    }

    public void Lose()
    {
        loseScreen.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
