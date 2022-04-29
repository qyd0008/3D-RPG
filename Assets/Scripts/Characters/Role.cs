using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Role : MonoBehaviour
{
    protected CharacterStats characterStats;
    protected NavMeshAgent agent;
    protected Animator anim;

    public Transform showBleedingPos;
    public GameObject bleedingPrefab;

    Transform cam;
    Canvas uiCanvas;
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        characterStats.BleedingOnAttack += Bleeding;
    }

    void OnEnable()
    {
        cam = Camera.main.transform;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            Debug.Log("canvas.name="+canvas.name);
            if (canvas.name == "WorldSpace Canvas")
            {
                uiCanvas = canvas;
                break;
            }
        }
    }

    private void Bleeding(int damage, bool isCritical)
    {
        GameObject bleedObj = Instantiate(bleedingPrefab,uiCanvas.transform);
        Text bleedText = bleedObj.transform.GetChild(0).GetComponent<Text>();
        bleedObj.transform.position = showBleedingPos.position;
        bleedObj.transform.forward = -cam.forward;
        bleedText.text = "-" + damage;

        if (isCritical)
        {
            RectTransform rectTran = bleedObj.GetComponent<RectTransform>();
            rectTran.localScale = new Vector3(2f,2f,2f);
        }

        Destroy(bleedObj,2);
    }
}
