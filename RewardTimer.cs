using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardTimer : MonoBehaviour
{
    [SerializeField] private int _waitTime;

    [SerializeField] private TMP_Text _timerText;
    
    private int _rewardTimeTotal;
    private int _rewardTimeDay;
    private int _rewardTimeHour;
    private int _rewardTimeMinute;
    private int _rewardTimeSecond;


    private int _startTimeTotal;

    private int _remainderTime;

    public bool CanItBeTakenReward;

    void Start()
    {
        GetTimerOnStartGame();

        _rewardTimeTotal = PlayerPrefs.GetInt("RewardTime", 0);
        _remainderTime = _waitTime - (_startTimeTotal - _rewardTimeTotal);
        if (_remainderTime > 0)
        {
            StartTimer();
        }
        else
        {
            _timerText.text = "Free Reward";
            CanItBeTakenReward = true;
        }

    }

    public void StartTimer()
    {
        StartCoroutine(CO_StartTimer());

    }

    public void GetRewardTime()
    {

        System.DateTime rewardTime = System.DateTime.Now;

        _rewardTimeDay = rewardTime.Day;
        _rewardTimeHour = rewardTime.Hour;
        _rewardTimeMinute = rewardTime.Minute;
        _rewardTimeSecond = rewardTime.Second;

        _rewardTimeTotal = CalculateTotalSecond(_rewardTimeDay, _rewardTimeHour, _rewardTimeMinute, _rewardTimeSecond);

        PlayerPrefs.SetInt("RewardTime", _rewardTimeTotal);
    }

    private IEnumerator CO_GetReward()
    {
        if(AdManager.Instance.RewardedAd.CanShowAd() && AdManager.Instance.RewardedAd != null && CanItBeTakenReward)
        {
            GetRewardTime();
            yield return new WaitForEndOfFrame();
            AdManager.Instance.ShowRewardedAd();
            yield return new WaitForSeconds(.3f);
            StartTimer();
        }
    }

    public void GetReward()
    {
        StartCoroutine(CO_GetReward());
    }

    private void GetTimerOnStartGame()
    {
        System.DateTime currentTime = System.DateTime.Now;

        int day = currentTime.Day;
        int hours = currentTime.Hour;
        int minute = currentTime.Minute;
        int second = currentTime.Second;

        _startTimeTotal = CalculateTotalSecond(day, hours, minute, second);
    }

    private int CalculateTotalSecond(int day, int hours, int minute, int second)
    {
        int totalSecond = (day * 24 * 60 * 60) + (hours * 60 * 60) + (minute * 60) + second;
        return totalSecond;
    }

    private IEnumerator CO_StartTimer()
    {
        CanItBeTakenReward = false;

        GetTimerOnStartGame();

        _rewardTimeTotal = PlayerPrefs.GetInt("RewardTime", 0);
        _remainderTime = _waitTime - (_startTimeTotal - _rewardTimeTotal);

        while (_remainderTime > 0)
        {
            yield return new WaitForSeconds(1f);
            _remainderTime--;

            int minute = _remainderTime / 60;
            int second = _remainderTime % 60;

            _timerText.text = minute.ToString("00") + ":" + second.ToString("00");

        }

        _timerText.text = "Free Reward";
        CanItBeTakenReward = true;
    }
}
