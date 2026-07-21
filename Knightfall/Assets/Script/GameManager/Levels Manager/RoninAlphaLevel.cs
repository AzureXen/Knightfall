using UnityEngine;

public class RoninAlphaLevel : MonoBehaviour
{
    public void openExitGate()
    {
        GameObject map = GameObject.FindGameObjectWithTag("Map");
        GameObject spikes = map.transform.Find("Spikes_Exit").gameObject;
        if (spikes != null)
        {
            spikes.SetActive(false);
        }
        else
        {
            Debug.Log("Cannot find gate.");
        }
    }
    public void closeEntranceGate()
    {
        GameObject map = GameObject.FindGameObjectWithTag("Map");
        GameObject spikes = map.transform.Find("Spikes_Entrance").gameObject;
        if (spikes != null)
        {
            spikes.SetActive(true);
        }
        else
        {
            Debug.Log("Cannot find gate.");
        }
    }

    public void openEntranceGate()
    {
        GameObject map = GameObject.FindGameObjectWithTag("Map");
        GameObject spikes = map.transform.Find("Spikes_Entrance").gameObject;
        if (spikes != null)
        {
            spikes.SetActive(false);
        }
        else
        {
            Debug.Log("Cannot find gate.");
        }
    }
}
