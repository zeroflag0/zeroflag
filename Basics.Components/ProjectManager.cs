using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Basics.Components;

namespace Basics.Components
{
	public abstract class ProjectManager<TProject> : ProjectManager
		where TProject : Project, new()
	{
		public static ProjectManager<TProject> Instance { get; private set; }
		public ProjectManager()
		{
			Instance = this;
			this.ConnectWindow();
		}

		private TProject _Project;
		/// <summary>
		/// the active project
		/// </summary>
		public TProject Project
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

		protected override Project ProjectInternal
		{
			get { return this.Project; }
			set { this.Project = (TProject)value; }
		}

		protected override Project CreateProject()
		{
			return new TProject();
		}
	}

	public abstract class ProjectManager : Component
	{
		public ProjectManager()
		{
			this.ConnectWindow();
		}

		protected abstract Project ProjectInternal { get; set; }

		protected string DefaultStorageDirectory
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
		}

		const string DefaultFilename = "unnamed";
		const string DefaultExtension = ".analysis";
		public bool IsDefaultProjectFile(string file)
		{
			string filename = Path.GetFileNameWithoutExtension(file);
			if (!filename.StartsWith(DefaultFilename))
				return false;
			string ext = Path.GetExtension(file);
			if (ext != null && ext.ToLower().Trim('.') == DefaultExtension.ToLower().Trim('.'))
				return true;
			return false;
		}

		public string DefaultProjectFile
		{
			get
			{
				string dir = this.DefaultStorageDirectory;
				int? i = null;
				Func<string> filepath = () => Path.Combine(dir, DefaultFilename + i + DefaultExtension);
				while (File.Exists(filepath()))
				{
					if (i == null)
						i = 0;
					else
						i++;
				}
				return filepath();
			}
		}

		public Command ChangeCultureCommand
		{
			get
			{
				return new Command(p =>
				{
					CultureInfo culture = p as CultureInfo;
					if (culture == null)
					{
						if (p is string)
						{
							string name = p as string;
							culture = CultureInfo.GetCultureInfo(name);
						}
						if (culture == null)
							culture = CultureInfo.InstalledUICulture;
					}
					Application.Current.Dispatcher.Thread.CurrentCulture = culture;
					Application.Current.Dispatcher.Thread.CurrentUICulture = culture;

				});
			}
		}

