using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        Score, Speed, Jump, ScoreBoost
    }

    public PickupType type;
    private Vector3 startingPosition;
    private bool available = true;

    void Start()
    {
        startingPosition = transform.position;

        Color color = Color.yellow;

        if (type == PickupType.Speed)
            color = new Color(1f, 0.5f, 0f);
        if (type == PickupType.Jump)
            color = Color.blue;
        if (type == PickupType.ScoreBoost)
            color = Color.magenta;

        GetComponent<Renderer>().material.color = color;
    }

    void Update()
    {
        transform.Rotate(0, 90 * Time.deltaTime, 0);

        transform.position = startingPosition +
            Vector3.up * Mathf.Sin(Time.time * 3f) * 0.25f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!available || !other.CompareTag("Player"))
            return;

        CharacterMovement player =
            other.GetComponent<CharacterMovement>();

        if (type == PickupType.Score)
        {
            GameManager.Instance.AddScore(50);
            Destroy(gameObject);
        }
        else if (type == PickupType.Jump)
        {
            StartCoroutine(JumpBoost(player));
        }
        else if (type == PickupType.Speed)
        {
            StartCoroutine(SpeedBoost(player));
        }
        else if (type == PickupType.ScoreBoost)
        {
            GameManager.Instance.scoreBoostUntil =
                Time.time + 10f;
            StartCoroutine(Respawn());
        }
    }

    IEnumerator JumpBoost(CharacterMovement player)
    {
        player.canDoubleJump = true;
        StartCoroutine(Respawn());

        yield return new WaitForSeconds(30f);

        if (player != null)
            player.canDoubleJump = false;
    }

    IEnumerator SpeedBoost(CharacterMovement player)
    {
        player.walkSpeed *= 1.5f;
        player.runSpeed *= 1.5f;

        StartCoroutine(Respawn());

        yield return new WaitForSeconds(5f);

        if (player != null)
        {
            player.walkSpeed /= 1.5f;
            player.runSpeed /= 1.5f;
        }
    }

    IEnumerator Respawn()
    {
        available = false;

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(30f);

        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        available = true;
    }
}
