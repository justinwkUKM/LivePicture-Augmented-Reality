using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fps : MonoBehaviour
{

    string label = "";
    float count;

    public Text fpsCounter;

    IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);
                count = (1 / Time.deltaTime);
                label = "FPS :" + (Mathf.Round(count));
            }
            else
            {
                label = "Pause";
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        fpsCounter.text = label;
    }
}