namespace Nsinsert.Tests
{
    public class InserterTests
    {
        [Fact]
        public void InsertsCorrectly()
        {
            string[] allLines = File.ReadAllLines("template.nsi");
            var inserter = new Inserter(allLines);
            List<string> insertResult = inserter.Insert();
            allLines = File.ReadAllLines("expected.nsi");

            for(int i = 0; i < allLines.Length; i++)
            {
                string expected = insertResult[i];
                string actual = allLines[i];
                Assert.Equal(expected, actual);
            }
        }

        

    }
}
