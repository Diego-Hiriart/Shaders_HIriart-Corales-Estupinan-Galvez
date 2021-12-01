using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private int score = 0;
    private List<BallBase> gameBalls = new List<BallBase>();//Balls in the game to keep track of
    [SerializeField]
    private GameObject Canvas;
    private UIController UI;//Controller for UI elements
    [SerializeField]
    private AudioSource backgroundMusic;
    [SerializeField]
    private GameObject cueStick;
    //Starting position the cue stick
    private Vector3 cueStickStartPos = new Vector3(0f, 0.35f, -1.8f);
    //Starting rotation the cue stick
    private Quaternion cueStickStartRot = new Quaternion(0.74314481f, 0, 0, 0.669130683f);
    private bool gameWon = false;

    //Key to store and retrieve background music volume setting
    private const string musicVolumeKey = "music_volume";

    // Start is called before the first frame update
    void Start()
    {
        this.UI = this.Canvas.GetComponent<UIController>();
        this.UI.SetSettingsMenuActive(false);//Settings menu is always inactive on game start
        this.LoadMusicVolume();
    }

    // Update is called once per frame
    void Update()
    {
        //Open or close settings menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!this.UI.GetSettingsMenuActive())//If not on pause, pause the game
            {
                Time.timeScale = 0;
                this.UI.SetSettingsMenuActive(true);
            }
            else
            {
                this.UI.SetSettingsMenuActive(false);
                Time.timeScale = 1;
            }
            
        }

        if (this.score == 15)//End game, it has been won
        {
            this.gameWon = true;
            Time.timeScale = 0;
            this.UI.GameWon();
        }
    }

    //Tell the UI controller to update the slider's position
    private void SetVolumeSliderValue(float value)
    {
        this.UI.SetSliderValue(value);
    }

    //Get the last saved value for the background music's volume
    public void LoadMusicVolume()
    {
        if (PlayerPrefs.HasKey(musicVolumeKey))
        {
            float value = PlayerPrefs.GetFloat("music_volume");
            this.backgroundMusic.volume = value;
            this.SetVolumeSliderValue(value);
        }
        else
        {
            float value = this.backgroundMusic.volume;
            this.SetVolumeSliderValue(value);
        }
    }

    //Change the background music's volume
    public void VolumeChange(float value)
    {
        this.backgroundMusic.volume = value;
    }

    //Save the current value for the background music's volume
    public void SaveSettings(float value)
    {
        PlayerPrefs.SetFloat(musicVolumeKey, value);
        PlayerPrefs.Save();
    }

    //Add a ball to the list
    public void AddBall(BallBase ball)
    {
        this.gameBalls.Add(ball);
    }

    //Load a new game
    public void NewGame()
    {
        SceneManager.LoadScene("PoolGame");//Reload the scene
        Time.timeScale = 1;
    }

    //Create an instance of SaveGameData to be serialized as a binary file
    private SaveGameData CreateSaveGameData()
    {
        List<BallInfo> ballsInfo = new List<BallInfo>();
        //Create a list of balls that can be used by the SaveGameData class
        foreach (BallBase ball in this.gameBalls)
        {
            ballsInfo.Add(new BallInfo(ball.GetPosX(), ball.GetPosY(), ball.GetPosZ(), ball.GetBallNumber()));
        }
        return new SaveGameData(this.score, ballsInfo);
    }

    //Serialize an instance of SaveGameData and save it as a binary file
    public void SaveGame()
    {
        SaveGameData save = this.CreateSaveGameData();

        //Save as binary so player can't cheat
        var bf = new BinaryFormatter();
        var filePath = Application.persistentDataPath + "/gamesave.data";

        var fs = File.Create(filePath);
        bf.Serialize(fs, save);
    }

    //Load a binary file and deserialize it a SaveGameData instance to put the balls and score as they where on the saved game
    public void LoadGame()
    {
        var filePath = Application.persistentDataPath + "/gamesave.data";

        if (File.Exists(filePath))
        {
            var bf = new BinaryFormatter();
            var fs = File.Open(filePath, FileMode.Open);
            var saveData = (SaveGameData)bf.Deserialize(fs);

            //Set the points equal to the ones the player had on the saved game
            this.score = saveData.GetPoints();
            //Place the balls on the positions they where on the saved game
            this.gameBalls = this.LoadBalls(saveData);
            //Reset the cue stick position and rotation
            this.cueStick.transform.position = this.cueStickStartPos;
            this.cueStick.transform.rotation = this.cueStickStartRot;
            //Set the score equal the the saved game's
            this.SetScore(this.score);
        }
    }

    //Use the saved data file to create a list of balls with the positions of the saved game
    private List<BallBase> LoadBalls(SaveGameData saveData)
    {
        List<BallBase> balls = new List<BallBase>();
        for (int i = 0; i<saveData.GetBallsStatus().Count; i++)
        {
            BallInfo ball = saveData.GetBallsStatus()[i];
            //Set the ball's position equal to that of the file's ball
            this.gameBalls[i].transform.position = new Vector3(ball.GetPosX(), ball.GetPosY(), ball.GetPosZ());
            //Set the ball's number  equal to that of the file's ball
            this.gameBalls[i].SetBallNumber(ball.GetBallNumber());
        }
        return balls;
    }

    //Quit game
    public void QuitGame()
    {
        Application.Quit();
    }

    //Update the score and tell the UI controller to update the points display
    public void SetScore(int score)
    {
        this.score = score;
        this.UI.UpdatePointsDisplay(this.score);
    }

    public int GetPoints()
    {
        return this.score;
    }

    //Increase points by 1 and tell the UI controller to update the points display
    public void AddPoint()
    {
        this.score++;
        this.UI.UpdatePointsDisplay(this.score);
    }
  
    public bool IsGameWon()
    {
        return this.gameWon;
    }
}
