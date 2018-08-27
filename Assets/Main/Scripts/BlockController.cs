using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlockController : MonoBehaviour
{
    public UnityEvent OnDestroy;

    // Use this for initialization
    void Start()
    {
        if (OnDestroy == null)
            OnDestroy = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        // ブロックを消す
        Destroy(this.gameObject);
        // ブロック削除イベントを発生させる
        OnDestroy.Invoke();
    }
}