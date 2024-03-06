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
        yield return new WaitForSeconds(Random.Range(1f, 4f));

        GameObject gameObject = Instantiate(sliceableObjectPrefab, new Vector3(0, 1f, 0), Quaternion.identity);
        Rigidbody gameObjectRb = gameObject.GetComponent<Rigidbody>();

        float force = 9.5f;
        float randX = Random.Range(-0.1f, 0.1f);
        Vector3 directionRandomizer = new(randX, 0, -0.05f);
        gameObjectRb.AddForce((gameObject.transform.up + directionRandomizer) * force, ForceMode.Impulse);
    }

    public void UpdateScoreText()
    {
        scoreText.text = $"Points: {points} \nStrikes: {strikes} \nHigh Score: {highScore}";
    }
}
