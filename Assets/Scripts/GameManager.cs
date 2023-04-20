using System.Linq;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Security.Cryptography;
using System.Text;

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
    public AudioSource newGame;
    public Text textScore;
    public Text textHighScore;
    public Text live;
    public Text up;
    public Text level;
    private int round;
    public static bool gamePause = false;
    public GameObject pauseMenu;
    private List<PelletData> collectedPelletsData = new List<PelletData>();
    private static byte[] key = { 0x48, 0x7A, 0x4E, 0x71, 0x6B, 0x57, 0x28, 0x3A, 0x65, 0x52, 0x59, 0x34, 0x79, 0x46, 0x58, 0x5A };
    private static byte[] iv = { 0x50, 0x2A, 0x3D, 0x29, 0x72, 0x35, 0x47, 0x67, 0x39, 0x58, 0x49, 0x54, 0x43, 0x56, 0x30, 0x2B };
    // Start is called before the first frame update
    private void Start()
    {
        newGame.Stop();
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
        round = 1;
    }

    private void NewRound()
    {
        level.text = round + "UP";
        round++;
        up.gameObject.SetActive(false);
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
        live.text = "Live: " + this.lives;
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
        live.text = "Live: 0";
        up.gameObject.SetActive(true);
        up.text = "Game Over";
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
        PelletData pelletData = new PelletData();
        pelletData.pos = pellet.transform.position;
        collectedPelletsData.Add(pelletData);
        if (!CheckPellet())
        {
            victory.Play();
            gameStart.Stop();
            for (int i = 0; i < this.ghosts.Length; i++)
            {
                this.ghosts[i].gameObject.SetActive(false);
            }
            this.pacman.gameObject.SetActive(false);
            up.gameObject.SetActive(true);
            up.text = "Victory";
            
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
        gameStart.volume= 0.5f;
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        gamePause = false;
    }

    public void Pause()
    {
        gameStart.volume= 0.1f;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gamePause = true;
    }

    
    public void Return(){
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Intro");
    }

    public void SaveGame()
    {
        data = new PacmanDataClass();
        data.lives = this.lives;
        data.score = this.score;
        data.level = this.round;
        data.pos = pacman.transform.position;
        data.rotationAngle = pacman.movement.direction;
        data.eatenPelletPositions = new List<Vector3>();
        foreach (Transform pellet in this.pellets)
        {
            if (!pellet.gameObject.activeSelf)
            {
                data.eatenPelletPositions.Add(pellet.transform.position);
            }
        }
        foreach (Ghost ghost in ghosts)
        {
            GhostData ghostData = new GhostData();
            ghostData.position = ghost.transform.position;
            ghostData.direction = ghost.movement.direction;
            ghostData.currentState = GetCurrentState(ghost);
            data.ghostData.Add(ghostData);
        }
        string json = JsonUtility.ToJson(data, true);
        byte[] encryptedBytes = EncryptStringToBytes_Aes(json, key, iv);
        File.WriteAllBytes(Application.dataPath + "/PacmanData.json", encryptedBytes);
    }

    public void LoadGame()
    {
        byte[] encryptedBytes = File.ReadAllBytes(Application.dataPath + "/PacmanData.json");
        string json = DecryptStringFromBytes_Aes(encryptedBytes, key, iv);
        data = JsonUtility.FromJson<PacmanDataClass>(json);
        this.round = data.level;
        this.lives = data.lives;
        this.score = data.score;
        level.text = data.level + "UP";
        textScore.text = "Score: " + data.score;
        live.text = "Live: " + data.lives;
        this.pacman.transform.position = data.pos;
        this.pacman.movement.SetDirection(data.rotationAngle);
        for (int i = 0; i < ghosts.Length; i++)
        {
            this.ghosts[i].transform.position = data.ghostData[i].position;
            this.ghosts[i].movement.SetDirection(data.ghostData[i].direction);
            SetCurrentState(ghosts[i], data.ghostData[i].currentState);
        }
        foreach (Transform pellet in this.pellets)
        {
            if (data.eatenPelletPositions.Contains(pellet.transform.position))
            {
                pellet.gameObject.SetActive(false);
            }
            else
            {
                pellet.gameObject.SetActive(true);
            }
        }
    }
    private byte[] EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] iv)
    {
        byte[] encrypted;
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    encrypted = ms.ToArray();
                }
            }
        }

        return encrypted;
    }

    private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)

    {
        string decrypted;
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(cipherText))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        decrypted = sr.ReadToEnd();
                    }
                }
            }
        }

        return decrypted;
    }

    private int GetCurrentState(Ghost ghost)
    {
        if (ghost.scatter.enabled) return 0;
        if (ghost.home.enabled) return 1;
        if (ghost.scared.enabled) return 2;
        if (ghost.chase.enabled) return 3;
        return -1;
    }

    private void SetCurrentState(Ghost ghost, int state)
    {
        ghost.ResetStateOnly();
        switch (state)
        {
            case 0:
                ghost.scatter.Enable();
                break;
            case 1:
                ghost.home.Enable();
                break;
            case 2:
                ghost.scared.Enable();
                break;
            case 3:
                ghost.chase.Enable();
                break;
        }
    }
}
