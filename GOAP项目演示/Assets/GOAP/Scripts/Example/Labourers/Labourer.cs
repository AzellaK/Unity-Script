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
///��ͨ�Ͷ��߽׼���
///��Ӧ��Ϊ�ض���Laborer�����໯��ʵ����
///����Ŀ��״̬�������÷��������GOAP��Ŀ��
///�滮ʦ��
///</summary>
public abstract class Labourer : MonoBehaviour, IGoap
{
    /// <summary>
    /// ��ʱ����
    /// </summary>
    public BackpackComponent backpack;
    /// <summary>
    /// �ƶ��ٶ�
    /// </summary>
    public float moveSpeed = 1;
    /// <summary>
    /// �Ƿ�������־
    /// </summary>
    public bool EnableLog = false;

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
        worldData["hasOre"] = backpack.numOre > 0;
        worldData["hasLogs"] = backpack.numLogs > 0;
        worldData["hasFirewood"] = backpack.numFirewood > 0;
        worldData["hasTool"] = backpack.tool != null;
        worldData["hasMeat"] = backpack.numMeat > 0;

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
            backpack = gameObject.AddComponent<BackpackComponent>() as BackpackComponent;
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

        if (Brain == null)
            Brain = gameObject.GetComponent<Brain>();
        Brain.Init();

        //init world data ��ʼ����������
        worldData.Add("hasOre", (backpack.numOre > 0));
        worldData.Add("hasLogs", (backpack.numLogs > 0));
        worldData.Add("hasFirewood", (backpack.numFirewood > 0));
        worldData.Add("hasTool", (backpack.tool != null));
        worldData.Add("hasMeat", (backpack.numMeat > 0));

        //init blackboard ��ʼ���ڰ�
        bb.AddData("backpack", backpack);
        bb.AddData("brain", Brain);
        bb.AddData("ironRock", FindObjectsOfType(typeof(IronRockComponent)));
        bb.AddData("appleTree", FindObjectsOfType(typeof(AppleTreeComponet)));
        bb.AddData("forge", FindObjectsOfType(typeof(ForgeComponent)));
        bb.AddData("tree", FindObjectsOfType(typeof(TreeComponent)));
        bb.AddData("wolfDen", FindObjectsOfType(typeof(WolfDen)));
        bb.AddData("choppingBlock", FindObjectsOfType(typeof(ChoppingBlockComponent)));
        bb.AddData("supplyPiles", FindObjectsOfType(typeof(SupplyPileComponent)));
        bb.AddData("camp", FindObjectsOfType(typeof(CampComponent)));
    }

    public virtual void Tick()
    {
        Brain.Tick(this);
    }
    /// <summary>
    /// �Ƴ���Ա����
    /// </summary>
    public virtual void Release()
    {
        Brain.Release();
    }
    /// <summary>
    /// ����
    /// </summary>
    public IAgent Agent { get; set; }
    /// <summary>
    /// ��Ա����
    /// </summary>
    public IBrain Brain { get; set; }
}

