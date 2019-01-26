using System.Collections;
using System.ComponentModel;

namespace FarsiLibrary.Win.Helpers
{
	public abstract class CollectionWithEvents : CollectionBase
	{
		private int _suspendCount;

		[Browsable(false)]
		public event CollectionClear Clearing;

		[Browsable(false)]
		public event CollectionClear Cleared;

		[Browsable(false)]
		public event CollectionChange Inserting;

		[Browsable(false)]
		public event CollectionChange Inserted;

		[Browsable(false)]
		public event CollectionChange Removing;

		[Browsable(false)]
		public event CollectionChange Removed;

		public CollectionWithEvents()
		{
			this._suspendCount = 0;
		}

		public void SuspendEvents()
		{
			++this._suspendCount;
		}

		public void ResumeEvents()
		{
			--this._suspendCount;
		}

		[Browsable(false)]
		public bool IsSuspended
		{
			get
			{
				return this._suspendCount > 0;
			}
		}

		protected override void OnClear()
		{
			if (this.IsSuspended || this.Clearing == null)
				return;
			this.Clearing();
		}

		protected override void OnClearComplete()
		{
			if (this.IsSuspended || this.Cleared == null)
				return;
			this.Cleared();
		}

		protected override void OnInsert(int index, object value)
		{
			if (this.IsSuspended || this.Inserting == null)
				return;
			this.Inserting(index, value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			if (this.IsSuspended || this.Inserted == null)
				return;
			this.Inserted(index, value);
		}

		protected override void OnRemove(int index, object value)
		{
			if (this.IsSuspended || this.Removing == null)
				return;
			this.Removing(index, value);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			if (this.IsSuspended || this.Removed == null)
				return;
			this.Removed(index, value);
		}

		protected int IndexOf(object value)
		{
			return this.List.IndexOf(value);
		}
	}
}
