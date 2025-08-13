using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMainManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button backButton;


    void Start()
    {
        startButton.onClick.AddListener(OnPlayClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene("GamePlay");
    }
      public void OnBackClicked()
    {
        SceneManager.LoadScene("Menu");
    }

}
