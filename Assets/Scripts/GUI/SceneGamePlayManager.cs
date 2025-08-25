using System.Collections;
using UnityEngine;

public class SceneGamePlayManager : MonoBehaviour
{
    [SerializeField] private GameObject popupDie;
    [SerializeField] private Health playerHealth;
    [SerializeField] private float diePopupDelay = 0.5f; 

    void Start()
    {
    
        popupDie.SetActive(false);
        playerHealth.onDeath.AddListener(OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        StartCoroutine(ShowDiePopupAfterDelay());
    }

    private IEnumerator ShowDiePopupAfterDelay()
    {
        yield return new WaitForSeconds(diePopupDelay);
        popupDie.SetActive(true);
        Time.timeScale = 0;
    }
}
