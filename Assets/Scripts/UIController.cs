using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject controller;
    private GameController gameControl;//Reference to the game controller
    [SerializeField]
    private GameObject settingsMenu;//The settings menu that can be accessed pressing Escape
    [SerializeField]
    private TextMeshProUGUI points;//Points display
    [SerializeField]
    private TextMeshProUGUI force;//Force with which the stick is moved
    [SerializeField]
    private Slider volumeSlider;//Slider to change music volume
    [SerializeField]
    private Button saveSettings;//Save music volume setting
    [SerializeField]
    private Button newGame;
    [SerializeField]
    private Button saveGame;
    [SerializeField]
    private Button loadGame;
    [SerializeField]
    private Button quitGame;
    [SerializeField]
    private GameObject gameWonMessage;

    // Start is called before the first frame update
    void Start()
    {
        this.SetSettingsMenuActive(false);//Settings menu is always inactive on game start
        this.gameWonMessage.SetActive(false);//Game has not been won yet
        this.gameControl = this.controller.GetComponent<GameController>();
        //Add listeners for the slider and buttons
        this.volumeSlider.onValueChanged.AddListener(delegate { SliderValueChange(); });
        this.saveSettings.onClick.AddListener(delegate { SaveSetingsClicked(); });
        this.newGame.onClick.AddListener(delegate { NewGameClicked(); });
        this.saveGame.onClick.AddListener(delegate { SaveGameClicked(); });
        this.loadGame.onClick.AddListener(delegate { LoadGameClicked(); });
        this.quitGame.onClick.AddListener(delegate { QuitGameClicked(); });
    }

    //Change the slider's value
    public void SetSliderValue(float value)
    {
        this.volumeSlider.value = value;
    }

    //Change the SettingsMenu to active or inactive (visible or not)
    public void SetSettingsMenuActive(bool status)
    {
        this.settingsMenu.SetActive(status);
    }

    //Check status of the settings menu
    public bool GetSettingsMenuActive()
    {
        return this.settingsMenu.activeSelf;
    }

    //Update the points displayed on the screen
    public void UpdatePointsDisplay(int points)
    {
        this.points.text = "Points: " + points.ToString();
    }

    //Update the indicator of the force with with the stick is moved
    public void UpdateForceDisplay(float force)
    {
        this.force.text = "Force: " + int.Parse((force*100).ToString("000."));//Show force
    }

    //Tell the game controller to change the music volume
    private void SliderValueChange()
    {
        this.gameControl.VolumeChange(this.volumeSlider.value);
    }

    //Tell the game controller to save the settings
    private void SaveSetingsClicked()
    {
        this.gameControl.SaveSettings(this.volumeSlider.value);
    }

    //Tell the game controller to load a new game
    private void NewGameClicked()
    {
        this.gameControl.NewGame();
    }

    //Tell the game controller to save the current game
    private void SaveGameClicked()
    {
        this.gameControl.SaveGame();
    }

    //Tell the game controller to load the last saved game
    private void LoadGameClicked()
    {
        this.gameControl.LoadGame();
    }

    //Tell the game controller to quit the game
    private void QuitGameClicked()
    {
        this.gameControl.QuitGame();
    }

    public void GameWon()
    {
        this.gameWonMessage.SetActive(true);
    }
}
