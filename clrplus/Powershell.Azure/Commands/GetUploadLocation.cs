﻿//-----------------------------------------------------------------------
// <copyright company="CoApp Project">
//     Copyright (c) 2010-2013 Garrett Serack and CoApp Contributors. 
//     Contributors can be discovered using the 'git log' command.
//     All rights reserved.
// </copyright>
// <license>
//     The software is licensed under the Apache 2.0 License (the "License")
//     You may not use the software except in compliance with the License. 
// </license>
//-----------------------------------------------------------------------

namespace ClrPlus.Powershell.Azure.Commands {
    using System;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using ClrPlus.Core.Extensions;
    using Core;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;
    #if USING_RESTABLE_CMDLET
    using Rest.Commands;
#endif 

    [Cmdlet(VerbsCommon.Get, "UploadLocation")]
#if USING_RESTABLE_CMDLET
    public class GetUploadLocation : RestableCmdlet<GetUploadLocation> {
#else 
    public class GetUploadLocation : BaseCmdlet {
#endif
        public const string DELETEABLE = "deleteable";

        [Parameter]
        [ValidateNotNullOrEmpty]
        public PSCredential AzureStorageCredential { get; set;}

        protected override void ProcessRecord() {
#if USING_RESTABLE_CMDLET
            // must use this to support processing record remotely.
            if (Remote)
            {
                ProcessRecordViaRest();
                return;
            }
#endif
            //this actually connects to the Azure service
            CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials(AzureStorageCredential.UserName, AzureStorageCredential.Password.ToUnsecureString()), true);

            var contName = "deletable" + Guid.NewGuid().ToString("D").ToLowerInvariant();

            var container = account.CreateCloudBlobClient().GetContainerReference(contName);
            try {
                container.CreateIfNotExists();
            } catch (StorageException e) {
                
                WriteError(new ErrorRecord(e, "0", ErrorCategory.ConnectionError, null));
                return;
            }
            

            WriteObject(container.Name);
            WriteObject(container.Uri);
        }
    }
}