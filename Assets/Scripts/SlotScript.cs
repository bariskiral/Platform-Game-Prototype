using UnityEngine;
using UnityEngine.UI;


public class SlotScript : MonoBehaviour
{
    private BuffScript buffSlot;
    private Button buffButton;
    private Image buffImage;

    private void Awake()
    {
        buffButton = GetComponent<Button>();
        buffImage = GetComponent<Image>();
    }

    public void Setup(BuffScript buff)
    {
        buffSlot = buff;
        buffImage.sprite = buff.buffIcon;
        buffButton.onClick.AddListener(controlSlot);
    }

    private void controlSlot()
    {
        if (buffSlot.buffNumber == "1")
        {
            Debug.Log("1");
        }
        else if(buffSlot.buffNumber == "2")
        {
            Debug.Log("2");
        }
        else if (buffSlot.buffNumber == "3")
        {
            Debug.Log("3");
        }
        else if (buffSlot.buffNumber == "4")
        {
            Debug.Log("4");
        }
    }
}