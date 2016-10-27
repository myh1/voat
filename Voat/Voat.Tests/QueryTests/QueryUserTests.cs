﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voat.Common;
using Voat.Data;
using Voat.Domain.Command;
using Voat.Domain.Query;
using Voat.Tests.Repository;

namespace Voat.Tests.QueryTests
{
    [TestClass]
    public class QueryUserTests 
    {
        
        [TestMethod, TestCategory("UserData"), ExpectedException(typeof(ArgumentException))]
        public void UserData_ValidateUser_Constructor_1()
        {
            string user = "";
            var voatData = new Domain.UserData(user, true);
        }
        [TestMethod, TestCategory("UserData"), ExpectedException(typeof(ArgumentException))]
        public void UserData_ValidateUser_Constructor_2()
        {
            string user = null;
            var voatData = new Domain.UserData(user, true);

        }
        [TestMethod, TestCategory("UserData"), ExpectedException(typeof(ArgumentException))]
        public void UserData_ValidateUser_Constructor_3()
        {
            string user = "____________Doesn't__Exist_________";
            var voatData = new Domain.UserData(user, true);
        }

        [TestMethod, TestCategory("UserData"), ExpectedException(typeof(VoatNotFoundException))]
        public void UserData_ValidateUser_Constructor_4()
        {
            string user = "MyName-Is-Me";
            var voatData = new Domain.UserData(user, true);
        }
        [TestMethod, TestCategory("UserData"), ExpectedException(typeof(VoatNotFoundException))]
        public void UserData_ValidateUser_Constructor_5()
        {
            string user = "DoesntExist";
            var voatData = new Domain.UserData(user, true);
        }

        [TestMethod]
        [TestCategory("UserData"), TestCategory("Subscriptions")]
        public async Task UserData_Information_SubscriptionTests()
        {
            //ensure users with no subscriptions don't error out
            var noSubUserName = "NoSubs";
            VoatDataInitializer.CreateUser(noSubUserName);

            var userData = new Domain.UserData(noSubUserName);
            //var userData = new Domain.UserData(noSubUserName);
            Assert.AreEqual(0, userData.Subscriptions.Count());

            //test subscription
            var subUserName = "HasSubs";
            VoatDataInitializer.CreateUser(subUserName);

            TestHelper.SetPrincipal(subUserName);
            var cmd = new SubscriptionCommand(Domain.Models.DomainType.Subverse, Domain.Models.SubscriptionAction.Subscribe, "unit");
            var x = await cmd.Execute();

            userData = new Domain.UserData(subUserName);

            Assert.AreEqual(1, userData.Subscriptions.Count());
            Assert.AreEqual("unit", userData.Subscriptions.First());

        }
        [TestMethod]
        [TestCategory("Query"), TestCategory("Query.Comment"), TestCategory("Comment"), TestCategory("UserData")]
        public async Task QueryUserComments()
        {
            var userName = "UnitTestUser21";
            TestHelper.SetPrincipal(userName);
            var cmd = new CreateCommentCommand(1, null, "My pillow looks like jello");
            var x = await cmd.Execute();
            Assert.AreEqual(Status.Success, x.Status);

            var q = new QueryUserComments(userName, SearchOptions.Default);
            var r = await q.ExecuteAsync();
            Assert.AreEqual(true, r.Any(w => w.Content == "My pillow looks like jello"));

        }
        [TestMethod]
        [TestCategory("Query"), TestCategory("Query.Comment"), TestCategory("Anon"), TestCategory("Comment"), TestCategory("Comment.Segment")]
        public async Task QueryUserComments_Anon()
        {
            var userName = "UnitTestUser22";
            TestHelper.SetPrincipal(userName);
            var cmd = new CreateCommentCommand(2, null, "You can never know I said this: Bollocks");
            var x = await cmd.Execute();
            Assert.AreEqual(Status.Success, x.Status);

            var q = new QueryUserComments(userName, SearchOptions.Default);
            var r = await q.ExecuteAsync();
            Assert.AreEqual(false, r.Any(w => w.Content == "You can never know I said this: Bollocks"));
        }

        [TestMethod]
        [TestCategory("Query"), TestCategory("Query.Submission"), TestCategory("Submission"), TestCategory("UserData")]
        public async Task QueryUserSubmissions()
        {
            Assert.Inconclusive("TODO");
        }


        [TestMethod]
        [TestCategory("Query"), TestCategory("Query.Comment"), TestCategory("Anon"), TestCategory("Comment"), TestCategory("Comment.Segment")]
        public async Task QueryUserSubmissions_Anon()
        {
            Assert.Inconclusive();
        }
    }
}
