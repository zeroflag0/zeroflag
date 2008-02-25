using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.BridgeGenerator
{
	public class Member
	{
		StringBuilder _Content = new StringBuilder();

		public StringBuilder Content
		{
			get { return _Content; }
		}

		string _Name;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		Type _OwnerType;

		public Type OwnerType
		{
			get { return _OwnerType; }
			set { _OwnerType = value; }
		}

		Type _ReturnType;

		public Type ReturnType
		{
			get { return _ReturnType; }
			set { _ReturnType = value; }
		}

		List<Type> _Parameters = new List<Type>();

		public List<Type> Parameters
		{
			get { return _Parameters; }
			set { _Parameters = value; }
		}

		public Member AddParam(params Type[] parameters)
		{
			this.Parameters.AddRange(parameters);
			return this;
		}

		public override bool Equals(object obj)
		{
			if (obj is Member)
			{
				Member other = obj as Member;
				if (other.Name == this.Name &&
					//other.ReturnType == this.ReturnType &&
					other.Parameters.Count == this.Parameters.Count)
				{
					for (int i = 0; i < this.Parameters.Count; i++)
						if (other.Parameters[i] != this.Parameters[i])
							return false;
					return true;
				}
				else
					return false;
			}
			else return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			int value = this.Name.GetHashCode();
				//^ (this.ReturnType ?? typeof(object)).GetHashCode();
			foreach (Type param in this.Parameters)
				value ^= param.GetHashCode();
			return value;
		}
	}
}
