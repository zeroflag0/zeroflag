using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Basics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;

public static class Extensions
{
	public static TimeSpan TimeOffset;
	[DebuggerStepThrough]
	static Extensions()
	{
		TimeOffset = DateTime.Now - DateTime.UtcNow;
		IsParallel = true;
	}

	public static DateTime Now
	{
		get
		{
			DateTime time = DateTime.UtcNow;
			time += TimeOffset;
			return time;
		}
	}

	[DebuggerStepThrough]
	public static bool isempty(this string text)
	{
		return string.IsNullOrEmpty(text);
	}

	public static string FillVariables(this string command, Action<Dictionary<string, Func<string>>> createConversions = null, DateTime? time = null)
	{
		time = time ?? DateTime.Now;
		command = command ?? "";
		Dictionary<string, Func<string>> conversions = new Dictionary<string, Func<string>>();

		//conversions.Add("file", () => "" + filepath.FileName + "");
		//conversions.Add("filepath", () => "" + filepath.FullPath + "");
		//conversions.Add("directory", () => "" + filepath.Directory + "");
		conversions.Add("fulltimestamp", () => "" + time.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") + "");
		conversions.Add("timestamp", () => "" + time.Value.ToString("yyyyMMddHHmmss") + "");
		conversions.Add("year", () => "" + time.Value.ToString("yyyy") + "");
		conversions.Add("month", () => "" + time.Value.ToString("MM") + "");
		conversions.Add("day", () => "" + time.Value.ToString("dd") + "");
		conversions.Add("hour", () => "" + time.Value.ToString("HH") + "");
		conversions.Add("minute", () => "" + time.Value.ToString("mm") + "");
		conversions.Add("second", () => "" + time.Value.ToString("ss") + "");
		conversions.Add("millisecond", () => "" + time.Value.ToString("fff") + "");

		if (createConversions != null)
		{
			createConversions(conversions);
		}

		int ix = 0;
		do
		{
			ix = command.IndexOf("$(", ix);
			if (ix < 0)
				break;
			int ie = command.IndexOf(")", ix);

			string pname = command.Substring(ix + 2, ie - ix - 2).ToLower();
			string value = null;
			//this.CreateConversions(conversions);
			if (conversions.ContainsKey(pname))
			{
				value = conversions[pname]();
			}

			if (value != null)
			{
				command = command.Remove(ix, ie - ix + 1);
				command = command.Insert(ix, value);
			}
			else
			{
				// skip unknown placeholder
				ix++;
			}

		} while (ix >= 0);
		return command;
	}

	[DebuggerStepThrough]
	public static int IndexOf<TItem>(this TItem[] array, TItem value)
	{
		return Array.IndexOf<TItem>(array, value);
	}
	[DebuggerStepThrough]
	public static int LastIndexOf<TItem>(this TItem[] array, TItem value)
	{
		return Array.LastIndexOf<TItem>(array, value);
	}

	[DebuggerStepThrough]
	public static ICollection<TItem> AddRange<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> items)
	{
		if (items != null)
			foreach (TItem item in items)
			{
				collection.Add(item);
			}
		return collection;
	}

	[DebuggerStepThrough]
	public static IEnumerable<T> enumerator<T>(this object any, params T[] values) { return values; }
	[DebuggerStepThrough]
	public static IEnumerable<T> enumerator<T>(this T self) { yield return self; }
	[DebuggerStepThrough]
	public static IEnumerable<T> concat<T>(this T self, params T[] values) { return self.concat<T>((IEnumerable<T>)values); }
	[DebuggerStepThrough]
	public static IEnumerable<T> concat<T>(this T self, IEnumerable<T> values)
	{
		yield return self;
		foreach (T value in values)
			yield return value;
	}
	public static IEnumerable<T> concat<T>(this T self, IEnumerator<T> values)
	{
		yield return self;
		while (values.MoveNext())
			yield return values.Current;
	}


	[DebuggerStepThrough]
	public static IEnumerable<T> First<T>(this IEnumerable<T> list, int number)
	{
		var enu = list.GetEnumerator();
		enu.MoveNext();
		yield return enu.Current;
		yield break;
	}

	[DebuggerStepThrough]
	public static TValue get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
	{
		if (dict.ContainsKey(key))
			return dict[key];
		return default(TValue);
	}

	public static ICollection<T> Dispose<T>(this ICollection<T> collection)
		where T : IDisposable
	{
		foreach (T item in collection)
		{
			item.Dispose();
		}
		collection.Clear();
		return collection;
	}

	public static Dictionary<TKey, TValue> Dispose<TKey, TValue>(this Dictionary<TKey, TValue> collection)
		where TValue : IDisposable
	{
		foreach (TValue item in collection.Values)
		{
			item.Dispose();
		}
		collection.Clear();
		return collection;
	}

