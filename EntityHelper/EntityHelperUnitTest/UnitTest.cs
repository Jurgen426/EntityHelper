using FakeXrmEasy;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Middleware;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.CodeActivities;
using ACCOUNT;

namespace EntityHelperTest
{
    [TestClass]
    public class UnitTest
    {
        IXrmFakedContext context = MiddlewareBuilder
                       .New()
                       .AddCrud()
                       .UseCrud()
                       .SetLicense(FakeXrmEasyLicense.NonCommercial)
                       .Build();

        List<Entity> eAccounts = new List<Entity>()
        {
            new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes = new Microsoft.Xrm.Sdk.AttributeCollection()
                {
                    {"name", "account1"},
                    {"accountnumber", "11111111"},
                }
            },
            new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes = new Microsoft.Xrm.Sdk.AttributeCollection()
                {
                    {"name", "account2"},
                    {"accountnumber", "222222222"},
                }
            },
            new Entity("account")
            {
                Id = Guid.NewGuid(),
                Attributes = new Microsoft.Xrm.Sdk.AttributeCollection()
                {
                    {"name", "account3"},
                    {"accountnumber", "333333333"},
                }
            }
        };

        [TestMethod]
        public void NotFoundAccounts()
        {
            context.Initialize(eAccounts);

            var inputs = new Dictionary<string, object>() {
                { "_name", "" },
                { "_accountnumber", "111" }
            };

            var result = context.ExecuteCodeActivity<Find>(inputs, null);
            Assert.IsTrue(
                (int)result["_status"] == (int)ResultStatus.NotFound && (EntityReference)result["_account"] == null,
                $"status: {(int)result["_status"]}\n"
            );
        }

        [TestMethod]
        public void OneAccountFound()
        {
            context.Initialize(eAccounts);

            var inputs = new Dictionary<string, object>() {
                { "_name", "" },
                { "_accountnumber", "222222222" }
            };

            var result = context.ExecuteCodeActivity<Find>(inputs, null);
            Assert.IsTrue(
                (int)result["_status"] == (int)ResultStatus.OneFound && (EntityReference)result["_account"] != null,
                $"status: {(int)result["_status"]}\n"
            );
        }

        [TestMethod]
        public void ManyAccountFound()
        {
            context.Initialize(eAccounts);

            var inputs = new Dictionary<string, object>() {
                { "_name", "" },
                { "_accountnumber", "" }
            };

            var result = context.ExecuteCodeActivity<Find>(inputs, null);
            Assert.IsTrue(
                (int)result["_status"] == (int)ResultStatus.ManyFound && (EntityReference)result["_account"] != null,
                $"status: {(int)result["_status"]}\n"
            );
        }
    }
}
