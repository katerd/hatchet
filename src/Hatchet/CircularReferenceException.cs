namespace Hatchet;

public class CircularReferenceException(object item)
    : HatchetException($"Circular reference detected {item} {item.GetType()}")
{
    public object Item { get; } = item;
}
