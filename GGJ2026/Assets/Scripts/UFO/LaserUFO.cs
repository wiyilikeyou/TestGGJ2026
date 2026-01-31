using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserUFO : MonoBehaviour
{
    PlayerController player;
    public GameObject laserTips;
    [SerializeField] private float rotationSpeed = 5f;
    private float attackTime = -1;
    public Vector3 speed;
    public float summonTimer = -1;
    [SerializeField] private float preAttackDuration = 2;
    // Start is called before the first frame update
    private Material material;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        if(laserTips != null)laserTips.SetActive(false);
        summonTimer = Time.time;
        material = laserTips.GetComponent<Renderer>().material;
        // 初始化时直接朝向玩家
        if (player != null && laserTips != null)
        {
            Vector3 directionToPlayer = player.transform.position - laserTips.transform.position;
            directionToPlayer.y = 0;
            if (directionToPlayer.sqrMagnitude > 0.001f)
            {
                laserTips.transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }
        }
    }
    private bool isAttacked = false;

    // Update is called once per frame
    void Update()
    {
        if(Time.time - summonTimer >= 10)Destroy(gameObject);
        transform.position += speed * Time.deltaTime;
        if(player == null || laserTips == null) return;
        if (attackTime <= 0) attackTime = Time.time;
        if (Time.time - attackTime >= preAttackDuration)
        {
            if(!isAttacked)
            {
                isAttacked = true;
            }
        }
        else
        {
            if(!laserTips.activeSelf)laserTips.SetActive(true);
            // 计算朝向玩家的方向
            Vector3 directionToPlayer = player.transform.position - laserTips.transform.position;
            // 只保留水平方向（忽略Y轴高度差）
            directionToPlayer.y = 0;

            // 如果方向不为零，则平滑旋转朝向玩家
            if (directionToPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                laserTips.transform.rotation = Quaternion.Slerp(laserTips.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
