using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private Vector3[] scoreTextPosition = {
        new Vector3(-811, 462, 0),
        new Vector3(0,0,0)
    };
    private Vector3[] restartButtonPosition = {
        new Vector3(871, 463, 0),
        new Vector3(0, -150, 0)
    };

    public GameObject scoreText;
    public Transform restartButton;

    public GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameStart()
    {
        // hide gameover panel
        gameOverScreen.SetActive(false);
        scoreText.transform.localPosition = scoreTextPosition[0];
        restartButton.localPosition = restartButtonPosition[0];
    }

    public void SetScore(int score)
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        scoreText.transform.localPosition = scoreTextPosition[1];
        restartButton.localPosition = restartButtonPosition[1];
    }

}
