// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class GhostChase : GhostBehaviour
{
    private void OnDisable()
    {
        this.ghost.scatter.Enable();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();

        if(node != null && this.enabled && !this.ghost.scared.enabled){
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            foreach(Vector2 availDirection in node.availDirection){
                Vector3 newPos = this.transform.position + new Vector3(availDirection.x, availDirection.y, 0.0f);
                float distance = (this.ghost.target.position - newPos).sqrMagnitude;

                if(distance < minDistance){
                    direction = availDirection;
                    minDistance = distance;
                }
            }

            this.ghost.movement.SetDirection(direction);
        }
    }
}
