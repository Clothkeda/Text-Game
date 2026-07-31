using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FlashLightPost : MonoBehaviour
{
    [Header("把层级里的Player拖进这里")]
    public PlayerController player;
    private Material mat;

    void Start()
    {
        mat = new Material(Shader.Find("Custom/FlashLightMask"));
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(player.transform.position);
        Vector2 playerUV = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        mat.SetVector("_PlayerUV", playerUV);

        Vector2 faceDir = new Vector2(player.lastH, player.lastV);
        float angle = Mathf.Atan2(faceDir.y, faceDir.x) * Mathf.Rad2Deg;
        mat.SetFloat("_Angle", angle);

        Graphics.Blit(src, dest, mat);
    }
}