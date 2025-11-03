using System.Collections;
using TMPro;
using UnityEngine;

public class TxtHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] txts;
    private float waitTime;

    private void Update()
    {
        if( waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
            {
                InActiveTxt();
                return;
            }
        }
        
    }

    public void ActiveTxt(int idx)
    {
        for (int i = 0; i < txts.Length; i++)
        {
            txts[i].gameObject.SetActive(false);
        }

        txts[idx].gameObject.SetActive(true);
        waitTime = 2f;
    }

    public void InActiveTxt()
    {
        for (int i = 0; i < txts.Length; i++)
        {
            txts[i].gameObject.SetActive(false);
        }
    }
}
