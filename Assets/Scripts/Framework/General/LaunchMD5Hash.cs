using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Launch
{	
	public static class LaunchMD5Hash
	{
		public static string GetFileMD5(string filePath)
		{
			if(!File.Exists(filePath))
			{
				Debug.LogError("File["+filePath+"] not exists when check md5!");
				return null;
			}
			MD5CryptoServiceProvider md5Generator = new MD5CryptoServiceProvider();
			string strMD5 = null;
			using(FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{ 
				byte[] hash = md5Generator.ComputeHash(file);
				strMD5 = System.BitConverter.ToString(hash);
				file.Close();
			}
			return strMD5;
		}

		public static string Get(string input)
		{
			return Get(Encoding.Default.GetBytes(input));
		}

		public static string Get(byte[] input)
		{
			MD5 md5Hasher = MD5.Create();
			byte[] data = md5Hasher.ComputeHash(input);
			StringBuilder stringBuilder = new StringBuilder();

			foreach (var c in data)
				stringBuilder.Append(c.ToString("x2"));

			return stringBuilder.ToString();
		}

		public static string Get(Stream stream)
		{
			MD5 md5 = MD5.Create();
			byte[] data = md5.ComputeHash(stream);
			StringBuilder stringBuilder = new StringBuilder();

			foreach (var c in data)
				stringBuilder.Append(c.ToString("x2"));

			return stringBuilder.ToString();
		}

		public static bool Verify(string input, string hash)
		{
			string hashOfInput = Get(input);
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;
			return (0 == comparer.Compare(hashOfInput, hash));
		}

		public static bool Verify(byte[] input, string hash)
		{
			string hashOfInput = Get(input);
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;
			return (0 == comparer.Compare(hashOfInput, hash));
		}

		public static bool Verify(Stream input, string hash)
		{
			string hashOfInput = Get(input);
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;
			return (0 == comparer.Compare(hashOfInput, hash));
		}
	}
}
