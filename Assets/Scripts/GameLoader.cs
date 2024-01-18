using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{

    [SerializeField] private Button startButton;

    private void Awake()
    {
        startButton.onClick.AddListener(() =>
        {
            SceneData.nextScene = "GameScene";
            SceneManager.LoadScene("LoadingScene");
        });
    }
}
