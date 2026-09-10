using UnityEngine;
public class TestSimple : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("✅ Awake执行！");
    }
    void Start()
    {
        Debug.Log("✅ Start执行！");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("✅ T键触发");
        }
    }
}
