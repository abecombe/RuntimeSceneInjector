using UnityEngine;

public class Hoge3 : MonoBehaviour, IHoge3
{
    [SerializeField] private string _logMessage = "Hoge3 executed";

    public void Hoge()
    {
        Debug.Log(_logMessage);
    }
}