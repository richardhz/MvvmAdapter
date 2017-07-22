
using System;
using Xunit;

namespace UnitTestMvvmAdapters
{
    public class BasicAdapterTest
    {

        private PocoTestClass _tester;

        
        public void Initialize()
        {
            _tester = new PocoTestClass
            {
                TestId = 25,
                TestName = "Roger",
                TestBool = false
            };
        }

        [Fact]
        public void OriginalModelShouldBeInModelProperty()
        {
            Initialize();
            
            var adapter = new PocoTestAdapter(_tester);
            Assert.Equal(_tester, adapter.Model);
        }

        [Fact]
        public void ThrowArgumentNullExceptionIfModelIsNull()
        {
            Initialize();
            try
            {
                var adapter = new PocoTestAdapter(null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.Equal("model", ex.ParamName);
                
            }
            
        }

        [Fact]
        public void ShouldGetValueOfContainedModelProperty()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            Assert.Equal(_tester.TestName, adapter.TestName);
        }

        [Fact]
        public void ModelPropertyShouldBeSetToAdapterProperty()
        {
            Initialize();
            var adapter = new PocoTestAdapter(_tester);
            adapter.TestName = "Oranges";
            Assert.Equal("Oranges", _tester.TestName);
        }
    }
}
