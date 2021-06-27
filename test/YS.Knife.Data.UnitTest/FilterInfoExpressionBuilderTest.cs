using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class FilterInfoExpressionBuilderTest
    {
        private static readonly Exam[] Exams =
        {
            new Exam {ExamName = "mid term", ExamDate = DateTimeOffset.Parse("2021-01-01")},
            new Exam {ExamName = "final exam", ExamDate = DateTimeOffset.Parse("2021-07-01")}
        };
        private static readonly List<Student> Students = new List<Student>
        {
            new Student{ Id="001", Name = "zhang san",Age = 15,
                Address = new Address{ City="bei jing", ZipCode="00001"},
                Scores = new []
            {
                new ScoreRecord{ ClassName = "Mathematics", Score = 98, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 89, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 100, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Mathematics", Score = 99, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 95, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 98, Exam = Exams[0]}
            }},
            new Student{ Id="002", Name = "li si",Age = 13,
                 Address = new Address{ City="bei jing", ZipCode="00001"},
                Scores = new []
            {
                new ScoreRecord{ ClassName = "Mathematics", Score = 78, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 80, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 49, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Mathematics", Score = 32, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 69, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 56, Exam = Exams[0]}
            }},
            new Student{ Id="003", Name = null, Age = 17,
                 Address = new Address{ City="xi'an", ZipCode="00002"},
                Scores = new []
            {
                new ScoreRecord{ ClassName = "Mathematics", Score = 18, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 60, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 9, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Mathematics", Score = 40, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 59, Exam = null},
                new ScoreRecord{ ClassName = "English", Score = 30, Exam = null}
            }},
            new Student{ Id="004", Name = "zhao si", Age = 17, Address = new Address()},
            new Student{ Id="005", Name = "qian wu", Age = 20, Scores = new ScoreRecord[0]},
            new Student{ Id="006", Name = "feng wu", Age = 14}
        };

        #region For Model
        [TestClass]
        public class ModelTest
        {
            [TestMethod]
            public void ShouldGetAllWhenFilterIsNull()
            {
                var result = FilterStudentIds(null);
                result.Should().Be("001,002,003,004");
            }

            [DataTestMethod]
            [DataRow("Name", FilterType.Equals, "li si", "002")]
            [DataRow("Name", FilterType.Equals, null, "003")]


            public void ShouldFilterSingleItem(string fieldName, FilterType filterType, object value, string expectedIds)
            {
                TestSdudentForSingleItem(fieldName, filterType, value, expectedIds);
            }

            private string FilterStudentIds(FilterInfo filter)
            {
                var exp = new FilterInfoExpressionBuilder().CreateFilterExpression<Student>(filter);
                var ids = Students.AsQueryable().Where(exp)
                        .Select(p => p.Id);
                return string.Join(",", ids);
            }

            private void TestSdudentForSingleItem(string fieldName, FilterType filterType, object value, string expectedIds)
            {
                var filter = FilterInfo.CreateItem(fieldName, filterType, value);
                var ids = FilterStudentIds(filter);
                string.Join(",", ids).Should().Be(expectedIds);
            }

        }




        #endregion

        [TestClass]
        public class DtoModelTest
        {

            [DataTestMethod]
            [DataRow("", "001,002,003,004,005,006")]
            [DataRow("tName=\"li si\"", "002")]
            [DataRow("tName=null", "003")]
            [DataRow("tAddress!.city=null", "004")]
            public void ShouldFilterSingleItem(string filterExpressionForDto, string expectedIds)
            {
                FilterDtoStudents(filterExpressionForDto).Should().Be(expectedIds);
            }


            [DataTestMethod]
            [DataRow("tAddress=null", "005,006")]
            [DataRow("tAddress!.city=null", "004")]
            [DataRow("tAddress?.city=null", "004,005,006")]
            [DataRow("tAddress?.city!.Length=5", "003,005,006")]
            [DataRow("tAddress?.city?.Length=5", "003,004,005,006")]
            [DataRow("tAddress!.city!.Length=5", "003")]
            [DataRow("tAddress!.city?.Length=5", "003,004")]
            public void ShouldFilterWithOptionalField(string filterExpressionForDto, string expectedIds)
            {
                FilterDtoStudents(filterExpressionForDto).Should().Be(expectedIds);
            }
            [DataTestMethod]

            [DataRow("TName!.Count()=5", "002")]

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

                var exp = new FilterInfoExpressionBuilder().CreateSourceFilterExpression(studentMapper, filter);
                var ids = Students.AsQueryable().Where(exp)
                        .Select(p => p.Id);
                return string.Join(",", ids);
            }

        }

        [TestMethod]
        public void MyTestMethod()
        {
            
            var exp = Students.AsQueryable()
                     .Where(p => p.Name.LongCount()==5);

        }
        public class Address
        {
          
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
            public string ExamName { get; set; }
            public DateTimeOffset ExamDate { get; set; }
        }

        public class Student
        {
            public string Id;
            public string Name;
            public int Age { get; set; }
            public int Height { get; set; }
            public IEnumerable<ScoreRecord> Scores { get; set; }
            public Address Address { get; set; }
        }

        public class ScoreRecord
        {
            public string ClassName { get; set; }
            public decimal Score { get; set; }
            public Exam Exam { get; set; }
        }
    }
}
