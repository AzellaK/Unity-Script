using UnityEngine;
using System.Collections;

/**
 * A tool used for mining ore and chopping wood.
 * Tools have strength that gets used up each time
 * they are used. When their strength is depleted
 * they should be destroyed by the user.
 */
///<summary>
///���ڿ��ɿ�ʯ�Ϳ���ľ�ĵĹ��ߡ�
///���ߵ�ǿ��ÿ�ζ������Ĵ���
///���Ǳ�ʹ�á� �����ǵ������ľ�ʱ
///����Ӧ�ñ��û����١�
///��ע�������;ù���
///</summary>
public class ToolComponent : MonoBehaviour
{
    /// <summary>
    /// 0..1]��0����100��
    /// </summary>
    public float strength; // [0..1] or 0% to 100%

    void Start()
    {
        //ȫ��
        strength = 1; // full strength
    }

    /**
	 * Use up the tool by causing damage. Damage should be a percent
	 * from 0 to 1, where 1 is 100%.
	 */
    ///<summary>
    ///����𻵣����깤�ߡ� ��Ӧ����һ���ٷֱ�
    ///��0��1������1��100����
    ///</summary>
    public void use(float damage)
    {
        strength -= damage;
    }
    /// <summary>
    /// ����û���;õĹ���
    /// </summary>
    /// <returns></returns>
    public bool destroyed()
    {
        return strength <= 0f;
    }
}

