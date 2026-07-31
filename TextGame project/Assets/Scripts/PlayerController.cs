using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator ani;
    public Rigidbody2D rBody;
    // 保存上一帧有效移动方向
    public float lastH;
    public float lastV;
    
    void Start()
    {
        ani = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 dir = Vector2.zero;

        // 单轴互斥逻辑：左右优先
        if (horizontal != 0)
        {
            dir = new Vector2(horizontal, 0);
            lastH = horizontal;
            lastV = 0;
        }
        else if (vertical != 0)
        {
            dir = new Vector2(0, vertical);
            lastH = 0;
            lastV = vertical;
        }
        
        ani.SetFloat("Horizontal", lastH);
        ani.SetFloat("Vertical", lastV);
        ani.SetFloat("Speed", dir.magnitude);
        
        rBody.linearVelocity = dir * 2f;
    }
}