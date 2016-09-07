using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Assignment
{
    class ContratoAsucursal : CodeActivity
    {
        [Input("Supervisor")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> user { get; set; }

        
    }
}
