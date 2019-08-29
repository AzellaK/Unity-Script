using UnityEngine;
using System.Collections;
using System.Collections.Generic;


///<summary>
///��ɫ����
///����Ŀ��״̬�������÷��������GOAP��Ŀ��
///</summary>
public abstract class Character : MonoBehaviour, IGoap
{
    /// <summary>
    /// ��ʱ����
    /// </summary>
    public Backpack backpack;
    /// <summary>
    /// �ƶ��ٶ�
    /// </summary>
    public float moveSpeed = 1;
    /// <summary>
    /// �Ƿ�������־
    /// </summary>
    public bool EnableLog = false;
    /// <summary>
    /// ����
    /// </summary>
    public IAgent Agent { get; set; }
    /// <summary>
    /// ��ɫ״̬
    /// </summary>
    public CharacterStatus State { get; set; }
    void Start()
    {
        Init();
    }

    void Update()
    {
        Tick();
    }
    /// <summary>
    /// ��������
    /// </summary>
    Dictionary<string, bool> worldData = new Dictionary<string, bool>();
    /**
	 * Key-Value data that will feed the GOAP actions and system while planning.
     * �ڹ滮ʱ��ΪGOAP������ϵͳ�ṩ��Ϣ�ļ�ֵ���ݡ�
	 */
    public Dictionary<string, bool> GetWorldState()
    {
        worldData["hasTool"] = backpack.tool != null;
        worldData["hasTree"] = backpack.woodNum > 0;
        worldData["hasMining"] = backpack.stoneNum > 0;
        return worldData;
    }

    BlackBoard bb = new BlackBoard();
    /// <summary>
    /// ��úڰ�
    /// </summary>
    /// <returns></returns>
    public BlackBoard GetBlackBoard()
    {
        return bb;
    }

    /**
     * ��������ʵ��
	 */
    public abstract Dictionary<string, bool> CreateGoalState();


    public void PlanFailed(Dictionary<string, bool> failedGoal)
    {
        //��Ϊ����ȷ�����ǵ�Ŀ����Զ�ɹ������Բ��ڴ˴���
        //��ͨ������������֮ǰȷ������״̬�Ѿ��ı�
        //�ٴ�ʹ����ͬ��Ŀ�꣬��������ʧ�ܡ�
    }
    /// <summary>
    /// �ƻ�����
    /// </summary>
    /// <param name="goal"></param>
    /// <param name="actions"></param>
    public void PlanFound(KeyValuePair<string, bool> goal, Queue<GoapAction> actions)
    {
        //����Ϊ���ǵ�Ŀ���ҵ���һ���ƻ�
        if (EnableLog)
            Debug.Log("<color=green> �ƻ�����</color> " + GoapAgent.PrettyPrint(actions));
    }
    /// <summary>
    /// �������
    /// </summary>
    public void ActionsFinished()
    {
        //һ�ж�����ˣ�����Ϊ���gool��������ǵ��ж������꣡
        if (EnableLog)
            Debug.Log("<color=blue>�������</color>");
    }
    /// <summary>
    /// �ƻ���ֹ
    /// </summary>
    /// <param name="aborter"></param>
    public void PlanAborted(GoapAction aborter)
    {
        //һ����ƻ����ж��� �����Ѿ����üƻ��ˡ�
        //���·��������鲢ȷ���ٴ�������ͬ��Ŀ��
        //�����Գɹ�
        if (EnableLog)
            Debug.Log("<color=red>�ƻ���ֹ</color> " + GoapAgent.PrettyPrint(aborter));
    }
    /// <summary>
    /// �ƶ�����
    /// </summary>
    /// <param name="nextAction">��һ������</param>
    /// <returns></returns>
    public bool MoveAgent(GoapAction nextAction)
    {
        //����>��һ��������Ŀ��
        float step = moveSpeed * Time.deltaTime;
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position, step);

        if (gameObject.transform.position.Equals(nextAction.target.transform.position))
        {
            //������Ŀ��λ�ã����������
            nextAction.SetInRange(true);
            return true;
        }
        else
            return false;
    }
    /// <summary>
    /// ��ʼ��
    /// </summary>
    public virtual void Init()
    {
        if (backpack == null)
            backpack = gameObject.AddComponent<Backpack>() as Backpack;
        if (backpack.tool == null)
        {
            //���ع���
            GameObject prefab = Resources.Load<GameObject>(backpack.toolType);
            //ʵ����
            GameObject tool = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            backpack.tool = tool;
            //���Ϲ���
            tool.transform.parent = transform;
        }

        if (State == null)
            State = gameObject.GetComponent<CharacterStatus>();
        State.Init();

        //init world data ��ʼ����������
        worldData.Add("hasTool", (backpack.tool != null));
        worldData.Add("hasTree", (backpack.woodNum > 0));
        worldData.Add("hasMining", (backpack.stoneNum > 0));
        //init blackboard ��ʼ���ڰ�
        bb.AddData("backpack", backpack);
        bb.AddData("state", State);
        bb.AddData("Warehouse", FindObjectsOfType(typeof(Warehouse)));
       
    }

    public virtual void Tick()
    {
        State.Tick(this);
    }
    /// <summary>
    /// �Ƴ���Ա����
    /// </summary>
    public virtual void Release()
    {
        State.Release();
    }

}

