using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag
{
	public class Translated : ICollection<Translated.Translation>, IEnumerable<KeyValuePair<string, string>>//ICollection<string>
	{
		Dictionary<string, string> _Translations = new Dictionary<string, string>();

		public Dictionary<string, string> Translations
		{
			get
			{
				return _Translations;
			}
		}

		public struct Translation
		{
			#region Language

			private string _Language;

			public string Language
			{
				get { return _Language; }
				set
				{
					if (_Language != value)
					{
						_Language = value;
					}
				}
			}
			#endregion Language

			#region Value

			private string _Value;

			public string Value
			{
				get { return _Value; }
				set
				{
					if (_Value != value)
					{
						_Value = value;
					}
				}
			}
			#endregion Value

			public static implicit operator KeyValuePair<string, string>(Translation trans)
			{
				return new KeyValuePair<string, string>(trans.Language, trans.Value);
			}

			public static implicit operator Translation(KeyValuePair<string, string> pair)
			{
				return new Translation() { Language = pair.Key, Value = pair.Value };
			}

			public static implicit operator string(Translation trans)
			{
				return trans.Value;
			}
		}

		public static implicit operator string(Translated trans)
		{
			var cult = System.Globalization.CultureInfo.CurrentUICulture;
			return trans[cult.TwoLetterISOLanguageName];
		}

		public static explicit operator Translated(string text)
		{
			var cult = System.Globalization.CultureInfo.CurrentUICulture;
			var trans = new Translated();
			trans[cult.TwoLetterISOLanguageName] = text;
			return trans;
		}

		public string this[string language]
		{
			get
			{
				try
				{
					if (this.Translations.ContainsKey(language))
						return this.Translations[language];
					else
						return this.Translations[this.Languages.Find(l => l != null && (l.ToLower().Contains(language.ToLower()) || language.ToLower().Contains(l.ToLower())))];
				}
				catch
				{
				}
				foreach (string key in this.Translations.Keys)
					return this.Translations[key];
				return null;
			}
			set
			{
				try
				{
					if (this.Translations.ContainsKey(language))
						this.Translations[language] = value;
					else
						this.Translations[this.Languages.Find(l => l != null && (l.ToLower().Contains(language.ToLower()) || language.ToLower().Contains(l.ToLower())))] = value;
				}
				catch
				{
				}
			}
		}

		#region Languages

		private List<string> _Languages = new List<string>();

		public List<string> Languages
		{
			get
			{
				if (_Languages.Count != this.Count)
				{
					_Languages.Clear();
					_Languages.AddRange(this.Translations.Keys);
				}
				return _Languages;
			}
		}
		#endregion Languages

		#region ICollection<string> Members

		public void Add(string item)
		{
			var cult = System.Globalization.CultureInfo.CurrentUICulture;

			this.Translations.Add(cult.TwoLetterISOLanguageName, item);
		}

		public void Clear()
		{
			this.Translations.Clear();
		}

		public bool Contains(string item)
		{
			return this.Translations.ContainsValue(item);
		}

		public void CopyTo(string[] array, int arrayIndex)
		{
			this.Translations.Values.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return this.Translations.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(string item)
		{
			bool removed = false;
			while (this.Contains(item))
			{
				foreach (var key in this.Translations.Keys)
					if (this.Translations[key] == item)
					{
						removed |= this.Translations.Remove(key);
						break;
					}
			}
			return removed;
		}

		#endregion

		#region ICollection<KeyValuePair<string,string>> Members

		public void Add(KeyValuePair<string, string> item)
		{
			((ICollection<KeyValuePair<string, string>>)this.Translations).Add(item);
		}

		public bool Contains(KeyValuePair<string, string> item)
		{
			return ((ICollection<KeyValuePair<string, string>>)this.Translations).Contains(item);
		}

		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, string>>)this.Translations).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, string> item)
		{
			return ((ICollection<KeyValuePair<string, string>>)this.Translations).Remove(item);
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,string>> Members

		IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
		{
			return ((ICollection<KeyValuePair<string, string>>)this.Translations).GetEnumerator();
		}

		#endregion

		#region ICollection<Translation> Members

		public void Add(Translated.Translation item)
		{
			this.Translations.Add(item.Language, item.Value);
		}

		public bool Contains(Translated.Translation item)
		{
			return this.Translations.ContainsKey(item.Language);
		}

		public void CopyTo(Translated.Translation[] array, int arrayIndex)
		{
			int i = 0;
			foreach (Translated.Translation translation in this)
				array[arrayIndex + i++] = translation;
		}

		public bool Remove(Translated.Translation item)
		{
			return this.Remove(item.Value) || this.Translations.Remove(item.Language);
		}

		#endregion

		#region IEnumerable<Translation> Members

		public IEnumerator<Translated.Translation> GetEnumerator()
		{
			return this.Iterate().GetEnumerator();
		}

		protected IEnumerable<Translation> Iterate()
		{
			foreach (var pair in (IEnumerable<KeyValuePair<string, string>>)this.Translations)
			{
				yield return pair;
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}
