using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameDirecter : MonoBehaviour
{
    //プレイヤー
    public Player Player { get; private set; } //Jsonで管理
    public PlayerModel PlayerModel { get; private set; }
    public GameObject PlayerObject { get; private set; }
    public PlayerPresender PlayerPresender { get; private set; }

    private PlayerGenerator playerGenerator; //プレイヤープレハブからプレイヤーオブジェクトを生成するためのクラス

    //エネミー
    public Enemy Enemy { get; private set; } //Jsonでで管理
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
    public List<EnemyModel> EnemyModel { get; private set; } = new List<EnemyModel>();
    public List<GameObject> EnemyObject { get; private set; } = new List<GameObject>();
    public List<EnemyPresender> EnemyPresender { get; private set; } = new List<EnemyPresender>();
    public List<Transform> EnemyTransforms { get; private set; } = new List<Transform>(); //プレイヤーの攻撃の当たり判定に使用する

    private EnemyGenerator enemyGenerator; //エネミープレハブからエネミーオブジェクトを生成するためのクラス

    //プロジェクティル
    public Dictionary<string, Projectile> Projectile { get; private set; } = new Dictionary<string, Projectile>(); //Jsonで管理
    public Dictionary<KeyCode, Projectile> PlayerProjectile { get; private set; } = new Dictionary<KeyCode, Projectile>(); //キーごとに割り振る

    private PlayerProjectileEvent playerProjectileEvent;
    private EnemyProjectileEvent enemyProjectileEvent;


    private void Awake()
    {
        //Jsonで管理
        Projectile.Add("Knife", new Projectile("Knife", true, 2.0f, 0.2f, 0.0625f, 2.5f, 0, -0.8f));
        Projectile.Add("Fire", new Projectile("Fire", false, 0f, 0f, 3.0f, -8.5f, -90.0f, 0.8f));
        Projectile.Add("Blast", new Projectile("Blast", false, 0f, 0f, 0.5f, -8.5f, -90.0f, 0.4f));

        //Jsonで管理
        Player = new Player("Sworder", 15.0f, 0.4f);
        //Enemy = new Enemy("Devil", 15.0f, 3.75f, 10, 1, 8, enemyProjectileName);

        Enemies.Add(JsonConvertToEnemy("Devil"));
        Enemies.Add(JsonConvertToEnemy("Devil"));

        PlayerProjectile.Add(KeyCode.UpArrow, Projectile["Knife"]);

        //プレイヤーとエネミーのインスタンス生成
        GeneratePlayer();
        GenerateEnemy();

        //イベント設定
        playerProjectileEvent = new PlayerProjectileEvent(PlayerPresender, EnemyPresender, PlayerProjectile, EnemyTransforms);
        PlayerObject.GetComponent<PlayerController>().UpArrowKey += playerProjectileEvent.ThrowProjectile;
        enemyProjectileEvent = new EnemyProjectileEvent(PlayerPresender, PlayerObject.GetComponent<Transform>(), Projectile);
        EnemyObject.ForEach(x => x.GetComponent<EnemyController>().ThrowProjectile += enemyProjectileEvent.ThrowProjectile);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EnemyPresender.ForEach(x => x.SelfHealing(Time.deltaTime));
        PlayerPresender.RecoverGuts();
    }

    private void GeneratePlayer()
    {
        playerGenerator = GetComponent<PlayerGenerator>();
        PlayerModel = new PlayerModel(Player); //
        PlayerObject = playerGenerator.Generate(Player); //スピードやスプライトなどのデータをplayerを参照し設定される
        PlayerPresender = new PlayerPresender(PlayerModel, PlayerObject.GetComponent<PlayerController>());
    }

    private void GenerateEnemy()
    { 
        enemyGenerator = GetComponent<EnemyGenerator>();
        Enemies.ForEach(x => EnemyModel.Add(new EnemyModel(x)));
        Enemies.ForEach(x => EnemyObject.Add(enemyGenerator.Generate(x)));
        for (int i = 0; i < Enemies.Count; i++)
        {
            EnemyPresender.Add(new EnemyPresender(EnemyModel[i], EnemyObject[i].GetComponent<EnemyController>()));
            EnemyTransforms.Add(EnemyObject[i].GetComponent<Transform>());
        }
    }

    private Enemy JsonConvertToEnemy(string name)
    {
        StreamReader reader;
        reader = new StreamReader(Application.dataPath + "/JsonData/JsonEnemy" + $"/{name}.json");
        string data = reader.ReadToEnd();
        reader.Close();

        Debug.Log(data);

        JsonEnemy enemy = JsonUtility.FromJson<JsonEnemy>(data);

        Debug.Log(enemy.Name);
        Debug.Log(enemy.Projectiles);

        return new Enemy(enemy.Name, enemy.Hp, enemy.Heal, enemy.Speed, enemy.Span, enemy.Power, enemy.Projectiles);
    }
}
