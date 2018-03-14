using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TimerManager : MonoBehaviour
{
    public static TimerManager instance = null;

    List<Timer> timers;

    List<int> removalPending;

    private int idCounter;

    class Timer
    {
        public int id;
        public bool isActive;

        public float rate;
        public int ticks;
        public int ticksElapsed;
        public float last;
        public Action callBack;

        public Timer(int id_, float rate_, int ticks_, Action callback_)
        {
            id = id_;
            rate = rate_ < 0 ? 0 : rate_;
            ticks = ticks_ < 0 ? 0 : ticks_;
            callBack = callback_;
            last = 0;
            ticksElapsed = 0;
            isActive = true;
        }

        public void Tick()
        {
            last += Time.deltaTime;

            if (isActive && last >= rate)
            {
                last = 0;
                ticksElapsed++;
                callBack.Invoke();

                if (ticks > 0 && ticks == ticksElapsed)
                {
                    isActive = false;
                    TimerManager.instance.RemoveTimer(id);
                }
            }
        }
    }

    void Awake()
    {
        instance = this;

        timers = new List<Timer>();

        removalPending = new List<int>();

        DontDestroyOnLoad(gameObject);
    }

    public int AddTimer(float rate, Action callback)
    {
        return AddTimer(rate, 0, callback);
    }

    public int AddTimer(float rate, int ticks, Action callback)
    {
        Timer newTimer = new Timer(++idCounter, rate, ticks, callback);
        timers.Add(newTimer);
        return newTimer.id;
    }

    public void RemoveTimer(int timerId)
    {
        removalPending.Add(timerId);
    }

    void Remove()
    {
        if (removalPending.Count > 0)
        {
            foreach (int id in removalPending)
            {
                for (int i = 0; i < timers.Count; i++)
                {
                    if (timers[i].id == id)
                    {
                        timers.RemoveAt(i);
                        break;
                    }
                }
            }
            removalPending.Clear();
        }
    }

    void Tick()
    {
        for (int i = 0; i < timers.Count; i++)
        {
            timers[i].Tick();
        }
    }

    void Update()
    {
        Remove();
        Tick();
    }

}