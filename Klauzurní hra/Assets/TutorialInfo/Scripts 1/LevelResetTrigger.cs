using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelResetTrigger : MonoBehaviour
{
    [Tooltip("Tag objektu, který mùže spustit reset (napø. hráè)")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Zkontroluje, zda do triggeru vlezl hráè (aby ho nespustil napø. poletující papír)
        if (other.CompareTag(playerTag))
        {
            // Znovu naète aktuální scénu
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}