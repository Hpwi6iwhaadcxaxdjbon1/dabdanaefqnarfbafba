using System;
using Rust;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x0200083C RID: 2108
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Scroll Rect Ex", 37)]
	[SelectionBase]
	public class ScrollRectEx : UIBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutGroup, ILayoutController
	{
		// Token: 0x040028F6 RID: 10486
		public PointerEventData.InputButton scrollButton;

		// Token: 0x040028F7 RID: 10487
		public PointerEventData.InputButton altScrollButton;

		// Token: 0x040028F8 RID: 10488
		[SerializeField]
		private RectTransform m_Content;

		// Token: 0x040028F9 RID: 10489
		[SerializeField]
		private bool m_Horizontal = true;

		// Token: 0x040028FA RID: 10490
		[SerializeField]
		private bool m_Vertical = true;

		// Token: 0x040028FB RID: 10491
		[SerializeField]
		private ScrollRectEx.MovementType m_MovementType = ScrollRectEx.MovementType.Elastic;

		// Token: 0x040028FC RID: 10492
		[SerializeField]
		private float m_Elasticity = 0.1f;

		// Token: 0x040028FD RID: 10493
		[SerializeField]
		private bool m_Inertia = true;

		// Token: 0x040028FE RID: 10494
		[SerializeField]
		private float m_DecelerationRate = 0.135f;

		// Token: 0x040028FF RID: 10495
		[SerializeField]
		private float m_ScrollSensitivity = 1f;

		// Token: 0x04002900 RID: 10496
		[SerializeField]
		private RectTransform m_Viewport;

		// Token: 0x04002901 RID: 10497
		[SerializeField]
		private Scrollbar m_HorizontalScrollbar;

		// Token: 0x04002902 RID: 10498
		[SerializeField]
		private Scrollbar m_VerticalScrollbar;

		// Token: 0x04002903 RID: 10499
		[SerializeField]
		private ScrollRectEx.ScrollbarVisibility m_HorizontalScrollbarVisibility;

		// Token: 0x04002904 RID: 10500
		[SerializeField]
		private ScrollRectEx.ScrollbarVisibility m_VerticalScrollbarVisibility;

		// Token: 0x04002905 RID: 10501
		[SerializeField]
		private float m_HorizontalScrollbarSpacing;

		// Token: 0x04002906 RID: 10502
		[SerializeField]
		private float m_VerticalScrollbarSpacing;

		// Token: 0x04002907 RID: 10503
		[SerializeField]
		private ScrollRectEx.ScrollRectEvent m_OnValueChanged = new ScrollRectEx.ScrollRectEvent();

		// Token: 0x04002908 RID: 10504
		private Vector2 m_PointerStartLocalCursor = Vector2.zero;

		// Token: 0x04002909 RID: 10505
		private Vector2 m_ContentStartPosition = Vector2.zero;

		// Token: 0x0400290A RID: 10506
		private RectTransform m_ViewRect;

		// Token: 0x0400290B RID: 10507
		private Bounds m_ContentBounds;

		// Token: 0x0400290C RID: 10508
		private Bounds m_ViewBounds;

		// Token: 0x0400290D RID: 10509
		private Vector2 m_Velocity;

		// Token: 0x0400290E RID: 10510
		private bool m_Dragging;

		// Token: 0x0400290F RID: 10511
		private Vector2 m_PrevPosition = Vector2.zero;

		// Token: 0x04002910 RID: 10512
		private Bounds m_PrevContentBounds;

		// Token: 0x04002911 RID: 10513
		private Bounds m_PrevViewBounds;

		// Token: 0x04002912 RID: 10514
		[NonSerialized]
		private bool m_HasRebuiltLayout;

		// Token: 0x04002913 RID: 10515
		private bool m_HSliderExpand;

		// Token: 0x04002914 RID: 10516
		private bool m_VSliderExpand;

		// Token: 0x04002915 RID: 10517
		private float m_HSliderHeight;

		// Token: 0x04002916 RID: 10518
		private float m_VSliderWidth;

		// Token: 0x04002917 RID: 10519
		[NonSerialized]
		private RectTransform m_Rect;

		// Token: 0x04002918 RID: 10520
		private RectTransform m_HorizontalScrollbarRect;

		// Token: 0x04002919 RID: 10521
		private RectTransform m_VerticalScrollbarRect;

		// Token: 0x0400291A RID: 10522
		private DrivenRectTransformTracker m_Tracker;

		// Token: 0x0400291B RID: 10523
		private readonly Vector3[] m_Corners = new Vector3[4];

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06002DAE RID: 11694 RVA: 0x000237BD File Offset: 0x000219BD
		// (set) Token: 0x06002DAF RID: 11695 RVA: 0x000237C5 File Offset: 0x000219C5
		public RectTransform content
		{
			get
			{
				return this.m_Content;
			}
			set
			{
				this.m_Content = value;
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06002DB0 RID: 11696 RVA: 0x000237CE File Offset: 0x000219CE
		// (set) Token: 0x06002DB1 RID: 11697 RVA: 0x000237D6 File Offset: 0x000219D6
		public bool horizontal
		{
			get
			{
				return this.m_Horizontal;
			}
			set
			{
				this.m_Horizontal = value;
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06002DB2 RID: 11698 RVA: 0x000237DF File Offset: 0x000219DF
		// (set) Token: 0x06002DB3 RID: 11699 RVA: 0x000237E7 File Offset: 0x000219E7
		public bool vertical
		{
			get
			{
				return this.m_Vertical;
			}
			set
			{
				this.m_Vertical = value;
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06002DB4 RID: 11700 RVA: 0x000237F0 File Offset: 0x000219F0
		// (set) Token: 0x06002DB5 RID: 11701 RVA: 0x000237F8 File Offset: 0x000219F8
		public ScrollRectEx.MovementType movementType
		{
			get
			{
				return this.m_MovementType;
			}
			set
			{
				this.m_MovementType = value;
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06002DB6 RID: 11702 RVA: 0x00023801 File Offset: 0x00021A01
		// (set) Token: 0x06002DB7 RID: 11703 RVA: 0x00023809 File Offset: 0x00021A09
		public float elasticity
		{
			get
			{
				return this.m_Elasticity;
			}
			set
			{
				this.m_Elasticity = value;
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06002DB8 RID: 11704 RVA: 0x00023812 File Offset: 0x00021A12
		// (set) Token: 0x06002DB9 RID: 11705 RVA: 0x0002381A File Offset: 0x00021A1A
		public bool inertia
		{
			get
			{
				return this.m_Inertia;
			}
			set
			{
				this.m_Inertia = value;
			}
		}

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06002DBA RID: 11706 RVA: 0x00023823 File Offset: 0x00021A23
		// (set) Token: 0x06002DBB RID: 11707 RVA: 0x0002382B File Offset: 0x00021A2B
		public float decelerationRate
		{
			get
			{
				return this.m_DecelerationRate;
			}
			set
			{
				this.m_DecelerationRate = value;
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06002DBC RID: 11708 RVA: 0x00023834 File Offset: 0x00021A34
		// (set) Token: 0x06002DBD RID: 11709 RVA: 0x0002383C File Offset: 0x00021A3C
		public float scrollSensitivity
		{
			get
			{
				return this.m_ScrollSensitivity;
			}
			set
			{
				this.m_ScrollSensitivity = value;
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06002DBE RID: 11710 RVA: 0x00023845 File Offset: 0x00021A45
		// (set) Token: 0x06002DBF RID: 11711 RVA: 0x0002384D File Offset: 0x00021A4D
		public RectTransform viewport
		{
			get
			{
				return this.m_Viewport;
			}
			set
			{
				this.m_Viewport = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06002DC0 RID: 11712 RVA: 0x0002385C File Offset: 0x00021A5C
		// (set) Token: 0x06002DC1 RID: 11713 RVA: 0x000E584C File Offset: 0x000E3A4C
		public Scrollbar horizontalScrollbar
		{
			get
			{
				return this.m_HorizontalScrollbar;
			}
			set
			{
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.m_HorizontalScrollbar = value;
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06002DC2 RID: 11714 RVA: 0x00023864 File Offset: 0x00021A64
		// (set) Token: 0x06002DC3 RID: 11715 RVA: 0x000E58B8 File Offset: 0x000E3AB8
		public Scrollbar verticalScrollbar
		{
			get
			{
				return this.m_VerticalScrollbar;
			}
			set
			{
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.m_VerticalScrollbar = value;
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06002DC4 RID: 11716 RVA: 0x0002386C File Offset: 0x00021A6C
		// (set) Token: 0x06002DC5 RID: 11717 RVA: 0x00023874 File Offset: 0x00021A74
		public ScrollRectEx.ScrollbarVisibility horizontalScrollbarVisibility
		{
			get
			{
				return this.m_HorizontalScrollbarVisibility;
			}
			set
			{
				this.m_HorizontalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06002DC6 RID: 11718 RVA: 0x00023883 File Offset: 0x00021A83
		// (set) Token: 0x06002DC7 RID: 11719 RVA: 0x0002388B File Offset: 0x00021A8B
		public ScrollRectEx.ScrollbarVisibility verticalScrollbarVisibility
		{
			get
			{
				return this.m_VerticalScrollbarVisibility;
			}
			set
			{
				this.m_VerticalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06002DC8 RID: 11720 RVA: 0x0002389A File Offset: 0x00021A9A
		// (set) Token: 0x06002DC9 RID: 11721 RVA: 0x000238A2 File Offset: 0x00021AA2
		public float horizontalScrollbarSpacing
		{
			get
			{
				return this.m_HorizontalScrollbarSpacing;
			}
			set
			{
				this.m_HorizontalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06002DCA RID: 11722 RVA: 0x000238B1 File Offset: 0x00021AB1
		// (set) Token: 0x06002DCB RID: 11723 RVA: 0x000238B9 File Offset: 0x00021AB9
		public float verticalScrollbarSpacing
		{
			get
			{
				return this.m_VerticalScrollbarSpacing;
			}
			set
			{
				this.m_VerticalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06002DCC RID: 11724 RVA: 0x000238C8 File Offset: 0x00021AC8
		// (set) Token: 0x06002DCD RID: 11725 RVA: 0x000238D0 File Offset: 0x00021AD0
		public ScrollRectEx.ScrollRectEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06002DCE RID: 11726 RVA: 0x000E5924 File Offset: 0x000E3B24
		protected RectTransform viewRect
		{
			get
			{
				if (this.m_ViewRect == null)
				{
					this.m_ViewRect = this.m_Viewport;
				}
				if (this.m_ViewRect == null)
				{
					this.m_ViewRect = (RectTransform)base.transform;
				}
				return this.m_ViewRect;
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06002DCF RID: 11727 RVA: 0x000238D9 File Offset: 0x00021AD9
		// (set) Token: 0x06002DD0 RID: 11728 RVA: 0x000238E1 File Offset: 0x00021AE1
		public Vector2 velocity
		{
			get
			{
				return this.m_Velocity;
			}
			set
			{
				this.m_Velocity = value;
			}
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06002DD1 RID: 11729 RVA: 0x000238EA File Offset: 0x00021AEA
		private RectTransform rectTransform
		{
			get
			{
				if (this.m_Rect == null)
				{
					this.m_Rect = base.GetComponent<RectTransform>();
				}
				return this.m_Rect;
			}
		}

		// Token: 0x06002DD2 RID: 11730 RVA: 0x000E5970 File Offset: 0x000E3B70
		protected ScrollRectEx()
		{
		}

		// Token: 0x06002DD3 RID: 11731 RVA: 0x0002390C File Offset: 0x00021B0C
		public virtual void Rebuild(CanvasUpdate executing)
		{
			if (executing == null)
			{
				this.UpdateCachedData();
			}
			if (executing == 2)
			{
				this.UpdateBounds();
				this.UpdateScrollbars(Vector2.zero);
				this.UpdatePrevData();
				this.m_HasRebuiltLayout = true;
			}
		}

		// Token: 0x06002DD4 RID: 11732 RVA: 0x000E59F8 File Offset: 0x000E3BF8
		private void UpdateCachedData()
		{
			Transform transform = base.transform;
			this.m_HorizontalScrollbarRect = ((this.m_HorizontalScrollbar == null) ? null : (this.m_HorizontalScrollbar.transform as RectTransform));
			this.m_VerticalScrollbarRect = ((this.m_VerticalScrollbar == null) ? null : (this.m_VerticalScrollbar.transform as RectTransform));
			bool flag = this.viewRect.parent == transform;
			bool flag2 = !this.m_HorizontalScrollbarRect || this.m_HorizontalScrollbarRect.parent == transform;
			bool flag3 = !this.m_VerticalScrollbarRect || this.m_VerticalScrollbarRect.parent == transform;
			bool flag4 = flag && flag2 && flag3;
			this.m_HSliderExpand = (flag4 && this.m_HorizontalScrollbarRect && this.horizontalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_VSliderExpand = (flag4 && this.m_VerticalScrollbarRect && this.verticalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_HSliderHeight = ((this.m_HorizontalScrollbarRect == null) ? 0f : this.m_HorizontalScrollbarRect.rect.height);
			this.m_VSliderWidth = ((this.m_VerticalScrollbarRect == null) ? 0f : this.m_VerticalScrollbarRect.rect.width);
		}

		// Token: 0x06002DD5 RID: 11733 RVA: 0x000E5B58 File Offset: 0x000E3D58
		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		// Token: 0x06002DD6 RID: 11734 RVA: 0x000E5BC4 File Offset: 0x000E3DC4
		protected override void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			this.m_HasRebuiltLayout = false;
			this.m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			base.OnDisable();
		}

		// Token: 0x06002DD7 RID: 11735 RVA: 0x00023939 File Offset: 0x00021B39
		public override bool IsActive()
		{
			return base.IsActive() && this.m_Content != null;
		}

		// Token: 0x06002DD8 RID: 11736 RVA: 0x00023951 File Offset: 0x00021B51
		private void EnsureLayoutHasRebuilt()
		{
			if (!this.m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
			{
				Canvas.ForceUpdateCanvases();
			}
		}

		// Token: 0x06002DD9 RID: 11737 RVA: 0x00023967 File Offset: 0x00021B67
		public virtual void StopMovement()
		{
			this.m_Velocity = Vector2.zero;
		}

		// Token: 0x06002DDA RID: 11738 RVA: 0x000E5C54 File Offset: 0x000E3E54
		public virtual void OnScroll(PointerEventData data)
		{
			if (!this.IsActive())
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			Vector2 scrollDelta = data.scrollDelta;
			scrollDelta.y *= -1f;
			if (this.vertical && !this.horizontal)
			{
				if (Mathf.Abs(scrollDelta.x) > Mathf.Abs(scrollDelta.y))
				{
					scrollDelta.y = scrollDelta.x;
				}
				scrollDelta.x = 0f;
			}
			if (this.horizontal && !this.vertical)
			{
				if (Mathf.Abs(scrollDelta.y) > Mathf.Abs(scrollDelta.x))
				{
					scrollDelta.x = scrollDelta.y;
				}
				scrollDelta.y = 0f;
			}
			Vector2 vector = this.m_Content.anchoredPosition;
			vector += scrollDelta * this.m_ScrollSensitivity;
			if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
			{
				vector += this.CalculateOffset(vector - this.m_Content.anchoredPosition);
			}
			this.SetContentAnchoredPosition(vector);
			this.UpdateBounds();
		}

		// Token: 0x06002DDB RID: 11739 RVA: 0x00023974 File Offset: 0x00021B74
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			this.m_Velocity = Vector2.zero;
		}

		// Token: 0x06002DDC RID: 11740 RVA: 0x000E5D64 File Offset: 0x000E3F64
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			this.UpdateBounds();
			this.m_PointerStartLocalCursor = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, ref this.m_PointerStartLocalCursor);
			this.m_ContentStartPosition = this.m_Content.anchoredPosition;
			this.m_Dragging = true;
		}

		// Token: 0x06002DDD RID: 11741 RVA: 0x0002399E File Offset: 0x00021B9E
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			this.m_Dragging = false;
		}

		// Token: 0x06002DDE RID: 11742 RVA: 0x000E5DE0 File Offset: 0x000E3FE0
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			Vector2 a;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, ref a))
			{
				return;
			}
			this.UpdateBounds();
			Vector2 b = a - this.m_PointerStartLocalCursor;
			Vector2 vector = this.m_ContentStartPosition + b;
			Vector2 vector2 = this.CalculateOffset(vector - this.m_Content.anchoredPosition);
			vector += vector2;
			if (this.m_MovementType == ScrollRectEx.MovementType.Elastic)
			{
				if (vector2.x != 0f)
				{
					vector.x -= ScrollRectEx.RubberDelta(vector2.x, this.m_ViewBounds.size.x);
				}
				if (vector2.y != 0f)
				{
					vector.y -= ScrollRectEx.RubberDelta(vector2.y, this.m_ViewBounds.size.y);
				}
			}
			this.SetContentAnchoredPosition(vector);
		}

		// Token: 0x06002DDF RID: 11743 RVA: 0x000E5EEC File Offset: 0x000E40EC
		protected virtual void SetContentAnchoredPosition(Vector2 position)
		{
			if (!this.m_Horizontal)
			{
				position.x = this.m_Content.anchoredPosition.x;
			}
			if (!this.m_Vertical)
			{
				position.y = this.m_Content.anchoredPosition.y;
			}
			if (position != this.m_Content.anchoredPosition)
			{
				this.m_Content.anchoredPosition = position;
				this.UpdateBounds();
			}
		}

		// Token: 0x06002DE0 RID: 11744 RVA: 0x000E5F5C File Offset: 0x000E415C
		protected virtual void LateUpdate()
		{
			if (!this.m_Content)
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateScrollbarVisibility();
			this.UpdateBounds();
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			Vector2 vector = this.CalculateOffset(Vector2.zero);
			if (!this.m_Dragging && (vector != Vector2.zero || this.m_Velocity != Vector2.zero))
			{
				Vector2 vector2 = this.m_Content.anchoredPosition;
				for (int i = 0; i < 2; i++)
				{
					if (this.m_MovementType == ScrollRectEx.MovementType.Elastic && vector[i] != 0f)
					{
						float value = this.m_Velocity[i];
						vector2[i] = Mathf.SmoothDamp(this.m_Content.anchoredPosition[i], this.m_Content.anchoredPosition[i] + vector[i], ref value, this.m_Elasticity, float.PositiveInfinity, unscaledDeltaTime);
						this.m_Velocity[i] = value;
					}
					else if (this.m_Inertia)
					{
						ref Vector2 ptr = ref this.m_Velocity;
						int index = i;
						ptr[index] *= Mathf.Pow(this.m_DecelerationRate, unscaledDeltaTime);
						if (Mathf.Abs(this.m_Velocity[i]) < 1f)
						{
							this.m_Velocity[i] = 0f;
						}
						ptr = ref vector2;
						index = i;
						ptr[index] += this.m_Velocity[i] * unscaledDeltaTime;
					}
					else
					{
						this.m_Velocity[i] = 0f;
					}
				}
				if (this.m_Velocity != Vector2.zero)
				{
					if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
					{
						vector = this.CalculateOffset(vector2 - this.m_Content.anchoredPosition);
						vector2 += vector;
					}
					this.SetContentAnchoredPosition(vector2);
				}
			}
			if (this.m_Dragging && this.m_Inertia)
			{
				Vector3 b = (this.m_Content.anchoredPosition - this.m_PrevPosition) / unscaledDeltaTime;
				this.m_Velocity = Vector3.Lerp(this.m_Velocity, b, unscaledDeltaTime * 10f);
			}
			if (this.m_ViewBounds != this.m_PrevViewBounds || this.m_ContentBounds != this.m_PrevContentBounds || this.m_Content.anchoredPosition != this.m_PrevPosition)
			{
				this.UpdateScrollbars(vector);
				this.m_OnValueChanged.Invoke(this.normalizedPosition);
				this.UpdatePrevData();
			}
		}

		// Token: 0x06002DE1 RID: 11745 RVA: 0x000E61F8 File Offset: 0x000E43F8
		private void UpdatePrevData()
		{
			if (this.m_Content == null)
			{
				this.m_PrevPosition = Vector2.zero;
			}
			else
			{
				this.m_PrevPosition = this.m_Content.anchoredPosition;
			}
			this.m_PrevViewBounds = this.m_ViewBounds;
			this.m_PrevContentBounds = this.m_ContentBounds;
		}

		// Token: 0x06002DE2 RID: 11746 RVA: 0x000E624C File Offset: 0x000E444C
		private void UpdateScrollbars(Vector2 offset)
		{
			if (this.m_HorizontalScrollbar)
			{
				if (this.m_ContentBounds.size.x > 0f)
				{
					this.m_HorizontalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.x - Mathf.Abs(offset.x)) / this.m_ContentBounds.size.x);
				}
				else
				{
					this.m_HorizontalScrollbar.size = 1f;
				}
				this.m_HorizontalScrollbar.value = this.horizontalNormalizedPosition;
			}
			if (this.m_VerticalScrollbar)
			{
				if (this.m_ContentBounds.size.y > 0f)
				{
					this.m_VerticalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.y - Mathf.Abs(offset.y)) / this.m_ContentBounds.size.y);
				}
				else
				{
					this.m_VerticalScrollbar.size = 1f;
				}
				this.m_VerticalScrollbar.value = this.verticalNormalizedPosition;
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06002DE3 RID: 11747 RVA: 0x000239C4 File Offset: 0x00021BC4
		// (set) Token: 0x06002DE4 RID: 11748 RVA: 0x000239D7 File Offset: 0x00021BD7
		public Vector2 normalizedPosition
		{
			get
			{
				return new Vector2(this.horizontalNormalizedPosition, this.verticalNormalizedPosition);
			}
			set
			{
				this.SetNormalizedPosition(value.x, 0);
				this.SetNormalizedPosition(value.y, 1);
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06002DE5 RID: 11749 RVA: 0x000E6364 File Offset: 0x000E4564
		// (set) Token: 0x06002DE6 RID: 11750 RVA: 0x000239F3 File Offset: 0x00021BF3
		public float horizontalNormalizedPosition
		{
			get
			{
				this.UpdateBounds();
				if (this.m_ContentBounds.size.x <= this.m_ViewBounds.size.x)
				{
					return (float)((this.m_ViewBounds.min.x > this.m_ContentBounds.min.x) ? 1 : 0);
				}
				return (this.m_ViewBounds.min.x - this.m_ContentBounds.min.x) / (this.m_ContentBounds.size.x - this.m_ViewBounds.size.x);
			}
			set
			{
				this.SetNormalizedPosition(value, 0);
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06002DE7 RID: 11751 RVA: 0x000E6404 File Offset: 0x000E4604
		// (set) Token: 0x06002DE8 RID: 11752 RVA: 0x000239FD File Offset: 0x00021BFD
		public float verticalNormalizedPosition
		{
			get
			{
				this.UpdateBounds();
				if (this.m_ContentBounds.size.y <= this.m_ViewBounds.size.y)
				{
					return (float)((this.m_ViewBounds.min.y > this.m_ContentBounds.min.y) ? 1 : 0);
				}
				return (this.m_ViewBounds.min.y - this.m_ContentBounds.min.y) / (this.m_ContentBounds.size.y - this.m_ViewBounds.size.y);
			}
			set
			{
				this.SetNormalizedPosition(value, 1);
			}
		}

		// Token: 0x06002DE9 RID: 11753 RVA: 0x000239F3 File Offset: 0x00021BF3
		private void SetHorizontalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 0);
		}

		// Token: 0x06002DEA RID: 11754 RVA: 0x000239FD File Offset: 0x00021BFD
		private void SetVerticalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 1);
		}

		// Token: 0x06002DEB RID: 11755 RVA: 0x000E64A4 File Offset: 0x000E46A4
		private void SetNormalizedPosition(float value, int axis)
		{
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			float num = this.m_ContentBounds.size[axis] - this.m_ViewBounds.size[axis];
			float num2 = this.m_ViewBounds.min[axis] - value * num;
			float num3 = this.m_Content.localPosition[axis] + num2 - this.m_ContentBounds.min[axis];
			Vector3 localPosition = this.m_Content.localPosition;
			if (Mathf.Abs(localPosition[axis] - num3) > 0.01f)
			{
				localPosition[axis] = num3;
				this.m_Content.localPosition = localPosition;
				this.m_Velocity[axis] = 0f;
				this.UpdateBounds();
			}
		}

		// Token: 0x06002DEC RID: 11756 RVA: 0x00023A07 File Offset: 0x00021C07
		private static float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
		}

		// Token: 0x06002DED RID: 11757 RVA: 0x00023A32 File Offset: 0x00021C32
		protected override void OnRectTransformDimensionsChange()
		{
			this.SetDirty();
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06002DEE RID: 11758 RVA: 0x00023A3A File Offset: 0x00021C3A
		private bool hScrollingNeeded
		{
			get
			{
				return !Application.isPlaying || this.m_ContentBounds.size.x > this.m_ViewBounds.size.x + 0.01f;
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06002DEF RID: 11759 RVA: 0x00023A6D File Offset: 0x00021C6D
		private bool vScrollingNeeded
		{
			get
			{
				return !Application.isPlaying || this.m_ContentBounds.size.y > this.m_ViewBounds.size.y + 0.01f;
			}
		}

		// Token: 0x06002DF0 RID: 11760 RVA: 0x000E6580 File Offset: 0x000E4780
		public virtual void SetLayoutHorizontal()
		{
			this.m_Tracker.Clear();
			if (this.m_HSliderExpand || this.m_VSliderExpand)
			{
				this.m_Tracker.Add(this, this.viewRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
				this.viewRect.anchorMin = Vector2.zero;
				this.viewRect.anchorMax = Vector2.one;
				this.viewRect.sizeDelta = Vector2.zero;
				this.viewRect.anchoredPosition = Vector2.zero;
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_VSliderExpand && this.vScrollingNeeded)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_HSliderExpand && this.hScrollingNeeded)
			{
				this.viewRect.sizeDelta = new Vector2(this.viewRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_VSliderExpand && this.vScrollingNeeded && this.viewRect.sizeDelta.x == 0f && this.viewRect.sizeDelta.y < 0f)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
			}
		}

		// Token: 0x06002DF1 RID: 11761 RVA: 0x000E67DC File Offset: 0x000E49DC
		public virtual void SetLayoutVertical()
		{
			this.UpdateScrollbarLayout();
			this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
			this.m_ContentBounds = this.GetBounds();
		}

		// Token: 0x06002DF2 RID: 11762 RVA: 0x000E6838 File Offset: 0x000E4A38
		private void UpdateScrollbarVisibility()
		{
			if (this.m_VerticalScrollbar && this.m_VerticalScrollbarVisibility != ScrollRectEx.ScrollbarVisibility.Permanent && this.m_VerticalScrollbar.gameObject.activeSelf != this.vScrollingNeeded)
			{
				this.m_VerticalScrollbar.gameObject.SetActive(this.vScrollingNeeded);
			}
			if (this.m_HorizontalScrollbar && this.m_HorizontalScrollbarVisibility != ScrollRectEx.ScrollbarVisibility.Permanent && this.m_HorizontalScrollbar.gameObject.activeSelf != this.hScrollingNeeded)
			{
				this.m_HorizontalScrollbar.gameObject.SetActive(this.hScrollingNeeded);
			}
		}

		// Token: 0x06002DF3 RID: 11763 RVA: 0x000E68CC File Offset: 0x000E4ACC
		private void UpdateScrollbarLayout()
		{
			if (this.m_VSliderExpand && this.m_HorizontalScrollbar)
			{
				this.m_Tracker.Add(this, this.m_HorizontalScrollbarRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.SizeDeltaX);
				this.m_HorizontalScrollbarRect.anchorMin = new Vector2(0f, this.m_HorizontalScrollbarRect.anchorMin.y);
				this.m_HorizontalScrollbarRect.anchorMax = new Vector2(1f, this.m_HorizontalScrollbarRect.anchorMax.y);
				this.m_HorizontalScrollbarRect.anchoredPosition = new Vector2(0f, this.m_HorizontalScrollbarRect.anchoredPosition.y);
				if (this.vScrollingNeeded)
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
				else
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(0f, this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
			}
			if (this.m_HSliderExpand && this.m_VerticalScrollbar)
			{
				this.m_Tracker.Add(this, this.m_VerticalScrollbarRect, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaY);
				this.m_VerticalScrollbarRect.anchorMin = new Vector2(this.m_VerticalScrollbarRect.anchorMin.x, 0f);
				this.m_VerticalScrollbarRect.anchorMax = new Vector2(this.m_VerticalScrollbarRect.anchorMax.x, 1f);
				this.m_VerticalScrollbarRect.anchoredPosition = new Vector2(this.m_VerticalScrollbarRect.anchoredPosition.x, 0f);
				if (this.hScrollingNeeded)
				{
					this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
					return;
				}
				this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, 0f);
			}
		}

		// Token: 0x06002DF4 RID: 11764 RVA: 0x000E6AD4 File Offset: 0x000E4CD4
		private void UpdateBounds()
		{
			this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
			this.m_ContentBounds = this.GetBounds();
			if (this.m_Content == null)
			{
				return;
			}
			Vector3 size = this.m_ContentBounds.size;
			Vector3 center = this.m_ContentBounds.center;
			Vector3 vector = this.m_ViewBounds.size - size;
			if (vector.x > 0f)
			{
				center.x -= vector.x * (this.m_Content.pivot.x - 0.5f);
				size.x = this.m_ViewBounds.size.x;
			}
			if (vector.y > 0f)
			{
				center.y -= vector.y * (this.m_Content.pivot.y - 0.5f);
				size.y = this.m_ViewBounds.size.y;
			}
			this.m_ContentBounds.size = size;
			this.m_ContentBounds.center = center;
		}

		// Token: 0x06002DF5 RID: 11765 RVA: 0x000E6C14 File Offset: 0x000E4E14
		private Bounds GetBounds()
		{
			if (this.m_Content == null)
			{
				return default(Bounds);
			}
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Matrix4x4 worldToLocalMatrix = this.viewRect.worldToLocalMatrix;
			this.m_Content.GetWorldCorners(this.m_Corners);
			for (int i = 0; i < 4; i++)
			{
				Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(this.m_Corners[i]);
				vector = Vector3.Min(lhs, vector);
				vector2 = Vector3.Max(lhs, vector2);
			}
			Bounds result = new Bounds(vector, Vector3.zero);
			result.Encapsulate(vector2);
			return result;
		}

		// Token: 0x06002DF6 RID: 11766 RVA: 0x000E6CCC File Offset: 0x000E4ECC
		private Vector2 CalculateOffset(Vector2 delta)
		{
			Vector2 zero = Vector2.zero;
			if (this.m_MovementType == ScrollRectEx.MovementType.Unrestricted)
			{
				return zero;
			}
			Vector2 vector = this.m_ContentBounds.min;
			Vector2 vector2 = this.m_ContentBounds.max;
			if (this.m_Horizontal)
			{
				vector.x += delta.x;
				vector2.x += delta.x;
				if (vector.x > this.m_ViewBounds.min.x)
				{
					zero.x = this.m_ViewBounds.min.x - vector.x;
				}
				else if (vector2.x < this.m_ViewBounds.max.x)
				{
					zero.x = this.m_ViewBounds.max.x - vector2.x;
				}
			}
			if (this.m_Vertical)
			{
				vector.y += delta.y;
				vector2.y += delta.y;
				if (vector2.y < this.m_ViewBounds.max.y)
				{
					zero.y = this.m_ViewBounds.max.y - vector2.y;
				}
				else if (vector.y > this.m_ViewBounds.min.y)
				{
					zero.y = this.m_ViewBounds.min.y - vector.y;
				}
			}
			return zero;
		}

		// Token: 0x06002DF7 RID: 11767 RVA: 0x00023AA0 File Offset: 0x00021CA0
		protected void SetDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		}

		// Token: 0x06002DF8 RID: 11768 RVA: 0x00023AB6 File Offset: 0x00021CB6
		protected void SetDirtyCaching()
		{
			if (!this.IsActive())
			{
				return;
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		}

		// Token: 0x06002DF9 RID: 11769 RVA: 0x000E6E44 File Offset: 0x000E5044
		public void CenterOnPosition(Vector2 pos)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			Vector2 vector = new Vector2(this.content.localScale.x, this.content.localScale.y);
			pos.x *= vector.x;
			pos.y *= vector.y;
			Vector2 vector2 = new Vector2(this.content.rect.width * vector.x - rectTransform.rect.width, this.content.rect.height * vector.y - rectTransform.rect.height);
			pos.x = pos.x / vector2.x + this.content.pivot.x;
			pos.y = pos.y / vector2.y + this.content.pivot.y;
			if (this.movementType != ScrollRectEx.MovementType.Unrestricted)
			{
				pos.x = Mathf.Clamp(pos.x, 0f, 1f);
				pos.y = Mathf.Clamp(pos.y, 0f, 1f);
			}
			this.normalizedPosition = pos;
		}

		// Token: 0x06002DFA RID: 11770 RVA: 0x00002ECE File Offset: 0x000010CE
		public void LayoutComplete()
		{
		}

		// Token: 0x06002DFB RID: 11771 RVA: 0x00002ECE File Offset: 0x000010CE
		public void GraphicUpdateComplete()
		{
		}

		// Token: 0x06002DFC RID: 11772 RVA: 0x0001339B File Offset: 0x0001159B
		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		// Token: 0x0200083D RID: 2109
		public enum MovementType
		{
			// Token: 0x0400291D RID: 10525
			Unrestricted,
			// Token: 0x0400291E RID: 10526
			Elastic,
			// Token: 0x0400291F RID: 10527
			Clamped
		}

		// Token: 0x0200083E RID: 2110
		public enum ScrollbarVisibility
		{
			// Token: 0x04002921 RID: 10529
			Permanent,
			// Token: 0x04002922 RID: 10530
			AutoHide,
			// Token: 0x04002923 RID: 10531
			AutoHideAndExpandViewport
		}

		// Token: 0x0200083F RID: 2111
		[Serializable]
		public class ScrollRectEvent : UnityEvent<Vector2>
		{
		}
	}
}
