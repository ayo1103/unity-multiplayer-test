using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    public void UpdateScoreUI(int previousValue, int newValue)
    {
        scoreText.text = "Score: " + newValue;
    }
}