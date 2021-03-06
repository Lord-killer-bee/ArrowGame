﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class GameEventManager
    {
        private static GameEventManager m_instance;

        public static GameEventManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new GameEventManager();

                return m_instance;
            }
        }

        private delegate void EventDelegate(GameEvent e);

        public delegate void EventDelegate<T>(T e) where T : GameEvent;

        private Queue<GameEvent> eventQueue = new Queue<GameEvent>();
        bool isQueueProcessing = false;

        private Dictionary<Type, EventDelegate> listenerMap = new Dictionary<Type, EventDelegate>(); 

        public void AddListener<T>(EventDelegate<T> func) where T : GameEvent
        {
            EventDelegate convertedDel = (e) => func((T)e);

            if (!listenerMap.ContainsKey(typeof(T)))
            {       
                EventDelegate tempDel = null;
                tempDel += convertedDel;
                listenerMap.Add(typeof(T), tempDel);
            }
            else
            {
                listenerMap[typeof(T)] += convertedDel;
            }
        }

        public void RemoveListener<T>(EventDelegate<T> func) where T : GameEvent
        {
            EventDelegate convertedDel = (e) => func((T)e);

            if (!listenerMap.ContainsKey(typeof(T)))
            {
                return;
            }
            else
            {
                listenerMap[typeof(T)] -= convertedDel;

                if(listenerMap[typeof(T)] == null)
                {
                    listenerMap.Remove(typeof(T));
                }
            }
        }

        public void TriggerAsyncEvent(GameEvent eve)
        {
            TriggerEvent(eve);
        }

        public void TriggerSyncEvent(GameEvent eve)
        {
            eventQueue.Enqueue(eve);

            if (!isQueueProcessing)
                ProcessEventQueue();
        }

        private void TriggerEvent(GameEvent eve)
        {
            if(listenerMap.ContainsKey(eve.GetType()))
                listenerMap[eve.GetType()](eve);
        }

        private void ProcessEventQueue()
        {
            isQueueProcessing = true;

            TriggerEvent(eventQueue.Peek());

            eventQueue.Dequeue();

            if (eventQueue.Count > 0)
                ProcessEventQueue();
            else
                isQueueProcessing = false;
        }
    }

    public class GameEvent
    {

    }
}