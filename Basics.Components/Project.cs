using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data;
using Basics.Components;

namespace Basics.Components
{
	public abstract class Project : Controller
	{
		public Project()
		{
			this.Results.Name = "runs";

			Application.Current.DispatcherUnhandledException += DispatcherUnhandledException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			this.Views.CollectionChanged += Views_CollectionChanged;
			//this.Background(CalculationFactoriesCreate);
		}

		void Views_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
			{
				foreach (ViewContent view in e.NewItems)
				{
					view.DataContext.Project = this;
				}
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
			Application.Current.DispatcherUnhandledException -= DispatcherUnhandledException;
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			this.HandleException((Exception)e.ExceptionObject);
		}

		void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			Exception exc = e.Exception;

			if (this.HandleException(exc))
			{
				this.Log("Unhandled exception: " + exc);
				this.Errors.Add(exc);
				e.Handled = true;
			}
		}

		private bool HandleException(Exception exc)
		{
			if (exc is InvalidOperationException
				&& exc.StackTrace.Contains("BindingExpression"))
			{
				this.Log("Binding exception: " + exc);
				this.Errors.Add(exc);
				var view = this.SelectedView;
				this.SelectedView = null;
				this.SelectedView = view;
				return true;
			}

			if (exc is InvalidOperationException
				&& exc.ToString().Contains("CollectionChanged"))
			{
				this.Log("CollectionChanged exception: " + exc);
				this.Errors.Add(exc);
				this.Log("Cannot recover. Shutting down!");
				Application.Current.Shutdown();
				return true;
			}
			return true;
		}

		private string _Path;
		/// <summary>
		/// the file path that for the project...
		/// </summary>
		public string Path
		{
			get { return _Path; }
			set
			{
				if (_Path != value)
				{
					_Path = value;
					this.OnPropertyChanged("Path");
				}
			}
		}

		public void Save()
		{
			DataSet data = new DataSet("project");
			this.Save(data);
			data.WriteXml(this.Path);
		}

		public void Load()
		{
			if (!System.IO.File.Exists(this.Path))
			{
				this.Log("Cannot find project file '" + this.Path + "'!");
				return;
			}
			this.Log("Loading project file '" + this.Path + "'...");
			DataSet data = new DataSet("project");
			data.ReadXml(this.Path);
			this.Load(data, 1);

			this.Log("Loading project file '" + this.Path + "'... done.");
		}


		//private EditorList<Basics.Components.Vehicle, Basics.Components.Vehicle> _Vehicles = new EditorList<Basics.Components.Vehicle, Basics.Components.Vehicle>("Vehicle", "vehicle");
		///// <summary>
		///// SavableList of Vehicles with sensors
		///// </summary>
		//public EditorList<Basics.Components.Vehicle, Basics.Components.Vehicle> Vehicles
		//{
		//	get
		//	{
		//		return _Vehicles;
		//	}
		//}

		private Results _Results = new Results();
		/// <summary>
		/// SavableList of Runs containing the results
		/// </summary>
		public Results Results
		{
			get
			{
				return _Results;
			}
		}

		private ViewContent _SelectedView;
		/// <summary>
		/// selected view
		/// </summary>
		public ViewContent SelectedView
		{
			get { return _SelectedView; }
			set
			{
				if (_SelectedView != value)
				{
					this.Log("Selecting view... changing to " + value);
					_SelectedView = value;
					if (value != null)
					{
						value.DataContext.Show();
					}
					this.IsNavigating = true;
					this.OnPropertyChanged("SelectedView");
				}
			}
		}

		private bool _IsNavigating;
		/// <summary>
		/// whether the view is currently being navigated...
		/// </summary>
		public bool IsNavigating
		{
			get { return _IsNavigating; }
			set
			{
				if (_IsNavigating != value)
				{
					_IsNavigating = value;
					this.OnPropertyChanged("IsNavigating");
				}
			}
		}

		private DisposableItemList<ViewContent> _Views = new DisposableItemList<ViewContent>();
		public DisposableItemList<ViewContent> Views
		{
			get
			{
				return _Views;
			}
		}

		public void OpenView(IContentView content, string uri)
		{
			this.Background(() =>
				{
					if (this.SelectedView == null || this.SelectedView.DataContext != content)
					{
						this.IsNavigating = true;
						this.SelectedView = null;
					}
					ViewContent existing = this.Views.FirstOrDefault(view => view.ContentUri == uri && view.DataContext == content);
					if (existing == null)
					{
						existing = new ViewContent();
						existing.ContentUri = uri;
						existing.DataContext = content;
						this.Views.Add(existing);
					}
					content.ActiveView = this.SelectedView = existing;
				});
		}


		public void CloseView(ViewContent view)
		{
			this.Invoke(() =>
				{
					int selected = -1;
					if (this.SelectedView == view)
					{
						selected = this.Views.IndexOf(view);
					}
					this.Views.Remove(view);

					if (selected >= 0 && this.Views.Count > 0)
					{
						if (selected > 0)
							this.SelectedView = this.Views[selected - 1];
						else
							this.SelectedView = this.Views[selected];
					}
				});
		}

		public Command ClearViewsCommand
		{
			get
			{
				return new Command(p =>
				{
					var views = this.Views.toarray();
					this.SelectedView = null;
					foreach (var view in views)
					{
						this.Views.Remove(view);
					}
				});
			}
		}

		private Calculator _Calculator;
		/// <summary>
		/// a calculation executor
		/// </summary>
		public Calculator Calculator
		{
			get { return _Calculator; }
			private set
			{
				if (_Calculator != value)
				{
					_Calculator = value;
					this.OnPropertyChanged("Calculator");
				}
			}
		}


		public Command RunSimulation
		{
			get
			{
				return new Command(p => this.Calculator.Run());
			}
		}


		public void ClearRuns()
		{
			var results = this.Results.ToArray();
			this.Results.Clear();
			foreach (Basics.Components.IResult result in results)
			{
				result.Dispose();
			}
		}

		public Command ClearRunsCommand
		{
			get
			{
				return new Command(p =>
				{
					this.ClearRuns();
				});
			}
		}


		protected override void SavableFields(SavableFields fields)
		{
			//fields.Add("Testcases", (data) => this.Testcases.Save(data), (data, value) => this.Testcases.Load(data, value));
			//fields.Add("Sensors", (data) => this.Sensors.Save(data), (data, value) => this.Sensors.Load(data, value));
			//fields.Add("Strategies", (data) => this.Strategies.Save(data), (data, value) => this.Strategies.Load(data, value));
			//fields.Add("Vehicles", (data) => this.Vehicles.Save(data), (data, value) => this.Vehicles.Load(data, value));
			//fields.Add("Analyses", (data) => this.Analyses.Save(data), (data, value) => this.Analyses.Load(data, value));
		}

		public override void ClearStorageIds()
		{
			base.ClearStorageIds();

			//this.Testcases.ClearStorageIds();
			//this.Vehicles.ClearStorageIds();
		}

		private ObservableCollection<Exception> _Errors = new ObservableCollection<Exception>();
		/// <summary>
		/// errors encountered in this project...
		/// </summary>
		public ObservableCollection<Exception> Errors
		{
			get { return _Errors; }
		}
	}
}
