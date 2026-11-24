using UnityEngine;
using TMPro;

public class ScrapMetalManager : MonoBehaviour
{
    public int ScrapMetalCount = 0;
    public TextMeshProUGUI ScrapMetalText;

    // Update is called once per frame
    void Update()
    {
        ScrapMetalText.text = "Scrap Metal: " + ScrapMetalCount;
    }
}
