using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.XR.ARCoreExtensions;
using NUnit.Framework;
using PocketBaseSdk;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlacementController : MonoBehaviour
{
    public XROrigin Origin;

    public ARSession SessionCore;

    public ARCoreExtensions Extensions;

    public ARAnchorManager AnchorManager;

    public ARPlaneManager PlaneManager;

    public ARRaycastManager RaycastManager;

    public DatabaseController DBController;

    [HideInInspector]
    public PlacementStep Step = PlacementStep.None;

    public HashSet<string> ResolvingSet = new HashSet<string>();

    public enum PlacementStep
    {
        None,
        Inventory,
        Place,
        Interact,
        Anchor
    }

    public Camera MainCamera
    {
        get
        {
            return Origin.Camera;
        }
    }

    public async void saveARObject(string name, string anchorId, string userId, string modelId, float scale)
    {
        AnchorDTO anchorDTO = new()
        {
            Name = name,
            AnchorId = anchorId,
            UserId = userId,
            ModelId = modelId,
            Scale = scale
        };

        try
        {
            var anchor = await DBController.pb.Collection("ar_objects").Create(anchorDTO);
        }
        catch (Exception e)
        {
            print(e.InnerException.ToString());
        }
    }

    public async Task<List<AnchorDTO>> getAllARObjects()
    {
        var rawAnchors = await DBController.pb.Collection("ar_objects").GetFullList();
        List<AnchorDTO> anchors = new List<AnchorDTO>();
        foreach (RecordModel a in rawAnchors)
        {
            anchors.Add(AnchorDTO.FromRecord(a));
        }
        return anchors;
    }

    public async Task<AnchorDTO> getARObjectFromAnchorId(string anchorId)
    {
        var anchors = await getAllARObjects();
        foreach (AnchorDTO anchorDTO in anchors)
        {
            if (anchorDTO.AnchorId == anchorId)
                return anchorDTO;
        }
        return new AnchorDTO();
    }
}