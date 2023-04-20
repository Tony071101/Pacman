using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public AudioSource newGame;
    private void Start()
    {
        newGame.Play();
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Pacman");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
