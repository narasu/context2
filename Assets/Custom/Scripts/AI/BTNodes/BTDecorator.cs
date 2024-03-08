/// <summary>
/// Base class for any BT node with a single child that responds to the result of its child in some way.
/// </summary>

public abstract class BTDecorator : BTBaseNode
{
    protected readonly BTBaseNode child;
    
    protected BTDecorator(string _name, BTBaseNode _child) : base (_name)
    {
        child = _child;
    }

    public override void OnTerminate()
    {
        base.OnTerminate();
        child.OnTerminate();
    }
}
