using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketController : MonoBehaviour
{
    [SerializeField]
    private Transform rightHandAnchor;

    [SerializeField]
    private Transform leftHandAnchor;

    [SerializeField]
    private Transform centerEyeAnchor;

    [SerializeField]
    private GameObject ball;

    // ボールの速度
    private float speed = 400.0f;

    // ボールを発射済みか
    private bool isBallMoving = false;

    private Transform Pointer
    {
        get
        {
            // 現在アクティブなコントローラーを取得
            if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
            {
                return rightHandAnchor;
            }
            else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
            {
                return leftHandAnchor;
            }
            // どちらも取れなければ目の間
            return centerEyeAnchor;
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MoveRacket();

        ShootBall();
    }

    void OnCollisionEnter(Collision collision)
    {
        CatchBall(collision);
    }

    private void MoveRacket()
    {
        // Oculus Go上で動いている場合は、Oculus Goのコントローラの少し先にラケットを表示する
        if (OVRManager.isHmdPresent)
        {
            var pointer = Pointer;
            if (pointer != null)
            {
                transform.parent.forward = pointer.forward;
                transform.parent.position = pointer.transform.position + (pointer.transform.forward * 2.5f);
            }
        }
        // 開発用にPCの場合は矢印キーで操作する
        else
        {
            float racketSpeed = 5.0f;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.parent.position += transform.up * racketSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.parent.position -= transform.up * racketSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.parent.position += transform.right * racketSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.parent.position -= transform.right * racketSpeed * Time.deltaTime;
            }
        }
    }

    private void ShootBall()
    {
        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (!isBallMoving)
            {
                isBallMoving = true;
                // ボールが自由に動くように親を解除
                ball.transform.parent = null;
                // Rigitbodyに力を加えてボールを発射
                var rb = ball.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(transform.forward * speed);
            }
        }
    }

    private void CatchBall(Collision collision)
    {
        // ラケットがボールと衝突したらボールをキャッチ
        if (collision.gameObject.name == "Ball")
        {
            var ballObj = collision.gameObject;
            // ボールがラケットについてくるようにRacketAreaの子にする
            ballObj.transform.parent = transform.parent;
            // ボールをラケットの手前に配置
            ballObj.transform.position = transform.position + (transform.forward * 0.5f);
            var rb = ballObj.GetComponent<Rigidbody>();
            // 物理演算を無効にして、ボールを止める
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            isBallMoving = false;
        }
    }
}