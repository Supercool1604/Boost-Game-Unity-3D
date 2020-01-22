using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip MainThrusting;
    [SerializeField] AudioClip DeathExplosion;
    [SerializeField] AudioClip NextLevel;
    [SerializeField] ParticleSystem MainThrustingParticle;
    [SerializeField] ParticleSystem DeathExplosionParticle;
    [SerializeField] ParticleSystem NextLevelParticle;
    Rigidbody rocketBody;
    AudioSource audioSource;
    
    enum State { alive, transcending, dying};
    State state = State.alive;
    
    void Start()
    {
        rocketBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.alive)
        {
            thrustFun();
            rotate();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(state!=State.alive) { return; }
        switch (collision.gameObject.tag) 
        {
            case "Friendly":
                break;
            case "Finish":
                ProcessLevelCompletion();
                break;
            default:
                ProcessDeath();
                break;
        }
    }

    private void ProcessDeath()
    {
        state = State.dying;
        audioSource.Stop();
        audioSource.PlayOneShot(DeathExplosion);
        DeathExplosionParticle.Play();
        Invoke("LoadFirstLevel", 1f);
    }

    private void ProcessLevelCompletion()
    {
        state = State.transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(NextLevel);
        NextLevelParticle.Play();
        Invoke("LoadNextScene", 1f);
    }

    private  void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private  void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    private void rotate()
    {
        // thrust();
        rocketBody.freezeRotation = true;
       
        float rotationSpeed = Time.deltaTime * rcsThrust;
        if (Input.GetKey(KeyCode.A))
        {
           
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }
        rocketBody.freezeRotation = false;
    }

    private void thrustFun()
    {
        ApplyThrust();
    }

    private void ApplyThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ThrustingNow();

        }
        else
        {
            audioSource.Stop();
            MainThrustingParticle.Stop();
        }
    }

    private void ThrustingNow()
    {
       
        float mainSpeed = Time.deltaTime * mainThrust;
        rocketBody.AddRelativeForce(Vector3.up * mainSpeed);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(MainThrusting);
        }
        MainThrustingParticle.Play();

    }
}
