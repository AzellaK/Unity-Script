using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * A general labourer class.
 * You should subclass this for specific Labourer classes and implement
 * the createGoalState() method that will populate the goal for the GOAP
 * planner.
 */

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
	 * Implement in subclasses
     * ��������ʵ��
	 */
    public abstract Dictionary<string, bool> CreateGoalState();


    public void PlanFailed(Dictionary<string, bool> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
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
        // Yay we found a plan for our goal
        //����Ϊ���ǵ�Ŀ���ҵ���һ���ƻ�
        if (EnableLog)
            Debug.Log("<color=green> �ƻ�����</color> " + GoapAgent.PrettyPrint(actions));
    }
    /// <summary>
    /// �������
    /// </summary>
    public void ActionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
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
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
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
            // we are at the target location, we are done
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
            // attach the tool
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
        bb.AddData("Tree", FindObjectsOfType(typeof(Trees)));
        bb.AddData("ChopFirewoodPoint", FindObjectsOfType(typeof(ChopFirewoodPoint)));
        bb.AddData("Mining", FindObjectsOfType(typeof(Mining)));
        bb.AddData("Forge", FindObjectsOfType(typeof(ForgeComponent)));

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

