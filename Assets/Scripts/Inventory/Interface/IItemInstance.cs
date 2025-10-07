using System.Collections.Generic;
using UnityEngine;

public interface IItemInstance
{
    ItemData ItemData { get; }

    Dictionary<string, string> GetStats();
}
