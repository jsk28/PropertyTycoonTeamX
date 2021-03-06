using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsPopUp : View.PopUp
{
    public Slider sfx;
    public Slider music;
    public Slider tokenSpeed;
    public TMPro.TMP_Dropdown VideoSettings;

    public static OptionsPopUp Create(Transform parent)
    {
        OptionsPopUp popUp = Instantiate(Asset.OptionsPopUpPreFab, parent).GetComponent<OptionsPopUp>();
        popUp.btn1.onClick.AddListener(() => popUp.closePopup());
        popUp.VideoSettings.value = QualitySettings.GetQualityLevel();
        return popUp;
    }

    public void Start()
    {
        sfx.value = SoundManager.sfxVolume;
        music.value = SoundManager.musicVolume;
        tokenSpeed.value = View.Piece.SPEED;
    }

    public void ChangeMusicVolume(float value)
    {
        SoundManager.musicVolume = value;
        soundManager.musicMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(value) * 20);
    }
    
    public void ChangeSFXVolume(float value)
    {
        SoundManager.sfxVolume = value;
        soundManager.soundMixerGroup.audioMixer.SetFloat("SFX Volume", Mathf.Log10(value) * 20);
    }

    public void ChangeTokenSpeed(float value)
    {
        View.Piece.SPEED = value;
    }

    public void ChangeVideoQuality(int value)
    {
        QualitySettings.SetQualityLevel(value, true);
    }
}
