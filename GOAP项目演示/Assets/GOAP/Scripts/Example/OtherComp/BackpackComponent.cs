using UnityEngine;
using System.Collections;

/**
 * Holds resources for the Agent.
 * �����������Դ��
 */


///<summary>
/// ��ʱ����
///</summary>
public class BackpackComponent : MonoBehaviour
{
    [Header("����")]
    ///<summary>
    ///����
    ///</summary>
    public GameObject tool;
    [Header("��������")]
    ///<summary>
    ///��������
    ///</summary>
    public int numLogs;
    [Header("ľ������")]
    ///<summary>
    ///ľ������
    ///</summary>
    public int numFirewood;
    ///<summary>
    ///
    ///</summary>
    public int numOre;
    [Header("������")]
    ///<summary>
    ///������
    ///</summary>
    public int numMeat;
    [Header("��������")]
    ///<summary>
    ///��������
    ///</summary>
    public string toolType = "ToolAxe";//Ĭ�ϸ�ͷ

}

