using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TowerMenuController : MonoBehaviour
{
    [SerializeField]private HashSet<Tower> towers = new HashSet<Tower>();

    public void Register(Tower t)
    {
        if (t) towers.Add(t);
    }

    public void Unregister(Tower t)
    {
        if (t) towers.Remove(t);
    }

    public IReadOnlyCollection<Tower> All => towers;



    private void OnTransformChildrenChanged()
    {

        if (!Application.isPlaying) return;

        towers.RemoveWhere(t => t == null);
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Tower tw))
                towers.Add(tw);
        }
        Debug.Log($"towers count = {towers.Count}");
    }
}
