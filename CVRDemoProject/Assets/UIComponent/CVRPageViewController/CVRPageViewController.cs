using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UIComponent.CVRPageViewController;
using UnityEditor.UI;

public class CVRPageViewController : MonoBehaviour,IBeginDragHandler,IEndDragHandler
{
    private ScrollRect m_PageViewControllerScrollRect;
    private RectTransform m_PageViewControllerRectTransform;
    private int m_PagesCount = 0;
    private float m_PagesScrollSpeed = 2f;
    private float m_targetPosition;
    
    /// <summary>
    /// 是否正在拖拽
    /// </summary>
    private bool m_IsDrag = true;
    /// <summary>
    /// 当前选中页面的索引
    /// </summary>
    private int m_SelectPageIndex = 0;
    /// <summary>
    /// 页面滑动偏移量多少算成功
    /// </summary>
    private float m_SlideSuccessRange;
    /// <summary>
    /// 开始滑动坐标点
    /// </summary>
    private float m_startMovePosition;
    /// <summary>
    /// 结束滑动的坐标点
    /// </summary>
    private float m_endMovePosition;
    /// <summary>
    /// Content物体的GridLayoutGroup组件
    /// </summary>
    private GridLayoutGroup contentGridLayoutGroup;
    
    /// <summary>
    /// PageViewController的内容容器
    /// </summary>
    public RectTransform PageContent;
    /// <summary>
    /// PageViewController可视窗口
    /// </summary>
    public RectTransform PageViewPort;
    /// <summary>
    /// 存放页面列表
    /// </summary>
    public List<GameObject> PagesList = new List<GameObject>();
    /// <summary>
    /// 页面的滚动方向
    /// </summary>
    public ScrollDirection PageScrollDirection;
    
    public delegate void MoveToPageDelegate(int Index);

    public MoveToPageDelegate PageViewControllerMoveToPage;

    private void Awake()
    {
        InitPageViewController();
    }

