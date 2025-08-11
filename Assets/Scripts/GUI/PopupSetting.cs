using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene("Main");
    }


    public void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Thoát trong Editor
#else
        Application.Quit(); // Thoát game khi build
#endif
    }
    

  
}