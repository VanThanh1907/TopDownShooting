using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopupPause : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pausePopUp;



    void Start()
    {
        exitButton.onClick.AddListener(OnExitClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        resumeButton.onClick.AddListener(OnResumeClicked);
        pauseButton.onClick.AddListener(OnpauseClicked);
    }

    public void OnExitClicked()
    {
        SetActiveFalse(pausePopUp);
        SceneManager.LoadScene("Main");
        Time.timeScale = 1;
    }
     public void OnResumeClicked()
    {
        SetActiveFalse(pausePopUp);
        Time.timeScale = 1;
    }
    public void OnRestartClicked()
    {
        SceneManager.LoadScene("GamePlay");
        Time.timeScale = 1;
    }
    public void OnpauseClicked()
    {
        SetActiveTrue(pausePopUp);
        Time.timeScale = 0;
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