    private void Update()
    {
        if (!m_IsDrag && m_PagesCount > 0)
        {
            ChangeContentPosition();
            if (PageViewControllerMoveToPage != null)
            {
                PageViewControllerMoveToPage(m_SelectPageIndex);
            }
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        if (contentGridLayoutGroup && m_PageViewControllerRectTransform)
        {
            contentGridLayoutGroup.cellSize = m_PageViewControllerRectTransform.rect.size;
            m_SlideSuccessRange = PageScrollDirection == ScrollDirection.Horizontal
                ? m_PageViewControllerRectTransform.rect.size.x / 2
                : m_PageViewControllerRectTransform.rect.size.y / 2;
            LayoutRebuilder.ForceRebuildLayoutImmediate(PageContent);
        }
    }

    private void InitPageViewController()
    {
        ConfigPageViewController();
        AddScrollRectComponent();
        ConfigContent();
    }

    private void ConfigPageViewController()
    {
        m_SelectPageIndex = 0;
        m_PageViewControllerRectTransform = gameObject.GetComponent<RectTransform>();
        m_PagesCount = PagesList.Count;
        m_SlideSuccessRange = PageScrollDirection == ScrollDirection.Horizontal
            ? m_PageViewControllerRectTransform.rect.size.x / 2
            : m_PageViewControllerRectTransform.rect.size.y / 2;
    }

    private void AddScrollRectComponent()
    {
        m_PageViewControllerScrollRect = gameObject.AddComponent<ScrollRect>();
        // m_PageViewControllerScrollRect.hideFlags = HideFlags.HideInInspector;
        m_PageViewControllerScrollRect.content = PageContent;
        m_PageViewControllerScrollRect.viewport = PageViewPort;
        m_PageViewControllerScrollRect.movementType = ScrollRect.MovementType.Elastic;
        m_PageViewControllerScrollRect.horizontal = PageScrollDirection == ScrollDirection.Horizontal;
        m_PageViewControllerScrollRect.vertical = PageScrollDirection == ScrollDirection.Vertical;
        m_PageViewControllerScrollRect.inertia = false;
    }

    /// <summary>
    /// 配置Content
    /// </summary>
    private void ConfigContent()
    {
        //配置ContentSizeFitter组件
        ContentSizeFitter contentSizeFitter = PageContent.gameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        //配置GridLayoutGroup组件
        contentGridLayoutGroup = PageContent.gameObject.AddComponent<GridLayoutGroup>();
        contentGridLayoutGroup.cellSize = m_PageViewControllerRectTransform.rect.size;
        contentGridLayoutGroup.constraint = (PageScrollDirection == ScrollDirection.Horizontal)
            ? GridLayoutGroup.Constraint.FixedRowCount
            : GridLayoutGroup.Constraint.FixedColumnCount;
        contentGridLayoutGroup.constraintCount = 1;
        
        //修正Page页面层级
        foreach (var pageObject in PagesList)
        {
            pageObject.transform.SetParent(PageContent.transform);
        }
        //剔除没有绑定的Content里面的子物体
        for (int i = 0; i < PageContent.childCount; i++)
        {
            if(!PagesList.Contains(PageContent.GetChild(i).gameObject))
            {
                GameObject abandonGameObject = PageContent.GetChild(i).gameObject;
                abandonGameObject.transform.parent = null;
                Destroy(abandonGameObject);
            }
        }
    }

    private void MovePage(PositionDirection direction)
    {
        if (direction == PositionDirection.Up)
        {
            m_SelectPageIndex = m_SelectPageIndex >= m_PagesCount
                ? m_PagesCount - 1
                : m_SelectPageIndex + 1 >= m_PagesCount 
                    ? m_PagesCount - 1 
                    : m_SelectPageIndex + 1;
            m_targetPosition = m_SelectPageIndex * m_PageViewControllerRectTransform.rect.height;
        }
        else if (direction == PositionDirection.Down)
        {
            m_SelectPageIndex = m_SelectPageIndex < 0
                ? 0 
                : m_SelectPageIndex - 1 < 0 
                    ? 0 
                    : m_SelectPageIndex - 1;
            m_targetPosition = m_SelectPageIndex * m_PageViewControllerRectTransform.rect.height;
        }
        else if (direction == PositionDirection.Left)
        {
            m_SelectPageIndex = m_SelectPageIndex >= m_PagesCount
                ? m_PagesCount - 1 
                : m_SelectPageIndex + 1 >= m_PagesCount
                    ? m_PagesCount - 1 
                    : m_SelectPageIndex + 1;
            m_targetPosition = -m_SelectPageIndex * m_PageViewControllerRectTransform.rect.width;
        }
        else if (direction == PositionDirection.Right)
        {
            m_SelectPageIndex = m_SelectPageIndex < 0
                ? 0 
                : m_SelectPageIndex - 1 < 0 
                    ? 0 
                    : m_SelectPageIndex - 1;
            m_targetPosition = -m_SelectPageIndex * m_PageViewControllerRectTransform.rect.width;
        }
    }

    private void ChangeContentPosition()
    {
        Vector3 newLocalPosition = PageScrollDirection == ScrollDirection.Horizontal
            ? new Vector3(
                m_targetPosition, 0, 0)
            : new Vector3(
                0,m_targetPosition, 0);
        PageContent.localPosition = newLocalPosition;
        m_IsDrag = true;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        m_IsDrag = true;
        m_startMovePosition = PageScrollDirection == ScrollDirection.Horizontal
            ? eventData.position.x
            : eventData.position.y;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_IsDrag = false;
        m_endMovePosition = PageScrollDirection == ScrollDirection.Horizontal
            ? eventData.position.x
            : eventData.position.y;
        if (PageScrollDirection == ScrollDirection.Horizontal)
        {
            if (m_endMovePosition > m_startMovePosition &&
                m_endMovePosition - m_startMovePosition >= m_SlideSuccessRange)
            {
                MovePage(PositionDirection.Right);
            }
            else if(m_endMovePosition <= m_startMovePosition &&
                    m_startMovePosition - m_endMovePosition >= m_SlideSuccessRange)
            {
                MovePage(PositionDirection.Left);
            }
        }
        else if (PageScrollDirection == ScrollDirection.Vertical)
        {
            if (m_endMovePosition > m_startMovePosition &&
                m_endMovePosition - m_startMovePosition >= m_SlideSuccessRange)
            {
                MovePage(PositionDirection.Up);
            }
            else if(m_endMovePosition <= m_startMovePosition &&
                    m_startMovePosition - m_endMovePosition >= m_SlideSuccessRange)
            {
                MovePage(PositionDirection.Down);
            }
        }
    }

    public void MovePageWithIndex(int Index)
    {
        if (0 <= Index && Index < m_PagesCount)
        {
            m_SelectPageIndex = Index;
            m_targetPosition = PageScrollDirection == ScrollDirection.Horizontal
                ? -m_SelectPageIndex * m_PageViewControllerRectTransform.rect.width
                : m_SelectPageIndex * m_PageViewControllerRectTransform.rect.height;
            ChangeContentPosition();
        }
    }
}
