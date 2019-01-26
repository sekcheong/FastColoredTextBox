using FarsiLibrary.Win.Design;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FarsiLibrary.Win
{
	[DefaultEvent("Changed")]
	[Designer(typeof(FATabStripItemDesigner))]
	[ToolboxItem(false)]
	[DefaultProperty("Title")]
	public class FATabStripItem : Panel
	{
		private RectangleF stripRect = (RectangleF)Rectangle.Empty;
		private bool canClose = true;
		private bool visible = true;
		private string title = string.Empty;
		private Image image;
		private bool selected;
		private bool isDrawn;

		public event EventHandler Changed;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;
			}
		}

		[DefaultValue(true)]
		public new bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				if (this.visible == value)
					return;
				this.visible = value;
				this.OnChanged();
			}
		}

		internal RectangleF StripRect
		{
			get
			{
				return this.stripRect;
			}
			set
			{
				this.stripRect = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(false)]
		[Browsable(false)]
		public bool IsDrawn
		{
			get
			{
				return this.isDrawn;
			}
			set
			{
				if (this.isDrawn == value)
					return;
				this.isDrawn = value;
			}
		}

		[DefaultValue(null)]
		public Image Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
			}
		}

		[DefaultValue(true)]
		public bool CanClose
		{
			get
			{
				return this.canClose;
			}
			set
			{
				this.canClose = value;
			}
		}

		[DefaultValue("Name")]
		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				if (this.title == value)
					return;
				this.title = value;
				this.OnChanged();
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		public bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				if (this.selected == value)
					return;
				this.selected = value;
			}
		}

		[Browsable(false)]
		public string Caption
		{
			get
			{
				return this.Title;
			}
		}

		public FATabStripItem()
		  : this(string.Empty, (Control)null)
		{
		}

		public FATabStripItem(Control displayControl)
		  : this(string.Empty, displayControl)
		{
		}

		public FATabStripItem(string caption, Control displayControl)
		{
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.ContainerControl, true);
			this.selected = false;
			this.Visible = true;
			this.UpdateText(caption, displayControl);
			if (displayControl == null)
				return;
			this.Controls.Add(displayControl);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing || this.image == null)
				return;
			this.image.Dispose();
		}

		public bool ShouldSerializeIsDrawn()
		{
			return false;
		}

		public bool ShouldSerializeDock()
		{
			return false;
		}

		public bool ShouldSerializeControls()
		{
			if (this.Controls != null)
				return this.Controls.Count > 0;
			return false;
		}

		public bool ShouldSerializeVisible()
		{
			return true;
		}

		private void UpdateText(string caption, Control displayControl)
		{
			if (displayControl != null && displayControl is ICaptionSupport)
				this.Title = (displayControl as ICaptionSupport).Caption;
			else if (caption.Length <= 0 && displayControl != null)
				this.Title = displayControl.Text;
			else if (caption != null)
				this.Title = caption;
			else
				this.Title = string.Empty;
		}

		public void Assign(FATabStripItem item)
		{
			this.Visible = item.Visible;
			this.Text = item.Text;
			this.CanClose = item.CanClose;
			this.Tag = item.Tag;
		}

		protected internal virtual void OnChanged()
		{
			if (this.Changed == null)
				return;
			this.Changed((object)this, EventArgs.Empty);
		}

		public override string ToString()
		{
			return this.Caption;
		}
	}
}