	public static int GetHashCode(this object self, params object[] values)
	{
		unchecked
		{
			// Choose large primes to avoid hashing collisions
			const int HashingBase = (int)2166136261;
			const int HashingMultiplier = 16777619;

			int hash = HashingBase;
			foreach (object value in values)
			{
				if (value != null)
					hash = (hash * HashingMultiplier) ^ value.GetHashCode();
			}
			return hash;
		}
	}

	/// <summary>
	/// in release builds doesn't do anything, in debug-builds attempts to create a array so the debugger has something to show...
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	[DebuggerStepThrough]
	public static IEnumerable<T> debug<T>(this IEnumerable<T> list)
	{
#if DEBUG
		if (list == null)
			return null;
		return list.ToArray();
#else
		return list;
#endif
	}

	[DebuggerStepThrough]
	public static T[] toarray<T>(this IEnumerable<T> list)
	{
		if (list == null)
			return null;
		if (list.GetType().IsArray)
		{
			try
			{
				return (T[])list;
			}
			catch { }
		}
		for (int i = 0; ; i++)
		{
			try
			{
				return list.ToArray();
			}
			catch (Exception)
			{
				if (i >= 1000)
					throw;
				System.Threading.Thread.Sleep(1);
			}
		}
	}

	[DebuggerStepThrough]
	public static T IgnoreError<T>(this object self, Func<T> action, T fallback = default(T), Action<Exception> exchandler = null)
	{
		try
		{
			return action();
		}
		catch (Exception exc)
		{
			if (exchandler != null)
				exchandler(exc);
			else
			{
				Console.WriteLine(exc);
			}
			return fallback;
		}
	}
	[DebuggerStepThrough]
	public static void IgnoreError(this object self, Action action, Action<Exception> exchandler = null)
	{
		try
		{
			action();
		}
		catch (Exception exc)
		{
			if (exchandler != null)
				exchandler(exc);
			else
			{
				Console.WriteLine(exc);
			}
		}
	}

	public static void Clear(this MemoryStream source)
	{
		byte[] buffer = source.GetBuffer();
		Array.Clear(buffer, 0, buffer.Length);
		source.Position = 0;
		source.SetLength(0);
	}

	public static string ToString<T>(this IEnumerable<T> list, char separator = ',')
	{
		StringBuilder sb = new StringBuilder();
		foreach (T item in list)
		{
			sb.Append(item).Append(separator);
		}
		return sb.ToString().TrimEnd(separator);
	}


	public static string ToBin(this byte value, string separator = null, int? length = null, bool reverse = true)
	{
		return new byte[] { value }.ToBin(separator, length, reverse);
	}

	public static string ToBin(this ulong value, string separator = null, int? length = null, bool reverse = true)
	{
		return BitConverter.GetBytes(value).ToBin(separator, length, reverse);
	}

	public static string ToBin(this long value, string separator = null, int? length = null, bool reverse = true)
	{
		return BitConverter.GetBytes(value).ToBin(separator, length, reverse);
	}

	public static string ToBin(this int value, string separator = null, int? length = null, bool reverse = true)
	{
		return BitConverter.GetBytes(value).ToBin(separator, length, reverse);
	}

	public static string ToBin(this short value, string separator = null, int? length = null, bool reverse = true)
	{
		return BitConverter.GetBytes(value).ToBin(separator, length, reverse);
	}

	public static string ToBin(this ushort value, string separator = null, int? length = null, bool reverse = true)
	{
		return BitConverter.GetBytes(value).ToBin(separator, length, reverse);
	}

	public static string ToBin(this uint value, string separator = null, int? length = null, bool reverse = true)
	{
		return BitConverter.GetBytes(value).ToBin(separator, length, reverse);
	}
	public static string ToBin(this byte[] enu, string separator = null, int? length = null, bool reverse = true)
	{
		int i = 0;
		int end = length ?? enu.Length;
		separator = separator ?? " ";
		int step = 1;

		if (reverse)
		{
			i = end - 1;//enu.Length - 1;//
			end = -1;//enu.Length - end - 1;//
			step = -1;
		}

		StringBuilder result = new StringBuilder();
		for (; i != end; i += step)
		{
			if (i < 0 || i >= enu.Length)
				continue;

			result.Append(Convert.ToString(enu[i], 2).PadLeft(8, '0')).Append(separator);
		}
		result.Remove(result.Length - separator.Length, separator.Length);

		return result.ToString();
	}
	public static string ToHex(this byte value, string separator = "", int? length = null, bool reverse = false)
	{
		return new byte[] { value }.ToHex(separator, length, reverse);
	}

	public static string ToHex(this ulong value, string separator = "", int? length = null, bool reverse = false)
	{
		return BitConverter.GetBytes(value).ToHex(separator, length, reverse);
	}

