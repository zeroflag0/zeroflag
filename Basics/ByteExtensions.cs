using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public static class ByteExtensions
{
	static ByteExtensions()
	{
		MethodInfo[] methods = typeof(BitConverter).GetMethods();
		foreach (MethodInfo m in methods)
		{
			if (m.IsStatic)
			{
				if (m.Name.StartsWith("To")
					&& m.GetParameters().Length == 2
					&& m.GetParameters()[0].ParameterType == typeof(byte[])
					&& m.GetParameters()[1].ParameterType == typeof(int)
					)
				{
					Type returntype = m.ReturnType;
					Func<byte[], object> conv =
						raw => m.Invoke(null, new object[] { raw, 0 });

					ToValueMap.Add(returntype, conv);
				}
				if (m.Name.StartsWith("GetBytes")
					&& m.GetParameters().Length == 1
					&& m.ReturnType == typeof(byte[])
					)
				{
					Type valuetype = m.GetParameters()[0].ParameterType;
					Func<object, byte[]> conv =
						value => (byte[])m.Invoke(null, new object[] { value });

					ToBytesMap.Add(valuetype, conv);
				}
			}
		}
	}

	public static Dictionary<Type, Func<byte[], object>> ToValueMap = new Dictionary<Type, Func<byte[], object>>();
	public static Dictionary<Type, Func<object, byte[]>> ToBytesMap = new Dictionary<Type, Func<object, byte[]>>();
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="list"></param>
	/// <param name="reverse">
	/// Intel-PC is LSB, reverse = false
	/// Ethernet is MSB, reverse = true
	/// </param>
	/// <returns></returns>
	private static TValue Parse<TValue>(this IEnumerable<byte> list, bool reverse = false)
		where TValue : struct
	{
		return new List<byte>(list).Parse<TValue>(reverse);
	}
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="list"></param>
	/// <param name="reverse">
	/// Intel-PC is LSB, reverse = false
	/// Ethernet is MSB, reverse = true
	/// </param>
	/// <returns></returns>
	public static TValue Parse<TValue>(this byte[] list, bool reverse = false)
		where TValue : struct
	{
		Type type = typeof(TValue);
		if (type.IsEnum)
			type = type.GetEnumUnderlyingType();
		TValue value = (TValue)list.Parse(type, reverse);
		return value;
	}
	public static object Parse(this byte[] list, Type type, bool reverse = false)
	{
		if (type == typeof(byte))
		{
			// byte doesn't have to be parsed...
			try
			{
				return (object)list.First();
			}
			finally
			{
			}
		}
		// get the actual size of the value type (struct)...
		int length = System.Runtime.InteropServices.Marshal.SizeOf(type);


		// reverse the relevant bytes... (MSB<->LSB)
		if (list.Length < length)
		{
			byte[] temp = new byte[length];
			list.CopyTo(temp, 0);// length - list.Length);
			list = temp;
		}
		byte[] source;
		if (reverse)
			source = list.Reverse(length);
		else
			source = list.toarray();

		// use the converter to get the value...
		Func<byte[], object> conv = ToValueMap[type];
		object value = conv(source);

		return value;
	}

	public static byte[] ToBytes(this string value, bool reverse = false)
	{
		byte[] raw = Encoding.UTF8.GetBytes(value);
		// reverse the relevant bytes... (MSB<->LSB)
		if (reverse)
			raw = raw.Reverse(raw.Length);
		return raw;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="list"></param>
	/// <param name="reverse">
	/// Intel-PC is LSB, reverse = false
	/// Ethernet is MSB, reverse = true
	/// </param>
	/// <returns></returns>
	public static byte[] ToBytes<TValue>(this TValue value, bool reverse = false)
		where TValue : struct
	{
		Type type = typeof(TValue);
		if (type.IsEnum)
			type = type.GetEnumUnderlyingType();
		return value.ToBytes(type, reverse);
	}
	public static byte[] ToBytes(this object value, Type type, bool reverse = false)
	{
		if (type == typeof(byte))
			// byte doesn't have to be converted...
			return new byte[] { (byte)(object)value };

		// get the actual size of the value type (struct)...
		int length = System.Runtime.InteropServices.Marshal.SizeOf(type);

		byte[] raw;

		// use the converter to get the value...
		Func<object, byte[]> conv = ToBytesMap[type];
		raw = conv(value);

		// reverse the relevant bytes... (MSB<->LSB)
		if (reverse)
			raw = raw.Reverse(length);
		return raw;
	}

	public static TItem[] Reverse<TItem>(this IEnumerable<TItem> list, int length)
	{
		TItem[] source = new TItem[length];
		{
			// copy the relevant bytes to the source array...
			int i = 0;
			foreach (TItem item in list)
			{
				source[i] = item;
				i++;
				if (i >= length)
					break;
			}
		}
		TItem[] result = new TItem[length];

		for (int i = 0; i < length; i++)
		// reverse the relevant bytes...
		{
			result[i] = source[length - i - 1];
		}
		return result;
	}

}
