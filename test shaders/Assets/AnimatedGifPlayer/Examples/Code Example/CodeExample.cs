using UnityEngine;
using UnityEngine.UI;
using OldMoatGames;

public class CodeExample : MonoBehaviour {
    private AnimatedGifPlayer AnimatedGifPlayer;

    public Button PlayButton;
    public Button PauseButton;

    public void Awake() {
        // Get the GIF player component
        AnimatedGifPlayer = GetComponent<AnimatedGifPlayer>();

        // Set the file to use. File has to be in StreamingAssets folder or a remote url (For example: http://www.example.com/example.gif).
        AnimatedGifPlayer.FileName = "AnimatedGIFPlayerExampe 3.gif";

        // Disable autoplay
        AnimatedGifPlayer.AutoPlay = false;

        // Add ready event to start play when GIF is ready to play
        AnimatedGifPlayer.OnReady += OnGifLoaded;
        
        // Add ready event for when loading has failed
        AnimatedGifPlayer.OnLoadError += OnGifLoadError;

        // Init the GIF player
        AnimatedGifPlayer.Init();
    
    }

    private void OnGifLoaded() {
        PlayButton.interactable = true;

        Debug.Log("GIF size: width: " + AnimatedGifPlayer.Width + "px, height: " + AnimatedGifPlayer.Height + " px");
    }

    private void OnGifLoadError() {
        Debug.Log("Error Loading GIF");
    }

    public void Play() {
        // Start playing the GIF
        AnimatedGifPlayer.Play();

        // Disable the play button
        PlayButton.interactable = false;

        // Enable the pause button
        PauseButton.interactable = true;
    }

    public void Pause() {
        // Stop playing the GIF
        AnimatedGifPlayer.Pause();

        // Enable the play button
        PlayButton.interactable = true;

        // Disable the pause button
        PauseButton.interactable = false;
    }

    public void OnDisable() {
        AnimatedGifPlayer.OnReady -= Play;
    }
}
