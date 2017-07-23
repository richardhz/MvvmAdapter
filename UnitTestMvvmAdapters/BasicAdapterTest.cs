using System.Collections.Generic;
using System;
using Xunit;

namespace UnitTestMvvmAdapters
{
    public class BasicAdapterTest
    {

        private PocoTestClass _tester;
        private PocoListItem _itemList;


        public void Initialize()
        {
            _itemList = new PocoListItem { Id = 2, Title = "TestItem02", Description = "Description02" };
            _tester = new PocoTestClass
            {
                TestId = 25,
                TestName = "Roger",
                TestBool = false,
                Items = new List<PocoListItem>
                {
                    new PocoListItem { Id = 1, Title = "TestItem01", Description = "Description01" },
                    _itemList
                }
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
