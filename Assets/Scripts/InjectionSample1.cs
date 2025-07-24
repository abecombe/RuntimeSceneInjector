using System.Collections.Generic;
using System.Linq;
using RuntimeSceneInjector;
using UnityEngine;

public class InjectionSample1 : MonoBehaviour
{
    [Inject] private IHoge1 _hoge1;

    [Inject] private IHoge2[] _hoge2Array;
    [Inject] private List<IHoge2> _hoge2List;
    [Inject] private IEnumerable<IHoge2> _hoge2Enumerable;

    [Inject, Nullable] private IHoge3 _hoge3;

    private void Awake()
    {
        _hoge1.Hoge();

        Debug.Log($"_hoge2Array length: {_hoge2Array.Length}");
        foreach (var hoge2 in _hoge2Array)
        {
            hoge2.Hoge();
        }

        Debug.Log($"_hoge2List count: {_hoge2List.Count}");
        foreach (var hoge2 in _hoge2List)
        {
            hoge2.Hoge();
        }

        Debug.Log($"_hoge2Enumerable count: {_hoge2Enumerable.Count()}");
        foreach (var hoge2 in _hoge2Enumerable)
        {
            hoge2.Hoge();
        }

        if (_hoge3 != null)
        {
            _hoge3.Hoge();
        }
        else
        {
            Debug.LogWarning("Hoge3 is null, but it is marked as nullable.");
        }
    }
}