using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basics
{
	public class DataTableWrapper
	{
		/// <summary>
		/// Wrapper für ein DataTable Objekt und zugehörige Excel-Formatvorgaben für die Zeilen und Spalten der Tabelle
		/// Beispiele für verfügbare Excel-Formatstrings: http://www.online-excel.de/excel/singsel.php?f=71
		/// </summary>
		/// <param name="dataTable">Die Datentabelle</param>
		/// <param name="columnFormats">Liste der Spalten-Formatvorgaben (Key: Spaltenindex, Value: Excel-Spaltenformat)</param>
		/// <param name="rowFormats">Liste der Zeilen-Formatvorgaben (Key: Zeilenindex, Value: Excel-Zeilenformat)</param>
		public DataTableWrapper(System.Data.DataTable dataTable, Dictionary<int, string> columnFormats = null, Dictionary<int, string> rowFormats = null)
		{
			DataTable = dataTable;
			ColumnFormats = columnFormats;
			RowFormats = rowFormats;
		}

		public System.Data.DataTable DataTable { get; set; }
		public Dictionary<int, string> ColumnFormats { get; set; }
		public Dictionary<int, string> RowFormats { get; set; }
	}
}
