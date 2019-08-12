
using System;
using UnityEngine;

/// <summary>
/// �������ද��
/// </summary>
public class DropOffMeatAction : GoapAction
{
    /// <summary>
    /// �Ƿ������
    /// </summary>
	private bool droppedOffMeat = false;
    /// <summary>
    /// Ŀ�깩Ӧ׮ ���Ƿ��¹��ߵĵط�
    /// </summary>
	private SupplyPileComponent targetSupplyPile; // where we drop off the  tools

    public DropOffMeatAction()
    {
        ////������ǻ�û�й��ߣ��Ͳ��ܷ��¹���
        AddPrecondition("hasMeat", true); // can't drop off tools if we don't already have some
        //��������û�й���
        AddEffect("hasMeat", false); // we now have no tools
        //�����ռ��˹���
        AddEffect(Goals.FillOther, true); // we collected tools
    }

    /// <summary>
    /// ����
    /// </summary>
    public override void Reset()
    {
        droppedOffMeat = false;
        targetSupplyPile = null;
    }
    /// <summary>
    /// �Ƿ��Ѿ������
    /// </summary>
    /// <returns></returns>
    public override bool IsDone()
    {
        return droppedOffMeat;
    }
    /// <summary>
    /// Ҫ��Χ
    /// </summary>
    /// <returns></returns>
    public override bool RequiresInRange()
    {
        //�ǵģ�������Ҫ������Ӧ�ѣ��������ǲ��ܷ��¹���
        return true; // yes we need to be near a supply pile so we can drop off the tools
    }
    /// <summary>
    /// ������ǰ������
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="bb"></param>
    /// <returns></returns>
    public override bool CheckProceduralPrecondition(GameObject agent, BlackBoard bb)
    {
        // find the nearest supply pile that has spare tools
        //�ҵ�������б��ù��ߵĹ�Ӧ��
        SupplyPileComponent[] supplyPiles = (SupplyPileComponent[])bb.GetData("supplyPiles");
        SupplyPileComponent closest = null;
        float closestDist = 0;

        foreach (SupplyPileComponent supply in supplyPiles)
        {
            if (closest == null)
            {
                // first one, so choose it for now
                //��һ������������ѡ����
                closest = supply;
                closestDist = (supply.gameObject.transform.position - agent.transform.position).magnitude;
            }
            else
            {
                // is this one closer than the last?
                //��������һ��������
                float dist = (supply.gameObject.transform.position - agent.transform.position).magnitude;
                if (dist < closestDist)
                {
                    // we found a closer one, use it
                    //�����ҵ���һ�������ģ�ʹ����
                    closest = supply;
                    closestDist = dist;
                }
            }
        }
        if (closest == null)
            return false;

        targetSupplyPile = closest;
        target = targetSupplyPile.gameObject;

        return closest != null;
    }
    /// <summary>
    /// ִ��
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="bb"></param>
    /// <returns></returns>
    public override bool Perform(GameObject agent, BlackBoard bb)
    {
        BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
        targetSupplyPile.numMeat += backpack.numMeat;
        backpack.numMeat = 0;
        droppedOffMeat = true;
        // TODO����Ч����������Աͼ��

        return true;
    }
}