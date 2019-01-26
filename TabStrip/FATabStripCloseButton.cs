using System.Drawing;
using System.Windows.Forms;

namespace FarsiLibrary.Win
{
	internal class FATabStripCloseButton
	{
		private Rectangle crossRect = Rectangle.Empty;
		private bool isMouseOver;
		private ToolStripProfessionalRenderer renderer;

		public bool IsMouseOver
		{
			get
			{
				return this.isMouseOver;
			}
			set
			{
				this.isMouseOver = value;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				return this.crossRect;
			}
			set
			{
				this.crossRect = value;
			}
		}

		internal FATabStripCloseButton(ToolStripProfessionalRenderer renderer)
		{
			this.renderer = renderer;
		}

		public void DrawCross(Graphics g)
		{
			if (this.isMouseOver) {
				Color selectedHighlight = this.renderer.ColorTable.ButtonSelectedHighlight;
				g.FillRectangle((Brush)new SolidBrush(selectedHighlight), this.crossRect);
				Rectangle crossRect = this.crossRect;
				--crossRect.Width;
				--crossRect.Height;
				g.DrawRectangle(SystemPens.Highlight, crossRect);
			}
			using (Pen pen = new Pen(Color.Black, 1.6f)) {
				g.DrawLine(pen, this.crossRect.Left + 3, this.crossRect.Top + 3, this.crossRect.Right - 5, this.crossRect.Bottom - 4);
				g.DrawLine(pen, this.crossRect.Right - 5, this.crossRect.Top + 3, this.crossRect.Left + 3, this.crossRect.Bottom - 4);
			}
		}
	}
}
