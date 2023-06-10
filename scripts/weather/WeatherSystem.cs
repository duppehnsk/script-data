using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherSystem : MonoBehaviour
{
    [Header("Time")]
    public float seconds;
    public int minutes;
    public int hours;
    public float time_speed = 1;
    public Text Time_Text;
    

    void Update()
    {
        TimeManager();//Запуск Системы времени
        FixTime(); // Исправляем время на экране
    }

    void NextDay()
    {

    }

    void TimeManager()
    {
        Time_Text.text = hours + " : " + minutes;
        seconds += Time.deltaTime*time_speed;
        if (seconds >= (5f))
        {
            minutes++;
            seconds = 0f;
        }
        if (minutes > 59)
        {
            seconds = 0f;
            minutes = 0;
            hours++;
        }
        if (hours > 23)
        {
            seconds = 0f;
            minutes = 0;
            hours = 0;
            NextDay();//Просто пустышка для начала нового дня календаря
        }
    }
    void FixTime()
    {
        if (hours < 10)
        {
            if (minutes < 10)
            {
                Time_Text.text = "0" + hours + ":" + "0" + minutes;
            }
            else
                Time_Text.text = "0" + hours + ":" + minutes;
        }
        if (hours >= 10)
        {
            if (minutes >= 10)
            {
                Time_Text.text = hours + ":" + minutes;
            }
            else
                Time_Text.text = hours + ":" + "0" + minutes;
        }

    }
}