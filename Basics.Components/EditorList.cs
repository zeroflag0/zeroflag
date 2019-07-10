using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public interface IEditorList
	{
		string ItemName { get; set; }
		Command EditCommand { get; }
		Command NewCommand { get; }
		Command DuplicateCommand { get; }
		Command DeleteCommand { get; }
	}

	public class EditorList<TModel, TWrapper>
		: SavableList<TWrapper>
		where TModel : Content<TModel>, new()
		where TWrapper : ContentView<TModel, TWrapper>, new()
	{
		public EditorList()
		{
			//this.CollectionChanged += EditorList_CollectionChanged;
		}

		public EditorList(string contentType, string itemName, string itemNamePlural = null)
			: this()
		{
			this.ItemName = itemName;
			this.ItemNamePlural = itemNamePlural ?? (ItemName + "s");
			this.ContentType = contentType;
		}

		public override IEnumerator<TWrapper> GetEnumerator()
		{
			return base.GetEnumerator();
		}

		void EditorList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			//this.Log("EditorList.CollectionChanged(" + this.Count + ")");
		}

		protected override void SavableFields(SavableFields fields)
		{
			base.SavableFields(fields);
			//fields.Add("ItemName", data => ItemName, value => ItemName = value);
			//fields.Add("ItemNamePlural", data => ItemNamePlural, value => ItemNamePlural = value);
		}

		public override void ClearStorageIds()
		{
			base.ClearStorageIds();
		}

		private string _ItemName;
		/// <summary>
		/// the name of a item to be used in dialogs
		/// </summary>
		public string ItemName
		{
			get { return _ItemName; }
			set
			{
				if (_ItemName != value)
				{
					_ItemName = value;
					this.OnPropertyChanged("ItemName");
				}
			}
		}


		private string _ItemNamePlural;

		/// <summary>
		/// the name in plural for items to be used in the ui
		/// </summary>
		public string ItemNamePlural
		{
			get { return _ItemNamePlural; }
			set
			{
				if (_ItemNamePlural != value)
				{
					_ItemNamePlural = value;
					this.OnPropertyChanged("ItemNamePlural");
				}
			}
		}


		private string _ContentType;

		/// <summary>
		/// the content type
		/// </summary>
		public string ContentType
		{
			get { return _ContentType; }
			set
			{
				if (_ContentType != value)
				{
					_ContentType = value;
					this.OnPropertyChanged("ContentType");
				}
			}
		}


		public Command EditCommand
		{
			get
			{
				return new Command(p => this.Busy(() =>
				{
					if (this.SelectedItem != null && this.SelectedItem.ShowCommand != null)
						this.SelectedItem.ShowCommand.Execute(p);
				}));
			}
		}

		public Command NewCommand
		{
			get
			{
				return new Command(p => this.Busy(() =>
				{
					TWrapper instance = new TWrapper();
					instance.CreateModel();
					this.SelectedItem = instance;
					this.Add(instance);
					this.SelectedItem.ShowCommand.Execute(p);
					instance.InitializeNew();
				}));
			}
		}

		public Command DuplicateCommand
		{
			get
			{
				return new Command(p => this.Busy(() =>
				{
					System.Windows.MessageBox.Show("Not implemented!");
					return;
					throw new NotImplementedException();
#if HIDDEN_CODE
					if (this.SelectedItem == null)
						return;
					TWrapper old = this.SelectedItem;
					TWrapper item = old.Duplicate<TModel, TWrapper>();
					//	= new T();
					//item.CreateModel();
					//System.Data.DataSet data = new System.Data.DataSet();
					//int id = old.Save(data);
					//item.Load(data, id);
					//item.StorageId = null;

					this.Add(item);
					this.SelectedItem = item;
					this.SelectedItem.ShowCommand.Execute(p);
#endif
				}));
			}
		}

		public Command DeleteCommand
		{
			get
			{
				return new Command(p =>
				{
					if (this.SelectedItem != null)
					{
						if (System.Windows.MessageBox.Show("Are you sure that you want to permanently delete the " + this.ItemName + "?", "Really delete?", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning, System.Windows.MessageBoxResult.Cancel) != System.Windows.MessageBoxResult.OK)
							return;

						TWrapper item = this.SelectedItem;
						this.Remove(item);
						this.SelectedItem = null;
						item.Dispose();
					}
				});
			}
		}
	}
}
