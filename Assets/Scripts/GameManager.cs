//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public PacmanDataClass data;
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
    public static bool gamePause = false;
    public GameObject pauseMenu;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
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

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        gamePause = false;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gamePause = true;
    }

    public void SaveGame()
    {
        data = new PacmanDataClass();
        data.lives = this.lives;
        data.score = this.score;
        data.pos = pacman.transform.position;
        data.rotationAngle = pacman.movement.direction;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.dataPath + "/PacmanData.json", json);
        // Time.timeScale = 1.0f;
        // SceneManager.LoadScene("");
    }

    public void LoadGame()
    {
        string json = File.ReadAllText(Application.dataPath + "/PacmanData.json");
        data = JsonUtility.FromJson<PacmanDataClass>(json);
        this.lives = data.lives;
        this.score = data.score;
        pacman.transform.position = data.pos;
        pacman.movement.SetDirection(data.rotationAngle);
    }
}
