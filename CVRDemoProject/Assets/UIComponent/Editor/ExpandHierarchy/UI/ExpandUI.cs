using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ExpandUI : MonoBehaviour
{
    public static string RESOURCES_PREFAB_PAGE_VIEW_CONTROLLER =  "Prefabs/Page View Controller";
    public static string RESOURCES_PREFAB_SEGMENT_VIEW_CONTROLLER =  "Prefabs/Segment View Controller";
    
    [MenuItem("GameObject/UI/PageViewController",false,0)]
    static void CreatePageViewController()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject pageViewControllerPrefab = Resources.Load(RESOURCES_PREFAB_PAGE_VIEW_CONTROLLER) as GameObject;
                GameObject pageViewController = Instantiate(pageViewControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                pageViewController.transform.SetParent(Selection.activeTransform, false);
                pageViewController.name = "Page View Controller";
                Selection.activeTransform = pageViewController.transform;
            }
        }
    }
    
    [MenuItem("GameObject/UI/SegmentViewController",false,1)]
    static void CreateSegmentViewController()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject segmentViewControllerPrefab = Resources.Load(RESOURCES_PREFAB_SEGMENT_VIEW_CONTROLLER) as GameObject;
                GameObject segmentViewController = Instantiate(segmentViewControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                segmentViewController.transform.SetParent(Selection.activeTransform, false);
                segmentViewController.name = "Segment View Controller";
                Selection.activeTransform = segmentViewController.transform;
            }
        }
    }
}
