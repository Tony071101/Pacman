// using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public LayerMask wallLayer;
    public List<Vector2> availDirection {get; private set;}
    private void Start() {
        this.availDirection = new List<Vector2>();

        CheckAvailDirection(Vector2.up);
        CheckAvailDirection(Vector2.down);
        CheckAvailDirection(Vector2.left);
        CheckAvailDirection(Vector2.right);
    }
    private void CheckAvailDirection(Vector2 direction){
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, direction, 1.0f, this.wallLayer);
        if(hit.collider == null){
            this.availDirection.Add(direction);
        }
    } 
}
