using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System;
using System.IO;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance;
    public VideoPlayer videoPlayer;
    public TextMeshProUGUI DebugText;

    public static long currentFrame;
    public static ulong totalFrames;
    public static double currentTime;
    public static double totalDuration;
    public static double targetTime;

    public static Action<double> CurrentTimeChanged;
    public static Action VideoLoaded;
    public static bool VideoIsLoaded=false;
     

    private void Awake()
    {
        Instance = this; 
        Instance.videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnDestroy()
    {
        Instance.videoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    private void Update()
    {
        if (currentTime != videoPlayer.time)
        {
            currentTime = videoPlayer.time;
            InvokeTimeChangedEvent(); 
        }
        currentFrame = videoPlayer.frame;
        totalFrames = videoPlayer.frameCount;

        totalDuration = videoPlayer.length;

        string debugInfo = $"Current Frame: {currentFrame}/{totalFrames}\nCurrent Time: {currentTime:N2}/{totalDuration:N2} seconds";
        DebugText.text = (debugInfo);
        CheckTargetTime(); 
    }

    public static void InvokeTimeChangedEvent()
    {
        CurrentTimeChanged?.Invoke(currentTime);
    }

    private void CheckTargetTime()
    {
        if (videoPlayer.isPaused)
        {
            SetTime(targetTime);
            videoPlayer.Play();
            videoPlayer.Pause();
        }
        else
        {
            targetTime = currentTime;
        }
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void ChangeSpeed(float _speed)
    {
        videoPlayer.playbackSpeed = _speed;
    }     

    public void StepFrame(int _frames)
    {
        videoPlayer.Pause();
        float roundingOffset = .2f;
        float frameDuration = 1.0f / videoPlayer.frameRate;
        targetTime = videoPlayer.time + ((_frames + roundingOffset) * frameDuration);
    }

    public  void SetTime(double _time)
    {
        _time = Mathf.Clamp((float)_time, 0, (float)totalDuration - .5f);
        videoPlayer.time = _time;
        targetTime = _time;
    }

    public static void LoadVideo(string _filename)
    {
        try
        {
            Instance.videoPlayer.url = _filename;
            Instance.videoPlayer.Prepare();
        }
        catch (Exception e)
        {
            Debug.Log($"Issue loading {_filename} : e.message");
            
        }
    }
    private static void OnVideoPrepared(VideoPlayer source)
    {
        VideoLoaded?.Invoke();
        VideoIsLoaded = true; //currently once a video is loaded the only way to unload it is to load another video so this is on after the first load
    }

    public  bool IsPaused()
    {
        if (videoPlayer!=null)
        {
            return videoPlayer.isPaused;
        }
        return true;
    }
}
