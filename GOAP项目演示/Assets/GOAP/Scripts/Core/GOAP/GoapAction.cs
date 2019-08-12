
using UnityEngine;
using System.Collections.Generic;

public abstract class GoapAction : MonoBehaviour
{

    /// <summary>
    /// ǰ������
    /// </summary>
    private Dictionary<string, bool> preconditions;
    /// <summary>
    /// Ч��
    /// </summary>
    private Dictionary<string, bool> effects;

    private bool inRange = false;

    /* The Cost of performing the action. 
	 * Figure out a weight that suits the action. 
	 * Changing it will affect what actions are chosen during planning.
     *ִ�в����ĳɱ���
     *�ҳ��ʺ��ж���������
     *��������Ӱ��滮�ڼ�ѡ��Ĳ�����*/
    /// <summary>
    /// �ɱ�
    /// </summary>
    public float Cost = 1f;
    /// <summary>
    /// ��óɱ�
    /// </summary>
    /// <returns></returns>
    public virtual float GetCost()
    {
        return Cost;
    }

    /* The risk of performing the action. */
    /// <summary>
    /// ִ�в����ķ��ա�
    /// </summary>
    public float Risk = 0f;
    /* The Benefits of performing the action. */
    /// <summary>
    /// ִ�в����ĺô���
    /// </summary>
    public float Return = 1f;
    /* Figure out a weight that suits the action. */
    /// <summary>
    /// �ҳ��ʺ��ж���������
    /// </summary>
    /// <returns></returns>
    public virtual float GetWeight()
    {
        return (1 - Risk) * Return;
    }

    /**
	 * An action often has to perform on an object. This is that object. Can be null. */
    ///<summary>
    ///ͨ������Զ���ִ�в����� �����Ǹ����� ����Ϊnull��
    ///</summary>
    public GameObject target;
    /// <summary>
    /// Goap����
    /// </summary>
    public GoapAction()
    {
        preconditions = new Dictionary<string, bool>();
        effects = new Dictionary<string, bool>();
    }
    /// <summary>
    /// ����
    /// </summary>
    public void doReset()
    {
        inRange = false;
        target = null;
        Reset();
    }

    /// <summary>
    /// ��ȡĿ������
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 GetTargetPos()
    {
        return target.transform.position;
    }

    /**
	 * Reset any variables that need to be reset before planning happens again.
	 */
    ///<summary>
    ///�ڼƻ��ٴη���֮ǰ��������Ҫ���õ����б�����
    ///</summary>
    public abstract void Reset();

    /**
	 * Is the action done?
	 */

    ///<summary>
    ///�ж��������
    ///</summary>
    /// <inheritdoc cref="GoapAction"/>
    public abstract bool IsDone();

    /**
	 * Procedurally check if this action can run. Not all actions
	 * will need this, but some might.
	 */

    ///<summary>
    ///������˲����Ƿ�������С� ���������ж�
    ///��Ҫ���������Щ���ܡ�
    ///</summary>
    /// <inheritdoc/>
    public abstract bool CheckProceduralPrecondition(GameObject agent, BlackBoard bb);

    /**
	 * Run the action.
	 * Returns True if the action performed successfully or false
	 * if something happened and it can no longer perform. In this case
	 * the action queue should clear out and the goal cannot be reached.
	 */
    ///<summary>
    ///ִ��
    ///��������ɹ��򷵻�True,���򷵻�false��
    ///���������ʲô�£�������Ҳ�޷�ִ���ˡ� �����������
    ///�ж�����Ӧ������޷��ﵽĿ�ꡣ
    ///</summary>
    public abstract bool Perform(GameObject agent, BlackBoard bb);

    /**
	 * Does this action need to be within range of a target game object?
	 * If not then the moveTo state will not need to run for this action.
	 */
    ///<summary>
    ///�˲����Ƿ���Ҫ��Ŀ����Ϸ����ķ�Χ�ڣ�
    ///������ǣ���moveTo״̬����ҪΪ�˲������С�
    ///</summary>
    public abstract bool RequiresInRange();


    /**
	 * Are we in range of the target?
	 * The MoveTo state will set this and it gets reset each time this action is performed.
	 */
    ///<summary>
    ///�����Ƿ���Ŀ�귶Χ�ڣ�
    ///MoveTo״̬�����ô�ֵ������ÿ��ִ�д˲���ʱ���á�
    ///</summary>
    public bool IsInRange()
    {
        return inRange;
    }
    /// <summary>
    /// ���÷�Χ
    /// </summary>
    /// <param name="inRange"></param>
    public void SetInRange(bool inRange)
    {
        this.inRange = inRange;
    }
    /// <summary>
    /// ���ǰ������
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddPrecondition(string key, bool value)
    {
        preconditions.Add(key, value);
    }
    /// <summary>
    /// ɾ��ǰ������
    /// </summary>
    /// <param name="key"></param>
    public void RemovePrecondition(string key)
    {
        if (preconditions.ContainsKey(key))
            preconditions.Remove(key);
    }
    /// <summary>
    /// ���Ч��
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddEffect(string key, bool value)
    {
        effects.Add(key, value);
    }
    /// <summary>
    /// �Ƴ�Ч��
    /// </summary>
    /// <param name="key"></param>
    public void RemoveEffect(string key)
    {
        if (effects.ContainsKey(key))
            effects.Remove(key);
    }
    /// <summary>
    /// ǰ������
    /// </summary>
    public Dictionary<string, bool> Preconditions
    {
        get
        {
            return preconditions;
        }
    }
    /// <summary>
    /// Ч��
    /// </summary>
    public Dictionary<string, bool> Effects
    {
        get
        {
            return effects;
        }
    }
}