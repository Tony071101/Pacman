// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class GhostScatter : GhostBehaviour
{
    private void OnDisable()
    {
        this.ghost.chase.Enable();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();

        if(node != null && this.enabled && !this.ghost.scared.enabled){
            int index = Random.Range(0, node.availDirection.Count);

            if(node.availDirection[index] == -this.ghost.movement.direction && node.availDirection.Count > 1){
                index++;

                if(index >= node.availDirection.Count){
                    index = 0;
                }
            }

            this.ghost.movement.SetDirection(node.availDirection[index]);
        }
    }
}
