using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;

    [SerializeField]
    private GameObject blockPrefab;

    [SerializeField]
    private GameObject timerText;

    [SerializeField]
    private GameObject clearText;

    [SerializeField]
    private AudioClip clearBGM;

    // ゲームスタートしたか
    private bool isStarted = false;

    // ブロックの残数
    private int blockCount;

    // 経過時間
    private float totalTime = 0.0f;

    // Use this for initialization
    void Start()
    {
        CreateBlocks();
    }

    // Update is called once per frame
    void Update()
    {
        // トリガが押されたらゲームスタート
        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            isStarted = true;
        }

        // 戻るボタンが押されたらリセット
        if (Input.GetKeyDown(KeyCode.Return) || OVRInput.Get(OVRInput.Button.Back))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        CountTime();
    }

    private void CreateBlocks()
    {
        // 適当にブロックの色を用意
        var colors = new Color[] {
            new Color(1.0f, 0.0f, 0.0f, 0.5f),
            new Color(0.0f, 1.0f, 0.0f, 0.5f),
            new Color(0.0f, 0.0f, 1.0f, 0.5f),
            new Color(1.0f, 1.0f, 0.0f, 0.5f),
            new Color(0.0f, 1.0f, 1.0f, 0.5f),
            new Color(1.0f, 0.0f, 1.0f, 0.5f),
            new Color(1.0f, 0.5f, 0.5f, 0.5f),
            new Color(0.5f, 1.0f, 0.5f, 0.5f),
            new Color(0.5f, 0.5f, 1.0f, 0.5f)
        };
        // ブロックの生成
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    var position = new Vector3(-2.0f + x * 1.0f, 0.8f + y * 0.8f, -1.0f + z * 2.0f);
                    GameObject block = Instantiate(blockPrefab, position, new Quaternion());
                    block.GetComponent<Renderer>().material.color = colors[z];
                    // ブロックが削除されたときの処理を追加
                    block.GetComponent<BlockController>().OnDestroy.AddListener(HandleBlockDestroy);
                    blockCount++;
                }
            }
        }
    }

    private void HandleBlockDestroy()
    {
        blockCount--;

        if (blockCount == 0)
        {
            // ゲーム終了
            isStarted = false;
            // クリア表示
            clearText.GetComponent<Text>().enabled = true;
            // 物理演算を無効にして、ボールを止める
            ball.GetComponent<Rigidbody>().isKinematic = true;
            // クリアBGM再生
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().PlayOneShot(clearBGM);
        }
    }

    private void CountTime()
    {
        if (isStarted)
        {
            totalTime += Time.deltaTime;
            timerText.GetComponent<Text>().text = totalTime.ToString("F2");
        }
    }
}