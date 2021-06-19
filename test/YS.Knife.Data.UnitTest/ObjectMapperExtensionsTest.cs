using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class ObjectMapperExtensionsTest
    {

        private static readonly Exam[] Exams = 
        {
            new Exam {ExamName = "mid term", ExamDate = DateTimeOffset.Parse("2021-01-01")},
            new Exam {ExamName = "final exam", ExamDate = DateTimeOffset.Parse("2021-07-01")}
        };
        private static readonly List<Student> Students = new List<Student>
        {
            new Student{ Id="001", Name = "zhang san",Age = 15,Scores = new []
            {
                new ScoreRecord{ ClassName = "Mathematics", Score = 98, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 89, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 100, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Mathematics", Score = 99, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 95, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 98, Exam = Exams[0]}
            }},
            new Student{ Id="002", Name = "li si",Age = 13,Scores = new []
            {
                new ScoreRecord{ ClassName = "Mathematics", Score = 78, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 80, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 49, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Mathematics", Score = 32, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 69, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 56, Exam = Exams[0]}
            }},
            new Student{ Id="003", Name = null, Age = 17, Scores = new []
            {
                new ScoreRecord{ ClassName = "Mathematics", Score = 18, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 60, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "English", Score = 9, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Mathematics", Score = 40, Exam = Exams[0]},
                new ScoreRecord{ ClassName = "Chinese", Score = 59, Exam = null},
                new ScoreRecord{ ClassName = "English", Score = 30, Exam = null}
            }},
            new Student{ Id="004", Name = "wang wu", Age = 16, Scores = null}
        };
        
        


        [TestMethod]
        public void ShouldGetAllWhenFilterIsNull()
        {
            var result = FilterStudentData(null);
            result.Should().Be("001,002,003,004");
        }
        [DataTestMethod]
       // [DataRow("001","Name",FilterType.Equals,"zhang san")]
       // [DataRow("003","Name",FilterType.Equals, null)]
        [DataRow("003","Address.City",FilterType.Equals, null)]
        public void ShouldFilterWithBasicSingleItem(string expectedId, string field, FilterType filterType,
            object value)
        {
            var filter = FilterInfo.CreateItem(field, filterType, value);
            FilterStudentData(filter).Should().Be(expectedId);
        }

    
        private string FilterStudentData(FilterInfo studentDtoFilter)
        {
            var exp = ObjectMapper<Student, StudentDto>.Default.CreateSourceFilterExpression(studentDtoFilter);
            var query = Students.AsQueryable().Where(exp);
            return string.Join(",", query.Select(p => p.Id));
        }

        
        public struct Address
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
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age{get;set;}
            public int? Height { get; set; }
            public List<ScoreDto> Scores { get; set; }
            public Address Address { get; set; }
        }

        public class ScoreDto
        {
            public string ClassName { get; set; }
            public decimal Score { get; set; }
            public IExam Exam { get; set; }
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
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age{get;set;}
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
