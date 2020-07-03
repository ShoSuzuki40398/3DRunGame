using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Range(1,10)]
    public float speed = 1;

    private AreaController areaController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, speed * Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }

    /// <summary>
    /// エリア制御設定
    /// </summary>
    public void SetAreaController(AreaController controller)
    {
        areaController = controller;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "AreaEntrance":
                areaController.RemoveLeadArea();
                areaController.AddArea();
                break;
        }
    }
}
