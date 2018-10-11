﻿using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB.Mapping;
using NUnit.Framework;
using Tests.Tools;

namespace Tests.Playground
{
	[TestFixture]
	public class ExpandTests : TestBase
	{
		[Table]
		class SampleClass
		{
			[Column] public int Id    { get; set; }
			[Column] public int Value { get; set; }
		}

		private static SampleClass[] GenerateData()
		{
			var sampleData = new[]
			{
				new SampleClass { Id = 1, Value = 1 },
				new SampleClass { Id = 2, Value = 2 },
				new SampleClass { Id = 3, Value = 3 },
			};
			return sampleData;
		}

		Expression<Func<SampleClass, bool>> GetTestPredicate(int v)
		{
			return c => c.Value == v;
		}

		[Test, Combinatorial]
		public void InvokationTestLocal([SQLiteDataSources] string context)
		{
			Expression<Func<SampleClass, bool>> predicate = c => c.Value > 1;
			var sampleData = GenerateData();

			using (var db = GetDataContext(context))
			using (var table = db.CreateLocalTable(sampleData))
			{
				var query = from t in table
					where predicate.Compile()(t)
					select t;
				var expected = from t in sampleData
					where predicate.Compile()(t)
					select t;

				AreEqual(expected, query, ComparerBuilder<SampleClass>.GetEqualityComparer());
			}
		}

		[Test, Combinatorial]
		public void CompileTestLocal([SQLiteDataSources] string context)
		{
			Expression<Func<SampleClass, bool>> predicate = c => c.Value > 1;
			var sampleData = GenerateData();

			using (var db = GetDataContext(context))
			using (var table = db.CreateLocalTable(sampleData))
			{
				var query = from t in table
					from t2 in table.Where(predicate.Compile())
					select t;

				var expected = from t in sampleData
					from t2 in table.Where(predicate.Compile())
					select t;

				AreEqual(expected, query, ComparerBuilder<SampleClass>.GetEqualityComparer());
			}
		}

		[Test, Combinatorial]
		public void NonCompileTestLocal([SQLiteDataSources] string context)
		{
			Expression<Func<SampleClass, bool>> predicate = c => c.Value > 1;
			var sampleData = GenerateData();

			using (var db = GetDataContext(context))
			using (var table = db.CreateLocalTable(sampleData))
			{
				var query = from t in table
					from t2 in table.Where(predicate)
					select t;

				var expected = from t in sampleData
					from t2 in table.Where(predicate)
					select t;

				AreEqual(expected, query, ComparerBuilder<SampleClass>.GetEqualityComparer());
			}
		}

		[Test, Combinatorial]
		public void InvokationTestFunction([SQLiteDataSources] string context)
		{
			var sampleData = GenerateData();

			using (var db = GetDataContext(context))
			using (var table = db.CreateLocalTable(sampleData))
			{
				var query = from t in table
					where GetTestPredicate(1).Compile()(t)
					select t;
				var expected = from t in sampleData
					where GetTestPredicate(1).Compile()(t)
					select t;

				AreEqual(expected, query, ComparerBuilder<SampleClass>.GetEqualityComparer());
			}
		}

		int SomeFunc(int value)
		{
			return value;
		}

		[Test, Combinatorial]
		public void LocalInvokation([SQLiteDataSources] string context)
		{
			var sampleData = GenerateData();

			using (var db = GetDataContext(context))
			using (var table = db.CreateLocalTable(sampleData))
			{
				var query = from t in table
					where t.Value == SomeFunc(1)
					select t;
				var expected = from t in sampleData
					where t.Value == SomeFunc(1)
					select t;

				AreEqual(expected, query, ComparerBuilder<SampleClass>.GetEqualityComparer());
			}
		}

	}
}
