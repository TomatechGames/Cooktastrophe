using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    static int startAtDay = 0;
    public static int StartAtDay => startAtDay;
    [SerializeField]
    Slider daySlider;
    [SerializeField]
    TextMeshProUGUI dayLabel;

    private void Start()
    {
        var maxDayCount = PlayerPrefs.GetInt("maxDays");
        if (maxDayCount <= 1)
        {
            daySlider.gameObject.SetActive(false);
            daySlider.value = 1;
        }
        else
          daySlider.maxValue = maxDayCount;
    }

    public void SetDayLabel(float value)
    {
        dayLabel.text = "Start Day " + (int)value;
    }

    public void StartTutorial()
    {
        startAtDay = 0;
        SceneManager.LoadScene(1);
    }
    public void StartDay()
    {
        startAtDay = (int)daySlider.value;
        SceneManager.LoadScene(1);
    }
}
