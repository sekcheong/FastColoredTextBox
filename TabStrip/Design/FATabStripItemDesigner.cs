using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace FarsiLibrary.Win.Design
{
	public class FATabStripItemDesigner : ParentControlDesigner
	{
		private FATabStripItem TabStrip;

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			this.TabStrip = component as FATabStripItem;
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);
			properties.Remove((object)"Dock");
			properties.Remove((object)"AutoScroll");
			properties.Remove((object)"AutoScrollMargin");
			properties.Remove((object)"AutoScrollMinSize");
			properties.Remove((object)"DockPadding");
			properties.Remove((object)"DrawGrid");
			properties.Remove((object)"Font");
			properties.Remove((object)"Padding");
			properties.Remove((object)"MinimumSize");
			properties.Remove((object)"MaximumSize");
			properties.Remove((object)"Margin");
			properties.Remove((object)"ForeColor");
			properties.Remove((object)"BackColor");
			properties.Remove((object)"BackgroundImage");
			properties.Remove((object)"BackgroundImageLayout");
			properties.Remove((object)"RightToLeft");
			properties.Remove((object)"GridSize");
			properties.Remove((object)"ImeMode");
			properties.Remove((object)"BorderStyle");
			properties.Remove((object)"AutoSize");
			properties.Remove((object)"AutoSizeMode");
			properties.Remove((object)"Location");
		}

		public override SelectionRules SelectionRules
		{
			get
			{
				return SelectionRules.None;
			}
		}

		public override bool CanBeParentedTo(IDesigner parentDesigner)
		{
			return parentDesigner.Component is FATabStrip;
		}

		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			if (this.TabStrip == null)
				return;
			using (Pen pen = new Pen(SystemColors.ControlDark)) {
				pen.DashStyle = DashStyle.Dash;
				pe.Graphics.DrawRectangle(pen, 0, 0, this.TabStrip.Width - 1, this.TabStrip.Height - 1);
			}
		}
	}
}
