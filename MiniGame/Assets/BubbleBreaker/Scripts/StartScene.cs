using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
  
    // Use this for initialization
    void Start()
    {
        Globals.GameScore = 0;
        (GameObject.Find("SoundToggle").GetComponent<Toggle>()).isOn = SettingsManager.Sound;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void GoToGameScene()
    {
        SceneManager.LoadScene("bubbleBreakerGameScene");
    }

    public void GoToHighScoreScene()
    {
        SceneManager.LoadScene("highScoresScene");
    }

    public void SetSound(bool value)
    {
        SettingsManager.Sound = value;
    }

   


}
