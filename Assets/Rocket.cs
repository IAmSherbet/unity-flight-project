using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;
    bool collisionsDisabled = false;

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
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild) // !! this ensures if we turn off development build when building, debug keys won't work!
        {
            ToggleCollisions();
            SkipLevel();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionsDisabled) { return; } // function stops here if dead or debugging = ignore collisions when dead

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
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(obstacleCollision);
        Invoke("HandleDeath", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int finalSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        int nextSceneIndex;

        if (currentSceneIndex == finalSceneIndex)
        {
            nextSceneIndex = 0;    
        } else
        {
            nextSceneIndex = currentSceneIndex + 1;
        }

        SceneManager.LoadScene(nextSceneIndex); // todo: allow for more than 2 levels, 
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
    }

    private void ToggleCollisions()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; // sets collisionsDisabled to the opposite of its value
        }
    }

    private void SkipLevel()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
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
        rigidBody.angularVelocity = Vector3.zero; // remove rotation due to physics

        float rotationThisFrame = rotationThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            
            transform.Rotate(Vector3.forward * rotationThisFrame); // transform is a method that Unity provides us

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }
}
