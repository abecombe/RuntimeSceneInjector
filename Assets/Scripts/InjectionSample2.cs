using System.Collections.Generic;
using System.Linq;
using RuntimeSceneInjector;
using UnityEngine;

public class InjectionSample2 : MonoBehaviour
{
    [Inject] private IHoge1 _hoge1;

    private IHoge2[] _hoge2Array;
    private List<IHoge2> _hoge2List;
    private IEnumerable<IHoge2> _hoge2Enumerable;

    private IHoge3 _hoge3;

    [Inject]
    private void Construct(IHoge2[] hoge2Array, List<IHoge2> hoge2List, IEnumerable<IHoge2> hoge2Enumerable, [Nullable] IHoge3 hoge3)
    {
        _hoge2Array = hoge2Array;
        _hoge2List = hoge2List;
        _hoge2Enumerable = hoge2Enumerable;
        _hoge3 = hoge3;
    }

    protected virtual void Awake()
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