		public IEnumerable<CultureInfo> Cultures
		{
			get
			{
				return CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);
			}
		}

		public Command ReloadProjectCommand
		{
			get
			{
				return new Command(p =>
				{
					string file = this.ProjectFile;

					this.Load(file);
				});
			}
		}

		public Command TestExceptionCommand
		{
			get
			{
				return new Command(p =>
				{
					throw new Exception("test exception");
				});
			}
		}

		public Command TestOverflowCommand
		{
			get
			{
				return new Command(p =>
				{
					this.OverflowException();
				});
			}
		}
		void OverflowException()
		{
			this.OverflowException();
		}

		public Command NewProjectCommand
		{
			get
			{
				return new Command(p => this.Busy(() =>
				{
					this.NewProject();
				}));
			}
		}

		public void NewProject()
		{
			Microsoft.Win32.SaveFileDialog diag = this.CreateSaveDialog();

			diag.CheckFileExists = false;

			if (diag.ShowDialog() == true)
			{
				System.IO.File.Delete(diag.FileName);
				this.Load(diag.FileName);
			}
		}

		public Command LoadProjectCommand
		{
			get
			{
				return new Command(p => this.Busy(() => this.LoadProject()));
			}
		}

		public void LoadProject()
		{
			Microsoft.Win32.OpenFileDialog diag = new Microsoft.Win32.OpenFileDialog();
			diag.AddExtension = true;
			diag.DefaultExt = DefaultExtension;
			string last = this.ProjectFileHistory.Count > 0 ? this.ProjectFileHistory[this.ProjectFileHistory.Count - 1] : null;
			diag.FileName = Path.GetFileName(last);

			diag.Filter = "SomeIp.Analyzer files|*" + DefaultExtension + ";*.aly|all files|*.*";
			diag.InitialDirectory = Path.GetDirectoryName(last ?? DefaultStorageDirectory);

			diag.Multiselect = false;
			diag.CheckFileExists = true;
			if (diag.ShowDialog() == true)
			{
				this.Load(diag.FileName);
			}
		}
		public Command SaveProjectCommand
		{
			get
			{
				return new Command(p => this.Busy(() =>
				{
					this.SaveProject();
				}));
			}
		}

		public bool SaveProject()
		{
			string file = this.ProjectInternal.Path;
			if (this.IsDefaultProjectFile(file))
			{
				return this.SaveAsProject();
			}
			else
				this.ProjectInternal.Save();
			return true;
		}
		public Command SaveAsProjectCommand
		{
			get
			{
				return new Command(p => this.Busy(() =>
				{
					this.SaveAsProject();
				}));
			}
		}

		private Microsoft.Win32.SaveFileDialog CreateSaveDialog()
		{
			Microsoft.Win32.SaveFileDialog diag = new Microsoft.Win32.SaveFileDialog();
			diag.AddExtension = true;
			diag.DefaultExt = DefaultExtension;
			string last = this.ProjectFileHistory.Count > 0 ? this.ProjectFileHistory[this.ProjectFileHistory.Count - 1] : null;
			diag.FileName = Path.GetFileName(last);

			diag.Filter = "SomeIp.Analyzer files|*" + DefaultExtension + ";*.aly|all files|*.*";
			diag.InitialDirectory = Path.GetDirectoryName(last ?? DefaultStorageDirectory);
			return diag;
		}
		public bool SaveAsProject()
		{
			Microsoft.Win32.SaveFileDialog diag = this.CreateSaveDialog();

			if (diag.ShowDialog() == true)
			{
				this.ProjectFile = this.ProjectInternal.Path = diag.FileName;
				this.ProjectInternal.Save();
				return true;
			}
			return false;
		}

		protected abstract void SaveSettings();
		protected abstract string SettingsProjectFile { get; set; }
		protected abstract System.Collections.Specialized.StringCollection SettingsProjectFileHistory { get; set; }

		/// <summary>
		/// the project's file path
		/// </summary>
		public string ProjectFile
		{
			get { return this.SettingsProjectFile; }
			set
			{
				string path = System.IO.Path.GetFullPath(value);
				if (this.SettingsProjectFile != path)
				{
					this.SettingsProjectFile = path;
					this.SaveSettings();
					this.OnPropertyChanged("ProjectFile");
				}
			}
		}

		/// <summary>
		/// project file paths that were used before...
		/// </summary>
		public System.Collections.Specialized.StringCollection ProjectFileHistory
		{
			get { return this.SettingsProjectFileHistory ?? (this.SettingsProjectFileHistory = new System.Collections.Specialized.StringCollection()); }
			set
			{
				if (this.SettingsProjectFileHistory != value)
				{
					this.SettingsProjectFileHistory = value;
					this.SaveSettings();
					this.OnPropertyChanged("ProjectFileHistory");
				}
			}
		}


		public void OnOpen()
		{
			string file = this.ProjectFile;
			if (file.isempty())
				file = this.DefaultProjectFile;
			this.Load(file);

			this.Background(() => this.Invoke(() => this.IsReady = true));
		}

		private bool _IsReady;
		/// <summary>
		/// whether the project manager has loaded and is ready
		/// </summary>
		public bool IsReady
		{
			get { return _IsReady; }
			set
			{
				if (_IsReady != value)
				{
					_IsReady = value;
					this.OnPropertyChanged("IsReady");
				}
			}
		}

		public void Load(string filepath)
		{
			filepath = this.ProjectFile = filepath;

			this.ProjectFileHistory.Add(filepath);
			while (this.ProjectFileHistory.Count > 100)
				this.ProjectFileHistory.RemoveAt(0);
			this.SaveSettings();
			this.OnPropertyChanged("ProjectFileHistory");

			if (this.ProjectInternal != null)
			{
				this.ProjectInternal.Dispose();
				this.ProjectInternal = null;
			}

			this.ProjectInternal = this.CreateProject();
			this.ProjectFile = this.ProjectInternal.Path = filepath;
			this.ProjectInternal.Busy(this.ProjectInternal.Load);
		}

		protected abstract Project CreateProject();

		/// <summary>
		/// 
		/// </summary>
		/// <returns>whether shutdown can conclude(true) or not(false)</returns>
		public bool OnClose()
		{
			//System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(p => this.Save()));
			if (this.ProjectInternal != null)
			{
				this.Log("Saving... project... " + this.ProjectInternal.Path);
				bool saved = false;
				while (!saved)
				{
					saved = this.SaveProject();
					if (saved)
						break;
					MessageBoxResult decision = MessageBox.Show("The current project is not saved!" + Environment.NewLine + Environment.NewLine + "Do you want to save it now?", "Project not saved!", MessageBoxButton.YesNoCancel);
					if (decision == MessageBoxResult.No)
						break;
					if (decision == MessageBoxResult.Cancel)
						return false;
				}

			}
			this.Log("Saving... user settings...");
			//this.Log("\t" + "RightAreaWidth" + " = " + SomeIp.Analyzer.Properties.Settings.Default.RightAreaWidth);
			//this.Log("\t" + "BottomAreaHeight" + " = " + SomeIp.Analyzer.Properties.Settings.Default.BottomAreaHeight);
			this.SaveSettings();
			this.Log("Saving... done.");
			return true;
		}

		protected abstract Window Window { get; }
		protected Frame Frame { get; set; }
		private ProjectManager _DataContext;
		/// <summary>
		/// data context
		/// </summary>
		protected ProjectManager DataContext
		{
			get { return _DataContext ?? this.FindDataContext(); }
			set
			{
				if (_DataContext != value)
				{
					_DataContext = value;
					this.OnPropertyChanged("DataContext");
				}
			}
		}

		protected void ConnectWindow()
		{
			this.DataContext = null;
			this.Frame = null;

			this.Window.Loaded += Window_Loaded;
			this.Window.Closing += Window_Closing;
			this.ConnectFrame();
		}

		private void ConnectFrame()
		{
			if (this.Frame != null)
				return;

			this.Frame = this.Window.Find<Frame>(ctrl =>
			{
				Frame f = ctrl as Frame;
				if (f != null)
					if (f.Name != null && f.Name == "MainFrame")
						return f;
				return null;
			});
			if (this.Frame != null)
				this.Frame.Navigated += Frame_Navigated;
		}
		protected ProjectManager FindDataContext()
		{
			this.DataContext = this.FindDataContext(this.Window);
			if (this.DataContext == null)
				throw new ArgumentNullException("Cannot find datacontext in window(" + this.Window + ")!");
			return this.DataContext;
		}

		private ProjectManager FindDataContext(System.Windows.FrameworkElement ctrl)
		{
			return ctrl.Find<ProjectManager>(c => (c != null && c.DataContext is ProjectManager) ? c.DataContext as ProjectManager : null);
			//if (ctrl == null)
			//	return null;
			//if (ctrl.DataContext is ProjectManager)
			//{
			//	return ctrl.DataContext as ProjectManager;
			//}
			//if (ctrl is ContentControl)
			//{
			//	ContentControl container = (ContentControl)ctrl;
			//	if (container.Content == null)
			//		return null;
			//	if (!(container.Content is FrameworkElement))
			//		return null;
			//	FrameworkElement content = container.Content as FrameworkElement;
			//	return this.FindDataContext(content);
			//}
			//if (ctrl is Panel)
			//{
			//	Panel container = (Panel)ctrl;
			//	foreach (UIElement element in container.Children)
			//	{
			//		if (element == null)
			//			continue;
			//		ProjectManager mgr = this.FindDataContext(element as FrameworkElement);
			//		if (mgr == null)
			//			continue;
			//		return mgr;
			//	}
			//}

			//return null;
		}


		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.ConnectFrame();
			if (DataContext != null)
				DataContext.OnOpen();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.ConnectFrame();
			if (DataContext != null)
				// ask the manager to close and if not, cancel closing...
				e.Cancel = !DataContext.OnClose();
		}

		private void Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			if (DataContext != null)
				DataContext.ProjectInternal.IsNavigating = false;
		}

	}
}
