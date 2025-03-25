using UnityEngine;
using PocketBaseSdk;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
public class DatabaseController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PocketBase pb;

    private void Start()
    {
        pb = new PocketBase("https://pocketbase.midnightstudio.me", "en-US", AsyncAuthStore.PlayerPrefs);
        if (pb.AuthStore == null || !pb.AuthStore.IsValid())
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Logout()
    {
        pb.AuthStore.Clear();
        SceneManager.LoadScene(0);
    }

    public async Task<List<AnchorDTO>> GetAllARObjects()
    {
        var rawAnchors = await pb.Collection("ar_objects").GetFullList();
        List<AnchorDTO> anchors = new List<AnchorDTO>();
        foreach (RecordModel a in rawAnchors)
        {
            anchors.Add(AnchorDTO.FromRecord(a));
        }
        return anchors;
    }

    public async Task<AnchorDTO> GetARObjectFromAnchorId(string anchorId)
    {
        var anchors = await GetAllARObjects();
        foreach (AnchorDTO anchorDTO in anchors)
        {
            if (anchorDTO.AnchorId == anchorId)
                return anchorDTO;
        }
        return new AnchorDTO();
    }
}