	public static string ToHex(this int value, string separator = "", int? length = null, bool reverse = false)
	{
		return BitConverter.GetBytes(value).ToHex(separator, length, reverse);
	}

	public static string ToHex(this ushort value, string separator = "", int? length = null, bool reverse = false)
	{
		return BitConverter.GetBytes(value).ToHex(separator, length, reverse);
	}

	public static string ToHex(this uint value, string separator = "", int? length = null, bool reverse = false)
	{
		return BitConverter.GetBytes(value).ToHex(separator, length, reverse);
	}
	public static string ToHex(this byte[] enu, string separator = "", int? length = null, bool reverse = false)
	{
		int i = 0;
		int end = length ?? enu.Length;
		int step = 1;

		if (reverse)
		{
			i = end - 1;//enu.Length - 1;//
			end = -1;//enu.Length - end - 1;//
			step = -1;
		}

		StringBuilder result = new StringBuilder();
		for (; i != end; i += step)
		{
			if (i < 0 || i >= enu.Length)
				continue;

			result.Append(enu[i].ToString("X").PadLeft(2, '0')).Append(separator);
		}
		result.Remove(result.Length - separator.Length, separator.Length);

		return result.ToString();
	}

	public static string ToHex(this IEnumerable<byte> enu, uint length, bool reverse = false)
	{
		return enu.ToHex("", (int)length, reverse);
	}
	public static string ToHex(this IEnumerable<byte> enu, string separator, uint length, bool reverse = false)
	{
		return enu.ToHex(separator, (int)length, reverse);
	}
	public static string ToHex(this IEnumerable<byte> enu, string separator = "", int? length = null, bool reverse = false)
	{
		List<byte> list = (enu as List<byte>) ?? new List<byte>(enu);

		if (reverse)
			list.Reverse();
		if (length.HasValue)
			if (reverse)
				while (list.Count > length.Value)
					list.RemoveAt(0);
			else
				while (list.Count > length.Value)
					list.RemoveAt(list.Count - 1);

		int valuelength = sizeof(byte);
		StringBuilder result = new StringBuilder();

		foreach (byte item in list)
		{
			result.Append(item.ToString("X").PadLeft(valuelength * 2, '0')).Append(separator);
		}

		if (result.Length > 0)
			result.Remove(result.Length - separator.Length, separator.Length);

		return result.ToString();
	}

	public static string ToString<T>(this IEnumerable<T> items, string separator = ",")
	{
		if (items == null || items.Count() == 0)
			return "";
		StringBuilder sb = new StringBuilder();
		foreach (var item in items)
		{
			sb.Append(item).Append(separator);
		}
		return sb.ToString();
	}

	public static string ToStringLines<T>(this IEnumerable<T> items, int indent = 0, string indentString = ".",
		Func<T, int, string, string> decorateItem = null)
	{
		if (items == null || items.Count() == 0)
			return "";
		if (indent > 10)
			return "...";
		decorateItem = decorateItem ?? ((T item, int ind, string indstr) => item + "");
		char[] indentChars = indentString.ToCharArray();
		StringBuilder sb = new StringBuilder();
		foreach (var item in items)
		{
			sb.Append(indentChars, 0, indent).Append(decorateItem(item, indent, indentString)).AppendLine();
		}
		return sb.ToString();
	}

	public static string ToTable(this DataTable table, string columnSeparator = ";", string rowSeparator = "\n", int cellpadding = 0)
	{
		StringBuilder result = new StringBuilder();
		foreach (DataColumn col in table.Columns)
		{
			result.Append(col.ColumnName.PadRight(cellpadding)).Append(columnSeparator);
		}
		if (result.Length > columnSeparator.Length)
			result.Remove(result.Length - columnSeparator.Length, columnSeparator.Length);
		result.Append(rowSeparator);

		foreach (DataRow row in table.Rows)
		{
			foreach (DataColumn col in table.Columns)
			{
				result.Append((row[col] + "").PadRight(cellpadding)).Append(columnSeparator);
			}
			result.Remove(result.Length - columnSeparator.Length, columnSeparator.Length);
			result.Append(rowSeparator);
		}
		return result.ToString();
	}

	public static IEnumerable<T> Segment<T>(this IList<T> list, int start, int length)
	{
		return list.Range(start, length);
	}
	public static IEnumerable<T> Range<T>(this IList<T> list, int start, int length)
	{
		int end = list.Count;
		for (int i = start; i < start + length && i < end; i++)
		{
			yield return list[i];
		}
	}

	public static IEnumerable<T> Segment<T>(this T[] list, int start, int length)
	{
		return list.Range(start, length);
	}
	public static IEnumerable<T> Range<T>(this T[] list, int start, int length)
	{
		int end = list.Length;
		for (int i = start; i < start + length && i < end; i++)
		{
			yield return list[i];
		}
	}

