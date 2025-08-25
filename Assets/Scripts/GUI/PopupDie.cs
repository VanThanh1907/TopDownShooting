using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopupDie : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject diePopUp;


    void Start()
    {
        exitButton.onClick.AddListener(OnExitClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        
    }
    void Update()
    {
      
    }

    public void OnExitClicked()
    {
        SetActiveFalse(diePopUp);
        SceneManager.LoadScene("Main");
        Time.timeScale = 1;
    }
   
    public void OnRestartClicked()
    {
        SceneManager.LoadScene("GamePlay");
        Time.timeScale = 1;
    }
   

    public void SetActiveTrue(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    public void SetActiveFalse(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
