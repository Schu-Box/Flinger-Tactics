using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawnExplosion : MonoBehaviour {

    private ParticleManager particleManager;
    private BallController spawnBall;
    private float appliedForce = 100f;

    private void Start() {
        particleManager = FindObjectOfType<ParticleManager>();
    }

    public IEnumerator ExpandAndExplode(BallController ball) {
        spawnBall = ball;

        Vector3 fullScale = transform.localScale;
        transform.localScale = Vector3.zero;

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        float duration = 0.3f;
        float timer = 0f;
        while(timer <= duration) {
            timer += Time.deltaTime;

            float step = timer/duration;

            transform.localScale = Vector3.Lerp(Vector3.zero, fullScale, step);

            yield return waiter;
        }

        GetComponent<SpriteRenderer>().color = Color.black;

        particleManager.PlayBallSpawnExplosionParticlePrefab(transform.position);

        timer = 0f;
        duration = 0.2f;
        while(timer <= duration) {
            timer += Time.deltaTime;

            float step = timer/duration;

            transform.localScale = Vector3.Lerp(fullScale, Vector3.zero, step);

            yield return waiter;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collie) {
        Rigidbody2D otherRB = collie.gameObject.GetComponent<Rigidbody2D>();

        Debug.Log("Trigger collision");

        if(otherRB != null) {
            BallController ball = collie.GetComponent<BallController>();
            if(ball == null || (ball != spawnBall)) {
                Vector2 target = collie.gameObject.transform.position;
                Vector2 direction = (target - (Vector2)transform.position).normalized;

                Debug.Log("Applying force!");

                otherRB.AddForce(direction * appliedForce);
            }
        }
    }
}
