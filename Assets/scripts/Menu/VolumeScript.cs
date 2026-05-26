using UnityEngine;
using UnityEngine.UI;

public class volumen : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        AudioListener.volume = slider.value;
    }

    public void ChangeSlider(float valor)
    {
        PlayerPrefs.SetFloat("volumenAudio", valor);
        PlayerPrefs.Save();

        AudioListener.volume = valor;
    }
}