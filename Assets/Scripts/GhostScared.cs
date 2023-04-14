// using System.Collections;
// using System.Collections.Generic;
using System;
using UnityEngine;

public class GhostScared : GhostBehaviour
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blueState;
    public SpriteRenderer whiteState;

    public AudioSource ghostScared;
    public bool eaten { get; private set; }

    public override void Enable(float duration)
    {
        base.Enable(duration);

        this.body.enabled = false;
        this.eyes.enabled = false;
        this.blueState.enabled = true;
        this.whiteState.enabled = false;
        ghostScared.Play();
        Invoke(nameof(Flash), duration / 2.0f);
    }

    public override void Disable()
    {
        base.Disable();

        this.body.enabled = true;
        this.eyes.enabled = true;
        this.blueState.enabled = false;
        this.whiteState.enabled = false;
        ghostScared.Stop();
    }

    private void Flash()
    {
        if (!this.eaten)
        {
            this.blueState.enabled = false;
            this.whiteState.enabled = true;
            this.whiteState.GetComponent<AnimatedSprite>().Restart();
        }
    }

    private void Eaten(){
        this.eaten = true;

        Vector3 pos = this.ghost.home.inside.position;
        pos.z = this.transform.position.z;
        this.ghost.transform.position = pos;

        this.ghost.home.Enable(this.duration);

        this.body.enabled = false;
        this.eyes.enabled = true;
        this.blueState.enabled = false;
        this.whiteState.enabled = false;
    }

    private void OnEnable()
    {
        blueState.GetComponent<AnimatedSprite>().Restart();
        this.ghost.movement.speedMultiplier = 0.5f;
        this.eaten = false;
    }

    private void OnDisable() {
        this.ghost.movement.speedMultiplier = 1f;
        eaten = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Pacman")){
            if(this.enabled){
                Eaten();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();

        if(node != null && this.enabled){
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue;

            foreach(Vector2 availDirection in node.availDirection){
                Vector3 newPos = this.transform.position + new Vector3(availDirection.x, availDirection.y, 0.0f);
                float distance = (this.ghost.target.position - newPos).sqrMagnitude;

                if(distance > maxDistance){
                    direction = availDirection;
                    maxDistance = distance;
                }
            }

            this.ghost.movement.SetDirection(direction);
        }
    }
}
