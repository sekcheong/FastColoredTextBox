using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace FarsiLibrary.Win.BaseClasses
{
	[ToolboxItem(false)]
	public class BaseStyledPanel : ContainerControl
	{
		private static ToolStripProfessionalRenderer renderer = new ToolStripProfessionalRenderer();

		public event EventHandler ThemeChanged;

		public BaseStyledPanel()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.UserPaint, true);
		}

		protected override void OnSystemColorsChanged(EventArgs e)
		{
			base.OnSystemColorsChanged(e);
			this.UpdateRenderer();
			this.Invalidate();
		}

		protected virtual void OnThemeChanged(EventArgs e)
		{
			if (this.ThemeChanged == null)
				return;
			this.ThemeChanged((object)this, e);
		}

		private void UpdateRenderer()
		{
			if (!this.UseThemes)
				BaseStyledPanel.renderer.ColorTable.UseSystemColors = true;
			else
				BaseStyledPanel.renderer.ColorTable.UseSystemColors = false;
		}

		[Browsable(false)]
		public ToolStripProfessionalRenderer ToolStripRenderer
		{
			get
			{
				return BaseStyledPanel.renderer;
			}
		}

		[DefaultValue(true)]
		[Browsable(false)]
		public bool UseThemes
		{
			get
			{
				if (VisualStyleRenderer.IsSupported && VisualStyleInformation.IsSupportedByOS)
					return Application.RenderWithVisualStyles;
				return false;
			}
		}
	}
}
