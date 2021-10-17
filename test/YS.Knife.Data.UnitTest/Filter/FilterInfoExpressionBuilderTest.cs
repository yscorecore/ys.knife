﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using YS.Knife.Data.Query;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query
{

    public class FilterInfoExpressionBuilderTest
    {
        private static readonly Exam[] Exams =
        {
            new Exam { Id="Exam01",ExamName = "mid term", ExamDate = DateTimeOffset.Parse("2021-01-01")},
            new Exam {Id="Exam02",ExamName = "final exam", ExamDate = DateTimeOffset.Parse("2021-07-01")}
        };
        private static readonly List<Student> Students = new List<Student>
        {
            new Student{ Id="001", Name = "zhang san",Age = 15,
                Address = new Address{ Id="Add001", City="bei jing", ZipCode="00001"},
                Scores = new []
            {
                new ScoreRecord{ Id="S001",ClassName = "Mathematics", Score = 98, Exam = Exams[0]},
                new ScoreRecord{ Id="S002",ClassName = "Chinese", Score = 89, Exam = Exams[0]},
                new ScoreRecord{ Id="S003",ClassName = "English", Score = 100, Exam = Exams[0]},
                new ScoreRecord{ Id="S004",ClassName = "Mathematics", Score = 99, Exam = Exams[0]},
                new ScoreRecord{ Id="S005",ClassName = "Chinese", Score = 95, Exam = Exams[0]},
                new ScoreRecord{ Id="S006",ClassName = "English", Score = 98, Exam = Exams[0]}
            }},
            new Student{ Id="002", Name = "li si",Age = 13,
                 Address = new Address{Id="Add002", City="bei jing", ZipCode="00001"},
                Scores = new []
            {
                new ScoreRecord{ Id="S011",ClassName = "Mathematics", Score = 78, Exam = Exams[0]},
                new ScoreRecord{ Id="S012",ClassName = "Chinese", Score = 80, Exam = Exams[0]},
                new ScoreRecord{ Id="S013",ClassName = "English", Score = 49, Exam = Exams[0]},
                new ScoreRecord{ Id="S014",ClassName = "Mathematics", Score = 32, Exam = Exams[0]},
                new ScoreRecord{ Id="S015",ClassName = "Chinese", Score = 69, Exam = Exams[0]},
                new ScoreRecord{ Id="S016",ClassName = "English", Score = 56, Exam = Exams[0]}
            }},
            new Student{ Id="003", Name = null, Age = 17,
                 Address = new Address{Id="Add003", City="xi'an", ZipCode="00002"},
                Scores = new []
            {
                new ScoreRecord{ Id="S031",ClassName = "Mathematics", Score = 18, Exam = Exams[0]},
                new ScoreRecord{ Id="S032",ClassName = "Chinese", Score = 60, Exam = Exams[0]},
                new ScoreRecord{ Id="S033",ClassName = "English", Score = 9, Exam = Exams[0]},
                new ScoreRecord{ Id="S034",ClassName = "Mathematics", Score = 40, Exam = Exams[0]},
                new ScoreRecord{ Id="S035",ClassName = "Chinese", Score = 59, Exam = null},
                new ScoreRecord{ Id="S036",ClassName = "English", Score = 30, Exam = null}
            }},
            new Student{ Id="004", Name = "zhao si", Age = 17, Address = new Address(){ Id="Add004"} },
            new Student{ Id="005", Name = "qian wu", Age = 20, Scores = new ScoreRecord[0]},
            new Student{ Id="006", Name = "feng wu", Age = 14}
        };

        #region For Model

        public class ModelTest
        {
            [Fact]
            public void ShouldGetAllWhenFilterIsNull()
            {
                var result = FilterStudentIds(null);
                result.Should().Be("001,002,003,004");
            }

            [Theory]
            [InlineData("Name", Operator.Equals, "li si", "002")]
            [InlineData("Name", Operator.Equals, null, "003")]


            public void ShouldFilterSingleItem(string fieldName, Operator filterType, object value, string expectedIds)
            {
                TestSdudentForSingleItem(fieldName, filterType, value, expectedIds);
            }

            private string FilterStudentIds(FilterInfo filter)
            {
                var ids = Students.AsQueryable().DoFilter(filter)
                        .Select(p => p.Id);
                return string.Join(",", ids);
            }

            private void TestSdudentForSingleItem(string fieldName, Operator filterType, object value, string expectedIds)
            {
                var filter = FilterInfo.CreateItem(fieldName, filterType, value);
                var ids = FilterStudentIds(filter);
                string.Join(",", ids).Should().Be(expectedIds);
            }

        }




        #endregion


        public class DtoModelTest
        {

            [Theory]
            [InlineData("", "001,002,003,004,005,006")]
            [InlineData("tName=\"li si\"", "002")]
            [InlineData("tName=null", "003")]
            [InlineData("tAddress!.city=null", "004")]
            public void ShouldFilterSingleItem(string filterExpressionForDto, string expectedIds)
            {
                FilterDtoStudents(filterExpressionForDto).Should().Be(expectedIds);
            }


            [Theory]
            [InlineData("tAddress=null", "005,006")]
            [InlineData("tAddress!.city=null", "004")]
            [InlineData("tAddress?.city=null", "004,005,006")]
            [InlineData("tAddress?.city!.Length=5", "003,005,006")]
            [InlineData("tAddress?.city?.Length=5", "003,004,005,006")]
            [InlineData("tAddress!.city!.Length=5", "003")]
            [InlineData("tAddress!.city?.Length=5", "003,004")]
            public void ShouldFilterWithOptionalField(string filterExpressionForDto, string expectedIds)
            {
                FilterDtoStudents(filterExpressionForDto).Should().Be(expectedIds);
            }
            [Theory]

            [InlineData("TName!.Count()=5", "002")]

            public void ShouldFilterWithFunction(string filterExpressionForDto, string expectedIds)
            {
                FilterDtoStudents(filterExpressionForDto).Should().Be(expectedIds);
            }

            private string FilterDtoStudents(string filterExpressionForDto)
            {
                var filter = FilterInfo.Parse(filterExpressionForDto);
                var studentMapper = new ObjectMapper<Student, StudentDto>();
                studentMapper.Append(p => p.TName, p => p.Name);
                studentMapper.Append(p => p.TAge, p => p.Age);
                studentMapper.Append(p => p.TId, p => p.Id);
                studentMapper.Append(p => p.THeight, p => (decimal?)p.Height);
                studentMapper.Append(p => p.TAddress, p => p.Address);

                var scoreMapper = new ObjectMapper<ScoreRecord, ScoreDto>();
                scoreMapper.Append(p => p.TClassName, p => p.ClassName);
                scoreMapper.Append(p => p.TScore, p => p.Score);
                scoreMapper.Append(p => p.TExam, p => p.Exam as IExam);
                studentMapper.AppendCollection(p => p.TScores, p => p.Scores, scoreMapper);

                var ids = Students.AsQueryable().DoFilter(filter)
                        .Select(p => p.Id);
                return string.Join(",", ids);
            }

        }



        public class EfcoreTest : IDisposable
        {

            private DataContext dataContext1;
            public class DataContext : DbContext
            {
                public DataContext(DbContextOptions<DataContext> options) : base(options)
                {
                }

                public DbSet<Student> Students { get; set; }

                public static DataContext CreateMemorySqlite()
                {
                    return new DataContext(new DbContextOptionsBuilder<DataContext>()
                                .UseSqlite(CreateInMemoryDatabase())
                                .Options);
                }

                private static DbConnection CreateInMemoryDatabase()
                {
                    var connection = new SqliteConnection("Filename=:memory:");

                    connection.Open();

                    return connection;
                }

            }

            public EfcoreTest()
            {
                dataContext1 = DataContext.CreateMemorySqlite();
                dataContext1.Database.EnsureCreated();
                SeedData(dataContext1);
            }
            void SeedData(DataContext dataContext)
            {
                dataContext1.Students.AddRange(Students);
                dataContext.SaveChanges();
            }


            [Theory]
            //[InlineData("", "001,002,003,004,005,006")]
            //[InlineData("tName=\"li si\"", "002")]
            [InlineData("tName=null", "003")]
            [InlineData("tName>null", "")]
            [InlineData("tName>=null", "")]
            [InlineData("tName<null", "")]
            [InlineData("tName<=null", "")]
            [InlineData("null=tName", "003")]
            public void ShouldFilterSingleItem(string filterExpressionForDto, string expectedIds)
            {
                FilterDtoStudents(filterExpressionForDto).Should().Be(expectedIds);
            }

            [Theory]
            [InlineData("tAddress=null", "005,006")]
            [InlineData("tAddress.city=null", "004,005,006")]
            [InlineData("tAddress.city.length=5", "003")]
            [InlineData("5=tAddress.city.Length", "003")]
            public void ShouldFilterWithNavigateField(string filterExpressionForDto, string expectedIds)
            {
                FilterDtoStudents(filterExpressionForDto).Should().Be(expectedIds);
            }

            [Theory]

            //[InlineData("Lower(tName)=\"zhang san\"", "001")]
            //[InlineData("Lower(tAddress.city)=\"xi'an\"", "001")]
            //[InlineData("TScores.Count()=6", "001,002,003")]
            [InlineData("TScores.Count(TClassName=\"Chinese\")=2", "001,002,003")]
            public void ShouldFilterWithFunction(string filterExpressionForDto, string expectedIds)
            {
                FilterDtoStudents(filterExpressionForDto).Should().Be(expectedIds);
            }
            [Fact]
            public void TestSelect()
            {


                var query = dataContext1.Students
                    .Select(p => new StudentDto
                    {
                        TAge = p.Age,
                        TScores = p.Scores.AsQueryable().OrderBy(p => (double)p.Score).Skip(1).Take(2).Select(t => new ScoreDto { TClassName = t.ClassName, TScore = t.Score }).ToList()

                    });

                Console.WriteLine(query.ToQueryString());

            }


            private string FilterDtoStudents(string filterExpressionForDto)
            {
                var filter = FilterInfo.Parse(filterExpressionForDto);
                var studentMapper = new ObjectMapper<Student, StudentDto>();
                studentMapper.Append(p => p.TName, p => p.Name);
                studentMapper.Append(p => p.TAge, p => p.Age);
                studentMapper.Append(p => p.TId, p => p.Id);
                studentMapper.Append(p => p.THeight, p => (decimal?)p.Height);
                studentMapper.Append(p => p.TAddress, p => p.Address);

                var scoreMapper = new ObjectMapper<ScoreRecord, ScoreDto>();
                scoreMapper.Append(p => p.TClassName, p => p.ClassName);
                scoreMapper.Append(p => p.TScore, p => p.Score);
                scoreMapper.Append(p => p.TExam, p => p.Exam as IExam);
                studentMapper.AppendCollection(p => p.TScores, p => p.Scores, scoreMapper);

                var ids = dataContext1.Students.DoFilter(filter, studentMapper).OrderBy(p => p.Id)
                        .Select(p => p.Id);
                Console.WriteLine(ids.ToQueryString());
                return string.Join(",", ids);
            }

            public void Dispose()
            {
                if (dataContext1 != null)
                {
                    dataContext1.Database.GetDbConnection().Close();
                    dataContext1.Dispose();
                    dataContext1 = null;
                }
            }
        }
        public class Address
        {
            public string Id { get; set; }
            public string City { get; set; }
            public string ZipCode { get; set; }
        }

        public enum Sex
        {
            Female,
            Male,
        }



        public class StudentDto
        {
            public string TId { get; set; }
            public string TName { get; set; }
            public int TAge { get; set; }
            public decimal? THeight { get; set; }
            public List<ScoreDto> TScores { get; set; }
            public Address TAddress { get; set; }
        }

        public class ScoreDto
        {
            public string TClassName { get; set; }
            public decimal TScore { get; set; }
            public IExam TExam { get; set; }
        }

        public interface IExam
        {
            string ExamName { get; set; }
            DateTimeOffset ExamDate { get; set; }
        }

        public class Exam
        {
            public string Id { get; set; }
            public string ExamName { get; set; }
            public DateTimeOffset ExamDate { get; set; }
        }

        public class Student
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public decimal? Height { get; set; }

            public decimal? Weight { get; set; }
            public IEnumerable<ScoreRecord> Scores { get; set; }
            public Address Address { get; set; }
        }

        public class ScoreRecord

        {
            public string Id { get; set; }
            public string ClassName { get; set; }
            public decimal Score { get; set; }
            public Exam Exam { get; set; }
        }
    }
}
