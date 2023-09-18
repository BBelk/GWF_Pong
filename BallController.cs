using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float speed = 2.0f;
    private Rigidbody rb;
    public int lastHitIndex;
    public List<ParticleSystem> allParticleSystems;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ResetBall();
    }
    void Update()
    {
        rb.velocity = rb.velocity.normalized * speed;
    }
    public void StartMovement()
    {
        float[] angles = { 45f, 135f, 225f, 315f };
        float angle = angles[Random.Range(0, angles.Length)];

        float radians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians);
        float y = Mathf.Sin(radians);

        Vector3 initialVelocity = new Vector3(speed * x, speed * y, 0);

        rb.velocity = initialVelocity;
        allParticleSystems[0].Play();
        allParticleSystems[1].Play();
        
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        this.gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    private void OnCollisionEnter(Collision other)
    {
            if(other.gameObject.GetComponent<PaddleController>() != null){
                lastHitIndex = other.gameObject.GetComponent<PaddleController>().playerIndex;
            }
    }

    public void ResetBall()
    {
        rb.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0f, 0f, 0.25f);
        foreach(ParticleSystem newPS in allParticleSystems){
            newPS.Stop();
        }
    }

    public void Scored(){
        rb.velocity = Vector3.zero;
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<SphereCollider>().enabled = false;
        allParticleSystems[0].Play();
        allParticleSystems[1].Stop();

    }
}
