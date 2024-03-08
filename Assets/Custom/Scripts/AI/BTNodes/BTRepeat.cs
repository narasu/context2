public class BTRepeat : BTDecorator
{
    public BTRepeat(BTBaseNode _child) : base("Repeat", _child) { }
    protected override TaskStatus Run()
    {
        TaskStatus childStatus = child.Tick();
        return childStatus == TaskStatus.Success ? TaskStatus.Running : TaskStatus.Success;
    }
}
