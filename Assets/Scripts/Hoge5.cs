using UnityEngine;

public class Hoge5 : MonoBehaviour, IHoge5
{
    [SerializeField] private string _logMessage = "Hoge5 executed";

    public void Hoge()
    {
        Debug.Log(_logMessage);
    }
}