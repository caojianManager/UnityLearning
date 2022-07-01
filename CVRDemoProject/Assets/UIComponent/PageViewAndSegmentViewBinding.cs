// ///<summary>
// ///ClassName:PageViewAndSegmentViewBinding.cs
// ///Explain:--------------
// ///Author:Cao Jian
// ///Date:2022-06-29
// ///</summary>

using System;
using UnityEngine;


public class PageViewAndSegmentViewBinding : MonoBehaviour
{
    private CVRPageViewController m_CVRPageViewController;
    private CVRSegmentViewController m_CVRSegmentViewController;
    
    public GameObject PageViewController;
    public GameObject SegmentViewController;

    private void Awake()
    {
        InitPageViewAndSegmentViewBinding();
    }

    private void InitPageViewAndSegmentViewBinding()
    {
        m_CVRPageViewController = PageViewController.GetComponent<CVRPageViewController>();
        m_CVRPageViewController.PageViewControllerMoveToPage = PageViewControllerMoveToPage;
        m_CVRSegmentViewController = SegmentViewController.GetComponentInChildren<CVRSegmentViewController>();
        m_CVRSegmentViewController.SegmentViewControllerSwitchoverTab = SegmentViewControllerSwitchoverTab;
    }

    private void SegmentViewControllerSwitchoverTab(int index)
    {
        m_CVRPageViewController.MovePageWithIndex(index);
    }

    private void PageViewControllerMoveToPage(int index)
    {
        m_CVRSegmentViewController.OnClickSegmentTab(index, false);
    }

}
