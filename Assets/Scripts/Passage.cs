// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class Passage : MonoBehaviour
{
    public Transform conn;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 position =  other.transform.position;
        position.x = this.conn.position.x;
        position.y= this.conn.position.y; 
        other.transform.position = position;
    }
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
