using FarsiLibrary.Win.BaseClasses;
using FarsiLibrary.Win.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FarsiLibrary.Win
{
	[DefaultProperty("Items")]
	[ToolboxItem(true)]
	[ToolboxBitmap("FATabStrip.bmp")]
	[Designer(typeof(FATabStripDesigner))]
	[DefaultEvent("TabStripItemSelectionChanged")]
	public class FATabStrip : BaseStyledPanel, ISupportInitialize, IDisposable
	{
		internal static int PreferredWidth = 350;
		internal static int PreferredHeight = 200;
		private static Font defaultFont = new Font("Tahoma", 8.25f, FontStyle.Regular);
		private int DEF_START_POS = 10;
		private Rectangle stripButtonRect = Rectangle.Empty;
		private bool alwaysShowClose = true;
		private bool alwaysShowMenuGlyph = true;
		private const int DEF_HEADER_HEIGHT = 19;
		private const int DEF_GLYPH_WIDTH = 40;
		private FATabStripItem selectedItem;
		private ContextMenuStrip menu;
		private FATabStripMenuGlyph menuGlyph;
		private FATabStripCloseButton closeButton;
		private FATabStripItemCollection items;
		private StringFormat sf;
		private bool isIniting;
		private bool menuOpen;

		public event TabStripItemClosingHandler TabStripItemClosing;

		public event TabStripItemChangedHandler TabStripItemSelectionChanged;

		public event HandledEventHandler MenuItemsLoading;

		public event EventHandler MenuItemsLoaded;

		public event EventHandler TabStripItemClosed;

		public HitTestResult HitTest(Point pt)
		{
			if (this.closeButton.Bounds.Contains(pt))
				return HitTestResult.CloseButton;
			if (this.menuGlyph.Bounds.Contains(pt))
				return HitTestResult.MenuGlyph;
			return this.GetTabItemByPoint(pt) != null ? HitTestResult.TabItem : HitTestResult.None;
		}

		public void AddTab(FATabStripItem tabItem)
		{
			this.AddTab(tabItem, false);
		}

		public void AddTab(FATabStripItem tabItem, bool autoSelect)
		{
			tabItem.Dock = DockStyle.Fill;
			this.Items.Add(tabItem);
			if ((!autoSelect || !tabItem.Visible) && (!tabItem.Visible || this.Items.DrawnCount >= 1))
				return;
			this.SelectedItem = tabItem;
			this.SelectItem(tabItem);
		}

		public void RemoveTab(FATabStripItem tabItem)
		{
			int num = this.Items.IndexOf(tabItem);
			if (num >= 0) {
				this.UnSelectItem(tabItem);
				this.Items.Remove(tabItem);
			}
			if (this.Items.Count <= 0)
				return;
			if (this.RightToLeft == RightToLeft.No) {
				if (this.Items[num - 1] != null)
					this.SelectedItem = this.Items[num - 1];
				else
					this.SelectedItem = this.Items.FirstVisible;
			}
			else if (this.Items[num + 1] != null)
				this.SelectedItem = this.Items[num + 1];
			else
				this.SelectedItem = this.Items.LastVisible;
		}

		public FATabStripItem GetTabItemByPoint(Point pt)
		{
			FATabStripItem faTabStripItem1 = (FATabStripItem)null;
			bool flag = false;
			for (int index = 0; index < this.Items.Count; ++index) {
				FATabStripItem faTabStripItem2 = this.Items[index];
				if (faTabStripItem2.StripRect.Contains((PointF)pt) && faTabStripItem2.Visible && faTabStripItem2.IsDrawn) {
					faTabStripItem1 = faTabStripItem2;
					flag = true;
				}
				if (flag)
					break;
			}
			return faTabStripItem1;
		}

		public virtual void ShowMenu()
		{
			if (this.menu.Visible || this.menu.Items.Count <= 0)
				return;
			if (this.RightToLeft == RightToLeft.No)
				this.menu.Show((Control)this, new Point(this.menuGlyph.Bounds.Left, this.menuGlyph.Bounds.Bottom));
			else
				this.menu.Show((Control)this, new Point(this.menuGlyph.Bounds.Right, this.menuGlyph.Bounds.Bottom));
			this.menuOpen = true;
		}

		internal void UnDrawAll()
		{
			for (int index = 0; index < this.Items.Count; ++index)
				this.Items[index].IsDrawn = false;
		}

		internal void SelectItem(FATabStripItem tabItem)
		{
			tabItem.Dock = DockStyle.Fill;
			tabItem.Visible = true;
			tabItem.Selected = true;
		}

		internal void UnSelectItem(FATabStripItem tabItem)
		{
			tabItem.Selected = false;
		}

		protected internal virtual void OnTabStripItemClosing(TabStripItemClosingEventArgs e)
		{
			if (this.TabStripItemClosing == null)
				return;
			this.TabStripItemClosing(e);
		}

		protected internal virtual void OnTabStripItemClosed(EventArgs e)
		{
			if (this.TabStripItemClosed == null)
				return;
			this.TabStripItemClosed((object)this, e);
		}

		protected virtual void OnMenuItemsLoading(HandledEventArgs e)
		{
			if (this.MenuItemsLoading == null)
				return;
			this.MenuItemsLoading((object)this, e);
		}

		protected virtual void OnMenuItemsLoaded(EventArgs e)
		{
			if (this.MenuItemsLoaded == null)
				return;
			this.MenuItemsLoaded((object)this, e);
		}

		protected virtual void OnTabStripItemChanged(TabStripItemChangedEventArgs e)
		{
			if (this.TabStripItemSelectionChanged == null)
				return;
			this.TabStripItemSelectionChanged(e);
		}

		protected virtual void OnMenuItemsLoad(EventArgs e)
		{
			this.menu.RightToLeft = this.RightToLeft;
			this.menu.Items.Clear();
			for (int index = 0; index < this.Items.Count; ++index) {
				FATabStripItem faTabStripItem = this.Items[index];
				if (faTabStripItem.Visible) {
					ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(faTabStripItem.Title);
					toolStripMenuItem.Tag = (object)faTabStripItem;
					toolStripMenuItem.Image = faTabStripItem.Image;
					this.menu.Items.Add((ToolStripItem)toolStripMenuItem);
				}
			}
			this.OnMenuItemsLoaded(EventArgs.Empty);
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			this.UpdateLayout();
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			this.SetDefaultSelected();
			Rectangle clientRectangle = this.ClientRectangle;
			--clientRectangle.Width;
			--clientRectangle.Height;
			this.DEF_START_POS = this.RightToLeft != RightToLeft.No ? this.stripButtonRect.Right : 10;
			e.Graphics.DrawRectangle(SystemPens.ControlDark, clientRectangle);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			for (int index = 0; index < this.Items.Count; ++index) {
				FATabStripItem currentItem = this.Items[index];
				if (currentItem.Visible || this.DesignMode) {
					this.OnCalcTabPage(e.Graphics, currentItem);
					currentItem.IsDrawn = false;
					if (this.AllowDraw(currentItem))
						this.OnDrawTabPage(e.Graphics, currentItem);
				}
			}
			if (this.RightToLeft == RightToLeft.No) {
				if (this.Items.DrawnCount == 0 || this.Items.VisibleCount == 0)
					e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, 19), new Point(this.ClientRectangle.Width, 19));
				else if (this.SelectedItem != null && this.SelectedItem.IsDrawn) {
					Point point = new Point((int)this.SelectedItem.StripRect.Left - 9, 19);
					e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, 19), point);
					point.X += (int)this.SelectedItem.StripRect.Width + 10;
					e.Graphics.DrawLine(SystemPens.ControlDark, point, new Point(this.ClientRectangle.Width, 19));
				}
			}
			else if (this.Items.DrawnCount == 0 || this.Items.VisibleCount == 0)
				e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, 19), new Point(this.ClientRectangle.Width, 19));
			else if (this.SelectedItem != null && this.SelectedItem.IsDrawn) {
				Point point = new Point((int)this.SelectedItem.StripRect.Left, 19);
				e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, 19), point);
				point.X += (int)this.SelectedItem.StripRect.Width + 20;
				e.Graphics.DrawLine(SystemPens.ControlDark, point, new Point(this.ClientRectangle.Width, 19));
			}
			if (this.AlwaysShowMenuGlyph || this.Items.DrawnCount > this.Items.VisibleCount)
				this.menuGlyph.DrawGlyph(e.Graphics);
			if (!this.AlwaysShowClose && (this.SelectedItem == null || !this.SelectedItem.CanClose))
				return;
			this.closeButton.DrawCross(e.Graphics);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button != MouseButtons.Left)
				return;
			switch (this.HitTest(e.Location)) {
				case HitTestResult.CloseButton:
					if (this.SelectedItem != null) {
						TabStripItemClosingEventArgs e1 = new TabStripItemClosingEventArgs(this.SelectedItem);
						this.OnTabStripItemClosing(e1);
						if (!e1.Cancel && this.SelectedItem.CanClose) {
							this.RemoveTab(this.SelectedItem);
							this.OnTabStripItemClosed(EventArgs.Empty);
							break;
						}
						break;
					}
					break;
				case HitTestResult.MenuGlyph:
					HandledEventArgs e2 = new HandledEventArgs(false);
					this.OnMenuItemsLoading(e2);
					if (!e2.Handled)
						this.OnMenuItemsLoad(EventArgs.Empty);
					this.ShowMenu();
					break;
				case HitTestResult.TabItem:
					FATabStripItem tabItemByPoint = this.GetTabItemByPoint(e.Location);
					if (tabItemByPoint != null) {
						this.SelectedItem = tabItemByPoint;
						break;
					}
					break;
			}
			this.Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (this.menuGlyph.Bounds.Contains(e.Location)) {
				this.menuGlyph.IsMouseOver = true;
				this.Invalidate(this.menuGlyph.Bounds);
			}
			else if (this.menuGlyph.IsMouseOver && !this.menuOpen) {
				this.menuGlyph.IsMouseOver = false;
				this.Invalidate(this.menuGlyph.Bounds);
			}
			if (this.closeButton.Bounds.Contains(e.Location)) {
				this.closeButton.IsMouseOver = true;
				this.Invalidate(this.closeButton.Bounds);
			}
			else {
				if (!this.closeButton.IsMouseOver)
					return;
				this.closeButton.IsMouseOver = false;
				this.Invalidate(this.closeButton.Bounds);
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			this.menuGlyph.IsMouseOver = false;
			this.Invalidate(this.menuGlyph.Bounds);
			this.closeButton.IsMouseOver = false;
			this.Invalidate(this.closeButton.Bounds);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (this.isIniting)
				return;
			this.UpdateLayout();
		}

		private bool AllowDraw(FATabStripItem item)
		{
			bool flag = true;
			if (this.RightToLeft == RightToLeft.No) {
				if ((double)item.StripRect.Right >= (double)this.stripButtonRect.Width)
					flag = false;
			}
			else if ((double)item.StripRect.Left <= (double)this.stripButtonRect.Left)
				return false;
			return flag;
		}

		private void SetDefaultSelected()
		{
			if (this.selectedItem == null && this.Items.Count > 0)
				this.SelectedItem = this.Items[0];
			for (int index = 0; index < this.Items.Count; ++index)
				this.Items[index].Dock = DockStyle.Fill;
		}

		private void OnMenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			this.SelectedItem = (FATabStripItem)e.ClickedItem.Tag;
		}

		private void OnMenuVisibleChanged(object sender, EventArgs e)
		{
			if (this.menu.Visible)
				return;
			this.menuOpen = false;
		}

		private void OnCalcTabPage(Graphics g, FATabStripItem currentItem)
		{
			Font font = this.Font;
			if (currentItem == this.SelectedItem)
				font = new Font(this.Font, FontStyle.Bold);
			SizeF sizeF = g.MeasureString(currentItem.Title, font, new SizeF(200f, 10f), this.sf);
			sizeF.Width += 20f;
			if (this.RightToLeft == RightToLeft.No) {
				RectangleF rectangleF = new RectangleF((float)this.DEF_START_POS, 3f, sizeF.Width, 17f);
				currentItem.StripRect = rectangleF;
				this.DEF_START_POS += (int)sizeF.Width;
			}
			else {
				RectangleF rectangleF = new RectangleF((float)((double)this.DEF_START_POS - (double)sizeF.Width + 1.0), 3f, sizeF.Width - 1f, 17f);
				currentItem.StripRect = rectangleF;
				this.DEF_START_POS -= (int)sizeF.Width;
			}
		}

		private void OnDrawTabPage(Graphics g, FATabStripItem currentItem)
		{
			bool flag = this.Items.IndexOf(currentItem) == 0;
			Font font = this.Font;
			if (currentItem == this.SelectedItem)
				font = new Font(this.Font, FontStyle.Bold);
			SizeF sizeF = g.MeasureString(currentItem.Title, font, new SizeF(200f, 10f), this.sf);
			sizeF.Width += 20f;
			RectangleF stripRect = currentItem.StripRect;
			GraphicsPath path = new GraphicsPath();
			int num = 3;
			if (this.RightToLeft == RightToLeft.No) {
				if (currentItem == this.SelectedItem || flag) {
					path.AddLine(stripRect.Left - 10f, stripRect.Bottom - 1f, (float)((double)stripRect.Left + (double)stripRect.Height / 2.0 - 4.0), (float)(num + 4));
				}
				else {
					path.AddLine(stripRect.Left, stripRect.Bottom - 1f, stripRect.Left, (float)((double)stripRect.Bottom - (double)stripRect.Height / 2.0 - 2.0));
					path.AddLine(stripRect.Left, (float)((double)stripRect.Bottom - (double)stripRect.Height / 2.0 - 3.0), (float)((double)stripRect.Left + (double)stripRect.Height / 2.0 - 4.0), (float)(num + 3));
				}
				path.AddLine((float)((double)stripRect.Left + (double)stripRect.Height / 2.0 + 2.0), (float)num, stripRect.Right - 3f, (float)num);
				path.AddLine(stripRect.Right, (float)(num + 2), stripRect.Right, stripRect.Bottom - 1f);
				path.AddLine(stripRect.Right - 4f, stripRect.Bottom - 1f, stripRect.Left, stripRect.Bottom - 1f);
				path.CloseFigure();
				LinearGradientBrush linearGradientBrush = currentItem != this.SelectedItem ? new LinearGradientBrush(stripRect, SystemColors.ControlLightLight, SystemColors.Control, LinearGradientMode.Vertical) : new LinearGradientBrush(stripRect, SystemColors.ControlLightLight, SystemColors.Window, LinearGradientMode.Vertical);
				g.FillPath((Brush)linearGradientBrush, path);
				g.DrawPath(SystemPens.ControlDark, path);
				if (currentItem == this.SelectedItem)
					g.DrawLine(new Pen((Brush)linearGradientBrush), stripRect.Left - 9f, stripRect.Height + 2f, (float)((double)stripRect.Left + (double)stripRect.Width - 1.0), stripRect.Height + 2f);
				PointF pointF = new PointF((float)((double)stripRect.Left + (double)stripRect.Height - 4.0), (float)((double)stripRect.Top + (double)stripRect.Height / 2.0 - (double)sizeF.Height / 2.0 - 3.0));
				RectangleF layoutRectangle = stripRect;
				layoutRectangle.Location = pointF;
				layoutRectangle.Width = (float)((double)stripRect.Width - ((double)layoutRectangle.Left - (double)stripRect.Left) - 4.0);
				layoutRectangle.Height = sizeF.Height + font.Size / 2f;
				if (currentItem == this.SelectedItem)
					g.DrawString(currentItem.Title, font, (Brush)new SolidBrush(this.ForeColor), layoutRectangle, this.sf);
				else
					g.DrawString(currentItem.Title, font, (Brush)new SolidBrush(this.ForeColor), layoutRectangle, this.sf);
			}
			if (this.RightToLeft == RightToLeft.Yes) {
				if (currentItem == this.SelectedItem || flag) {
					path.AddLine(stripRect.Right + 10f, stripRect.Bottom - 1f, (float)((double)stripRect.Right - (double)stripRect.Height / 2.0 + 4.0), (float)(num + 4));
				}
				else {
					path.AddLine(stripRect.Right, stripRect.Bottom - 1f, stripRect.Right, (float)((double)stripRect.Bottom - (double)stripRect.Height / 2.0 - 2.0));
					path.AddLine(stripRect.Right, (float)((double)stripRect.Bottom - (double)stripRect.Height / 2.0 - 3.0), (float)((double)stripRect.Right - (double)stripRect.Height / 2.0 + 4.0), (float)(num + 3));
				}
				path.AddLine((float)((double)stripRect.Right - (double)stripRect.Height / 2.0 - 2.0), (float)num, stripRect.Left + 3f, (float)num);
				path.AddLine(stripRect.Left, (float)(num + 2), stripRect.Left, stripRect.Bottom - 1f);
				path.AddLine(stripRect.Left + 4f, stripRect.Bottom - 1f, stripRect.Right, stripRect.Bottom - 1f);
				path.CloseFigure();
				LinearGradientBrush linearGradientBrush = currentItem != this.SelectedItem ? new LinearGradientBrush(stripRect, SystemColors.ControlLightLight, SystemColors.Control, LinearGradientMode.Vertical) : new LinearGradientBrush(stripRect, SystemColors.ControlLightLight, SystemColors.Window, LinearGradientMode.Vertical);
				g.FillPath((Brush)linearGradientBrush, path);
				g.DrawPath(SystemPens.ControlDark, path);
				if (currentItem == this.SelectedItem)
					g.DrawLine(new Pen((Brush)linearGradientBrush), stripRect.Right + 9f, stripRect.Height + 2f, (float)((double)stripRect.Right - (double)stripRect.Width + 1.0), stripRect.Height + 2f);
				PointF pointF = new PointF(stripRect.Left + 2f, (float)((double)stripRect.Top + (double)stripRect.Height / 2.0 - (double)sizeF.Height / 2.0 - 2.0));
				RectangleF layoutRectangle = stripRect;
				layoutRectangle.Location = pointF;
				layoutRectangle.Width = (float)((double)stripRect.Width - ((double)layoutRectangle.Left - (double)stripRect.Left) - 10.0);
				layoutRectangle.Height = sizeF.Height + font.Size / 2f;
				if (currentItem == this.SelectedItem) {
					--layoutRectangle.Y;
					g.DrawString(currentItem.Title, font, (Brush)new SolidBrush(this.ForeColor), layoutRectangle, this.sf);
				}
				else
					g.DrawString(currentItem.Title, font, (Brush)new SolidBrush(this.ForeColor), layoutRectangle, this.sf);
			}
			currentItem.IsDrawn = true;
		}

		private void UpdateLayout()
		{
			if (this.RightToLeft == RightToLeft.No) {
				this.sf.Trimming = StringTrimming.EllipsisCharacter;
				this.sf.FormatFlags |= StringFormatFlags.NoWrap;
				this.sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;
				this.stripButtonRect = new Rectangle(0, 0, this.ClientSize.Width - 40 - 2, 10);
				this.menuGlyph.Bounds = new Rectangle(this.ClientSize.Width - 40, 2, 16, 16);
				this.closeButton.Bounds = new Rectangle(this.ClientSize.Width - 20, 2, 16, 16);
			}
			else {
				this.sf.Trimming = StringTrimming.EllipsisCharacter;
				this.sf.FormatFlags |= StringFormatFlags.NoWrap;
				this.sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				this.stripButtonRect = new Rectangle(42, 0, this.ClientSize.Width - 40 - 15, 10);
				this.closeButton.Bounds = new Rectangle(4, 2, 16, 16);
				this.menuGlyph.Bounds = new Rectangle(24, 2, 16, 16);
			}
			this.DockPadding.Top = 20;
			this.DockPadding.Bottom = 1;
			this.DockPadding.Right = 1;
			this.DockPadding.Left = 1;
		}

		private void OnCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			FATabStripItem element = (FATabStripItem)e.Element;
			if (e.Action == CollectionChangeAction.Add) {
				this.Controls.Add((Control)element);
				this.OnTabStripItemChanged(new TabStripItemChangedEventArgs(element, FATabStripItemChangeTypes.Added));
			}
			else if (e.Action == CollectionChangeAction.Remove) {
				this.Controls.Remove((Control)element);
				this.OnTabStripItemChanged(new TabStripItemChangedEventArgs(element, FATabStripItemChangeTypes.Removed));
			}
			else
				this.OnTabStripItemChanged(new TabStripItemChangedEventArgs(element, FATabStripItemChangeTypes.Changed));
			this.UpdateLayout();
			this.Invalidate();
		}

		public FATabStrip()
		{
			this.BeginInit();
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.items = new FATabStripItemCollection();
			this.items.CollectionChanged += new CollectionChangeEventHandler(this.OnCollectionChanged);
			base.Size = new Size(350, 200);
			this.menu = new ContextMenuStrip();
			this.menu.Renderer = (ToolStripRenderer)this.ToolStripRenderer;
			this.menu.ItemClicked += new ToolStripItemClickedEventHandler(this.OnMenuItemClicked);
			this.menu.VisibleChanged += new EventHandler(this.OnMenuVisibleChanged);
			this.menuGlyph = new FATabStripMenuGlyph(this.ToolStripRenderer);
			this.closeButton = new FATabStripCloseButton(this.ToolStripRenderer);
			this.Font = FATabStrip.defaultFont;
			this.sf = new StringFormat();
			this.EndInit();
			this.UpdateLayout();
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(null)]
		public FATabStripItem SelectedItem
		{
			get
			{
				return this.selectedItem;
			}
			set
			{
				if (this.selectedItem == value)
					return;
				if (value == null && this.Items.Count > 0) {
					FATabStripItem faTabStripItem = this.Items[0];
					if (faTabStripItem.Visible) {
						this.selectedItem = faTabStripItem;
						this.selectedItem.Selected = true;
						this.selectedItem.Dock = DockStyle.Fill;
					}
				}
				else
					this.selectedItem = value;
				foreach (FATabStripItem tabItem in (CollectionBase)this.Items) {
					if (tabItem == this.selectedItem) {
						this.SelectItem(tabItem);
						tabItem.Dock = DockStyle.Fill;
						tabItem.Show();
					}
					else {
						this.UnSelectItem(tabItem);
						tabItem.Hide();
					}
				}
				this.SelectItem(this.selectedItem);
				this.Invalidate();
				if (!this.selectedItem.IsDrawn) {
					this.Items.MoveTo(0, this.selectedItem);
					this.Invalidate();
				}
				this.OnTabStripItemChanged(new TabStripItemChangedEventArgs(this.selectedItem, FATabStripItemChangeTypes.SelectionChanged));
			}
		}

		[DefaultValue(true)]
		public bool AlwaysShowMenuGlyph
		{
			get
			{
				return this.alwaysShowMenuGlyph;
			}
			set
			{
				if (this.alwaysShowMenuGlyph == value)
					return;
				this.alwaysShowMenuGlyph = value;
				this.Invalidate();
			}
		}

		[DefaultValue(true)]
		public bool AlwaysShowClose
		{
			get
			{
				return this.alwaysShowClose;
			}
			set
			{
				if (this.alwaysShowClose == value)
					return;
				this.alwaysShowClose = value;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FATabStripItemCollection Items
		{
			get
			{
				return this.items;
			}
		}

		[DefaultValue(typeof(Size), "350,200")]
		public new Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				if (base.Size == value)
					return;
				base.Size = value;
				this.UpdateLayout();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Control.ControlCollection Controls
		{
			get
			{
				return base.Controls;
			}
		}

		public bool ShouldSerializeFont()
		{
			if (this.Font != null)
				return !this.Font.Equals((object)FATabStrip.defaultFont);
			return false;
		}

		public bool ShouldSerializeSelectedItem()
		{
			return true;
		}

		public bool ShouldSerializeItems()
		{
			return this.items.Count > 0;
		}

		public new void ResetFont()
		{
			this.Font = FATabStrip.defaultFont;
		}

		public void BeginInit()
		{
			this.isIniting = true;
		}

		public void EndInit()
		{
			this.isIniting = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				this.items.CollectionChanged -= new CollectionChangeEventHandler(this.OnCollectionChanged);
				this.menu.ItemClicked -= new ToolStripItemClickedEventHandler(this.OnMenuItemClicked);
				this.menu.VisibleChanged -= new EventHandler(this.OnMenuVisibleChanged);
				foreach (FATabStripItem faTabStripItem in (CollectionBase)this.items) {
					if (faTabStripItem != null && !faTabStripItem.IsDisposed)
						faTabStripItem.Dispose();
				}
				if (this.menu != null && !this.menu.IsDisposed)
					this.menu.Dispose();
				if (this.sf != null)
					this.sf.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
