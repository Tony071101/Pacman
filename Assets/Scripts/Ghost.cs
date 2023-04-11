//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Movement movement {get; private set;}
    public GhostHome home {get; private set;}
    public GhostChase chase {get; private set;}
    public GhostScared scared {get; private set;}
    public GhostScatter scatter {get; private set;}
    public GhostBehaviour initialBehaviour;
    public Transform target;
    public int points = 200;
    private void Awake() {
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<GhostHome>();
        this.chase = GetComponent<GhostChase>();
        this.scared = GetComponent<GhostScared>();
        this.scatter = GetComponent<GhostScatter>();
    }
    
    private void Start() {
        ResetState();    
    }

    public void ResetState(){
        this.gameObject.SetActive(true);
        this.movement.ResetState();

        this.scared.Disable();
        this.chase.Disable();
        this.scatter.Enable();
        // if(this.scatter != this.initialBehaviour){
        //     this.scatter.Disable();
        // }
        // else if(this.scatter == this.initialBehaviour){
        //     this.scatter.Enable();
        // }
        if(this.home != this.initialBehaviour){
            this.home.Disable();
        }
        if(this.initialBehaviour != null){
            this.initialBehaviour.Enable();
        }
        // else if(this.home == this.initialBehaviour){
        //     this.home.Enable();
        // }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Pacman")){
            if(this.scared.enabled){
                FindObjectOfType<GameManager>().GhostEat(this);
            }else{
                FindObjectOfType<GameManager>().PacmanEat();
            }
        }
    }

}
