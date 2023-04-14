//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    private int highScore { get; set; }
    public int score { get; private set; }
    public int lives { get; private set; }
    public int ghostMultiplier { get; private set; } = 1;
    public AudioSource gameStart;
    public AudioSource death;
    public AudioSource munch2;
    public AudioSource gameOver;
    public AudioSource ghostEat;
    public AudioSource victory;
    public Text textScore;
    public Text textHighScore;
    // Start is called before the first frame update
    private void Start()
    {
        NewGame();
    }
    private void Update()
    {
        textScore.text = "Score: " + this.score;
        if (this.lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
        if (this.score > PlayerPrefs.GetInt("Your High Score"))
        {
            PlayerPrefs.SetInt("Your High Score", this.score);
        }
    }

    private void NewGame()
    {
        gameStart.Play();
        textHighScore.text = "High Score: " + PlayerPrefs.GetInt("Your High Score").ToString();
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        gameStart.Play();
        victory.Stop();
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }
        ResetState();
    }

    private void ResetState()
    {
        ResetGhostMultiplier();

        // for (int i = 0; i < this.ghosts.Length; i++)
        // {
        //     this.ghosts[i].ResetState();
        // }
        this.pacman.ResetState();
    }

    private void SetScore(int score)
    {
        this.score = score;
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
    }

    private void GameOver()
    {
        gameStart.Stop();
        gameOver.Play();
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }
        highScore = this.score;
        this.pacman.gameObject.SetActive(false);
    }

    public void GhostEat(Ghost ghost)
    {
        ghostEat.Play();
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);
        this.ghostMultiplier++;
    }

    public void PacmanEat()
    {
        death.Play();
        this.pacman.gameObject.SetActive(false);

        SetLives(this.lives - 1);
        if (this.lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f);
        }
        else
        {
            GameOver();
        }
    }

    public void PelletEat(Pellet pellet)
    {
        munch2.Play();
        SetScore(this.score + pellet.points);
        pellet.gameObject.SetActive(false);
        if (!CheckPellet())
        {
            victory.Play();
            gameStart.Stop();
            for (int i = 0; i < this.ghosts.Length; i++)
            {
                this.ghosts[i].gameObject.SetActive(false);
            }
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 6.0f);
        }
    }

    public void PowerPelletEat(PowerPellet pellet)
    {
        // Change Ghost's state
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].scared.Enable(pellet.duration);
        }
        PelletEat(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool CheckPellet()
    {
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 4;
    }
}
