using System;
using Xunit;
using System.Collections.Generic;
using ContactDiary.Models;
using ContactDiary.Controllers;

namespace ContactDiary.Tests.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var testContacts = GetTestContacts();
            var controller = new ContactController(testContacts);

            var result = controller.GetAll();
            Assert.Equal(testContacts.Count, result.Count);
        }

        private List<ContactItem> GetTestContacts()
        {
            var testContacts = new List<ContactItem>();
            testContacts.Add(new ContactItem { Id = 1, Name = "John", Number = "9876543211", Email = "john@test.com" });
            testContacts.Add(new ContactItem { Id = 2, Name = "Jack", Number = "9876543212", Email = "jack@test.com" });
            testContacts.Add(new ContactItem { Id = 3, Name = "Neil", Number = "9876543213", Email = "neil@test.com" });
            testContacts.Add(new ContactItem { Id = 4, Name = "Sara", Number = "9876543214", Email = "sara@test.com" });

            return testContacts;
        }
    }
}
