using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

public class Validating : Base, INotifyDataErrorInfo, IDataErrorInfo
{
	/// <summary>
	/// whether a property's error-state is automatically cleared when its PropertyChanged-event is fired.
	/// </summary>
	protected virtual bool ErrorsAutoClearOnChange
	{
		get { return true; }
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		if (this.ErrorsAutoClearOnChange)
		{
			lock (this.Errors)
				if (this.Errors.ContainsKey(e.PropertyName))
					this.Errors.Remove(e.PropertyName);
		}
		base.OnPropertyChanged(e);
	}

	private Dictionary<string, string> _Errors = new Dictionary<string, string>();
	/// <summary>
	/// errors
	/// </summary>
	public Dictionary<string, string> Errors
	{
		get { return _Errors; }
	}

	public virtual void SetError(string propertyName, object error)
	{
		this.Log("Errors['" + propertyName + "']=" + error);
		if (error != null)
		{
			string text = error + "";
			lock (this.Errors)
				this.Errors[propertyName] = text;
		}
		else
		{
			lock (this.Errors)
				this.Errors.Remove(propertyName);
		}
		this.OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
		//this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		this.OnPropertyChanged("Errors");
		this.OnPropertyChanged("HasErrors");
	}

	public void ClearError(string propertyName)
	{
		this.SetError(propertyName, null);
	}

	public void ClearAllErrors()
	{
		lock (Errors)
			while (this.Errors.Count > 0)
			{
				string prop = this.Errors.Keys.First();
				this.ClearError(prop);
			}
		this.OnPropertyChanged("Error");
		this.OnPropertyChanged("Errors");
		this.OnPropertyChanged("HasErrors");
	}

	protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
	{
		if (this.ErrorsChanged != null)
			this.Invoke(() => this.ErrorsChanged(this, e));
	}
	public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

	public IEnumerable GetErrors(string propertyName)
	{
		if (propertyName == null)
		{
			yield return this.Error;
			yield break;
		}
		lock (this.Errors)
			if (this.Errors.ContainsKey(propertyName))
				yield return this.Errors[propertyName];
		yield break;
	}

	IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
	{
		return this.GetErrors(propertyName);
	}

	public bool HasErrors
	{
		get
		{
			return this.Errors.Count > 0;
		}
	}

	public string Error
	{
		get
		{
			StringBuilder sb = new StringBuilder();
			foreach (string key in this.Errors.Keys)
			{
				sb.Append(key).Append(": ").Append(this[key]).AppendLine();
			}
			return sb.ToString();
		}
	}

	public string this[string columnName]
	{
		get
		{
			IEnumerable errors = this.GetErrors(columnName);
			if (errors == null)
				return null;
			object result = errors.Cast<object>().FirstOrDefault();
			if (result == null)
				return null;
			else
				return result.ToString();
		}
	}
}