	public static void ForeachSync<T>(this IEnumerable<T> list, Action<T> action, Action finish = null)
	{
		try
		{
			foreach (T item in list)
			{
				action(item);
			}
		}
		finally
		{
			if (finish != null) finish();
		}

	}

	public static bool IsParallel { get; set; }
	public static void ForeachWait<T>(this IEnumerable<T> list, Action<T> action, Action finish = null)
	{
		if (IsParallel && !Debugger.IsAttached)
			using (ManualResetEvent wait = new ManualResetEvent(false))
			{
				Action waitfinish = () =>
				{
					if (finish != null)
						finish();
					wait.Set();
				};

				list.Foreach(action, waitfinish);

				wait.WaitOne();
			}
		else
		{
			foreach (var item in list)
			{
				action(item);
			}
			if (finish != null)
				finish();
		}
	}

	public static void Foreach<T>(this IEnumerable<T> list, Action<T> action, Action finish = null)
	{
#if SINGLETHREAD
		try
		{
			foreach (T item in list)
			{
				action(item);
			}
		}
		finally
		{
			if (finish != null) finish();
		}
#elif CUSTOM_TASKS
		TaskGroup group = new TaskGroup();

		//SpinWait wait = new SpinWait();
		//Action userfinish = finish;
		//finish = () =>
		//	{
		//		wait.SpinOnce();
		//		if (userfinish != null)
		//			userfinish();
		//	};
		if (finish != null)
			group.FinishAction += t => finish();
		foreach (T item in list)
		{
			group.Tasks.Add(CreateTask(item, action));
		}
		group.RunAsync();
#else
		System.Threading.Tasks.Parallel.ForEach(list, action);
		if (finish != null)
			finish();
#endif
	}

	private static TaskBase CreateTask<T>(T item, Action<T> action)
	{
		return new Task(() => action(item));
	}

	public static void Background(this object any, Action action, Action finish = null)
	{
#if SINGLETHREAD
		try
		{
			action();
		}
		finally
		{
			if (finish != null) finish();
		}
#else
		new Task(action, finish).RunAsync();
#endif
	}

	public static void Invoke(this object any, Action action, Action<Exception> exceptionHandler = null)
	{
		any.InvokeInternal(action, exceptionHandler, InvokeHandler);
	}
	public static void InvokeSynchronized(this object any, Action action, Action<Exception> exceptionHandler = null)
	{
		any.InvokeInternal(action, exceptionHandler, InvokeSynchronizedHandler);
	}

	private static void InvokeInternal(this object any, Action action, Action<Exception> exceptionHandler = null, Action<Action> invoke = null)
	{
		Exception outer = new Exception("Invoke Exception");
		Action act = () =>
		{
			try
			{
				action();
			}
			catch (Exception exc)
			{
				Exception exc1 = new Exception(outer.ToString(), exc);
				if (exceptionHandler != null)
				{
					exceptionHandler(exc1);
				}
				else
					throw exc1;
			}
		};
#if SINGLETHREAD
		bool requiresInvoke = false;
#else
		bool requiresInvoke = true;

		if (System.Threading.Thread.CurrentThread == InvokeThread)
			requiresInvoke = false;
#endif
		if (!requiresInvoke)
			act();
		else
		{
			if (invoke != null)
				invoke(act);
			else
			{
#if DEBUG
				Logger.Instance.WriteLog("WARNING: InvokeHandler is not set!");
#endif
				act();
			}
		}
	}

	class UsingCleanup : IDisposable
	{
		public Action Shutdown { get; set; }

		public void Dispose()
		{
			Action shutdown = this.Shutdown;
			this.Shutdown = null;
			if (shutdown != null)
			{
				shutdown();
			}
		}
	}

	public static IDisposable Init()
	{
		InvokeThread = System.Threading.Thread.CurrentThread;
		InvokeHandler = a => a();
		InvokeSynchronizedHandler = a => a();
		return new UsingCleanup { Shutdown = Shutdown };
	}

	public static void Init(this Base self)
	{
		InvokeThread = System.Threading.Thread.CurrentThread;
		InvokeHandler = a => a();
		InvokeSynchronizedHandler = a => a();
		self.Disposing += Shutdown;
	}
#if WPF
	public static void Init(this System.Windows.Application app)
	{
		Logger.Instance.WriteLog(app, 0, "UI THREAD!");
		app.DispatcherUnhandledException += app_DispatcherUnhandledException;
		InvokeThread = app.Dispatcher.Thread;
		InvokeHandler = a => app.Dispatcher.BeginInvoke(a);
		InvokeSynchronizedHandler = a => app.Dispatcher.Invoke(a);

		app.Exit += (object sender, System.Windows.ExitEventArgs e) =>
			{
				Extensions.Shutdown();
				app.DispatcherUnhandledException -= app_DispatcherUnhandledException;
			};
	}

