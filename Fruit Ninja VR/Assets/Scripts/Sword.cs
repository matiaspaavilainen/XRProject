using UnityEngine;

public class Sword : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }
    public Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }
}
