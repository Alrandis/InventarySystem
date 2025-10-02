using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public bool IsPaused { get; private set; }

    // Событие для подписчиков
    public event Action<bool> OnPauseChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TogglePause()
    {
        SetPause(!IsPaused);
    }

    public void SetPause(bool pause)
    {
        if (IsPaused == pause) return;

        IsPaused = pause;

        // Если нужно, блокируем Time.timeScale
        Time.timeScale = pause ? 0f : 1f;

        OnPauseChanged?.Invoke(IsPaused);
    }
}

