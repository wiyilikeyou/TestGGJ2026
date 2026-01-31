using UnityEngine;

public class InfiniteRoad : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("拖入两个地板物体")]
    public Transform road1; 
    public Transform road2;
    public Transform spawnTransform;

    [Tooltip("移动速度")]
    public float speed = 10f;

    [Tooltip("手动指定路段长度（如果自动计算不准，取消勾选此项并手动输入）")]
    public float manualLength = 0f;

    // 内部记录的路段长度
    private float roadLength;

    void Start()
    {
        // 1. 自动计算路段长度
        // 我们尝试从 road1 的 Renderer 或 Collider 获取 Z 轴的长度
        if (manualLength > 0)
        {
            roadLength = manualLength;
        }
        else
        {
            Renderer renderer = road1.GetComponent<Renderer>();
            if (renderer != null)
            {
                roadLength = renderer.bounds.size.z;
            }
            else
            {
                Debug.LogError("地板上没有 Renderer 组件，无法自动计算长度！请手动设置 manualLength。");
            }
        }

        Debug.Log($"路段长度为: {roadLength}");
    }

    void Update()
    {
        // 2. 移动两个地板
        // 使用 Vector3.back 让物体向 Z 轴负方向移动 (世界坐标)
        float moveDistance = speed * Time.deltaTime;
        
        road1.position += Vector3.back * moveDistance;
        road2.position += Vector3.back * moveDistance;
        spawnTransform.position += Vector3.back * moveDistance;
        
        // 3. 检查并重置位置
        CheckAndReposition(road1, road2);
        CheckAndReposition(road2, road1);
    }

    // 检查 currentRoad 是否跑到了后面，如果是，就拼接到 targetRoad 的前面
    void CheckAndReposition(Transform currentRoad, Transform targetRoad)
    {
        // 如果当前路段的位置 Z 坐标 < -长度 (意味着完全离开了屏幕中心区域)
        // 注意：这里假设你的相机看着 Z 轴正方向，且路面中心点在 (0,0,0) 开始
        // 如果你的原点不在中心，可能需要调整这个判断条件，比如 <= -roadLength * 1.5f
        if (currentRoad.position.z <= -roadLength)
        {
            // 核心逻辑：把当前路段移动到目标路段的“前方”
            // 新位置 Z = 目标路段 Z + 路段长度
            // 这样能保证无缝拼接，即使发生掉帧也不会有缝隙
            Vector3 newPos = currentRoad.position;
            newPos.z = targetRoad.position.z + roadLength;
            
            currentRoad.position = newPos;
        }
    }
}