	static void app_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
	{
		e.Handled = true;
		Logger.Instance.WriteLog(sender, 0);
		Logger.Instance.WriteLog(sender, 0, "UNHANDLED EXCEPTION" + Environment.NewLine + e.Exception);
	}

	public static bool IsDesignMode(this System.Windows.DependencyObject self)
	{
		return System.ComponentModel.DesignerProperties.GetIsInDesignMode(self);
	}

	public static bool IsDesignMode(this object self)
	{
		if (self is System.Windows.DependencyObject)
			return System.ComponentModel.DesignerProperties.GetIsInDesignMode((System.Windows.DependencyObject)self);

		if (System.Windows.Application.Current == null || System.Windows.Application.Current.MainWindow == null)
			return true;

		return System.ComponentModel.DesignerProperties.GetIsInDesignMode(System.Windows.Application.Current.MainWindow);
	}
#endif
	public static void Shutdown()
	{
		Logger.Instance.IsRunning = false;
	}

	public static System.Threading.Thread InvokeThread { get; set; }
	public static Action<Action> InvokeHandler { get; set; }
	public static Action<Action> InvokeSynchronizedHandler { get; set; }


	/// <summary>
	/// Writes a DataTable into a csv file via the specified TextWriter
	/// </summary>
	public static void WriteCSV(this DataTable sourceTable, TextWriter writer, bool includeHeaders, string separator = null)
	{
		if (sourceTable != null)
		{
			if (separator == null)
				separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

			if (includeHeaders)
			{
				List<string> headerValues = new List<string>();
				foreach (DataColumn column in sourceTable.Columns)
				{
					headerValues.Add(QuoteValue(column.ColumnName));
				}

				writer.WriteLine(String.Join(separator, headerValues.ToArray()));
			}

			string[] items = null;
			foreach (DataRow row in sourceTable.Rows)
			{
				items = row.ItemArray.Select(o => QuoteValue(o.ToString())).ToArray();
				writer.WriteLine(String.Join(separator, items));
			}
		}

		writer.Flush();
	}

	private static string QuoteValue(string value)
	{
		return String.Concat("\"", value.Replace("\"", "\"\""), "\"");
	}

#if EXCEL

	public static string WriteExcel(this List<DataTable> sourceTables, string filename, string previousFileName, string tracepath = null, bool useColorsForErrorsAndWarnings = false, string templateFile = null, string wiresharkPath = "")
	{
		return sourceTables.Select(o => new DataTableWrapper(o)).ToList().WriteExcel(filename, previousFileName, tracepath, useColorsForErrorsAndWarnings, templateFile, wiresharkPath);
	}

