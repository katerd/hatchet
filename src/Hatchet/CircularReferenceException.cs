namespace Hatchet;

public class CircularReferenceException : HatchetException
{
    public object Item { get; }

    public CircularReferenceException(object item) 
        : base($"Circular reference detected {item} {item.GetType()}")
    {
        Item = item;
    }
}