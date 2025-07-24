using UnityEngine;

public class Hoge1 : MonoBehaviour, IHoge1
{
    [SerializeField] private string _logMessage = "Hoge1 executed";

    public void Hoge()
    {
        Debug.Log(_logMessage);
    }
}