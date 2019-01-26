using FarsiLibrary.Win.Helpers;
using System;
using System.ComponentModel;

namespace FarsiLibrary.Win
{
	public class FATabStripItemCollection : CollectionWithEvents
	{
		private int lockUpdate;

		[Browsable(false)]
		public event CollectionChangeEventHandler CollectionChanged;

		public FATabStripItemCollection()
		{
			this.lockUpdate = 0;
		}

		public FATabStripItem this[int index]
		{
			get
			{
				if (index < 0 || this.List.Count - 1 < index)
					return (FATabStripItem)null;
				return (FATabStripItem)this.List[index];
			}
			set
			{
				this.List[index] = (object)value;
			}
		}

		[Browsable(false)]
		public virtual int DrawnCount
		{
			get
			{
				int count = this.Count;
				int num = 0;
				if (count == 0)
					return 0;
				for (int index = 0; index < count; ++index) {
					if (this[index].IsDrawn)
						++num;
				}
				return num;
			}
		}

		public virtual FATabStripItem LastVisible
		{
			get
			{
				for (int index = this.Count - 1; index > 0; --index) {
					if (this[index].Visible)
						return this[index];
				}
				return (FATabStripItem)null;
			}
		}

		public virtual FATabStripItem FirstVisible
		{
			get
			{
				for (int index = 0; index < this.Count; ++index) {
					if (this[index].Visible)
						return this[index];
				}
				return (FATabStripItem)null;
			}
		}

		[Browsable(false)]
		public virtual int VisibleCount
		{
			get
			{
				int count = this.Count;
				int num = 0;
				if (count == 0)
					return 0;
				for (int index = 0; index < count; ++index) {
					if (this[index].Visible)
						++num;
				}
				return num;
			}
		}

		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
		{
			if (this.CollectionChanged == null)
				return;
			this.CollectionChanged((object)this, e);
		}

		protected virtual void BeginUpdate()
		{
			++this.lockUpdate;
		}

		protected virtual void EndUpdate()
		{
			if (--this.lockUpdate != 0)
				return;
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, (object)null));
		}

		public virtual void AddRange(FATabStripItem[] items)
		{
			this.BeginUpdate();
			try {
				foreach (object obj in items)
					this.List.Add(obj);
			}
			finally {
				this.EndUpdate();
			}
		}

		public virtual void Assign(FATabStripItemCollection collection)
		{
			this.BeginUpdate();
			try {
				this.Clear();
				for (int index = 0; index < collection.Count; ++index) {
					FATabStripItem faTabStripItem1 = collection[index];
					FATabStripItem faTabStripItem2 = new FATabStripItem();
					faTabStripItem2.Assign(faTabStripItem1);
					this.Add(faTabStripItem2);
				}
			}
			finally {
				this.EndUpdate();
			}
		}

		public virtual int Add(FATabStripItem item)
		{
			int num = this.IndexOf(item);
			if (num == -1)
				num = this.List.Add((object)item);
			return num;
		}

		public virtual void Remove(FATabStripItem item)
		{
			if (!this.List.Contains((object)item))
				return;
			this.List.Remove((object)item);
		}

		public virtual FATabStripItem MoveTo(int newIndex, FATabStripItem item)
		{
			int index = this.List.IndexOf((object)item);
			if (index < 0)
				return (FATabStripItem)null;
			this.RemoveAt(index);
			this.Insert(0, item);
			return item;
		}

		public virtual int IndexOf(FATabStripItem item)
		{
			return this.List.IndexOf((object)item);
		}

		public virtual bool Contains(FATabStripItem item)
		{
			return this.List.Contains((object)item);
		}

		public virtual void Insert(int index, FATabStripItem item)
		{
			if (this.Contains(item))
				return;
			this.List.Insert(index, (object)item);
		}

		protected override void OnInsertComplete(int index, object item)
		{
			(item as FATabStripItem).Changed += new EventHandler(this.OnItem_Changed);
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}

		protected override void OnRemove(int index, object item)
		{
			base.OnRemove(index, item);
			(item as FATabStripItem).Changed -= new EventHandler(this.OnItem_Changed);
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}

		protected override void OnClear()
		{
			if (this.Count == 0)
				return;
			this.BeginUpdate();
			try {
				for (int index = this.Count - 1; index >= 0; --index)
					this.RemoveAt(index);
			}
			finally {
				this.EndUpdate();
			}
		}

		protected virtual void OnItem_Changed(object sender, EventArgs e)
		{
			this.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}
	}
}
