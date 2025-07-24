using UnityEngine;

public class Hoge2 : MonoBehaviour, IHoge2
{
    [SerializeField] private string _logMessage = "Hoge2 executed";

    public void Hoge()
    {
        Debug.Log(_logMessage);
    }
}