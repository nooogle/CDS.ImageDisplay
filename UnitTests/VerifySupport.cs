[assembly: UsesVerify]


namespace UnitTests
{
    [TestClass]
    public partial class VerifySupport
    {
        [TestMethod]
        public Task Run() => VerifyChecks.Run();
    }
}
