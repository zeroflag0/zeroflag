using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace Basics.Components
{
	public class IgnoreErrorsAttribute : Attribute
	{
	}

	public abstract class ErrorInfo : Savable, IDataErrorInfo
	{
		public ErrorInfo()
		{
			this.CustomErrors.CollectionChanged += CustomErrors_CollectionChanged;
		}

		public override void Dispose()
		{
			this.CustomErrors.CollectionChanged -= CustomErrors_CollectionChanged;
			base.Dispose();
		}

		void CustomErrors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("HasErrors");
			this.OnPropertyChanged("Error");
		}

		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.PropertyName != "HasErrors")
			{
				this.OnPropertyChanged("HasErrors");

				if (e.PropertyName != "Error")
				{
					this.OnPropertyChanged("Error");
				}
			}
		}
		public bool HasErrors
		{
			get
			{
				var properties = this.GetType().GetProperties();
				foreach (var property in properties)
				{
					return this[property.Name] != null;
				}
				return false;
			}
		}

		public virtual string GetAllErrors(ErrorTracing trace = null)
		{
			if (trace == null)
				trace = new ErrorTracing();

			Dictionary<string, int> messages = new Dictionary<string, int>();
			var properties = this.GetType().GetProperties();
			foreach (var property in properties)
			{
				string prop = this[property, trace];
				if (prop.isempty())
					continue;
				messages[prop] = 0;
			}
			StringBuilder s = new StringBuilder();
			foreach (string prop in messages.Keys)
			{
				s.Append(prop).AppendLine();
			}
			if (s.Length == 0)
				return null;
			return s.ToString();
		}

		public string Error
		{
			get
			{
				return this.GetAllErrors();
			}
		}


		private ObservableCollection<string> _CustomErrors = new ObservableCollection<string>();

		/// <summary>
		/// custom errors
		/// </summary>
		public ObservableCollection<string> CustomErrors
		{
			get { return _CustomErrors; }
		}


		public virtual string this[string columnName, ErrorTracing trace]
		{
			get
			{
				var property = this.GetType().GetProperty(columnName, new Type[0]);
				if (property != null)
					return this[property, trace];

				return null;
			}
		}

		public class ErrorTracing
		{
			Dictionary<object, HashSet<System.Reflection.PropertyInfo>> _Processed = new Dictionary<object, HashSet<System.Reflection.PropertyInfo>>();
			public Dictionary<object, HashSet<System.Reflection.PropertyInfo>> Processed { get { return _Processed; } }

			List<KeyValuePair<object, System.Reflection.PropertyInfo>> _Trace = new List<KeyValuePair<object, System.Reflection.PropertyInfo>>();
			public List<KeyValuePair<object, System.Reflection.PropertyInfo>> Trace { get { return _Trace; } }

			public bool IsCyclic(object owner, System.Reflection.PropertyInfo property)
			{
				this.Trace.Add(new KeyValuePair<object, System.Reflection.PropertyInfo>(owner, property));
				if (!Processed.ContainsKey(this))
					Processed[this] = new HashSet<System.Reflection.PropertyInfo>();
				if (!Processed[this].Contains(property))
					Processed[this].Add(property);
				else
					return true;

				return false;
			}

			public string TraceToString()
			{
				StringBuilder sb = new StringBuilder();
				foreach (KeyValuePair<object, System.Reflection.PropertyInfo> entry in this.Trace.toarray())
				{
					sb.Append(entry.Key.ToString()).Append(" -> ").AppendLine(entry.Value.ToString());
				}
				return sb.ToString();
			}
		}

		public string this[System.Reflection.PropertyInfo property, ErrorTracing trace = null]
		{
			get
			{
				return this.Group(property.Name, this.CheckError(property, trace));
			}
		}

		string CheckError(System.Reflection.PropertyInfo property, ErrorTracing trace)
		{
			foreach (object attribute in property.GetCustomAttributes(typeof(IgnoreErrorsAttribute), true))
			{
				if (attribute is IgnoreErrorsAttribute)
					return null;
			}

			if (trace == null)
				trace = new ErrorTracing();

			if (trace.IsCyclic(this, property))
			{
				//try
				//{
				//	throw new Exception("Cyclic error check on '" + this + "'.'" + property + "'!");
				//}
				//catch (Exception exc)
				//{
				//	this.Log(exc.ToString());
				//	return exc.ToString();
				//}
				//return "Cyclic error check on '" + this + "'.'" + property + "'!" + Environment.NewLine + trace.TraceToString() + Environment.NewLine + Environment.NewLine;
				return null;
			}

			//return null;
			//if (new HashSet<string>() { "Input", "ResultView", "ResultColumns", "Fields" }.Contains(property.Name))
			//	this.Log("Error[" + property.Name + "]");
			//if (new System.Diagnostics.StackTrace().FrameCount > 50)
			//{
			//	this.Log("Stack growing too deep! " + property + Environment.NewLine + new System.Diagnostics.StackTrace());
			//	return "Error trace for " + property + " running too deep!";
			//}
			if (new System.Diagnostics.StackTrace().FrameCount > 1000)
			{
				throw new Exception("Stack growing too deep!");
			}

			if (property.Name == "CustomErrors")
			{
				if (this.CustomErrors.Count > 0)
				{
					StringBuilder sb = new StringBuilder();
					foreach (string error in this.CustomErrors.toarray())
					{
						sb.AppendLine(error);
					}
					return sb.ToString();
				}
				return null;
			}

			if (typeof(ErrorInfo).IsAssignableFrom(property.PropertyType))
			{
				var target = ((ErrorInfo)property.GetValue(this, new object[0]));
				if (target != null)
					return target.GetAllErrors(trace);
			}
			else if (typeof(IDataErrorInfo).IsAssignableFrom(property.PropertyType))
			{
				var target = ((IDataErrorInfo)property.GetValue(this, new object[0]));
				if (target != null)
					return target.Error;
			}

			return null;
		}

		string Group(string groupname, string content)
		{
#if DEBUG
			if (content.isempty())
				return content;
			StringBuilder sb = new StringBuilder();
			string[] lines = content.Split('\n');
			sb.Append(groupname).AppendLine();
			foreach (string line in lines)
			{
				sb.Append("  ").Append(line);
			}
			return sb.ToString();
#else
			return content;
#endif
		}

		public string this[string columnName]
		{
			get { return this[columnName, null]; }
		}
	}
}
