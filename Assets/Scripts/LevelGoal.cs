
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour
{
    private bool levelCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("FinishLine touched by: " + other.name);

        Transform player = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform.root;

        if (!other.CompareTag("Player") &&
            !player.CompareTag("Player"))
            return;

        if (levelCompleted) return;

        string currentLevel = SceneManager.GetActiveScene().name;
        string nextLevel = "";

        if (currentLevel == "Level1")
            nextLevel = "Level2";
        else if (currentLevel == "Level2")
            nextLevel = "Level3";
        else if (currentLevel == "Level3")
            nextLevel = "EndScene";

        if (nextLevel == "")
        {
            Debug.LogError("Unknown level: " + currentLevel);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextLevel))
        {
            Debug.LogError("Scene not in Build Profiles: " + nextLevel);
            return;
        }

        levelCompleted = true;

        Debug.Log("Loading " + nextLevel);
        SceneManager.LoadScene(nextLevel);
    }
}
