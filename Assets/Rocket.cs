using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive; // use debug mode to expose this in Unity Inspector

    [SerializeField] float rotationThrust = 100f;
    [SerializeField] float mainThrust = 1750f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip obstacleCollision;
    [SerializeField] AudioClip levelCompleted;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>(); // Unity provided function. Gets all components of type RigidBody within context
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; } // function stops here if dead = ignore collisions when dead

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(obstacleCollision);
        Invoke("HandleDeath", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelCompleted);
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void HandleDeath()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Staircase"); // todo: allow for more than 2 levels, 
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame); // Position of the ship is a vector three, three floating point numbers

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void RespondToRotateInput()
    {
        float rotationThisFrame = rotationThrust * Time.deltaTime;

        rigidBody.freezeRotation = true; // take manual control of rotation, suspend physics

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            
            transform.Rotate(Vector3.forward * rotationThisFrame); // transform is a method that Unity provides us

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
}
