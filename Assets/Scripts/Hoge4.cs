using UnityEngine;

public class Hoge4 : MonoBehaviour, IHoge4
{
    [SerializeField] private string _logMessage = "Hoge4 executed";

    public void Hoge()
    {
        Debug.Log(_logMessage);
    }
}