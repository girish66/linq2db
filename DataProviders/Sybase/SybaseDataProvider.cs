﻿using System;
using System.Data;
using System.Xml;
using System.Xml.Linq;

using Sybase.Data.AseClient;

namespace LinqToDB.DataProvider
{
	using Mapping;
	using SqlProvider;

	public class SybaseDataProvider : DataProviderBase
	{
		#region Init

		public SybaseDataProvider(string name)
			: this(name, new SybaseMappingSchema(name))
		{
		}

		public SybaseDataProvider(string name, MappingSchema mappingSchema)
			: base(name, mappingSchema)
		{
			SqlProviderFlags.AcceptsTakeAsParameter = false;
			SqlProviderFlags.IsSkipSupported        = false;

			SetCharField("char",  (r,i) => r.GetString(i).TrimEnd());
			SetCharField("nchar", (r,i) => r.GetString(i).TrimEnd());

			SetProviderField<IDataReader,TimeSpan,DateTime>((r,i) => r.GetDateTime(i) - new DateTime(1900, 1, 1));
			SetProviderField<IDataReader,DateTime,DateTime>((r,i) => GetDateTime(r, i));
		}

		static DateTime GetDateTime(IDataReader dr, int idx)
		{
			var value = dr.GetDateTime(idx);

			if (value.Year == 1900 && value.Month == 1 && value.Day == 1)
				return new DateTime(1, 1, 1, value.Hour, value.Minute, value.Second, value.Millisecond);

			return value;
		}

		#endregion

		#region Public Properties

		public override Type ConnectionType { get { return typeof(AseConnection); } }
		public override Type DataReaderType { get { return typeof(AseDataReader); } }

		#endregion

		#region Overrides

		public override IDbConnection CreateConnection(string connectionString)
		{
			return new AseConnection(connectionString);
		}

		public override ISqlProvider CreateSqlProvider()
		{
			return new SybaseSqlProvider(SqlProviderFlags);
		}

		public override void SetParameter(IDbDataParameter parameter, string name, DataType dataType, object value)
		{
			switch (dataType)
			{
				case DataType.SByte      : 
					dataType = DataType.Int16;
					if (value is sbyte)
						value = (short)(sbyte)value;
					break;

				case DataType.Time       :
					if (value is TimeSpan) value = new DateTime(1900, 1, 1) + (TimeSpan)value;
					break;

				case DataType.Xml        :
					dataType = DataType.NVarChar;
					     if (value is XDocument)   value = value.ToString();
					else if (value is XmlDocument) value = ((XmlDocument)value).InnerXml;
					break;

				case DataType.Guid       :
					if (value != null)
						value = value.ToString();
					dataType = DataType.Char;
					parameter.Size = 36;
					break;

				case DataType.Undefined  :
					if (value == null)
						dataType = DataType.Char;
					break;
			}

			base.SetParameter(parameter, "@" + name, dataType, value);
		}

		protected override void SetParameterType(IDbDataParameter parameter, DataType dataType)
		{
			switch (dataType)
			{
				case DataType.VarNumeric    : parameter.DbType = DbType.Decimal;                                break;
				case DataType.UInt16        : ((AseParameter)parameter).AseDbType = AseDbType.UnsignedSmallInt; break;
				case DataType.UInt32        : ((AseParameter)parameter).AseDbType = AseDbType.UnsignedInt;      break;
				case DataType.UInt64        : ((AseParameter)parameter).AseDbType = AseDbType.UnsignedBigInt;   break;
				case DataType.Text          : ((AseParameter)parameter).AseDbType = AseDbType.Text;             break;
				case DataType.NText         : ((AseParameter)parameter).AseDbType = AseDbType.Unitext;          break;
				case DataType.Binary        : ((AseParameter)parameter).AseDbType = AseDbType.Binary;           break;
				case DataType.VarBinary     : ((AseParameter)parameter).AseDbType = AseDbType.VarBinary;        break;
				case DataType.Image         : ((AseParameter)parameter).AseDbType = AseDbType.Image;            break;
				case DataType.Money         : ((AseParameter)parameter).AseDbType = AseDbType.Money;            break;
				case DataType.SmallMoney    : ((AseParameter)parameter).AseDbType = AseDbType.SmallMoney;       break;
				case DataType.Date          : ((AseParameter)parameter).AseDbType = AseDbType.Date;             break;
				case DataType.Time          : ((AseParameter)parameter).AseDbType = AseDbType.Time;             break;
				case DataType.SmallDateTime : ((AseParameter)parameter).AseDbType = AseDbType.SmallDateTime;    break;
				case DataType.Timestamp     : ((AseParameter)parameter).AseDbType = AseDbType.TimeStamp;        break;
				default                     : base.SetParameterType(parameter, dataType);                       break;
			}
		}

		#endregion
	}
}