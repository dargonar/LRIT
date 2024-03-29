﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataCenterLogic.DDPServerTypes {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://gisis.imo.org/XML/LRIT/2008", ConfigurationName="DDPServerTypes.ddpPortType")]
    public interface ddpPortType {
        
        // CODEGEN: Generating message contract since the operation DDPRequest is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        DataCenterLogic.DDPServerTypes.DDPRequestResponse DDPRequest(DataCenterLogic.DDPServerTypes.DDPRequestRequest request);
        
        // CODEGEN: Generating message contract since the operation Receipt is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        DataCenterLogic.DDPServerTypes.ReceiptResponse Receipt(DataCenterLogic.DDPServerTypes.ReceiptRequest request);
        
        // CODEGEN: Generating message contract since the operation SystemStatus is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        DataCenterLogic.DDPServerTypes.SystemStatusResponse SystemStatus(DataCenterLogic.DDPServerTypes.SystemStatusRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gisis.imo.org/XML/LRIT/ddpRequest/2008")]
    public partial class DDPRequestType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private messageTypeType messageTypeField;
        
        private string messageIdField;
        
        private string referenceIdField;
        
        private DDPRequestTypeUpdateType updateTypeField;
        
        private string archivedDDPVersionNumField;
        
        private System.DateTime archivedDDPTimeStampField;
        
        private bool archivedDDPTimeStampFieldSpecified;
        
        private string originatorField;
        
        private System.DateTime timeStampField;
        
        private string dDPVersionNumField;
        
        private testType testField;
        
        private decimal schemaVersionField;
        
        public DDPRequestType() {
            this.testField = testType.Item0;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public messageTypeType MessageType {
            get {
                return this.messageTypeField;
            }
            set {
                this.messageTypeField = value;
                this.RaisePropertyChanged("MessageType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string MessageId {
            get {
                return this.messageIdField;
            }
            set {
                this.messageIdField = value;
                this.RaisePropertyChanged("MessageId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string ReferenceId {
            get {
                return this.referenceIdField;
            }
            set {
                this.referenceIdField = value;
                this.RaisePropertyChanged("ReferenceId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public DDPRequestTypeUpdateType UpdateType {
            get {
                return this.updateTypeField;
            }
            set {
                this.updateTypeField = value;
                this.RaisePropertyChanged("UpdateType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string ArchivedDDPVersionNum {
            get {
                return this.archivedDDPVersionNumField;
            }
            set {
                this.archivedDDPVersionNumField = value;
                this.RaisePropertyChanged("ArchivedDDPVersionNum");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public System.DateTime ArchivedDDPTimeStamp {
            get {
                return this.archivedDDPTimeStampField;
            }
            set {
                this.archivedDDPTimeStampField = value;
                this.RaisePropertyChanged("ArchivedDDPTimeStamp");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ArchivedDDPTimeStampSpecified {
            get {
                return this.archivedDDPTimeStampFieldSpecified;
            }
            set {
                this.archivedDDPTimeStampFieldSpecified = value;
                this.RaisePropertyChanged("ArchivedDDPTimeStampSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string Originator {
            get {
                return this.originatorField;
            }
            set {
                this.originatorField = value;
                this.RaisePropertyChanged("Originator");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public System.DateTime TimeStamp {
            get {
                return this.timeStampField;
            }
            set {
                this.timeStampField = value;
                this.RaisePropertyChanged("TimeStamp");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public string DDPVersionNum {
            get {
                return this.dDPVersionNumField;
            }
            set {
                this.dDPVersionNumField = value;
                this.RaisePropertyChanged("DDPVersionNum");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(testType.Item0)]
        public testType test {
            get {
                return this.testField;
            }
            set {
                this.testField = value;
                this.RaisePropertyChanged("test");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal schemaVersion {
            get {
                return this.schemaVersionField;
            }
            set {
                this.schemaVersionField = value;
                this.RaisePropertyChanged("schemaVersion");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gisis.imo.org/XML/LRIT/ddpRequest/2008")]
    public enum messageTypeType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        Item9,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://gisis.imo.org/XML/LRIT/ddpRequest/2008")]
    public enum DDPRequestTypeUpdateType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Item0,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Item2,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Item3,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Item4,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gisis.imo.org/XML/LRIT/types/2008")]
    public enum testType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Item0,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gisis.imo.org/XML/LRIT/systemStatus/2008")]
    public partial class SystemStatusType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private messageTypeType2 messageTypeField;
        
        private string messageIdField;
        
        private System.DateTime timeStampField;
        
        private string dDPVersionNumField;
        
        private systemStatusIndicatorType systemStatusField;
        
        private string messageField;
        
        private string originatorField;
        
        private testType testField;
        
        private decimal schemaVersionField;
        
        public SystemStatusType() {
            this.testField = testType.Item0;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public messageTypeType2 MessageType {
            get {
                return this.messageTypeField;
            }
            set {
                this.messageTypeField = value;
                this.RaisePropertyChanged("MessageType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string MessageId {
            get {
                return this.messageIdField;
            }
            set {
                this.messageIdField = value;
                this.RaisePropertyChanged("MessageId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public System.DateTime TimeStamp {
            get {
                return this.timeStampField;
            }
            set {
                this.timeStampField = value;
                this.RaisePropertyChanged("TimeStamp");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string DDPVersionNum {
            get {
                return this.dDPVersionNumField;
            }
            set {
                this.dDPVersionNumField = value;
                this.RaisePropertyChanged("DDPVersionNum");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public systemStatusIndicatorType SystemStatus {
            get {
                return this.systemStatusField;
            }
            set {
                this.systemStatusField = value;
                this.RaisePropertyChanged("SystemStatus");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
                this.RaisePropertyChanged("Message");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string Originator {
            get {
                return this.originatorField;
            }
            set {
                this.originatorField = value;
                this.RaisePropertyChanged("Originator");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(testType.Item0)]
        public testType test {
            get {
                return this.testField;
            }
            set {
                this.testField = value;
                this.RaisePropertyChanged("test");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal schemaVersion {
            get {
                return this.schemaVersionField;
            }
            set {
                this.schemaVersionField = value;
                this.RaisePropertyChanged("schemaVersion");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="messageTypeType", Namespace="http://gisis.imo.org/XML/LRIT/systemStatus/2008")]
    public enum messageTypeType2 {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("11")]
        Item11,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gisis.imo.org/XML/LRIT/systemStatus/2008")]
    public enum systemStatusIndicatorType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Item0,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gisis.imo.org/XML/LRIT/receipt/2008")]
    public partial class ReceiptType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private messageTypeType1 messageTypeField;
        
        private string messageIdField;
        
        private string referenceIdField;
        
        private receiptCodeType receiptCodeField;
        
        private string destinationField;
        
        private string originatorField;
        
        private string messageField;
        
        private System.DateTime timeStampField;
        
        private string dDPVersionNumField;
        
        private testType testField;
        
        private decimal schemaVersionField;
        
        public ReceiptType() {
            this.testField = testType.Item0;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public messageTypeType1 MessageType {
            get {
                return this.messageTypeField;
            }
            set {
                this.messageTypeField = value;
                this.RaisePropertyChanged("MessageType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string MessageId {
            get {
                return this.messageIdField;
            }
            set {
                this.messageIdField = value;
                this.RaisePropertyChanged("MessageId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string ReferenceId {
            get {
                return this.referenceIdField;
            }
            set {
                this.referenceIdField = value;
                this.RaisePropertyChanged("ReferenceId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public receiptCodeType ReceiptCode {
            get {
                return this.receiptCodeField;
            }
            set {
                this.receiptCodeField = value;
                this.RaisePropertyChanged("ReceiptCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string Destination {
            get {
                return this.destinationField;
            }
            set {
                this.destinationField = value;
                this.RaisePropertyChanged("Destination");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string Originator {
            get {
                return this.originatorField;
            }
            set {
                this.originatorField = value;
                this.RaisePropertyChanged("Originator");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
                this.RaisePropertyChanged("Message");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public System.DateTime TimeStamp {
            get {
                return this.timeStampField;
            }
            set {
                this.timeStampField = value;
                this.RaisePropertyChanged("TimeStamp");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public string DDPVersionNum {
            get {
                return this.dDPVersionNumField;
            }
            set {
                this.dDPVersionNumField = value;
                this.RaisePropertyChanged("DDPVersionNum");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(testType.Item0)]
        public testType test {
            get {
                return this.testField;
            }
            set {
                this.testField = value;
                this.RaisePropertyChanged("test");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal schemaVersion {
            get {
                return this.schemaVersionField;
            }
            set {
                this.schemaVersionField = value;
                this.RaisePropertyChanged("schemaVersion");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="messageTypeType", Namespace="http://gisis.imo.org/XML/LRIT/receipt/2008")]
    public enum messageTypeType1 {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Item7,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gisis.imo.org/XML/LRIT/receipt/2008")]
    public enum receiptCodeType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("0")]
        Item0,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Item2,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3")]
        Item3,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4")]
        Item4,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("5")]
        Item5,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("6")]
        Item6,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7")]
        Item7,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("8")]
        Item8,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("9")]
        Item9,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://gisis.imo.org/XML/LRIT/2008")]
    public partial class Response : object, System.ComponentModel.INotifyPropertyChanged {
        
        private responseType responseField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public responseType response {
            get {
                return this.responseField;
            }
            set {
                this.responseField = value;
                this.RaisePropertyChanged("response");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gisis.imo.org/XML/LRIT/2008")]
    public enum responseType {
        
        /// <remarks/>
        Success,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DDPRequestRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gisis.imo.org/XML/LRIT/ddpRequest/2008", Order=0)]
        public DataCenterLogic.DDPServerTypes.DDPRequestType DDPRequest;
        
        public DDPRequestRequest() {
        }
        
        public DDPRequestRequest(DataCenterLogic.DDPServerTypes.DDPRequestType DDPRequest) {
            this.DDPRequest = DDPRequest;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DDPRequestResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gisis.imo.org/XML/LRIT/2008", Order=0)]
        public DataCenterLogic.DDPServerTypes.Response Response;
        
        public DDPRequestResponse() {
        }
        
        public DDPRequestResponse(DataCenterLogic.DDPServerTypes.Response Response) {
            this.Response = Response;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ReceiptRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gisis.imo.org/XML/LRIT/receipt/2008", Order=0)]
        public DataCenterLogic.DDPServerTypes.ReceiptType Receipt;
        
        public ReceiptRequest() {
        }
        
        public ReceiptRequest(DataCenterLogic.DDPServerTypes.ReceiptType Receipt) {
            this.Receipt = Receipt;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ReceiptResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gisis.imo.org/XML/LRIT/2008", Order=0)]
        public DataCenterLogic.DDPServerTypes.Response Response;
        
        public ReceiptResponse() {
        }
        
        public ReceiptResponse(DataCenterLogic.DDPServerTypes.Response Response) {
            this.Response = Response;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SystemStatusRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gisis.imo.org/XML/LRIT/systemStatus/2008", Order=0)]
        public DataCenterLogic.DDPServerTypes.SystemStatusType SystemStatus;
        
        public SystemStatusRequest() {
        }
        
        public SystemStatusRequest(DataCenterLogic.DDPServerTypes.SystemStatusType SystemStatus) {
            this.SystemStatus = SystemStatus;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SystemStatusResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gisis.imo.org/XML/LRIT/2008", Order=0)]
        public DataCenterLogic.DDPServerTypes.Response Response;
        
        public SystemStatusResponse() {
        }
        
        public SystemStatusResponse(DataCenterLogic.DDPServerTypes.Response Response) {
            this.Response = Response;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface ddpPortTypeChannel : DataCenterLogic.DDPServerTypes.ddpPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class ddpPortTypeClient : System.ServiceModel.ClientBase<DataCenterLogic.DDPServerTypes.ddpPortType>, DataCenterLogic.DDPServerTypes.ddpPortType {
        
        public ddpPortTypeClient() {
        }
        
        public ddpPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ddpPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ddpPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ddpPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DataCenterLogic.DDPServerTypes.DDPRequestResponse DataCenterLogic.DDPServerTypes.ddpPortType.DDPRequest(DataCenterLogic.DDPServerTypes.DDPRequestRequest request) {
            return base.Channel.DDPRequest(request);
        }
        
        public DataCenterLogic.DDPServerTypes.Response DDPRequest(DataCenterLogic.DDPServerTypes.DDPRequestType DDPRequest1) {
            DataCenterLogic.DDPServerTypes.DDPRequestRequest inValue = new DataCenterLogic.DDPServerTypes.DDPRequestRequest();
            inValue.DDPRequest = DDPRequest1;
            DataCenterLogic.DDPServerTypes.DDPRequestResponse retVal = ((DataCenterLogic.DDPServerTypes.ddpPortType)(this)).DDPRequest(inValue);
            return retVal.Response;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DataCenterLogic.DDPServerTypes.ReceiptResponse DataCenterLogic.DDPServerTypes.ddpPortType.Receipt(DataCenterLogic.DDPServerTypes.ReceiptRequest request) {
            return base.Channel.Receipt(request);
        }
        
        public DataCenterLogic.DDPServerTypes.Response Receipt(DataCenterLogic.DDPServerTypes.ReceiptType Receipt1) {
            DataCenterLogic.DDPServerTypes.ReceiptRequest inValue = new DataCenterLogic.DDPServerTypes.ReceiptRequest();
            inValue.Receipt = Receipt1;
            DataCenterLogic.DDPServerTypes.ReceiptResponse retVal = ((DataCenterLogic.DDPServerTypes.ddpPortType)(this)).Receipt(inValue);
            return retVal.Response;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DataCenterLogic.DDPServerTypes.SystemStatusResponse DataCenterLogic.DDPServerTypes.ddpPortType.SystemStatus(DataCenterLogic.DDPServerTypes.SystemStatusRequest request) {
            return base.Channel.SystemStatus(request);
        }
        
        public DataCenterLogic.DDPServerTypes.Response SystemStatus(DataCenterLogic.DDPServerTypes.SystemStatusType SystemStatus1) {
            DataCenterLogic.DDPServerTypes.SystemStatusRequest inValue = new DataCenterLogic.DDPServerTypes.SystemStatusRequest();
            inValue.SystemStatus = SystemStatus1;
            DataCenterLogic.DDPServerTypes.SystemStatusResponse retVal = ((DataCenterLogic.DDPServerTypes.ddpPortType)(this)).SystemStatus(inValue);
            return retVal.Response;
        }
    }
}
