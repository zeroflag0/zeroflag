using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	/// <summary>
	/// content model
	/// </summary>
	/// <typeparam name="TSelf"></typeparam>
	public abstract class Content<TSelf> : Content
		where TSelf : Content<TSelf>, new()
	{
		public TSelf Duplicate()
		{
			TSelf clone = new TSelf();
			this.FillClone(clone);
			return clone;
		}

		protected virtual void FillClone(TSelf clone)
		{
			SavableFields targetfieldslist = clone.CreateSavableFields();
			Dictionary<string, SavableField> targetfields = targetfieldslist.ToDictionary(field => field.Name);

			SavableFields sourcefields = this.CreateSavableFields();
			foreach (SavableField source in sourcefields)
			{
				System.Data.DataSet data = new System.Data.DataSet();
				object result = source.Save(data);
				SavableField target = targetfields[source.Name];
				target.Load(data, result);
			}
		}
	}

	/// <summary>
	/// content model
	/// </summary>
	public abstract class Content : ErrorInfo, IContent
	{
		public Content()
		{
			this.Errors.CollectionChanged += Errors_CollectionChanged;
		}

		public override void Dispose()
		{
			this.Errors.CollectionChanged -= Errors_CollectionChanged;
			base.Dispose();
		}

		void Errors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("HasErrors");
		}

		private string _Name;
		/// <summary>
		/// Short name of the content
		/// </summary>
		public virtual string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}

		private string _FullName;
		/// <summary>
		/// Full name of the content
		/// </summary>
		public string FullName
		{
			get { return _FullName; }
			set
			{
				if (_FullName != value)
				{
					_FullName = value;
					this.OnPropertyChanged("FullName");
				}
			}
		}
		

		private string _Description;
		/// <summary>
		/// Description of the content
		/// </summary>
		public virtual string Description
		{
			get { return _Description; }
			set
			{
				if (_Description != value)
				{
					_Description = value;
					this.OnPropertyChanged("Description");
				}
			}
		}

		private bool _IsSelected = false;
		/// <summary>
		/// whether this content is selected
		/// </summary>
		public virtual bool IsSelected
		{
			get { return _IsSelected; }
			set
			{
				if (_IsSelected != value)
				{
					_IsSelected = value;
					this.OnPropertyChanged("IsSelected");
				}
			}
		}

		private bool _IsLocked;
		/// <summary>
		/// whether changes to parameters are allowed
		/// </summary>
		public bool IsLocked
		{
			get { return _IsLocked; }
			set
			{
				if (_IsLocked != value)
				{
					_IsLocked = value;
					this.OnLockChanged(value);
					this.OnPropertyChanged("IsLocked");
				}
			}
		}

		ContentList<Error> _Errors = new ContentList<Error>();
		[IgnoreErrors]
		public ContentList<Error> Errors
		{
			get
			{
				return _Errors;
			}
		}

		public override string this[string columnName, ErrorTracing trace]
		{
			get
			{
				if (columnName == "Errors")
				{
					StringBuilder s = new StringBuilder();
					foreach (Error error in this.Errors)
					{
						s.AppendLine(error.Name + ": " + error.Description);
					}
					return s.ToString();
				}
				return base[columnName, trace];
			}
		}

		protected virtual void OnLockChanged(bool value)
		{
		}

		protected override void SavableFields(SavableFields fields)
		{
			fields.Add("Name", () => this.Name, value => this.Name = value);
			fields.Add("Description", () => this.Description, value => this.Description = value);
			fields.Add("IsSelected", () => this.IsSelected, value => this.IsSelected = value);
		}

		public override void ClearStorageIds()
		{
			base.ClearStorageIds();
		}

		public override string ToString()
		{
			return this.GetType().Name + "'" + this.Name + "'";
		}
	}
}
