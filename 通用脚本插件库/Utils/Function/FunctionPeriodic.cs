using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ִ��
/// </summary>
public class FunctionPeriodic
{

    /// <summary>
    /// �������ҹ���MonoBehaviour����
    /// </summary>
    private class MonoBehaviourHook : MonoBehaviour
    {
        /// <summary>
        /// �ڸ���ί��
        /// </summary>
        public Action OnUpdate;

        private void Update()
        {
            if (OnUpdate != null) OnUpdate();
        }

    }

    /// <summary>
    /// ���������л��ʱ��������
    /// </summary>
    private static List<FunctionPeriodic> funcList;
    /// <summary>
    /// ���ڳ�ʼ�����ȫ����Ϸ�����ڳ�������ʱ������
    /// </summary>
    private static GameObject initGameObject;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    private static void InitIfNeeded()
    {
        if (initGameObject == null)
        {
            initGameObject = new GameObject("FunctionPeriodic_Global");
            funcList = new List<FunctionPeriodic>();
        }
    }



    /// <summary>
    /// ����ͨ�ó�������
    /// </summary>
    /// <param name="action">ί��</param>
    /// <param name="testDestroy">�Ƿ����ٶ���ί��</param>
    /// <param name="timer">ʱ��</param>
    /// <returns></returns>
    public static FunctionPeriodic Create_Global(Action action, Func<bool> testDestroy, float timer)
    {
        FunctionPeriodic functionPeriodic = Create(action, testDestroy, timer, "", false, false, false);
        MonoBehaviour.DontDestroyOnLoad(functionPeriodic.gameObject);
        return functionPeriodic;
    }


    /// <summary>
    /// ����ָ��ʱ��[timer]����ί��[action]���ڴ���������ִ��[testDestroy]���������true������
    /// </summary>
    /// <param name="action">ί��</param>
    /// <param name="testDestroy">�Ƿ����ٶ���ί��</param>
    /// <param name="timer">ʱ��</param>
    /// <returns></returns>
    public static FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer)
    {
        return Create(action, testDestroy, timer, "", false);
    }
    /// <summary>
    /// ����ָ��ʱ��[timer]����ί��[action]
    /// </summary>
    /// <param name="action">ί��</param>
    /// <param name="timer">ʱ��</param>
    /// <returns></returns>
    public static FunctionPeriodic Create(Action action, float timer)
    {
        return Create(action, null, timer, "", false, false, false);
    }
    /// <summary>
    /// ����ָ��ʱ��[timer]����ί��[action]���ڴ���������ִ��[functionName]����
    /// </summary>
    /// <param name="action">ί��</param>
    /// <param name="timer">ʱ��</param>
    /// <param name="functionName">��������</param>
    /// <returns></returns>
    public static FunctionPeriodic Create(Action action, float timer, string functionName)
    {
        return Create(action, null, timer, functionName, false, false, false);
    }
    /// <summary>
    /// ����ָ��ʱ��[timer]����ί��[action]���ڴ���������ִ��[testDestroy]���������true������
    /// </summary>
    /// <param name="callback">����ί��</param>
    /// <param name="testDestroy">�Ƿ����ٶ���ί��</param>
    /// <param name="timer">ʱ��</param>
    /// <param name="functionName">��������</param>
    /// <param name="stopAllWithSameName">ֹͣȫ����������</param>
    /// <returns></returns>
    public static FunctionPeriodic Create(Action callback, Func<bool> testDestroy, float timer, string functionName, bool stopAllWithSameName)
    {
        return Create(callback, testDestroy, timer, functionName, false, false, stopAllWithSameName);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <param name="testDestroy"></param>
    /// <param name="timer"></param>
    /// <param name="functionName"></param>
    /// <param name="useUnscaledDeltaTime"></param>
    /// <param name="triggerImmediately"></param>
    /// <param name="stopAllWithSameName"></param>
    /// <returns></returns>
    public static FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer, string functionName, bool useUnscaledDeltaTime, bool triggerImmediately, bool stopAllWithSameName)
    {
        InitIfNeeded();

        if (stopAllWithSameName)
        {
            StopAllFunc(functionName);
        }

        GameObject gameObject = new GameObject("FunctionPeriodic Object " + functionName, typeof(MonoBehaviourHook));
        FunctionPeriodic functionPeriodic = new FunctionPeriodic(gameObject, action, timer, testDestroy, functionName, useUnscaledDeltaTime);
        gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionPeriodic.Update;

        funcList.Add(functionPeriodic);

        if (triggerImmediately) action();

        return functionPeriodic;
    }

    /// <summary>
    /// �Ƴ���ʱ��
    /// </summary>
    /// <param name="funcTimer"></param>
    public static void RemoveTimer(FunctionPeriodic funcTimer)
    {
        InitIfNeeded();
        funcList.Remove(funcTimer);
    }
    /// <summary>
    /// ֹͣ��ʱ��
    /// </summary>
    /// <param name="_name"></param>
    public static void StopTimer(string _name)
    {
        InitIfNeeded();
        for (int i = 0; i < funcList.Count; i++)
        {
            if (funcList[i].functionName == _name)
            {
                funcList[i].DestroySelf();
                return;
            }
        }
    }
    /// <summary>
    /// ֹͣ����Func
    /// </summary>
    /// <param name="_name"></param>
    public static void StopAllFunc(string _name)
    {
        InitIfNeeded();
        for (int i = 0; i < funcList.Count; i++)
        {
            if (funcList[i].functionName == _name)
            {
                funcList[i].DestroySelf();
                i--;
            }
        }
    }
    /// <summary>
    /// Func�Ƿ��Ծ
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool IsFuncActive(string name)
    {
        InitIfNeeded();
        for (int i = 0; i < funcList.Count; i++)
        {
            if (funcList[i].functionName == name)
            {
                return true;
            }
        }
        return false;
    }



    /// <summary>
    /// ��Ϸ����
    /// </summary>
    private GameObject gameObject;
    /// <summary>
    /// ʱ��
    /// </summary>
    private float timer;
    /// <summary>
    /// ����ʱ��
    /// </summary>
    private float baseTimer;
    /// <summary>
    /// ʹ�÷Ǳ궨����ʱ��
    /// </summary>
    private bool useUnscaledDeltaTime;
    /// <summary>
    /// ��������
    /// </summary>
    private string functionName;
    /// <summary>
    /// ί��
    /// </summary>
    public Action action;
    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public Func<bool> testDestroy;


    private FunctionPeriodic(GameObject gameObject, Action action, float timer, Func<bool> testDestroy, string functionName, bool useUnscaledDeltaTime)
    {
        this.gameObject = gameObject;
        this.action = action;
        this.timer = timer;
        this.testDestroy = testDestroy;
        this.functionName = functionName;
        this.useUnscaledDeltaTime = useUnscaledDeltaTime;
        baseTimer = timer;
    }
    /// <summary>
    /// ������ʱ��
    /// </summary>
    /// <param name="timer"></param>
    public void SkipTimerTo(float timer)
    {
        this.timer = timer;
    }

    void Update()
    {
        if (useUnscaledDeltaTime)
        {
            timer -= Time.unscaledDeltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            action();
            if (testDestroy != null && testDestroy())
            {
                //Destroy
                DestroySelf();
            }
            else
            {
                //Repeat
                timer += baseTimer;
            }
        }
    }
    /// <summary>
    /// �����Լ�
    /// </summary>
    public void DestroySelf()
    {
        RemoveTimer(this);
        if (gameObject != null)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
