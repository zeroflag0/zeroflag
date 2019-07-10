using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	/// <summary>
	/// Baseclass for user calculations. Provides convenience functions, helpers and general data API.
	/// </summary>
	//[Serializable]
	public abstract partial class CalculationBase : Base
	{
		public CalculationBase()
		{
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		/// <summary>
		/// short description of the calculation...
		/// </summary>
		public abstract string Name { get; }
		/// <summary>
		/// detailed description of the calculation...
		/// </summary>
		public abstract string Description { get; }

		//private JobContext _Context;
		///// <summary>
		///// The context for this calculation.
		///// </summary>
		//public JobContext Context
		//{
		//	get { return _Context; }
		//	set
		//	{
		//		if (_Context != value)
		//		{
		//			_Context = value;
		//			this.OnPropertyChanged("Context");
		//		}
		//	}
		//}

		//private Dictionary<string, Field> _NamedFields = new Dictionary<string, Field>();
		///// <summary>
		///// fields used in the calculation
		///// </summary>
		//private Dictionary<string, Field> NamedFields
		//{
		//	get { return _NamedFields; }
		//}

		///// <summary>
		///// returns all fields created in the calculation.
		///// </summary>
		//public Field[] Fields
		//{
		//	get
		//	{
		//		return this.NamedFields.Values.ToArray();
		//	}
		//}

		/// <summary>
		/// fields used in the calculation
		/// </summary>
		public Fields Fields
		{
			get { return this.Context.Fields; }
		}


		//public Field CreateField(string name)
		//{
		//	if (this.Fields.ContainsKey(name))
		//		return this.Fields[name];

		//	if (this.IsLocked)
		//	{
		//		throw new InvalidOperationException("Calculation is locked! Cannot create Field '" + name + "'. (Tried to create a field outside of CreateFields()?)");
		//	}
		//	Field field = new Field(name);
		//	this.NamedFields.Add(name, field);
		//	field.Calculation = this;

		//	return field;
		//}

		public Field this[string name]
		{
			get
			{
				return this.Fields[name];
			}
			set
			{
				this.Fields[name] = value;
			}
		}
		//public Field this[string name]
		//{
		//	get
		//	{
		//		return this.CreateField(name);
		//	}
		//	set
		//	{
		//		Field target = null;
		//		if (value == null)
		//			// if no value was given, create a new field...
		//			target = this.CreateField(name);
		//		else if (string.IsNullOrEmpty(value.Name) || name != value.Name)
		//			// if the indexed name does not match the field's name, create a new field...
		//			target = this.CreateField(name);
		//		if (target == null)
		//		{
		//			if (this.NamedFields.ContainsKey(value.Name) && this.NamedFields[value.Name] == value)
		//				// if the value already is in the fields collection we don't need to do anything, it was set by reference...
		//				return;
		//			else
		//				target = this.CreateField(name);
		//		}
		//		// if a different field was given than was indexed, transfer the value and dismiss the given field (it was just a value-transfer-helper)...
		//		if (value != null)
		//		{
		//			target.Value = value.Value;
		//			target.Description = value.Description;
		//		}
		//		else
		//		{
		//			target.Value = null;
		//			target.Description = null;
		//		}
		//		this.NamedFields[name] = target;
		//	}
		//}


		private JobContext _Context;

		/// <summary>
		/// the calculation's context
		/// </summary>
		[IgnoreErrors]
		public JobContext Context
		{
			get { return _Context; }
			set
			{
				if (_Context != value)
				{
					_Context = value;
					this.OnPropertyChanged("Context");
				}
			}
		}

		/// <summary>
		/// Parameters to control the calculation.
		/// </summary>
		public Parameters Parameters
		{
			get { return this.Context.Parameters; }
		}

		/// <summary>
		/// Summaries from the calculation.
		/// </summary>
		public Parameters Summaries
		{
			get { return this.Context.Summaries; }
		}

		/// <summary>
		/// 1. [optional] perform basic initialization (without data)
		/// </summary>
		public virtual void InitializeBasics()
		{
		}

		/// <summary>
		/// 2. create all required fields, parameters and summaries
		/// </summary>
		public abstract void CreateFields();

		/// <summary>
		/// 3. initialize fields, read parameters.
		/// </summary>
		public abstract void Initialize();
		/// <summary>
		/// 4. [optional] prepare calculation at timestep 0.
		/// </summary>
		public virtual void Prepare()
		{
		}

		/// <summary>
		/// 5. calculate the current timestep.
		/// </summary>
		/// <param name="step"></param>
		public abstract void Calculate();

		/// <summary>
		/// 5.a) performed after each calculation. decides whether to continue.
		/// </summary>
		/// <param name="step"></param>
		/// <returns></returns>
		public abstract bool CheckContinue();

		/// <summary>
		/// 6. additional steps performed after all timesteps (Calculate) are done. e.g. summaries.
		/// </summary>
		public abstract void Finish();

		/// <summary>
		/// validate parameters and runtime
		/// </summary>
		/// <returns></returns>
		public bool Validate()
		{
			lock (this)
			{
				bool result = true;
				foreach (Parameter param in this.Parameters)
				{
					if (param.Value == null)
					{
						param.Errors.Add(new Error("NullValue", "Parameter '" + param.Name + "' must not be null."));
						result = false;
					}
				}
				if (!result)
					return result;
				try
				{
					return this.ValidateRun();
				}
				catch (Exception exc)
				{
					this.Context.CustomErrors.Add(exc.ToString());
					return false;
				}
			}
		}
		/// <summary>
		/// validates runtime
		/// </summary>
		/// <returns></returns>
		public abstract bool ValidateRun();
	}
}
