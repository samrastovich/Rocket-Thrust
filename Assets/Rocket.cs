using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip levelUpSound;
    [SerializeField] AudioClip deathSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;


    Rigidbody rigidBody;
    AudioSource audioSource;
    bool engineIsRunning = false;

    enum State
    {
        Alive, Dying, Transcending
    }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
    }

    void FixedUpdate()
    {
        if (state == State.Alive)
        {
            Rotate();
            Thrust();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Enemy":
                state = State.Dying;
                audioSource.PlayOneShot(deathSound);
                deathParticles.Play();
                Invoke("LoadFirstScene", 1f);
                break;
            case "Friendly":
                state = State.Alive;
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.PlayOneShot(levelUpSound);
                successParticles.Play();
                Invoke("LoadNextScene", 1f);
                break;
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }
        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up);
            if (!engineIsRunning)
            {
                audioSource.PlayOneShot(mainEngine);
                engineIsRunning = true;
                mainEngineParticles.Play();
            }
        }
        else
        {
            audioSource.Stop();
            engineIsRunning = false;
            mainEngineParticles.Stop();
        }
    }
}