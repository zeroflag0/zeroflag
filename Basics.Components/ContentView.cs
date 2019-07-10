using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public interface IContentView : IContent
	{
		Project Project { get; set; }
		void Show();
		Command ShowCommand { get; }

		ViewContent ActiveView { get; set; }
	}

	/// <summary>
	/// content view
	/// </summary>
	public abstract class ContentView : ErrorInfo, IContentView
	{
		private Project _Project;
		/// <summary>
		/// the view's project
		/// </summary>
		public Project Project
		{
			get { return _Project; }
			set
			{
				if (_Project != value)
				{
					_Project = value;
					this.OnPropertyChanged("Project");
				}
			}
		}

		public Command ShowCommand
		{
			get
			{
				return new Command(p =>
				{
					this.Show();
				});
			}
		}
		public ViewContent ActiveView { get; set; }
		public virtual void Show()
		{
			this.Project.OpenView(this, this.PageUrl);
			//this.RefreshView();
		}

		public virtual void InitializeNew()
		{
		}

		public void RefreshView()
		{
			var properties = this.GetType().GetProperties();
			foreach (var property in properties)
			{
				this.OnPropertyChanged(property.Name);
			}
		}

		public override void Dispose()
		{
			if (ActiveView != null)
			{
				this.Project.CloseView(ActiveView);
				ActiveView = null;
			}
			base.Dispose();
		}

		/// <summary>
		/// the data object
		/// </summary>
		[IgnoreErrors]
		public abstract Content ModelBase { get; set; }

		public abstract Content CreateModel();

		public virtual string ContentType { get { return this.GetType().Name; } }

		public abstract string Name { get; set; }

		public abstract string Description { get; set; }

		public abstract bool IsSelected { get; set; }

		public abstract string PageUrl { get; }

		public abstract object DuplicateContent();
		public virtual TSelf Duplicate<TModel, TSelf>()
			where TModel : Content<TModel>, new()
			where TSelf : ContentView<TModel, TSelf>, new()
		{
			return (TSelf)this.DuplicateContent();
		}

		public override void ClearStorageIds()
		{
			base.ClearStorageIds();

			this.ModelBase.ClearStorageIds();
		}

		public abstract string TypeName { get; }
	}

	/// <summary>
	/// content view
	/// </summary>
	public abstract class ContentView<TModel> : ContentView, IContentView
		where TModel : Content<TModel>, new()
	{
		private TModel _Model;
		/// <summary>
		/// the data object
		/// </summary>
		public TModel Model
		{
			get { return _Model; }
			set
			{
				if (_Model != value)
				{
					if (_Model != null)
						this.UnlinkModel(_Model);
					_Model = value;
					if (_Model != null)
						this.LinkModel(_Model);
					this.OnPropertyChanged("Model");
				}
			}
		}
		public override string this[string columnName, ErrorTracing trace]
		{
			get
			{
				if (columnName == "Model")
				{
					if (Model == null)
						return "No model set!";
					if (Model.HasErrors)
						return Model.Error;
				}
				return base[columnName, trace];
			}
		}

		public override Content CreateModel()
		{
			return this.Model = new TModel();
		}

		[IgnoreErrors]
		public override Content ModelBase
		{
			get { return this.Model; }
			set { this.Model = (TModel)value; }
		}

		protected virtual void LinkModel(TModel model)
		{
			model.PropertyChanged += ModelPropertyChanged;
		}

		protected virtual void UnlinkModel(TModel model)
		{
			model.PropertyChanged -= ModelPropertyChanged;
		}

		protected virtual void ModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName);

			//if (e.PropertyName == "IsSelected")
			//{
			//	Project.Instance.RefreshContent();
			//}
		}

		/// <summary>
		/// Short name of the content
		/// </summary>
		public override string Name
		{
			get { return this.Model.Name; }
			set { this.Model.Name = value; }
		}

		/// <summary>
		/// Description of the content
		/// </summary>
		public override string Description
		{
			get { return this.Model.Description; }
			set { this.Model.Description = value; }
		}

		/// <summary>
		/// whether this content is selected
		/// </summary>
		public override bool IsSelected
		{
			get { return this.Model.IsSelected; }
			set { this.Model.IsSelected = value; }
		}


		public override void Dispose()
		{
			base.Dispose();
			this.Model = null;
		}

		protected override void SavableFields(SavableFields fields)
		{
			fields.Add("model", data => this.Model.Save(data), (data, value) => this.Model = Load<TModel>(data, value));
		}

		public override void ClearStorageIds()
		{
			base.ClearStorageIds();
			this.Model.ClearStorageIds();
		}
	}

	/// <summary>
	/// content view
	/// </summary>
	public abstract class ContentView<TModel, TSelf> : ContentView<TModel>, IContentView
		where TModel : Content<TModel>, new()
		where TSelf : ContentView<TModel, TSelf>, new()
	{
		public override object DuplicateContent()
		{
			return this.DuplicateInternal();
		}

		protected TSelf DuplicateInternal()
		{
			TSelf clone = new TSelf();
			this.FillClone(clone);
			clone.ClearStorageIds();
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
}
