using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Transform TargetTransform { get; set; }
    private List<Transform> TargetTransforms { get; set; }
    private ProjectileModel ProjectileModel { get; set; }
    private bool Player { get; set; }

    public event Action ProjectileHitPlayer;
    public event Action<List<int>, float> ProjectileHitEnemy;

    //[SerializeField] private Sprite fire;

    public void Constructor(List<Transform> targetTransforms, Projectile projectile)
    {
        TargetTransforms = targetTransforms;
        ProjectileModel = new ProjectileModel(projectile);
        Player = projectile.Player;
    }
    public void Constructor(Transform targetTransform, Projectile projectile)
    {
        TargetTransform = targetTransform;
        ProjectileModel = new ProjectileModel(projectile);
        Player = projectile.Player;
    }

    public void SetTargetTransforms(List<Transform> targetTransforms)
    {
        TargetTransforms = targetTransforms;
    }

    // Start is called before the first frame update
    void Start()
    {
        TransformControll(); //敵か味方かでトランスフォームを調整
    }

    // Update is called once per frame
    void Update()
    {
        ProjectileFlying(); //飛び道具を動かす

        ProjectileHittingTarget(); //当たり判定

        JudgeDestroy(); //一定範囲を超えたら破壊する
    }

    private void TransformControll()
    {
        if (Player)
        {
            this.transform.position = new Vector3(this.transform.position.x, -3.5f, 0);
            this.transform.localScale = new Vector3(1, 1, 1);
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
            this.GetComponent<SpriteRenderer>().sprite = Resources.Load(ProjectileModel.Name, typeof(Sprite)) as Sprite;
        }
        else
        {
            this.transform.position = new Vector3(this.transform.position.x, -1, 0);
            this.transform.localScale = new Vector3(0.2f, 0.2f, 1);
            this.transform.rotation = new Quaternion(180, 0, 0, 0);
            this.GetComponent<SpriteRenderer>().sprite = Resources.Load(ProjectileModel.Name, typeof(Sprite)) as Sprite;
        }
    }

    private void ProjectileFlying()
    {
        float[] move = new float[3];
        move = ProjectileModel.Fly();
        this.transform.Translate(0, move[0] * Time.deltaTime, 0);
        this.transform.Rotate(move[1] * Time.deltaTime, 0, 0);
        this.transform.localScale += new Vector3(move[2] * Time.deltaTime, move[2] * Time.deltaTime, 0);

        ProjectileModel.UpdateTime(Time.deltaTime);
    }

    private void ProjectileHittingTarget()
    {
        if (Player)
        {
            float projectile_x = this.transform.position.x;
            List<float> targets_x = TargetTransforms.Select(x => x.position.x).ToList();
            List<float> targets_scale = TargetTransforms.Select(x => x.lossyScale.x).ToList();
            List<int> hits = ProjectileModel.JudgeHit(projectile_x, targets_x, targets_scale);

            if (hits.Count > 0)
            {
                ProjectileHitEnemy.Invoke(hits, ProjectileModel.Attack);
                Destroy(gameObject);
            }
        }
        else
        {
            bool hit = ProjectileModel.JudgeHit(this.transform.position.x, this.transform.position.y, 
                                                this.transform.lossyScale.x * 0.5f, this.transform.lossyScale.y * 0.5f, 
                                                TargetTransform.position.x, TargetTransform.lossyScale.x * 0.5f);
            if (hit)
            {
                ProjectileHitPlayer.Invoke();
                Destroy(gameObject);
            }
        }
    }

    private void JudgeDestroy()
    {
        if (Player)
        {
            if (this.transform.position.y > 0)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (this.transform.position.y < -7)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
