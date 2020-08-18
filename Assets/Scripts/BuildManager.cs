using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    private BuffScript tempBuff;
    [SerializeField] private List<BuffScript> listBuff;
    [SerializeField] private GameObject Slot;
    [SerializeField] private Transform slotsContent1;
    [SerializeField] private Transform slotsContent2;
    [SerializeField] private Transform slotsContent3;

    private void Start()
    {
        GameObject slot1 = Instantiate(Slot, slotsContent1.position, Quaternion.identity);
        slot1.transform.SetParent(slotsContent1);
        tempBuff = listBuff[Random.Range(0, listBuff.Count)];
        slot1.GetComponent<SlotScript>().Setup(tempBuff);
        listBuff.Remove(tempBuff);

        GameObject slot2 = Instantiate(Slot, slotsContent2.position, Quaternion.identity);
        slot2.transform.SetParent(slotsContent2);
        tempBuff = listBuff[Random.Range(0, listBuff.Count)];
        slot2.GetComponent<SlotScript>().Setup(tempBuff);
        listBuff.Remove(tempBuff);

        GameObject slot3 = Instantiate(Slot, slotsContent3.position, Quaternion.identity);
        slot3.transform.SetParent(slotsContent3);
        tempBuff = listBuff[Random.Range(0, listBuff.Count)];
        slot3.GetComponent<SlotScript>().Setup(tempBuff);
        listBuff.Remove(tempBuff);
    }
}
