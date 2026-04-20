using UnityEngine;

public class CutsceneFixer : MonoBehaviour
{
    public CharacterController playerController;
    public Transform yBotModel;

    // Tuto funkci zavolej přes Animation Event na konci animace vstávání
    public void FixOffsetAfterCutscene()
    {
        // 1. Vypneme controller, aby nás Unity neshodilo při teleportaci
        playerController.enabled = false;

        // 2. Přesuneme celý Player objekt (s kapslí) na aktuální pozici modelu
        Vector3 newPos = yBotModel.position;
        // Zachováme Y pozici hráče, abychom se nepropadli do země
        playerController.transform.position = new Vector3(newPos.x, playerController.transform.position.y, newPos.z);

        // 3. Resetujeme lokální pozici modelu zpět na střed kapsle
        yBotModel.localPosition = Vector3.zero;

        // 4. Zapneme controller a hráč hraje dál
        playerController.enabled = true;
    }
}