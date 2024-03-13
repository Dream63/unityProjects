using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    [SerializeField] private TextMeshProUGUI scoreText, bestScoreText;
    private int score = 0;
    static int bestScore = 0;

    private void Start()
    {
        Instance = this;
        bestScore = PlayerPrefs.GetInt(key: "BestScore");
    }

    public void SetScore(int scoreAdd)
    {
        this.score += scoreAdd;
        scoreText.text = "Score: " + this.score.ToString();
    }
    public void BestScore() 
    {

        if (bestScore < score)
        {
            bestScore = score;
            PlayerPrefs.SetInt(key: "BestScore", bestScore);
            bestScoreText.text = "New best: " + bestScore;
        }
        else
        {
            bestScore = PlayerPrefs.GetInt(key: "BestScore");
            bestScoreText.text = "Best: " + bestScore;
        }
    }
}