	/// <summary>
	/// Writes a DataTable into a Excel (xlsx) file
	/// </summary>
	public static string WriteExcel(this List<DataTableWrapper> sourceTables, string filename, string previousFileName = null, string tracepath = null, bool useColorsForErrorsAndWarnings = false, string templateFile = null, string wiresharkPath = "")
	{
		if (sourceTables != null && sourceTables.Count > 0)
		{
			Microsoft.Office.Interop.Excel.Application excel;
			Microsoft.Office.Interop.Excel.Workbook excelworkBook = null;
			Microsoft.Office.Interop.Excel.Workbooks excelworkBooks = null;
			Microsoft.Office.Interop.Excel.Worksheet excelSheet = null;
			Microsoft.Office.Interop.Excel.Sheets excelSheets = null;
			CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;

			//Start Excel and get Application object
			excel = new Microsoft.Office.Interop.Excel.Application();
			try
			{
				//Make excel invisible
				excel.Visible = false;
				excel.DisplayAlerts = false;

				excelworkBooks = excel.Workbooks;

				Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));

				//Open excel template
				if (templateFile != null)
				{
					File.Copy(templateFile, filename, true);
					Thread.Sleep(100);
					excelworkBook = excelworkBooks.Open(filename);
					//excelworkBook = excelworkBooks.Open(templateFile);
					excelSheets = excelworkBook.Worksheets;
				}
				//create a new Workbook
				else
				{
					excelworkBook = excelworkBooks.Add(Type.Missing);
					excelSheets = excelworkBook.Worksheets;
					while (excelSheets.Count > 1)
						((Microsoft.Office.Interop.Excel.Worksheet)excelSheets[2]).Delete();
				}

				excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;


				//Add data sheets
				List<string> usedSheetNames = new List<string>();
				for (int sheetNumber = 0; sheetNumber < sourceTables.Count; sheetNumber++)
				{
					//Create worksheet
					if (templateFile != null)
					{
						if (sheetNumber >= excelSheets.Count)
							excelSheets[1].Copy(Type.Missing, excelSheets[sheetNumber + 1]);

						excelSheet = excelSheets[sheetNumber + 2];
					}
					else
						excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelSheets.Add(Type.Missing, excelSheets[sheetNumber + 1]);

					DataTable dt = sourceTables[sheetNumber].DataTable;

					//Avoid the case, that two sheets may have the same name
					string tableName = dt.TableName;
					int counter = 0;
					while (usedSheetNames.Contains(tableName))
						tableName = tableName.Substring(0, tableName.Length - 1 - (++counter).ToString().Length) + "_" + counter.ToString();
					excelSheet.Name = tableName;
					usedSheetNames.Add(tableName);

					//Init data array
					var data = new object[dt.Rows.Count + 1, dt.Columns.Count];

					//Header
					for (int col = 0; col < dt.Columns.Count; col++)
					{
						data[0, col] = dt.Columns[col].ColumnName;

						//If the column has a custom format, use it
						if (sourceTables[sheetNumber].ColumnFormats != null && sourceTables[sheetNumber].ColumnFormats.ContainsKey(col))
							excelSheet.Cells[1, col + 1].EntireColumn.NumberFormat = sourceTables[sheetNumber].ColumnFormats[col];
						//Otherwise use default according to datatype
						else
						{

							//Apply format
							if (dt.Columns[col].DataType.Equals(typeof(string)) ||
								dt.Columns[col].DataType.Equals(typeof(TimeSpan)) ||
								dt.Columns[col].DataType.Equals(typeof(DateTime)))
							{
								excelSheet.Cells[1, col + 1].EntireColumn.NumberFormat = "@";
							}
							else if (dt.Columns[col].DataType.Equals(typeof(ushort)))
							{
								excelSheet.Cells[1, col + 1].EntireColumn.NumberFormat = "#####";
							}
							else if (dt.Columns[col].DataType.Equals(typeof(double)))
							{
								string pattern = string.Format("#{1}##0{0}0000", cultureInfo.NumberFormat.CurrencyDecimalSeparator, cultureInfo.NumberFormat.CurrencyGroupSeparator);
								excelSheet.Cells[1, col + 1].EntireColumn.NumberFormat = pattern;
							}
							//excelSheet.Cells[1, col + 1].EntireColumn.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
						}
					}

					//Data
					for (int row = 0; row < dt.Rows.Count; row++)
					{
						//If the row has a custom format, use it
						if (sourceTables[sheetNumber].RowFormats != null && sourceTables[sheetNumber].RowFormats.ContainsKey(row))
							excelSheet.Cells[row + 2, 1].EntireRow.NumberFormat = sourceTables[sheetNumber].RowFormats[row];

						for (int col = 0; col < dt.Columns.Count; col++)
						{
							if (dt.Rows[row][col] == DBNull.Value || dt.Rows[row][col] == null)
								continue;

							try
							{
								if (dt.Columns[col].DataType.Equals(typeof(TimeSpan)))
									data[row + 1, col] = dt.Rows[row].Field<TimeSpan>(col).ToString(@"hh\:mm\:ss\.fffffff");
								else if (dt.Columns[col].DataType.Equals(typeof(ushort)))
									data[row + 1, col] = dt.Rows[row].Field<ushort>(col).ToString("X");
								else if (dt.Columns[col].DataType.Equals(typeof(System.Single)))
									data[row + 1, col] = dt.Rows[row].Field<System.Single>(col);
								else if (dt.Columns[col].DataType.Equals(typeof(System.Double)))
									data[row + 1, col] = dt.Rows[row].Field<System.Double>(col);
								else if (dt.Columns[col].DataType.Equals(typeof(float)))
									data[row + 1, col] = dt.Rows[row].Field<float>(col);
								else if (dt.Columns[col].DataType.Equals(typeof(System.UInt16)))
									data[row + 1, col] = dt.Rows[row].Field<System.UInt16>(col);
								else if (dt.Columns[col].DataType.Equals(typeof(System.UInt32)))
									data[row + 1, col] = dt.Rows[row].Field<System.UInt32>(col);
								else if (dt.Columns[col].DataType.Equals(typeof(System.UInt64)))
									data[row + 1, col] = dt.Rows[row].Field<System.UInt64>(col);
								else if (dt.Columns[col].DataType.Equals(typeof(System.Int16)))
									data[row + 1, col] = dt.Rows[row].Field<System.Int16>(col);
								else if (dt.Columns[col].DataType.Equals(typeof(System.Int32)))
									data[row + 1, col] = dt.Rows[row].Field<System.Int32>(col);
								else if (dt.Columns[col].DataType.Equals(typeof(System.Int64)))
									data[row + 1, col] = dt.Rows[row].Field<System.Int64>(col);
								//else if (dt.Columns[col].DataType.Equals(typeof(ushort)))
								//	data[row + 1, col] = dt.Rows[row].Field<DateTime>(col).ToString("HH:mm:ss.ffffff");
								else
									data[row + 1, col] = dt.Rows[row][col].ToString();
							}
							catch (Exception)
							{
								try
								{
									data[row + 1, col] = dt.Rows[row][col].ToString();
								}
								catch (Exception)
								{
								}
							}
						}

						//Break if too many rows
						if (templateFile != null && row > 64998)
						{
							data[row + 1, 1] = "TOO MANY ROWS! EXPORT WILL BE STOPPED AT THIS ROW!";
							break;
						}
					}

					//Write data to excel
					Microsoft.Office.Interop.Excel.Range sheetData = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[dt.Rows.Count + 1, dt.Columns.Count]];
					sheetData.Value2 = data;

					//Hyperlinks
					if (templateFile != null)
					{
						for (int col = 0; col < dt.Columns.Count; col++)
						{
							if (dt.Columns[col].ColumnName == "Frame_Number")
							{
								Microsoft.Office.Interop.Excel.Hyperlinks links = excelSheet.Hyperlinks;
								Microsoft.Office.Interop.Excel.Range linkCell = null;

								for (int i = 2; i <= excelSheet.UsedRange.Rows.Count; i++)
								{
									linkCell = excelSheet.Cells[i, col + 1];
									links.Add(linkCell, "", Type.Missing, Type.Missing, linkCell.Value.ToString());
								}

								Marshal.ReleaseComObject(links);
								if (linkCell != null)
									Marshal.ReleaseComObject(linkCell);
								break;
							}
						}
					}

					//Append Additional Info if it exists
					if (dt.ExtendedProperties.Count > 0)
					{
						int i = 3;
						foreach (string key in dt.ExtendedProperties.Keys)
						{
							excelSheet.Cells[i, dt.Columns.Count + 2] = key;
							excelSheet.Cells[i++, dt.Columns.Count + 3] = dt.ExtendedProperties[key].ToString();
						}

						//Formatting
						excelSheet.Range[excelSheet.Cells[3, dt.Columns.Count + 2], excelSheet.Cells[i - 1, dt.Columns.Count + 3]].EntireColumn.AutoFit();
						excelSheet.Range[excelSheet.Cells[3, dt.Columns.Count + 2], excelSheet.Cells[i - 1, dt.Columns.Count + 2]].BorderAround(
							Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous,
							Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin
						);
						excelSheet.Range[excelSheet.Cells[3, dt.Columns.Count + 2], excelSheet.Cells[i - 1, dt.Columns.Count + 3]].BorderAround(
							Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous,
							Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium
						);

						excelSheet.Cells[2, dt.Columns.Count + 2] = "Extended Information:";
						excelSheet.Cells[2, dt.Columns.Count + 2].Font.Bold = true;
					}

					//Enable Filter
					excelSheet.get_Range("A1", Type.Missing).AutoFilter(1, Type.Missing, Microsoft.Office.Interop.Excel.XlAutoFilterOperator.xlAnd, Type.Missing, true);

					//Formatting
					excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[dt.Rows.Count + 1, dt.Columns.Count]].EntireColumn.AutoFit();
					excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1, dt.Columns.Count]].Interior.Color = System.Drawing.ColorTranslator.FromHtml("#b8cce4");

					//Colors for details
					if (useColorsForErrorsAndWarnings && dt.Columns.Count > 5 && dt.Columns[5].ColumnName == "Category")
					{
						string lastCategory = String.Empty;
						int rangeRowStart = -1;
						for (int i = 0; i <= dt.Rows.Count; i++)
						{
							string category = data[i, 5].ToString();

							if (lastCategory != category)
							{
								if (lastCategory == "Error")
									excelSheet.Range[excelSheet.Cells[rangeRowStart + 1, 1], excelSheet.Cells[i, dt.Columns.Count]].Interior.Color = 10066431;//5197823;
								else if (lastCategory == "Warning")
									excelSheet.Range[excelSheet.Cells[rangeRowStart + 1, 1], excelSheet.Cells[i, dt.Columns.Count]].Interior.Color = 10092543;
								else if (lastCategory == "Special")
									excelSheet.Range[excelSheet.Cells[rangeRowStart + 1, 1], excelSheet.Cells[i, dt.Columns.Count]].Interior.Color = 14536083;


								rangeRowStart = i;
							}

							lastCategory = category;
						}

						if (lastCategory == "Error")
							excelSheet.Range[excelSheet.Cells[rangeRowStart + 1, 1], excelSheet.Cells[dt.Rows.Count + 1, dt.Columns.Count]].Interior.Color = 10066431;//5197823;
						else if (lastCategory == "Warning")
							excelSheet.Range[excelSheet.Cells[rangeRowStart + 1, 1], excelSheet.Cells[dt.Rows.Count + 1, dt.Columns.Count]].Interior.Color = 10092543;
					}
				}

				//Remove empty sheets
				if (templateFile != null)
				{
					for (int sheetNumber = excelSheets.Count; sheetNumber > (sourceTables.Count + 1); sheetNumber--)
						excelSheets[sheetNumber].Delete();
				}


				//Add navigation sheet
				if (sourceTables.Count > 1) //IF LIMIT IS CHANGED, ALSO ADAPT IN LINE 441
				{
					excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.Sheets[1];//(Microsoft.Office.Interop.Excel.Worksheet)excelSheets.Add((Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.Sheets[1]);
					excelSheet.Name = "NAVIGATION";


					//Link to tracefile
					Microsoft.Office.Interop.Excel.Hyperlinks links2 = excelSheet.Hyperlinks;
					int additionalLinksRowOffset = 0;
					if (tracepath != null)
					{
						additionalLinksRowOffset += 3;
						excelSheet.Cells[1, 3] = "Tracefile:";
						links2.Add(excelSheet.Cells[2, 3], tracepath);
					}

					if (wiresharkPath != null && !wiresharkPath.isempty())
					{
						excelSheet.Cells[1 + additionalLinksRowOffset, 3] = "Wireshark path:";
						excelSheet.Cells[2 + additionalLinksRowOffset, 3] = wiresharkPath;
						additionalLinksRowOffset += 3;
					}


					if (previousFileName != null)
					{
						excelSheet.Cells[1 + additionalLinksRowOffset, 3] = "Previous result file:";
						links2.Add(excelSheet.Cells[2 + additionalLinksRowOffset, 3], previousFileName);
					}


					//Links to Sheets
					excelSheet.Cells[1, 1] = "Content summary:";
					for (int i = 0; i < sourceTables.Count; i++)
						links2.Add(excelSheet.Cells[i + 2, 1], "", String.Format("'{0}'!A1", sourceTables[i].DataTable.TableName), Type.Missing, sourceTables[i].DataTable.TableName);

					Marshal.ReleaseComObject(links2);

					//Formatting
					excelSheet.Cells[1, 1].EntireColumn.AutoFit();
				}
				else
					((Microsoft.Office.Interop.Excel.Worksheet)excelSheets[1]).Delete();

				//Activate first sheet
				((Microsoft.Office.Interop.Excel.Worksheet)excelSheets[1]).Activate();
			}
			finally
			{
				//Save file
				try
				{
					excelworkBook.SaveAs(filename);
					excelworkBook.Close();
					excel.Quit();
				}
				catch (Exception)
				{
					//If saving to this location is impossible (excel path restrictions), create the file at tmp path and move it afterwards
					string tmpPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(filename));
					excelworkBook.SaveAs(tmpPath);
					excelworkBook.Close();
					excel.Quit();

					if (File.Exists(filename))
						File.Delete(filename);

					File.Move(tmpPath, filename);
				}
				if (excelSheet != null) Marshal.FinalReleaseComObject(excelSheet);
				if (excelSheets != null) Marshal.ReleaseComObject(excelSheets);
				if (excelworkBook != null) Marshal.ReleaseComObject(excelworkBook);
				if (excelworkBooks != null) Marshal.ReleaseComObject(excelworkBooks);
				if (excel != null) Marshal.ReleaseComObject(excel);
			}

			return filename;
		}

		return String.Empty;
	}
#endif

	public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
	{
		// base case: 
		IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
		foreach (var sequence in sequences)
		{
			var s = sequence; // don't close over the loop variable 
							  // recursive case: use SelectMany to build the new product out of the old one 
			result =
			  from seq in result
			  from item in s
			  select seq.Concat(new[] { item });
		}
		return result;
	}


	/// <summary>
	/// Returns the corresponding messageID for given service and method/event IDs
	/// </summary>
	public static uint GetMessageID(ushort? serviceId, ushort? methodID)
	{
		uint msgid = (uint)(serviceId << 16);
		msgid += (ushort)methodID;
		return msgid;
	}

	/// <summary>
	/// Returns the corresponding methodID for given messageID
	/// </summary>
	public static ushort GetMethodID(uint messageID)
	{
		uint value = messageID & 0xffff;
		return (ushort)value;
	}

	/// <summary>
	/// Returns the corresponding serviceID for given messageID
	/// </summary>
	public static ushort GetServiceID(uint messageID)
	{
		uint value = messageID & 0xffff0000;
		value >>= 16;
		return (ushort)value;
	}

}
