using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp3;

namespace HealthCheckTest2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        // This method tests RegexMatch function in HealthCheck.cs - valid function for url str
        public void ValidRegexMatchUrlRegexTest()
        {
            string StrToCheck = "https://prime-dsa-trial.docusign.net:8081/sapiws/v1/";
            string StrUrlRegex = "https:\\/\\/[a-z,-]*.docusign*.net:[a-z,\\/, 0-9]*";
            int identifier = 0;
            bool result = ConsoleApp3.HealthCheck.RegexMatch(StrToCheck, StrUrlRegex, identifier);

            Assert.AreEqual(true, result);

        }

        // This method tests RegexMatch function in HealthCheck.cs - valid function for config file str
        [TestMethod]
        public void ValidRegexMatchConfigRegexTest()
        {
            string StrToCheck = "file.json";
            string StrconfigRegex = ".*.json";
            int identifier = 1;
            bool result = ConsoleApp3.HealthCheck.RegexMatch(StrToCheck, StrconfigRegex, identifier);

            Assert.AreEqual(true, result);
        }

        // This method tests RegexMatch function in HealthCheck.cs - invalid identifier
        [TestMethod]
        public void InvalidIdentifierTest()
        {
            string StrToCheck = "file.json";
            string StrRegexString = ".*.json";
            int identifier = 2;
            bool result = ConsoleApp3.HealthCheck.RegexMatch(StrToCheck, StrRegexString, identifier);

            Assert.AreEqual(false, result);
        }
    }
}
