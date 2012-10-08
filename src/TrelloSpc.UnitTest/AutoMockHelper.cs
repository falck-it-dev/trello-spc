using Moq;
using NUnit.Framework;
using Ninject;
using Ninject.MockingKernel.Moq;

namespace TrelloSpc.UnitTest
{
    public class AutoMockHelper
    {
        static readonly MoqMockingKernel Kernel = new MoqMockingKernel();

        [SetUp]
        public void Setup()
        {
            Kernel.Reset();
        }

        public T GetInstance<T>()
        {
            return Kernel.Get<T>();
        }

        public Mock<T> GetMock<T>() where T : class
        {
            return Kernel.GetMock<T>();
        }
    }
}