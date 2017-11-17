﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.296.
// 
#pragma warning disable 1591

namespace Parsers.CmsServiceService1 {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="IAI_CmsServiceSoapBinding", Namespace="http://rtsiai01.riits.net/IAI/services/IAI_CmsService")]
    public partial class CmsServiceService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback initializeOperationCompleted;
        
        private System.Threading.SendOrPostCallback getCmsInfoOperationCompleted;
        
        private System.Threading.SendOrPostCallback refreshUsersOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public CmsServiceService() {
            this.Url = global::Parsers.Properties.Settings.Default.Parsers_CmsServiceService_CmsServiceService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event initializeCompletedEventHandler initializeCompleted;
        
        /// <remarks/>
        public event getCmsInfoCompletedEventHandler getCmsInfoCompleted;
        
        /// <remarks/>
        public event refreshUsersCompletedEventHandler refreshUsersCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://external.regional.services.nateng.com", ResponseNamespace="http://rtsiai01.riits.net/IAI/services/IAI_CmsService")]
        public void initialize(string agencyName, string serviceName, string configFile) {
            this.Invoke("initialize", new object[] {
                        agencyName,
                        serviceName,
                        configFile});
        }
        
        /// <remarks/>
        public void initializeAsync(string agencyName, string serviceName, string configFile) {
            this.initializeAsync(agencyName, serviceName, configFile, null);
        }
        
        /// <remarks/>
        public void initializeAsync(string agencyName, string serviceName, string configFile, object userState) {
            if ((this.initializeOperationCompleted == null)) {
                this.initializeOperationCompleted = new System.Threading.SendOrPostCallback(this.OninitializeOperationCompleted);
            }
            this.InvokeAsync("initialize", new object[] {
                        agencyName,
                        serviceName,
                        configFile}, this.initializeOperationCompleted, userState);
        }
        
        private void OninitializeOperationCompleted(object arg) {
            if ((this.initializeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.initializeCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://external.regional.services.nateng.com", ResponseNamespace="http://rtsiai01.riits.net/IAI/services/IAI_CmsService")]
        [return: System.Xml.Serialization.SoapElementAttribute("getCmsInfoReturn")]
        public string getCmsInfo(string userName, string userPass, string xmlMessageRequest) {
            object[] results = this.Invoke("getCmsInfo", new object[] {
                        userName,
                        userPass,
                        xmlMessageRequest});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getCmsInfoAsync(string userName, string userPass, string xmlMessageRequest) {
            this.getCmsInfoAsync(userName, userPass, xmlMessageRequest, null);
        }
        
        /// <remarks/>
        public void getCmsInfoAsync(string userName, string userPass, string xmlMessageRequest, object userState) {
            if ((this.getCmsInfoOperationCompleted == null)) {
                this.getCmsInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetCmsInfoOperationCompleted);
            }
            this.InvokeAsync("getCmsInfo", new object[] {
                        userName,
                        userPass,
                        xmlMessageRequest}, this.getCmsInfoOperationCompleted, userState);
        }
        
        private void OngetCmsInfoOperationCompleted(object arg) {
            if ((this.getCmsInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getCmsInfoCompleted(this, new getCmsInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="http://services.nateng.com", ResponseNamespace="http://rtsiai01.riits.net/IAI/services/IAI_CmsService")]
        public void refreshUsers(string usr, string pwd, string agency) {
            this.Invoke("refreshUsers", new object[] {
                        usr,
                        pwd,
                        agency});
        }
        
        /// <remarks/>
        public void refreshUsersAsync(string usr, string pwd, string agency) {
            this.refreshUsersAsync(usr, pwd, agency, null);
        }
        
        /// <remarks/>
        public void refreshUsersAsync(string usr, string pwd, string agency, object userState) {
            if ((this.refreshUsersOperationCompleted == null)) {
                this.refreshUsersOperationCompleted = new System.Threading.SendOrPostCallback(this.OnrefreshUsersOperationCompleted);
            }
            this.InvokeAsync("refreshUsers", new object[] {
                        usr,
                        pwd,
                        agency}, this.refreshUsersOperationCompleted, userState);
        }
        
        private void OnrefreshUsersOperationCompleted(object arg) {
            if ((this.refreshUsersCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.refreshUsersCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void initializeCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void getCmsInfoCompletedEventHandler(object sender, getCmsInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getCmsInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getCmsInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void refreshUsersCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591