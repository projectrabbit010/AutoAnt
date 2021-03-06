using System;
using UnityEngine;

namespace ES3Types
{
	[ES3PropertiesAttribute("Time", "LastDateTime", "GameLeftCase")]
	public class ES3Type_GameHistory : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3Type_GameHistory() : base(typeof(HK.AutoAnt.UserControllers.GameHistory)){ Instance = this; }

		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (HK.AutoAnt.UserControllers.GameHistory)obj;
			
			writer.WriteProperty("Time", instance.Time, ES3Type_double.Instance);
			writer.WriteProperty("LastDateTime", instance.LastDateTime, ES3Type_DateTime.Instance);
			writer.WriteProperty("GameLeftCase", instance.GameLeftCase);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (HK.AutoAnt.UserControllers.GameHistory)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Time":
						instance.Time = reader.Read<System.Double>(ES3Type_double.Instance);
						break;
					case "LastDateTime":
						instance.LastDateTime = reader.Read<System.DateTime>(ES3Type_DateTime.Instance);
						break;
					case "GameLeftCase":
						instance.GameLeftCase = reader.Read<HK.AutoAnt.Constants.GameLeftCase>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new HK.AutoAnt.UserControllers.GameHistory();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}

	public class ES3Type_GameHistoryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_GameHistoryArray() : base(typeof(HK.AutoAnt.UserControllers.GameHistory[]), ES3Type_GameHistory.Instance)
		{
			Instance = this;
		}
	}
}