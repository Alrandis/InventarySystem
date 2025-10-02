using UnityEngine;

public interface IStackable
{
    int CurrentStack { get; }
    int MaxStack { get; }
    bool CanStackWith(IStackable other);
    void AddToStack(int amount);
    void RemoveFromStack(int amount);
}
