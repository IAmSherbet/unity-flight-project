using UnityEngine;

[DisallowMultipleComponent]

public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    // todo: remove from inspector later
    [Range(0,1)] [SerializeField] float movementFactor; // 0 for not moved, 1 for fully moved

    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // protect against period = 0 to prevent 0 dividing by 0
        if (period <= Mathf.Epsilon) { return; } // Mathf.Epsilon = the smallest float possible to calculate

        float cycles = Time.time / period; // grows continually from zero

        const float tau = Mathf.PI * 2f; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // generates number from -1 to +1

        movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
