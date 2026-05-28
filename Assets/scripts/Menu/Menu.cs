using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{

    public void ChangeSliderVolume(float valorVolumen)
    {
        PlayerPrefs.SetFloat("volumenAudio", valorVolumen);
        PlayerPrefs.Save();

        AudioListener.volume = valorVolumen;
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManagerGeneral.Instancia.InicioJuego();
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Said I quit");
    }

}