﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VNC.Logging.CustomTraceListeners.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://MyCompany.com", ConfigurationName="ServiceReference1.ILiveView")]
    public interface ILiveView {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://MyCompany.com/ILiveView/DisplayLogEntry", ReplyAction="http://MyCompany.com/ILiveView/DisplayLogEntryResponse")]
        void DisplayLogEntry(string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://MyCompany.com/ILiveView/DisplayLogEntry", ReplyAction="http://MyCompany.com/ILiveView/DisplayLogEntryResponse")]
        System.Threading.Tasks.Task DisplayLogEntryAsync(string message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILiveViewChannel : VNC.Logging.CustomTraceListeners.ServiceReference1.ILiveView, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LiveViewClient : System.ServiceModel.ClientBase<VNC.Logging.CustomTraceListeners.ServiceReference1.ILiveView>, VNC.Logging.CustomTraceListeners.ServiceReference1.ILiveView {
        
        public LiveViewClient() {
        }
        
        public LiveViewClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LiveViewClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LiveViewClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LiveViewClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void DisplayLogEntry(string message) {
            base.Channel.DisplayLogEntry(message);
        }
        
        public System.Threading.Tasks.Task DisplayLogEntryAsync(string message) {
            return base.Channel.DisplayLogEntryAsync(message);
        }
    }
}
