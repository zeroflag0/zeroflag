using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public class Parameter : ValueBase<Parameter>, IValue
	{
		public override decimal? Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;
			}
		}

		private string _Unit;
		/// <summary>
		/// the unit.
		/// </summary>
		public string Unit
		{
			get { return _Unit; }
			set
			{
				if (_Unit != value)
				{
					_Unit = value;
					this.OnPropertyChanged("Unit");
				}
			}
		}

		public void RefreshAll()
		{
			if (this.ProxyMaster == null)
				this.Refresh();
			else
				this.ProxyMaster.RefreshAll();
		}

		public override void Refresh()
		{
			if (this.ProxySlaves.Count == 0)
				return;
			lock (this.ProxySlaves)
			{
				if (this.ProxySlaves.Count == 0)
					return;

				var slaves = this.ProxySlaves.toarray();
				foreach (Parameter proxy in slaves)
				{
					if (proxy.ProxyMaster != this)
						proxy.ProxyMaster = this;
					if (this.HasCreated(proxy))
					{
						bool noevents = proxy.NoEvents;
						proxy.NoEvents = true;
						proxy.Value = this.Value;
						proxy.Creator = this.Creator;
						proxy.NoEvents = noevents;
					}

					proxy.Refresh();
				}
			}
		}

		protected bool HasCreated(Parameter proxy)
		{
			if (this.Creator == proxy.Creator)
				return true;
			if (this.ProxyMaster != null)
				return this.ProxyMaster.HasCreated(proxy);
			return false;
		}

		public void LinkSlave(Parameter slave)
		{
			lock (this.ProxySlaves)
				if (!this.ProxySlaves.Contains(slave))
					this.ProxySlaves.Add(slave);

			slave.ProxyMaster = this;
		}

		public void Unlink()
		{
			if (this.ProxyMaster != null)
				this.ProxyMaster.ProxySlaves.Remove(this);

			lock (this.ProxySlaves)
				foreach (var slave in this.ProxySlaves)
				{
					slave.ProxyMaster = null;
				}
		}

		public override void Dispose()
		{
			base.Dispose();
			this.Unlink();
		}

		private Parameter _ProxyMaster;
		/// <summary>
		/// the master for this proxy...
		/// </summary>
		[IgnoreErrors]
		public Parameter ProxyMaster
		{
			get { return _ProxyMaster; }
			set
			{
				if (_ProxyMaster != value)
				{
					_ProxyMaster = value;
					this.OnPropertyChanged("ProxyMaster");
				}
			}
		}

		private DisposableItemList<Parameter> _ProxySlaves = new DisposableItemList<Parameter>();
		/// <summary>
		/// the slaves for this master (in order)...
		/// </summary>
		public DisposableItemList<Parameter> ProxySlaves
		{
			get { return _ProxySlaves; }
		}


		protected override void SavableFields(SavableFields fields)
		{
			fields.Add("Name", () => this.Name, value => this.Name = value);
			fields.Add("Description", () => this.Description, value => this.Description = value);
			fields.Add("Value", () => this.Value, value => this.Value = value);
			fields.Add("IsSelected", () => this.IsSelected, value => this.IsSelected = value);
		}

		public override void ClearStorageIds()
		{
			base.ClearStorageIds();
		}

		#region casts
		public static implicit operator Parameter(Field value)
		{
			return new Parameter() { Value = value.Value };
		}

		public static implicit operator decimal?(Parameter value)
		{
			return value.Value;
		}

		public static implicit operator Parameter(decimal? value)
		{
			return new Parameter() { Value = value };
		}

		public static implicit operator decimal(Parameter value)
		{
			return value.Value.Value;
		}

		public static implicit operator Parameter(decimal value)
		{
			return new Parameter() { Value = value };
		}

		public static implicit operator Parameter(double? value)
		{
			return new Parameter() { Value = (decimal)value };
		}

		public static implicit operator Parameter(double value)
		{
			return new Parameter() { Value = (decimal)value };
		}

		public static implicit operator Parameter(long? value)
		{
			return new Parameter() { Value = (decimal)value };
		}

		public static implicit operator Parameter(long value)
		{
			return new Parameter() { Value = (decimal)value };
		}

		public static implicit operator Parameter(string description)
		{
			return new Parameter() { Description = description };
		}
		#endregion

		public override string ToString()
		{
			return "Parameter'" + this.Name + "'=" + this.Value + "[" + this.Creator + "]";
		}

		static readonly HashSet<string> _IgnoreValidationParameters = new HashSet<string> { "Creator", "Owner", "ProxyMaster", "ProxySlaves" };
		public override string this[string columnName, ErrorTracing trace]
		{
			get
			{
				if (_IgnoreValidationParameters.Contains(columnName))
					return null;

				if (columnName == "Name")
				{
					if (this.Name.isempty())
						return "Name must not be empty!";
					return null;
				}
				else if (columnName == "Value")
				{
					return base["Errors", trace];
				}
				return base[columnName, trace];
			}
		}
	}
}
