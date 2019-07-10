using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

public interface ISerializable
{
	void Serialize(Stream stream);
	void Deserialize(Stream stream);
}

public static class StreamExtensions
{
	public static void Write<T>(this Stream stream, T value, bool reverse
#if DEFAULT_REVERSE
		= true
#else
		= false
#endif
		)
		where T : struct
	{

		byte[] values;
		//if (typeof(T).IsEnum)
		//	values = value.ToBytes(typeof(T).GetEnumUnderlyingType(), reverse);
		//else
		values = value.ToBytes(reverse);
		stream.Write(values, 0, values.Length);
	}
	public static void Write(this Stream stream, string value)
	{
		byte[] values = Encoding.UTF8.GetBytes(value);
		stream.Write(values.Length);
		stream.Write(values, 0, values.Length);
	}
	public static string ReadString(this Stream stream)
	{
		int length;
		stream.Read(out length);
		byte[] buffer = new byte[length];
		stream.Read(buffer, 0, length);
		string value = Encoding.UTF8.GetString(buffer);
		return value;
	}

	public static void Write<T>(this Stream stream, T[] values)
		where T : struct
	{
		int length = values.Length;
		stream.Write(length);
		foreach (T item in values)
		{
			stream.Write<T>(item);
		}
	}
	public static void WriteS(this Stream stream, ISerializable[] values)
	{
		int length = values.Length;
		stream.Write(length);
		foreach (ISerializable item in values)
		{
			item.Serialize(stream);
		}
	}
	public static void Write<TKey, TValue>(this Stream stream, IDictionary<TKey, TValue> values)
	where TKey : struct
	where TValue : struct
	{
		stream.WriteDict<TKey, TValue>(values);
	}
	public static void WriteDict<TKey, TValue>(this Stream stream, IDictionary<TKey, TValue> values)
		where TKey : struct
		where TValue : struct
	{
		int length = values.Count;
		stream.Write(length);
		foreach (var pair in values)
		{
			stream.Write(pair.Key);
			stream.Write(pair.Value);
		}
	}
	public static void WriteDict<TKey>(this Stream stream, IDictionary<TKey, string> values)
			where TKey : struct
	{
		int length = values.Count;
		stream.Write(length);
		foreach (var pair in values)
		{
			stream.Write(pair.Key);
			stream.Write(pair.Value);
		}
	}
	public static T Read<T>(this Stream stream)
		where T : struct
	{
		T value;
		stream.Read(out value);
		return value;
	}
	public static void Read<T>(this Stream stream, out T value)
		where T : struct
	{
		Type t = typeof(T);
		if (t.IsEnum)
			t = typeof(T).GetEnumUnderlyingType();

		int size = t.GetSize();
		byte[] buffer = new byte[size];
		stream.Read(buffer, 0, size);
		value = buffer.Parse<T>();
		//var m = ByteConverter.ConvertFunctions[t];
		//value = (T)m.Invoke(null, new object[] { buffer, 0 });
	}

	public static void Read<T>(this Stream stream, out T[] values)
		where T : struct
	{
		int length = stream.Read<int>();
		values = new T[length];
		for (int i = 0; i < length; i++)
		{
			values[i] = stream.Read<T>();
		}
	}

	public static void ReadS<T>(this Stream stream, out T[] values)
			where T : ISerializable, new()
	{
		int length = stream.Read<int>();
		values = new T[length];
		for (int i = 0; i < length; i++)
		{
			T v = new T();
			v.Deserialize(stream);
			values[i] = v;
		}
	}

	public static IEnumerable<T> ReadList<T>(this Stream stream)
			where T : ISerializable, new()
	{
		int length = stream.Read<int>();
		for (int i = 0; i < length; i++)
		{
			T v = new T();
			v.Deserialize(stream);
			yield return v;
		}
	}

	public static void Read<TKey, TValue>(this Stream stream, ref IDictionary<TKey, TValue> values)
		where TKey : struct
		where TValue : struct
	{
		int length = stream.Read<int>();

		for (int i = 0; i < length; i++)
		{
			TKey key = stream.Read<TKey>();
			TValue value = stream.Read<TValue>();
			values[key] = value;
		}
	}

	public static void Read<TKey>(this Stream stream, IDictionary<TKey, string> values)
		where TKey : struct
	{ stream.ReadDict(values); }
	public static void ReadDict<TKey>(this Stream stream, IDictionary<TKey, string> values)
		where TKey : struct
	{
		int length = stream.Read<int>();

		for (int i = 0; i < length; i++)
		{
			TKey key = stream.Read<TKey>();
			string value = stream.ReadString();
			values[key] = value;
		}
	}

	public static void Write(this Stream stream, System.Net.IPAddress value)
	{
		byte[] values = value.GetAddressBytes();
		stream.Write(values, 0, values.Length);
	}
	public static void Write(this Stream stream, System.Net.NetworkInformation.PhysicalAddress value)
	{
		byte[] values = value.GetAddressBytes();
		stream.Write(values, 0, values.Length);
	}

	//public class ByteConverter
	//{
	//	public static Dictionary<Type, MethodInfo> ConvertFunctions;
	//	static ByteConverter()
	//	{
	//		var functions = new Dictionary<Type, MethodInfo>();
	//		Type t = typeof(BitConverter);
	//		var methods = t.GetMethods();
	//		foreach (var m in methods)
	//		{
	//			if (!m.IsStatic || !m.Name.StartsWith("To"))
	//				continue;
	//			ParameterInfo[] paras = m.GetParameters();
	//			if (paras.Length != 2 || paras[0].ParameterType != typeof(byte[]) || paras[1].ParameterType != typeof(int))
	//				continue;
	//			functions.Add(m.ReturnType, m);
	//		}
	//		ConvertFunctions = functions;
	//	}
	//}
}


