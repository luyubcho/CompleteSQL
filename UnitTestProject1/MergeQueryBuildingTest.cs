﻿using CompleteSQL;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestFixture]
    public class MergeQueryBuildingTest
    {
    
        [Test]
        public void OnWhenMathcedThenDeleteTest()
        {
            var people = new[]
            {
                new
                {
                    Id = 1,
                    Name = "John"
                }
            };

            DataContext context = new DataContext("CompleteSQL");

            var mergeExpression = context.CreateMergeUsing(people)
               .Target("TestTable")
               .On(p => p.Id)
               .WhenMatchedThenDelete();



            string query = mergeExpression.GetMergeQuery();

            string expectedQuery = 
@"Merge Into TestTable as tgt
Using #TestTable as src
	On tgt.Id = src.Id
When Matched
	Then Delete;";

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void OnWhenMathcedThenDeleteWithTwoPredicatesTest()
        {
            var people = new[]
            {
                new
                {
                    Number = 1,
                    DocumentNumber = 2,
                    Name = "John"
                }
            };

            DataContext context = new DataContext("CompleteSQL");

            var mergeExpression = context.CreateMergeUsing(people)
            .Target("TestTable")
            .On(p => new { p.Number, p.DocumentNumber })
            .WhenMatchedThenDelete();



            string query = mergeExpression.GetMergeQuery();


            string expectedQuery =
@"Merge Into TestTable as tgt
Using #TestTable as src
	On tgt.Number = src.Number
	And tgt.DocumentNumber = src.DocumentNumber
When Matched
	Then Delete;";

            Assert.AreEqual(expectedQuery, query);

        }

        [Test]
        public void WhenNotMatchedThenInsert()
        {
            var people = new[]
            {
                new
                {
                    Number = 1,
                    DocumentNumber = 2,
                    Name = "John"
                }
            };

            DataContext context = new DataContext("CompleteSQL");

            var mergeExpression = context.CreateMergeUsing(people)
            .Target("TestTable")
            .On(p => new { p.Number, p.DocumentNumber })
            .WhenNotMatchedThenInsert();

            string expectedQuery =
@"Merge Into TestTable as tgt
Using #TestTable as src
	On tgt.Number = src.Number
	And tgt.DocumentNumber = src.DocumentNumber
When Not Matched
	Then Insert(Number, DocumentNumber, Name)
		Values(src.Number, src.DocumentNumber, src.Name);";

            string query = mergeExpression.GetMergeQuery();

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void WhenNotMathcedThenInsertDeterminateColumnsTest()
        {
            var people = new[]
            {
                new
                {
                    Number = 1,
                    DocumentNumber = 2,
                    Name = "John"
                }
            };

            DataContext context = new DataContext("CompleteSQL");

            var mergeExpression = context.CreateMergeUsing(people)
                .Target("TestTable")
                .On(p=>new{p.Number, p.DocumentNumber})
                .WhenNotMatchedThenInsert(p => new { p.Number, p.DocumentNumber });

            string expectedQuery =
@"Merge Into TestTable as tgt
Using #TestTable as src
	On tgt.Number = src.Number
	And tgt.DocumentNumber = src.DocumentNumber
When Not Matched
	Then Insert(Number, DocumentNumber)
		Values(src.Number, src.DocumentNumber);";


            string query = mergeExpression.GetMergeQuery();

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void WhenNotMathcedThenInsertDeterminateColumnsAndConstantsTest()
        {
            var people = new[]
            {
                new
                {
                    Number = 1,
                    Name = "John",
                    Age = 33,
                }
            };

            DataContext context = new DataContext("CompleteSQL");

            var mergeExpression = context.CreateMergeUsing(people)
                .Target("Person")
                .On(p => p.Number )
                .WhenNotMatchedThenInsert(p => new { p.Number, p.Name, p.Age, GroupNumber = 77, GroupName = "SeventySeventGroup", Salary = 100.5123m  });

            string expectedQuery =
@"Merge Into Person as tgt
Using #Person as src
	On tgt.Number = src.Number
When Not Matched
	Then Insert(Number, Name, Age, GroupNumber, GroupName, Salary)
		Values(src.Number, src.Name, src.Age, 77, 'SeventySeventGroup', 100.5123);";


            string query = mergeExpression.GetMergeQuery();

            Assert.AreEqual(expectedQuery, query);
        }

        [Test]
        public void WhenNotMathcedAndGreaterThanThenInsertTest()
        {
            var people = new[]
            {
                new
                {
                    Number = 1,
                    Name = "John",
                    Age = 33,
                }
            };

            DataContext context = new DataContext("CompleteSQL");

            var mergeExpression = context.CreateMergeUsing(people)
                .Target("Person")
                .On(p => p.Number)
                .WhenNotMatchedAndThenInsert(p => p.Age > 17);

            string expectedQuery =
@"Merge Into Person as tgt
Using #Person as src
	On tgt.Number = src.Number
When Not Matched And src.Age > 17
	Then Insert(Number, Name, Age)
		Values(src.Number, src.Name, src.Age);";


            string query = mergeExpression.GetMergeQuery();

            Assert.AreEqual(expectedQuery, query);
        }



        static object[] AndParams =
    {
            new Tuple<string, Expression<Func<Person,bool>>>(">=", p=>p.Age>=17) ,
            new Tuple<string, Expression<Func<Person,bool>>>("<=", p=>p.Age<=17),
            new Tuple<string, Expression<Func<Person,bool>>>("=", p=>p.Age==17),
            new Tuple<string, Expression<Func<Person,bool>>>(">", p=>p.Age>17),
            new Tuple<string, Expression<Func<Person,bool>>>("<", p=>p.Age<17)
    };
    
        [Test, TestCaseSource("AndParams")]
        public void WhenNotMathcedAndThenInsertTest(Tuple<string, Expression<Func<Person,bool>>> tuple)
        {
            var people = new Person[]
            {
                new Person
                {
                    Number = 1,
                    Name = "John",
                    Age = 33,
                }
            };

            DataContext context = new DataContext("CompleteSQL");

            var mergeExpression = context.CreateMergeUsing(people)
                .Target("Person")
                .On(p => p.Number)
                .WhenNotMatchedAndThenInsert(tuple.Item2);

            string expectedQuery =
@"Merge Into Person as tgt
Using #Person as src
	On tgt.Number = src.Number
When Not Matched And src.Age " + tuple.Item1 + " 17" + Environment.NewLine + "\t" +
@"Then Insert(Number, Name, Age)
		Values(src.Number, src.Name, src.Age);";


            string query = mergeExpression.GetMergeQuery();

            Assert.AreEqual(expectedQuery, query);
        }
       

    }
}