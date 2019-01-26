using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace FarsiLibrary.Win.Design
{
	public class FATabStripDesigner : ParentControlDesigner
	{
		private IComponentChangeService changeService;

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			this.changeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			this.changeService.ComponentRemoving += new ComponentEventHandler(this.OnRemoving);
			this.Verbs.Add(new DesignerVerb("Add TabStrip", new EventHandler(this.OnAddTabStrip)));
			this.Verbs.Add(new DesignerVerb("Remove TabStrip", new EventHandler(this.OnRemoveTabStrip)));
		}

		protected override void Dispose(bool disposing)
		{
			this.changeService.ComponentRemoving -= new ComponentEventHandler(this.OnRemoving);
			base.Dispose(disposing);
		}

		private void OnRemoving(object sender, ComponentEventArgs e)
		{
			IDesignerHost service = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			if (e.Component is FATabStripItem) {
				FATabStripItem component = e.Component as FATabStripItem;
				if (this.FATabControl.Items.Contains(component)) {
					this.changeService.OnComponentChanging((object)this.Control, (MemberDescriptor)null);
					this.FATabControl.RemoveTab(component);
					this.changeService.OnComponentChanged((object)this.Control, (MemberDescriptor)null, (object)null, (object)null);
					return;
				}
			}
			if (!(e.Component is FATabStrip))
				return;
			for (int index = this.FATabControl.Items.Count - 1; index >= 0; --index) {
				FATabStripItem tabItem = this.FATabControl.Items[index];
				this.changeService.OnComponentChanging((object)this.Control, (MemberDescriptor)null);
				this.FATabControl.RemoveTab(tabItem);
				service.DestroyComponent((IComponent)tabItem);
				this.changeService.OnComponentChanged((object)this.Control, (MemberDescriptor)null, (object)null, (object)null);
			}
		}

		private void OnAddTabStrip(object sender, EventArgs e)
		{
			IDesignerHost service = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			DesignerTransaction transaction = service.CreateTransaction("Add TabStrip");
			FATabStripItem component = (FATabStripItem)service.CreateComponent(typeof(FATabStripItem));
			this.changeService.OnComponentChanging((object)this.Control, (MemberDescriptor)null);
			this.FATabControl.AddTab(component);
			int num = this.FATabControl.Items.IndexOf(component) + 1;
			component.Title = "TabStrip Page " + num.ToString();
			this.FATabControl.SelectItem(component);
			this.changeService.OnComponentChanged((object)this.Control, (MemberDescriptor)null, (object)null, (object)null);
			transaction.Commit();
		}

		private void OnRemoveTabStrip(object sender, EventArgs e)
		{
			DesignerTransaction transaction = ((IDesignerHost)this.GetService(typeof(IDesignerHost))).CreateTransaction("Remove Button");
			this.changeService.OnComponentChanging((object)this.Control, (MemberDescriptor)null);
			FATabStripItem tabItem = this.FATabControl.Items[this.FATabControl.Items.Count - 1];
			this.FATabControl.UnSelectItem(tabItem);
			this.FATabControl.Items.Remove(tabItem);
			this.changeService.OnComponentChanged((object)this.Control, (MemberDescriptor)null, (object)null, (object)null);
			transaction.Commit();
		}

		protected override bool GetHitTest(Point point)
		{
			switch (this.FATabControl.HitTest(point)) {
				case HitTestResult.CloseButton:
				case HitTestResult.MenuGlyph:
					return true;
				default:
					return false;
			}
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);
			properties.Remove((object)"DockPadding");
			properties.Remove((object)"DrawGrid");
			properties.Remove((object)"Margin");
			properties.Remove((object)"Padding");
			properties.Remove((object)"BorderStyle");
			properties.Remove((object)"ForeColor");
			properties.Remove((object)"BackColor");
			properties.Remove((object)"BackgroundImage");
			properties.Remove((object)"BackgroundImageLayout");
			properties.Remove((object)"GridSize");
			properties.Remove((object)"ImeMode");
		}

		protected override void WndProc(ref Message msg)
		{
			if (msg.Msg == 513) {
				FATabStripItem tabItemByPoint = this.FATabControl.GetTabItemByPoint(this.Control.PointToClient(Cursor.Position));
				if (tabItemByPoint != null) {
					this.FATabControl.SelectedItem = tabItemByPoint;
					((ISelectionService)this.GetService(typeof(ISelectionService))).SetSelectedComponents((ICollection)new ArrayList()
					{
			(object) tabItemByPoint
		  });
				}
			}
			base.WndProc(ref msg);
		}

		public override ICollection AssociatedComponents
		{
			get
			{
				return (ICollection)this.FATabControl.Items;
			}
		}

		private FATabStrip FATabControl
		{
			get
			{
				return base.Control as FATabStrip;
			}
		}
	}
}
