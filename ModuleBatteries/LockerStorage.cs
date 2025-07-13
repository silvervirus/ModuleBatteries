using ModuleBatteries;
using UnityEngine;
using ModuleBatteries.Behaviours;

  IEnumerable<WaitForEndOfFrame> WaitAndLock(GameObject obj)
{
    yield return new WaitForEndOfFrame(); // or multiple frames if needed

    var storage = obj.GetComponentInChildren<StorageContainer>();
    if (storage != null)
    {
        GameObjectExtensions.LockStorage(storage);
    }
}