namespace Catel.PoC.ConstructorCheck
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    public class Program
    {
        public static void Main(string[] args)
        {

            const int TestCount = 10000000;

            for (var i = 0; i < TestCount; i++)
            {           
                var testParentClass = new TestParentClass();
                if (testParentClass.Model.ChangeCount != 1)
                {
                    throw new Exception($"Unexpected change count of {testParentClass.Model.ChangeCount}, i = {i}");
                }

                var testClass1 = new ModelWithCheck();
                if (testClass1.ChangeCount != 0)
                {
                    throw new Exception($"Unexpected change count of {testClass1.ChangeCount}, i = {i}");
                }

                var testClass2 = new TestDerivedClass();
                if (testClass2.ChangeCount != 0)
                {
                    throw new Exception($"Unexpected change count of {testClass2.ChangeCount}, i = {i}");
                }

                testClass2.Value = "new value";
                if (testClass2.ChangeCount != 1)
                {
                    throw new Exception($"Unexpected change count of {testClass2.ChangeCount}, i = {i}");
                }
            }

            var summary1 = BenchmarkRunner.Run<Check_vs_WithoutCheck_Single>();
            var summary2 = BenchmarkRunner.Run<Check_vs_WithoutCheck_Multiple>();
        }
    }

    public class TestParentClass
    {
        public TestParentClass()
        {
            Model = new ModelWithCheck();
            Model.Value = "new value";
        }

        public ModelWithCheck Model { get; private set; }
    }

    public class TestDerivedClass : ModelWithCheck
    {
        public TestDerivedClass()
        {
            Value = "new value";
        }
    }

    [SimpleJob(RuntimeMoniker.Net80, baseline: true)]
    [RPlotExporter]
    public class Check_vs_WithoutCheck_Single
    {
        [Benchmark]
        public void WithCheck()
        {
            var model = new ModelWithCheck();

            model.Value = "new value";

            if (model.ChangeCount != 1)
            {
                throw new Exception($"Change count is not 1, but {model.ChangeCount}");
            }
        }

        [Benchmark]
        public void WithoutCheck()
        {
            var model = new ModelWithoutCheck();

            model.Value = "new value";

            if (model.ChangeCount != 2)
            {
                throw new Exception($"Change count is not 2, but {model.ChangeCount}");
            }
        }
    }

    [SimpleJob(RuntimeMoniker.Net80, baseline: true)]
    [RPlotExporter]
    public class Check_vs_WithoutCheck_Multiple
    {
        const int Iterations = 1000;

        [Benchmark]
        public void WithCheck()
        {
            var model = new ModelWithCheck();

            if (model.ChangeCount != 0)
            {
                throw new Exception($"Change count is not correct, expected 0 but got {model.ChangeCount}");
            }

            for (int i = 0; i < Iterations; i++)
            {
                model.Value = $"new value {i}";
            }

            if (model.ChangeCount != Iterations)
            {
                throw new Exception($"Change count is not correct, expected {Iterations} but got {model.ChangeCount}");
            }
        }

        [Benchmark]
        public void WithoutCheck()
        {
            var model = new ModelWithoutCheck();

            for (int i = 0; i < Iterations; i++)
            {
                model.Value = $"new value {i}";
            }

            if (model.ChangeCount != Iterations + 1)
            {
                throw new Exception($"Change count is not correct, expected {Iterations} but got {model.ChangeCount}");
            }
        }
    }
}
