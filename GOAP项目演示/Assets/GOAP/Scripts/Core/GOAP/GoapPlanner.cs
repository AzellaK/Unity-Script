using System.Collections.Generic;
using UnityEngine;

/**
 * Plans what actions can be completed in order to fulfill a goal state.
 */

///<summary>
/// �ƻ����������Щ������ʵ��Ŀ��״̬��
///</summary>
public class GoapPlanner
{
    /**
	 * Plan what sequence of actions can fulfill the goal.
	 * Returns null if a plan could not be found, or a list of the actions
	 * that must be performed, in order, to fulfill the goal.
     * �ƻ�����ʵ��Ŀ����ж�˳��
     * ����Ҳ����ƻ������߲����б��򷵻�null
     * ���밴˳��ִ�в���ʵ��Ŀ�ꡣ
	 */

    ///<summary>
    ///�ƻ�
    ///</summary>
    public Queue<GoapAction> Plan(GameObject agent,
        HashSet<GoapAction> availableActions,
        Dictionary<string, bool> worldState,
        KeyValuePair<string, bool> goal,
        IGoap goap)
    {
        // reset the actions so we can start fresh with them
        //���ö������Ա����ǿ������¿�ʼ
        foreach (var a in availableActions)
        {
            a.doReset();
        }

        // check what actions can run using their checkProceduralPrecondition
        //ʹ�ü�����ǰ���������������еĲ���
        var usableActions = NodeManager.GetFreeActionSet();
        foreach (var a in availableActions)
        {
            if (a.CheckProceduralPrecondition(agent, goap.GetBlackBoard()))
                usableActions.Add(a);
        }

        // we now have all actions that can run, stored in usableActions
        //��������ӵ�п������е����в������洢��usefulActions��
        // build up the tree and record the leaf nodes that provide a solution to the goal.
        //����������¼�ṩĿ���������Ľڵ㡣
        var leaves = new List<GoapNode>();

        // build graph
        //����ͼ��
        var start = NodeManager.GetFreeNode(null, 0, 0, worldState, null);
        //����ͼ
        var success = BuildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            // oh no, we didn't get a plan
            //��������û�еõ�һ���ƻ�
            //            Debug.Log("NO PLAN");
            return null;
        }

