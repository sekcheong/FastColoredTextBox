using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FarsiLibrary.Win
{
	internal class FATabStripMenuGlyph
	{
		private Rectangle glyphRect = Rectangle.Empty;
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
				return this.glyphRect;
			}
			set
			{
				this.glyphRect = value;
			}
		}

		internal FATabStripMenuGlyph(ToolStripProfessionalRenderer renderer)
		{
			this.renderer = renderer;
		}

		public void DrawGlyph(Graphics g)
		{
			if (this.isMouseOver) {
				Color selectedHighlight = this.renderer.ColorTable.ButtonSelectedHighlight;
				g.FillRectangle((Brush)new SolidBrush(selectedHighlight), this.glyphRect);
				Rectangle glyphRect = this.glyphRect;
				--glyphRect.Width;
				--glyphRect.Height;
				g.DrawRectangle(SystemPens.Highlight, glyphRect);
			}
			SmoothingMode smoothingMode = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.Default;
			using (Pen pen = new Pen(Color.Black)) {
				pen.Width = 2f;
				g.DrawLine(pen, new Point(this.glyphRect.Left + this.glyphRect.Width / 3 - 2, this.glyphRect.Height / 2 - 1), new Point(this.glyphRect.Right - this.glyphRect.Width / 3, this.glyphRect.Height / 2 - 1));
			}
			g.FillPolygon(Brushes.Black, new Point[3]
			{
		new Point(this.glyphRect.Left + this.glyphRect.Width / 3 - 2, this.glyphRect.Height / 2 + 2),
		new Point(this.glyphRect.Right - this.glyphRect.Width / 3, this.glyphRect.Height / 2 + 2),
		new Point(this.glyphRect.Left + this.glyphRect.Width / 2 - 1, this.glyphRect.Bottom - 4)
			});
			g.SmoothingMode = smoothingMode;
		}
	}
}
