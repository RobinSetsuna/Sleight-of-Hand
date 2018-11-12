using System.Collections.Generic;

public interface IAttributeGetter : IEnumerable<KeyValuePair<int, float>>
{
    float this[int id] { get; }
    float this[AttributeType attribute] { get; }
}
