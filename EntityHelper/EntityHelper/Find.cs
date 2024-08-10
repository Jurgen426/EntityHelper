using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCOUNT
{
    public class Find : CodeActivity
    {
        [Input("name")]
        public InArgument<string> _name { get; set; }

        [Input("accountnumber")]
        public InArgument<string> _accountnumber { get; set; }

        [Output("Status")]
        public OutArgument<int> _status { get; set; }

        [Output("account")]
        [ReferenceTarget("account")]
        public OutArgument<EntityReference> _account { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);

            _status.Set(context, (int)ResultStatus.NotFound);
            _account.Set(context, null);

            QueryExpression query = new QueryExpression("account");
            query.ColumnSet = new ColumnSet(false);
            query.AddOrder("createdon", OrderType.Descending);
            query.PageInfo.Count = 2;
            query.PageInfo.PageNumber = 1;
            if (!string.IsNullOrEmpty(_name.Get(context)))
            {
                query.Criteria.AddCondition("name", ConditionOperator.Equal, _name.Get(context));
            }
            if (!string.IsNullOrEmpty(_accountnumber.Get(context)))
            {
                query.Criteria.AddCondition("accountnumber", ConditionOperator.Equal, _accountnumber.Get(context));
            }
            DataCollection<Entity> entities = service.RetrieveMultiple(query).Entities;

            if (entities.Count > 0)
            {
                _account.Set(context, entities[0].ToEntityReference());
                _status.Set(context, entities.Count == 1 ? (int)ResultStatus.OneFound : (int)ResultStatus.ManyFound);
            }
        }
    }

    public enum ResultStatus
    {
        OneFound = 1,
        NotFound = 2,
        ManyFound = 3
    }
}
