using System;

namespace FarsiLibrary.Win
{
	public class TabStripItemClosingEventArgs : EventArgs
	{
		private bool _cancel;
		private FATabStripItem _item;

		public TabStripItemClosingEventArgs(FATabStripItem item)
		{
			this._item = item;
		}

		public FATabStripItem Item
		{
			get
			{
				return this._item;
			}
			set
			{
				this._item = value;
			}
		}

		public bool Cancel
		{
			get
			{
				return this._cancel;
			}
			set
			{
				this._cancel = value;
			}
		}
	}
}
