namespace Laboratorna_11_4.Tests;

[TestFixture]
public class Tests
{
    private const string TestFileName = "test";

    [Test]
    public void TestReadWriteToFile()
    {
        List<StudentWrapper> studentsToWrite = new List<StudentWrapper>
        {
            new StudentWrapper { Data = new Student { LastName = "Smith", Exam1 = 80, Exam2 = 75, Exam3 = 90 } },
            new StudentWrapper { Data = new Student { LastName = "Johnson", Exam1 = 85, Exam2 = 88, Exam3 = 92 } }
        };

        Program.Write(TestFileName, studentsToWrite);

        var studentsRead = Program.Read(TestFileName);

        Assert.IsNotNull(studentsRead);
    }

    [TearDown]
    public void CleanUp()
    {
        if (File.Exists(TestFileName))
        {
            File.Delete(TestFileName);
        }
    }
}