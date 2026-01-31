using UnityEngine;

[ExecuteInEditMode] // 可以在编辑器里直接看到效果
public class WorldCurver : MonoBehaviour
{
    [Range(-0.1f, 0.1f)]
    public float curveX = 0f;
    
    [Range(0f, 0.1f)]
    public float curveY = 0.005f;

    // 对应 Shader 中的变量名
    private int curveXID;
    private int curveYID;

    void OnEnable()
    {
        curveXID = Shader.PropertyToID("_CurveX");
        curveYID = Shader.PropertyToID("_CurveY");
    }

    void Update()
    {
        // 使用 SetGlobalFloat，所有使用该 Shader 的物体都会同步弯曲
        Shader.SetGlobalFloat(curveXID, curveX);
        Shader.SetGlobalFloat(curveYID, curveY);
    }
}