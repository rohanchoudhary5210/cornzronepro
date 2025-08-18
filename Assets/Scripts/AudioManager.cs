using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour, IAudioManager
{

    public AudioClip[] sounds;
    public AudioSource MusicSource, SoundSource;
    public Sprite[] soundSprites; // 0=SoundOn, 1=SoundOff, 2=MusicOn, 3=MusicOff
    public Image musicIcon;
    public Image soundIcon;

    private void Awake()
    {
        UpdateAudioStates();
    }

    public void ToggleSound()
    {
        AudioController.IsSoundOn = !AudioController.IsSoundOn;
        UpdateAudioStates();
    }

    public void ToggleMusic()
    {
        AudioController.IsMusicOn = !AudioController.IsMusicOn;
        UpdateAudioStates();
    }

    private void UpdateAudioStates()
    {
        SoundSource.mute = !AudioController.IsSoundOn;
        MusicSource.mute = !AudioController.IsMusicOn;

        soundIcon.sprite = AudioController.IsSoundOn ? soundSprites[0] : soundSprites[1];
        musicIcon.sprite = AudioController.IsMusicOn ? soundSprites[2] : soundSprites[3];
    }

    public void PlayClip(int soundNum)
    {
        if(soundNum < sounds.Length)
        {
            SoundSource.PlayOneShot(sounds[soundNum]);
        }
    }
}

public interface IAudioManager
{
    void ToggleSound();
    void ToggleMusic();
    void PlayClip(int soundNum);
}