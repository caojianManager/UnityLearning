using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIComponent.CVRPageViewController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;
using Object = System.Object;

public class CVRSegmentViewController : MonoBehaviour
{
    public static string RESOURCES_PREFAB_SEGMENT_TAB = "Prefabs/Segment Tab";
    
    private ScrollRect m_SegmentViewControllerScrollRect;
    private RectTransform m_SegmentViewControllerRectTransform;
    private ContentSizeFitter m_ContentSizeFitter;
    private GridLayoutGroup m_ContentGridLayoutGroup;
    private List<GameObject> m_SegmentTabsLists = new List<GameObject>();
    private ToggleGroup m_ContentToggleGroup;
    private int m_SelectTabIndex = 0;

    public RectTransform SegmentContent;
    public RectTransform SegmentViewport;
    public ScrollDirection SegmentScrollDirection;
    public List<string> SegmentTitleLists = new List<string>();
    /// <summary>
    /// Tab文本距离左右两边的边距
    /// </summary>
    public int LeftAndRightTextPadding = 10;
    
    public int TopAndBottomTextPadding = 10;

    public int SelectTabFontSize = 24;
    public int NoSelectTabFontSize = 20;
    
    public Color SelectTabColor = Color.black;
    public Color NoSelectTabColor = Color.gray;

    public delegate void SwitchoverTab(int Index);
    public SwitchoverTab SegmentViewControllerSwitchoverTab;
    

    private void Awake()
    {
        InitSegmentView();
    }

    private void InitSegmentView()
    {
        ConfigSegmentViewController();
        ConfigContent();
        AddSegmentTabs();
        RefreshAllSegmentTabWithIndex(selectedIndex:m_SelectTabIndex);
    }

    private void ConfigSegmentViewController()
    {
        m_SegmentViewControllerRectTransform = gameObject.GetComponent<RectTransform>();
    }
    
    private void ConfigContent()
    {
        m_ContentToggleGroup = gameObject.AddComponent<ToggleGroup>();
        m_SegmentViewControllerScrollRect = gameObject.AddComponent<ScrollRect>();
        // m_PageViewControllerScrollRect.hideFlags = HideFlags.HideInInspector;
        m_SegmentViewControllerScrollRect.content = SegmentContent;
        m_SegmentViewControllerScrollRect.viewport = SegmentViewport;
        m_SegmentViewControllerScrollRect.movementType = ScrollRect.MovementType.Clamped;
        m_SegmentViewControllerScrollRect.horizontal = SegmentScrollDirection == ScrollDirection.Horizontal;
        m_SegmentViewControllerScrollRect.vertical = SegmentScrollDirection == ScrollDirection.Vertical;
        m_SegmentViewControllerScrollRect.inertia = false;
        if (SegmentScrollDirection != ScrollDirection.Horizontal)
        {
            VerticalLayoutGroup verticalLayoutGroup = SegmentContent.gameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.spacing = 0;
        }
        else
        {
            HorizontalLayoutGroup horizontalLayoutGroup = SegmentContent.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.spacing = 0;
        }
    }

    private void AddSegmentTabs()
    {
        for (int i = 0; i < SegmentTitleLists.Count; i++)
        {
            GameObject SegmentTabPrefab = Resources.Load(RESOURCES_PREFAB_SEGMENT_TAB) as GameObject;
            GameObject SegmentTab = Instantiate(SegmentTabPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            SegmentTab.transform.SetParent(SegmentContent, false);
            SegmentTab.name = "SegmentTab" + i;
            TextMeshProUGUI titleText = SegmentTab.GetComponentInChildren<TextMeshProUGUI>();
            titleText.text = SegmentTitleLists[i];
            RectTransform titleTextRectTransform = titleText.GetComponent<RectTransform>();
            // RectTransform segmentTabRectTransform = SegmentTab.GetComponent<RectTransform>();
            if (SegmentScrollDirection != ScrollDirection.Horizontal)
            {
                VerticalLayoutGroup verticalLayoutGroup = SegmentTab.AddComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.spacing = 0;
                verticalLayoutGroup.padding.left = LeftAndRightTextPadding;
                verticalLayoutGroup.padding.right = LeftAndRightTextPadding;
                verticalLayoutGroup.padding.top = TopAndBottomTextPadding;
                verticalLayoutGroup.padding.bottom = TopAndBottomTextPadding;
                verticalLayoutGroup.childControlWidth = false;
                verticalLayoutGroup.childForceExpandWidth= false;
                titleTextRectTransform.sizeDelta = new Vector2(m_SegmentViewControllerRectTransform.rect.width,titleTextRectTransform.rect.height);
            }
            else
            {
                HorizontalLayoutGroup horizontalLayoutGroup = SegmentTab.AddComponent<HorizontalLayoutGroup>();
                horizontalLayoutGroup.spacing = 0;
                horizontalLayoutGroup.padding.left = LeftAndRightTextPadding;
                horizontalLayoutGroup.padding.right = LeftAndRightTextPadding;
                horizontalLayoutGroup.padding.top = TopAndBottomTextPadding;
                horizontalLayoutGroup.padding.bottom = TopAndBottomTextPadding;
                horizontalLayoutGroup.childControlHeight= false;
                horizontalLayoutGroup.childForceExpandHeight = false;
                titleTextRectTransform.sizeDelta = new Vector2(titleTextRectTransform.rect.width,m_SegmentViewControllerRectTransform.rect.height);
            }

            Toggle tabToggle = SegmentTab.GetComponent<Toggle>();
            tabToggle.group = m_ContentToggleGroup;
            int toggleIndex = i;
            tabToggle.onValueChanged.AddListener(call:(bool value) => OnClickSegmentTab(toggleIndex,true));
            m_SegmentTabsLists.Add(SegmentTab);
        }
    }

    private void RefreshAllSegmentTabWithIndex(int selectedIndex)
    {
        for (int i = 0; i < m_SegmentTabsLists.Count; i++)
        {
            bool isSelectIndex = i == selectedIndex;
            GameObject segmentTab = m_SegmentTabsLists[i];
            TextMeshProUGUI textMeshProUGUI = segmentTab.GetComponentInChildren<TextMeshProUGUI>();
            textMeshProUGUI.fontSize = isSelectIndex ? SelectTabFontSize : NoSelectTabFontSize;
            textMeshProUGUI.fontStyle = isSelectIndex ? FontStyles.Bold : FontStyles.Normal;
            textMeshProUGUI.color = isSelectIndex ? SelectTabColor : NoSelectTabColor;
            RectTransform imageRectTransform = textMeshProUGUI.transform.GetComponentInChildren<RectTransform>();
            imageRectTransform.transform.GetChild(0).gameObject.SetActive(isSelectIndex);
        }
    }

    public void OnClickSegmentTab(int Index, bool isSwitchoverTab)
    {
        if (m_SelectTabIndex == Index)
        {
            return;
        }
        m_SelectTabIndex = Index;
        if (SegmentViewControllerSwitchoverTab != null && isSwitchoverTab)
        {
            SegmentViewControllerSwitchoverTab(m_SelectTabIndex);
        }
        RefreshAllSegmentTabWithIndex(m_SelectTabIndex);
    }
   
}