        // get the cheapest leaf
        //�õ�����˵Ľڵ�
        GoapNode cheapest = null;
        foreach (var leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.BetterThen(cheapest))
                    cheapest = leaf;
            }
        }

        // get its node and work back through the parents
        //��ȡ��ڵ㲢ͨ�����ڵ㷵��
        var result = new List<GoapAction>();
        var n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                //��ǰ����붯��
                result.Insert(0, n.action); // insert the action in the front
            }
            n = n.parent;
        }

        NodeManager.Release();
        // we now have this action list in correct order
        //�������ڰ���ȷ��˳��ӵ�д˲����б�
        var queue = new Queue<GoapAction>();
        foreach (var a in result)
        {
            queue.Enqueue(a);
        }

        // hooray we have a plan!
        //���꣬������һ���ƻ���
        return queue;
    }

    /**
	 * Returns true if at least one solution was found.
	 * The possible paths are stored in the leaves list. Each leaf has a
	 * 'runningCost' value where the lowest Cost will be the best action
	 * sequence.
	 */

    ///<summary>
    ///����ҵ�����һ���⣬�򷵻�true��
    ///���ܵ�·���洢��Ҷ���б��С� ÿ���ڵ㶼��һ��
    ///runningCost ֵ��������ͳɱ�������Ѳ���
    ///���С�
    ///</summary>
    private bool BuildGraph(GoapNode parent, List<GoapNode> leaves
        , HashSet<GoapAction> usableActions, KeyValuePair<string, bool> goal)
    {
        var foundOne = false;

        // go through each action available at this node and see if we can use it here
        //����˽ڵ��ϵ�ÿ�����ò��������������Ƿ�����ڴ˴�ʹ����
        foreach (var action in usableActions)
        {
            // if the parent state has the conditions for this action's preconditions, we can use it here
            //�����״̬���д˲�����ǰ�����������������ǿ����ڴ˴�ʹ����
            if (InState(action.Preconditions, parent.state))
            {
                // apply the action's effects to the parent state
                //��������Ч��Ӧ���ڸ�״̬
                var currentState = PopulateState(parent.state, action.Effects);
                //Debug.Log(GoapAgent.prettyPrint(currentState));
                var node = NodeManager.GetFreeNode(parent, parent.runningCost + action.GetCost(), parent.weight + action.GetWeight(),
                    currentState, action);

                //force child.precondition in parent.effects or child.precondition is empty.
                //��parent.effects��ǿ��child.precondition��child.preconditionΪ�ա�
                if (action.Preconditions.Count == 0 && parent.action != null ||
                    parent.action != null && !CondRelation(action.Preconditions, parent.action.Effects))
                    continue;

                if (FillGoal(goal, currentState))
                {
                    // we found a solution!
                    //�����ҵ��˽��������
                    leaves.Add(node);
                    foundOne = true;
                }
                else
                {
                    // not at a solution yet, so test all the remaining actions and branch out the tree
                    //��û�н�����������Բ�������ʣ��Ķ�������֧����
                    var subset = ActionSubset(usableActions, action);
                    var found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                        foundOne = true;
                }
            }
        }

        return foundOne;
    }

    //if there is one true relationship
    //�����һ�������Ĺ�ϵ
    private bool CondRelation(Dictionary<string, bool> preconditions
                            , Dictionary<string, bool> effects)
    {
        foreach (var t in preconditions)
        {
            var match = effects.ContainsKey(t.Key) && effects[t.Key] == t.Value;
            if (match)
                return true;
        }
        return false;
    }

    /**
	 * Create a subset of the actions excluding the removeMe one. Creates a new set.
	 */
    ///<summary>
    ///������removeMe֮��Ĳ����Ӽ��� ����һ���¼���
    ///</summary>
    private HashSet<GoapAction> ActionSubset(HashSet<GoapAction> actions, GoapAction removeMe)
    {
        var subset = NodeManager.GetFreeActionSet();
        foreach (var a in actions)
        {
            if (!a.Equals(removeMe))
                subset.Add(a);
        }
        return subset;
    }

    /**
	 * Check that all items in 'test' are in 'state'. If just one does not match or is not there
	 * then this returns false.
	 */
    ///<summary>
    ///���'test'�е�������Ŀ�Ƿ���'state'״̬�� ���ֻ��һ����ƥ��򲻴���
    ///��ô��᷵��false��
    ///</summary>
    private bool InState(Dictionary<string, bool> test, Dictionary<string, bool> state)
    {
        var allMatch = true;
        foreach (var t in test)
        {
            var match = state.ContainsKey(t.Key) && state[t.Key] == t.Value;
            if (!match)
            {
                allMatch = false;
                break;
            }
        }
        return allMatch;
    }
    private bool FillGoal(KeyValuePair<string, bool> goal, Dictionary<string, bool> state)
    {
        var match = state.ContainsKey(goal.Key) && state[goal.Key] == goal.Value;
        return match;
    }

    /**
	 * Apply the stateChange to the currentState
	 */
    ///<summary>
    ///��״̬����Ӧ���ڵ�ǰ״̬
    ///</summary>
    private Dictionary<string, bool> PopulateState(Dictionary<string, bool> currentState,
        Dictionary<string, bool> stateChange)
    {
        Dictionary<string, bool> state = NodeManager.GetFreeState();
        state.Clear();
        foreach (var s in currentState)
        {
            state.Add(s.Key, s.Value);
        }

        foreach (var change in stateChange)
        {
            // if the key exists in the current state, update the Value
            //�����Կ�����ڵ�ǰ״̬�������Value
            if (state.ContainsKey(change.Key))
            {
                state[change.Key] = change.Value;
            }
            else
            {
                state.Add(change.Key, change.Value);
            }
        }
        return state;
    }

}