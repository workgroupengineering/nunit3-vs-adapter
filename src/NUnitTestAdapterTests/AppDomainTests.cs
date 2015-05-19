
using System;
using System.IO;
using System.Reflection;
using NUnit.Engine;
using NUnit.Framework;

namespace NUnit.VisualStudio.TestAdapter.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]

        public void CreateEngine()
        {
            var ad = AppDomain.CreateDomain("TestEngine");
            var enginewrapper = ad.CreateInstanceAndUnwrap(typeof(TestEngine).Assembly.FullName,typeof(TestEngine).FullName);
            Assert.That(enginewrapper, Is.Not.Null);
        }

        [Test]

        public void CreateEngine2()
        {
            var ad = AppDomain.CreateDomain("TestEngine");
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(dir, "nunit.core.engine.dll");
            var engine = ad.CreateInstanceFromAndUnwrap(path, "NUnit.Engine.TestEngine") as ITestEngine;
            Assert.That(engine, Is.Not.Null);
        }


        [Test]

        public void CreateEngineInCurrentDomain()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(dir, "nunit.core.engine.dll");
            var engine = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(path, "NUnit.Engine.TestEngine") as ITestEngine;
            Assert.That(engine, Is.Not.Null);
        }


        [Test]
        public void CreateEngingeWrapper()
        {

            var adsetup = new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) };

            var evidence = AppDomain.CurrentDomain.Evidence;

            var ad = AppDomain.CreateDomain("TestEngine",evidence,adsetup);
            var enginewrapperObject = ad.CreateInstanceAndUnwrap(typeof (LocalTestEngineWrapper).Assembly.FullName,
                typeof (LocalTestEngineWrapper).FullName);
            Assert.That(enginewrapperObject,Is.Not.Null);
            var enginewrapper = enginewrapperObject as LocalTestEngineWrapper;
            Assert.That(enginewrapper,Is.Not.Null);
            var engine = enginewrapper.TestEngine;
            Assert.That(engine,Is.Not.Null);
        }


        [Test]
        public void CreateEngingeWrapper2()
        {

            var adsetup = new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) };

            var evidence = AppDomain.CurrentDomain.Evidence;

            var ad = AppDomain.CreateDomain("TestEngine", evidence, adsetup);
            var enginewrapperObject = ad.CreateInstanceAndUnwrap(typeof(NUnit.EngineWrapper.TestEngineWrapper).Assembly.FullName,
                typeof(NUnit.EngineWrapper.TestEngineWrapper).FullName);
            Assert.That(enginewrapperObject, Is.Not.Null);
            var enginewrapper = enginewrapperObject as NUnit.EngineWrapper.TestEngineWrapper;
            Assert.That(enginewrapper, Is.Not.Null);
        }

    }

    [Serializable]
    public class LocalTestEngineWrapper : MarshalByRefObject
    {
        public ITestEngine TestEngine { get; private set; }

        public LocalTestEngineWrapper()
        {
            TestEngine = new TestEngine();
        }

        public void Unload()
        {
            TestEngine.Dispose();
            TestEngine = null;
        }

        public TestEngine Load()
        {
            return null;
        }
    }
}
