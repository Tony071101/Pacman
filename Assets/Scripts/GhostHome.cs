using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class GhostHome : GhostBehaviour
{
    public Transform inside;
    public Transform outside;

    public void OnEnable() {
        StopAllCoroutines();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(this.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Wall")){
            this.ghost.movement.SetDirection(-this.ghost.movement.direction);
        }
    }

    public void OnDisable() {
        if(this.gameObject.activeSelf){
            StartCoroutine(ExitTransition());        
        }
    }
    private IEnumerator ExitTransition(){
        this.ghost.movement.SetDirection(Vector2.up, true);
        this.ghost.movement.rigidbody.isKinematic = true;
        this.ghost.movement.enabled = false;

        Vector3 pos = this.transform.position;

        float duration = 0.5f;
        float elapsed = 0.0f;

        while(elapsed < duration){
            Vector3 newPos = Vector3.Lerp(pos, this.inside.position, elapsed / duration);
            newPos.z = pos.z;
            this.ghost.transform.position = newPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0.0f;

        while(elapsed < duration){
            Vector3 newPos = Vector3.Lerp(this.inside.position, this.outside.position, elapsed / duration);
            newPos.z = pos.z;
            this.ghost.transform.position = newPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        this.ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f), true);
        this.ghost.movement.rigidbody.isKinematic = false;
        this.ghost.movement.enabled = true;
    }
}
