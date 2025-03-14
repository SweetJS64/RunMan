using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Text CurrentScoreText;
    [SerializeField] private Text BestScoreText;
    
    private void OnEnable()
    {
        DistanceTracker.SetScore += SetScores;
    }

    private void OnDisable()
    {
        DistanceTracker.SetScore -= SetScores;
    }

    private void SetScores(int currentScore)
    {
        CurrentScoreText.text = $"SCORE: {currentScore.ToString()} M.";
        var distance = BestDistanceController.GetBestDistance();
        BestScoreText.text = $"BEST SCORE: {distance} M.";
    }
}