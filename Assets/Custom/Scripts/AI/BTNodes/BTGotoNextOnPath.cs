using UnityEngine;

public class BTGotoNextOnPath : BTBaseNode
{
    private readonly Blackboard blackboard;
    private PathNode[] pathNodes;
    private int currentNode;

    public BTGotoNextOnPath(Blackboard _blackboard) : base("GotoNextOnPath")
    {
        blackboard = _blackboard;
        pathNodes = _blackboard.GetVariable<PathNode[]>(Strings.PatrolNodes);
        
    }
    protected override TaskStatus Run()
    {
        int nextNode = currentNode + 1;
        if (nextNode >= pathNodes.Length)
        {
            nextNode = 0;
        }
        Vector3 destination = pathNodes[nextNode].Position;
        blackboard.SetVariable(Strings.Destination, destination);
        currentNode = nextNode;
        blackboard.SetVariable(Strings.PatrolNodeIndex, currentNode);
        
        return TaskStatus.Success;
    }
}