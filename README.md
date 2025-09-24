# 50.033-Labs-SuperMario
 Lab Submissions for 50.033 Foundations of Game Design and Development

 # Week 1
 Checkoff requirement: 
 
 - Game over screen with score and restart button

For this week's lab checkoff, we implemented the Game Over Screen as a hidden canvas overlay which activates only when Mario dies.

[Video submission](https://drive.google.com/file/d/1-OTH23pL2lAqp4brPepLthnKKvrNNOI1/view?usp=drive_link)

<details><summary> Click here to view a more detailed description of how we implemented it. Will be updating this part every week~</summary>

### UI (Hierarchy)

We created a new `Canvas` called "GameOverScreen" (it's inside 'GameManager' parent). Within this canvas is a `TextMesh`named FinalScoreText and the same replay button previously implemented. 

GameOverScreen is initially set as disabled.

### Code

The checkoff implementation can be seen in `PlayerMovement.cs`. We added a new `GameObject` variable called `gameOverScreen` which points to the canvas hidden canvas we created, and also a new 'TextMeshProGUI` called 'finalScore', also pointing to the finalScore inside the hidden canvas.

We created a new method called `ShowGameOverScreen()` which activates the hidden canvas layer. We also set the finalScoreText as the current score counted before time stopped. It’s better to use the score itself (the number) because that’s the real game data. This is better than reusing the scoreText we has made initially, since we prevent mixing game logic with UI, which can cause bugs and make it harder to use the score in calculations later.

```ruby
public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        finalScore.text = "Score: " + jumpOverGoomba.score;
    }
```

Then we call this method inside the OnTrigger function when Mario hits Goomba. 

```ruby
void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");
            Time.timeScale = 0.0f;
            ShowGameOverScreen();
        }
    }
```

Finally we disable the screen again when the replay button is pressed. This is inside the `ResetGame()` function we added previously.

```ruby
    jumpOverGoomba.score = 0;
    // hide game over screen
    gameOverScreen.SetActive(false);
```

</details>
