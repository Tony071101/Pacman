//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public int score { get; private set; }
    public int lives { get; private set; }
    public int ghostMultiplier {get; private set;} = 1;

    // Start is called before the first frame update
    private void Start()
    {
        NewGame();
    }
    private void Update()
    {
        if(this.lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        foreach(Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
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
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
    }

    public void GhostEat(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);
        this.ghostMultiplier++;
    }

    public void PacmanEat()
    {
        this.pacman.gameObject.SetActive(false);

        SetLives(this.lives - 1);
        if(this.lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f);
        }
        else
        {
            GameOver();
        }
    }

    public void PelletEat(Pellet pellet){
        SetScore(this.score + pellet.points);
        pellet.gameObject.SetActive(false);
        if(!CheckPellet()){
            this.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3.0f);
        }
    }

    public void PowerPelletEat(PowerPellet pellet){
        // Change Ghost's state
        for(int i = 0; i < this.ghosts.Length; i++){
            this.ghosts[i].scared.Enable(pellet.duration);
        }
        PelletEat(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool CheckPellet(){
        foreach(Transform pellet in this.pellets)
        {
            if(pellet.gameObject.activeSelf){
                return true;
            }
        }
        return false;
    }

    private void ResetGhostMultiplier(){
        this.ghostMultiplier = 1;
    }
}
