using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class StartGame : MonoBehaviour
{
    public GameObject sliceableObjectPrefab;
    public InputActionReference action;
    private bool isSpawning = false;

    private Coroutine spawnCoroutine;
    public float force;
    public int strikes = 0;
    public int points = 0;
    private int highScore = 0;
    public TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        action.action.Enable();
        action.action.performed += GameStart;
    }
    void Update()
    {
        if (strikes >= 3)
        {
            StopGame();
        }
    }

    private void GameStart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSpawning = !isSpawning;

            if (isSpawning && !IsInvoking(nameof(StartSpawning)))
            {   
                points = 0;
                strikes = 0;
                UpdateScoreText();
                InvokeRepeating(nameof(StartSpawning), 0, 2);
            }
            else if (!isSpawning)
            {
                StopGame();
            }
        }
    }

    private void StopGame()
    {
        foreach (var o in GameObject.FindGameObjectsWithTag("sliceable"))
        {
            Destroy(o);
        }

        if (points > highScore)
        {
            highScore = points;
        }

        isSpawning = false;
        CancelInvoke(nameof(StartSpawning));
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        strikes = 0;
        points = 0;
        UpdateScoreText();
    }

    private void StartSpawning()
    {
        spawnCoroutine = StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        GameObject gameObject = Instantiate(sliceableObjectPrefab, new Vector3(0, 1f, 0.5f), Quaternion.identity);
        Rigidbody gameObjectRb = gameObject.GetComponent<Rigidbody>();

        float randX = Random.Range(-0.04f, 0.04f);

        Vector3 directionRandomizer = new(randX, 0, -0.04f);
        gameObjectRb.AddForce((gameObject.transform.up + directionRandomizer) * force, ForceMode.Impulse);
        gameObjectRb.AddTorque(Random.rotation.eulerAngles * 0.00004f, ForceMode.Impulse);
    }

    public void UpdateScoreText()
    {
        scoreText.text = $"Points: {points} \nStrikes: {strikes} \nHigh Score: {highScore}";
    }
}
