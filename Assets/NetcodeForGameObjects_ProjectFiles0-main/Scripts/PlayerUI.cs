using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (player == null)
            return;
        
        scoreText.text = "Score: " + player.playerScore;
    }
}