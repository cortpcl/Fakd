namespace MailChimp {
    using System;
    using System.Collections;
    using CookComputing.XmlRpc;
    using System.Runtime.CompilerServices;

    public class ApiWrapper {
        private IMC api;
        string apikey;
        string datacenter;
        bool secure;
        
        public ApiWrapper(){
            this.api = XmlRpcProxyGen.Create<IMC>();
            this.api.UserAgent = "MailChimp.Net/1.1";
            this.api.Timeout = 90000; //default = 90 second timeout
            this.secure = false;
            this.datacenter = "us1";
            this.buildEndPoint();
        }
        
        /**
         ** Non-API method. Based on settings from setSecure() and setCurrentApiKey(), determine
         ** where the request should be sent.
         ** setSecure() method definition
         **/
        private void buildEndPoint(){
            string url;
            if (this.secure){
                url = "https://";
            } else {
                url = "http://";
            }
            this.api.Url = url + this.datacenter + ".api.mailchimp.com/1.1/";
        }
        
        /**
         ** Non-API method. Allows the user to manually specify HTTP or HTTPS usage
         ** setSecure() method definition
         **/
         public void setSecure(bool use_secure){
            this.secure = use_secure;
            this.buildEndPoint();
         }
         
        /**
         ** Non-API method. Allows the user to set API Key and skip login() calls
         ** setCurrentApiKey() method definition
         **/
        public void setCurrentApiKey(string apikey){
            string key = "";
            string dc = "us1";
            int i = 0;
            string[] parts = apikey.Split('-');
            foreach (string part in parts){
                if (i==0) key = part;
                if (i==1) dc = part;
                i++;
            }
            this.datacenter = dc;
            this.apikey = key;
            this.buildEndPoint();
        }

        /**
         ** Non-API method. Allows the user to get the API Key currently being used for calls
         ** getCurrentApiKey() method definition
         **/
        public string getCurrentApiKey(){
            return this.apikey;
        }
        
        /**
         ** Non-API method. Allows the user to set the Connection Timeout to be used (in milliseconds)
         ** setTimeout() method definition
         **/
        public void setTimeout(int t){
            this.api.Timeout = t;
        }
        
        /**
         ** Non-API method. Allows the user to retrieve the current Connection Timeout setting (in milliseconds)
         ** getTimeout() method definition
         **/
        public int getTimeout(){
            return this.api.Timeout;
        }
        
        /**
         ** Non-API method. Allows the user to retrieve current Url being connected to 
         ** getUrl() method definition
         **/
        public string getUrl(){
            return this.api.Url;
        }


        /**
         ** apikeys() method definition
         **/
        public MCApiKey[] apikeys(string username, string password, string apikey){
            return this.apikeys(username, password, apikey, false);
        }
        public MCApiKey[] apikeys(string username, string password, string apikey, bool expired){
            return this.api.apikeys(username, password, apikey, expired);
        }

        /**
         ** apikeyAdd() method definition
         **/
        public string apikeyAdd(string username, string password, string apikey){
            return this.api.apikeyAdd(username, password, apikey);
        }

        /**
         ** apikeyDel() method definition
         **/
        public bool apikeyExpire(string username, string password, string apikey){
            return this.api.apikeyExpire(username, password, apikey);
        }

        /**
         ** lists method definition
         **/
        public MCList[] lists(){
            try {
                return this.api.lists(this.apikey);
            } catch(XmlRpcTypeMismatchException){
                return new MCList[0];
            }
        }

        /**
         ** listMergeVars method definitions
         **/
        public MCMergeVar[] listMergeVars(string listId){
            return this.api.listMergeVars(this.apikey, listId);
        }
        
        /**
         ** listMergeVarAdd method definitions
         **/
        public bool listMergeVarAdd(string listId, string tag, string name){
            return this.listMergeVarAdd(listId, tag, name, false);
        }        
        public bool listMergeVarAdd(string listId, string tag, string name, bool req){
            return this.api.listMergeVarAdd(this.apikey, listId, tag, name, req);
        }

        /**
         ** listMergeVarDel method definitions
         **/
        public bool listMergeVarDel(string listId, string tag){
            return this.api.listMergeVarDel(this.apikey, listId, tag);
        }        
                
        /**
         ** listInterestGroups method definition
         **/
        public MCInterestGroups listInterestGroups(string listId){
            return this.api.listInterestGroups(this.apikey, listId);
        }

        /**
         ** listInterestGroupAdd method definitions
         **/
        public bool listInterestGroupAdd(string listId, string group_name){
            return this.api.listInterestGroupAdd(this.apikey, listId, group_name);
        }
        
        /**
         ** listInterestGroupAdd method definitions
         **/
        public bool listInterestGroupDel(string listId, string group_name){
            return this.api.listInterestGroupDel(this.apikey, listId, group_name);
        }
        
        /**
         ** listMemberInfo method definitions
         **/
        public MCMemberInfo listMemberInfo(string listId, string email_address){
            DummyMCMemberInfo tmp = this.api.listMemberInfo(this.apikey, listId, email_address);
            XmlRpcStruct s = (XmlRpcStruct)tmp.merges;
            MCMergeVar[] vars = new MCMergeVar[((Hashtable)s).Count];
            MCMemberInfo toret = new MCMemberInfo();
            //copy everything normal over
            toret.email = tmp.email;
            toret.email_type = tmp.email_type;
            toret.status = tmp.status;
            toret.timestamp = tmp.timestamp;
            int i = 0;
            foreach (string tag in s.Keys){
                MCMergeVar v = new MCMergeVar();
                v.tag = tag;
                v.val  = (string)s[tag];
                vars[i] =  v ;
                i++;
            }
            toret.merges = vars;
            return toret;

        }
        
        /**
         ** listMembers method definitions
         **/
        public MCListMember[] listMembers(string id){
            return this.listMembers(id, "subscribed");
        }
        public MCListMember[] listMembers(string id, string status){
            return this.listMembers(id, status, 0, 100);
        }
        public MCListMember[] listMembers(string id, string status, int start, int limit){
            try {
                return this.api.listMembers(this.apikey, id, status, start, limit);
            } catch(XmlRpcTypeMismatchException){
                return new MCListMember[0];
            }
        }
        
        /**
         ** listSubscribe method definitions
         **/
        public bool listSubscribe(string id, string email_address){
            return this.listSubscribe(id, email_address, new MCMergeVar[0]);
        }
        public bool listSubscribe(string id, string email_address, MCMergeVar[] merges){
            return this.listSubscribe(id, email_address, merges, "html");
        }
        public bool listSubscribe(string id, string email_address, MCMergeVar[] merges, string email_type){
            return this.listSubscribe(id, email_address, merges, email_type, true);
        }
        public bool listSubscribe(string id, string email_address, MCMergeVar[] merges, string email_type, bool double_optin){
            XmlRpcStruct mv = this.mergeArrayToStruct(merges);
            return this.api.listSubscribe(this.apikey, id, email_address, mv, email_type, double_optin);
        }

        /**
         ** listUnsubscribe method definitions
         **/
        public bool listUnsubscribe(string id, string email_address){
            return this.listUnsubscribe(id, email_address, false);
        }
        public bool listUnsubscribe(string id, string email_address, bool delete_member){
            return this.listUnsubscribe(id, email_address, delete_member, true);
        }
        public bool listUnsubscribe(string id, string email_address, bool delete_member, bool send_goodbye){
            return this.listUnsubscribe(id, email_address, delete_member, send_goodbye, true);
        }
        public bool listUnsubscribe(string id, string email_address, bool delete_member, bool send_goodbye, bool send_notify){
            return this.api.listUnsubscribe(this.apikey, id, email_address, delete_member, send_goodbye, send_notify);
        }

        
        
        /**
         ** listBatchSubscribe method definitions
         **/
        public MCBatchResult listBatchSubscribe(string id, MCMemberInfo[] batch){
            return this.listBatchSubscribe(id, batch, true);
        }
        public MCBatchResult listBatchSubscribe(string id, MCMemberInfo[] batch, bool double_optin){
            return this.listBatchSubscribe(id, batch, double_optin, false);
        }
        public MCBatchResult listBatchSubscribe(string id, MCMemberInfo[] batch, bool double_optin, bool update_existing){
            return this.listBatchSubscribe(id, batch, double_optin, update_existing, true);
        }
        public MCBatchResult listBatchSubscribe(string id, MCMemberInfo[] batch, bool double_optin, bool update_existing, bool replace_interests){
            //massage the input
            XmlRpcStruct xBatch = new XmlRpcStruct();
            int i = 0;
            foreach(MCMemberInfo info in batch){
                XmlRpcStruct mv = new XmlRpcStruct();
                mv.Add("EMAIL", info.email);
                mv.Add("EMAIL_TYPE", info.email_type);
                foreach(MCMergeVar var in info.merges){
                    mv.Add(var.tag, var.val);
                }
                xBatch.Add(i+"",mv);
                i++;
            }
            //make the call
            DummyMCBatchResult res = this.api.listBatchSubscribe(this.apikey, id, xBatch, double_optin, update_existing, replace_interests);

            //massage the output
            MCBatchResult toret = new MCBatchResult();
            toret.success_count = res.success_count;
            toret.error_count = res.error_count;
            toret.errors = new MCEmailResult[ res.errors.Length ];
            i = 0;
            foreach(DummyMCEmailResult err in res.errors){
                MCEmailResult tmp = new MCEmailResult();
                //copy everything normal over
                tmp.code = err.code;
                tmp.message = err.message;
                
                XmlRpcStruct s = (XmlRpcStruct)err.row;
                MCMemberInfo info = new MCMemberInfo();
                info.merges = new MCMergeVar[((Hashtable)s).Count];
                int j = 0;
                foreach (string tag in s.Keys){
                    if (tag == "EMAIL"){
                        info.email = (string)s[tag];
                    }
                    if (tag == "EMAIL_TYPE"){
                        info.email_type = (string)s[tag];
                    }
                    info.merges[j] = new MCMergeVar();
                    info.merges[j].tag = tag;
                    info.merges[j].val  = (string)s[tag];
                    j++;
                }
                tmp.row = info;
                toret.errors[i] = tmp;
                i++;
            }
            return toret;
        }//listBatchSubscribe

        /**
         ** listBatchUnsubscribe method definitions
         **/
        public MCBatchResult listBatchUnsubscribe(string id, MCMemberInfo[] batch){
            return this.listBatchUnsubscribe(id, batch, false);
        }
        public MCBatchResult listBatchUnsubscribe(string id, MCMemberInfo[] batch, bool delete_member){
            return this.listBatchUnsubscribe(id, batch, delete_member, true);
        }
        public MCBatchResult listBatchUnsubscribe(string id, MCMemberInfo[] batch, bool delete_member, bool send_goodbye){
            return this.listBatchUnsubscribe(id, batch, delete_member, send_goodbye, false);
        }
        public MCBatchResult listBatchUnsubscribe(string id, MCMemberInfo[] batch, bool delete_member, bool send_goodbye, bool send_notify){
            //massage the input
            XmlRpcStruct xBatch = new XmlRpcStruct();
            int i = 0;
            foreach(MCMemberInfo info in batch){
                xBatch.Add(i+"", info.email );
                i++;
            }
            //make the call
            DummyMCBatchResult res = this.api.listBatchUnsubscribe(this.apikey, id, xBatch, delete_member, send_goodbye, send_notify);
            //massage the output
            MCBatchResult toret = new MCBatchResult();
            toret.success_count = res.success_count;
            toret.error_count = res.error_count;
            toret.errors = new MCEmailResult[ res.errors.Length ];
            i = 0;
            foreach(DummyMCEmailResult err in res.errors){
                MCEmailResult tmp = new MCEmailResult();
                //copy everything normal over
                tmp.code = err.code;
                tmp.message = err.message;
                tmp.row = new MCMemberInfo();
                tmp.row.email = err.email;
                toret.errors[i] = tmp;
                i++;
            }
            return toret;
        }//listBatchUnsubscribe()
        
        
        /**
         ** listUpdateMember method definitions
         **/
        public bool listUpdateMember(string listId, string email_address, MCMergeVar[] merges){
            return this.listUpdateMember(listId, email_address, merges, "html");
        }
        public bool listUpdateMember(string listId, string email_address, MCMergeVar[] merges, string email_type){
            return this.listUpdateMember(listId, email_address, merges, email_type, true);
        }
        public bool listUpdateMember(string listId, string email_address, MCMergeVar[] merges, string email_type, bool replace_interests){
            XmlRpcStruct mv = this.mergeArrayToStruct(merges);
            return this.api.listUpdateMember(this.apikey, listId, email_address, mv, email_type, replace_interests );
        }

        /**
         ** campaigns() method definitions
         **/
        public MCCampaign[] campaigns(){
            return this.campaigns("");
        }
        public MCCampaign[] campaigns(string filter_id){
            return this.campaigns(filter_id, "");
        }
        public MCCampaign[] campaigns(string filter_id, string filter_folder){
            return this.campaigns(filter_id, filter_folder, "");
        }
        public MCCampaign[] campaigns(string filter_id, string filter_folder, string filter_fromname){
            return this.campaigns(filter_id, filter_folder, filter_fromname, "");
        }
        public MCCampaign[] campaigns(string filter_id, string filter_folder, string filter_fromname, string filter_fromemail){
            return this.campaigns(filter_id, filter_folder, filter_fromname, filter_fromemail, "");
        }
        public MCCampaign[] campaigns(string filter_id, string filter_folder, string filter_fromname, string filter_fromemail, string filter_title){
            return this.campaigns(filter_id, filter_folder, filter_fromname, filter_fromemail, filter_title, "");
        }
        public MCCampaign[] campaigns(string filter_id, string filter_folder, string filter_fromname, string filter_fromemail, string filter_title, string filter_subject){
            return this.campaigns(filter_id, filter_folder, filter_fromname, filter_fromemail, filter_title, filter_subject, "");
        }
        public MCCampaign[] campaigns(string filter_id, string filter_folder, string filter_fromname, string filter_fromemail, string filter_title, string filter_subject, string filter_sendtimestart){
            return this.campaigns(filter_id, filter_folder, filter_fromname, filter_fromemail, filter_title, filter_subject, filter_sendtimestart, "");
        }
        public MCCampaign[] campaigns(string filter_id, string filter_folder, string filter_fromname, string filter_fromemail, string filter_title, string filter_subject, string filter_sendtimestart, string filter_sendtimeend){
            return this.campaigns(filter_id, filter_folder, filter_fromname, filter_fromemail, filter_title, filter_subject, filter_sendtimestart, filter_sendtimeend, false);
        }
        public MCCampaign[] campaigns(string filter_id, string filter_folder, string filter_fromname, string filter_fromemail, string filter_title, string filter_subject, string filter_sendtimestart, string filter_sendtimeend, bool filter_exact){
            return this.campaigns(filter_id, filter_folder, filter_fromname, filter_fromemail, filter_title, filter_subject, filter_sendtimestart, filter_sendtimeend, filter_exact, 0);
        }
        public MCCampaign[] campaigns(string filter_id, string filter_folder, string filter_fromname, string filter_fromemail, string filter_title, string filter_subject, string filter_sendtimestart, string filter_sendtimeend, bool filter_exact, int start){
            return this.campaigns(filter_id, filter_folder, filter_fromname, filter_fromemail, filter_title, filter_subject, filter_sendtimestart, filter_sendtimeend, filter_exact, start, 25);
        }
         
        public MCCampaign[] campaigns(string filter_id, string filter_folder, string filter_fromname, string filter_fromemail, string filter_title, string filter_subject, string filter_sendtimestart, string filter_sendtimeend, bool filter_exact, int start, int limit){
            try {
                return this.api.campaigns(this.apikey, filter_id, filter_folder, filter_fromname, filter_fromemail, filter_title, filter_subject, filter_sendtimestart, filter_sendtimeend, filter_exact, start, limit);
            } catch(XmlRpcTypeMismatchException){
                return new MCCampaign[0];
            }
        }
        
        /**
         ** campaignContent() method definition
         **/
        public MCCampaignContent campaignContent(string cid){
            return this.api.campaignContent(this.apikey, cid);
        }

        /**
         ** campaignFolders() method definition
         **/
        public MCCampaignFolder[] campaignFolders(){
            try {
            return this.api.campaignFolders(this.apikey);
            } catch(XmlRpcTypeMismatchException){
                return new MCCampaignFolder[0];
            }
        }

        /**
         ** campaignTemplates() method definition
         **/
        public MCTemplate[] campaignTemplates(){
            try {
                DummyMCTemplate[] tmp = this.api.campaignTemplates(this.apikey);
                MCTemplate[] toret = new MCTemplate[tmp.Length];
                int i = 0;
                foreach(DummyMCTemplate t in tmp){
                    toret[i].id =  t.id;
                    toret[i].name =  t.name;
                    toret[i].layout =  t.layout;
                    int j = 0;
                    if (t.sections is System.String[]){
                        toret[i].sections = new string[ ((System.String[])t.sections).Length ];
                        foreach (System.String name in (System.String[])t.sections){
                            toret[i].sections[j] = name;
                            j++;
                        }
                    } else if (t.sections is XmlRpcStruct){
                        XmlRpcStruct s = (XmlRpcStruct)t.sections;
                        toret[i].sections = new string[((Hashtable)s).Count];
                        foreach (string name in s.Keys){
                            toret[i].sections[j] = (string)s[name];
                            j++;
                        }
                    }
                    i++;
                }
                return toret;
            
            } catch(XmlRpcTypeMismatchException){
                return new MCTemplate[0];
            }
        }

        /**
         ** campaignAbuseReports() method definition
         **/
        public string[] campaignAbuseReports(string cid){
            return this.campaignAbuseReports(cid, 0);
        }
        public string[] campaignAbuseReports(string cid, int start){
            return this.campaignAbuseReports(cid, start, 1000);
        }
        public string[] campaignAbuseReports(string cid, int start, int limit){
            try {
                return this.api.campaignAbuseReports(this.apikey, cid, start, limit);
            } catch(XmlRpcTypeMismatchException){
                return new string[0];
            }
        }
        
        /**
         ** campaignClickStats() method definition
         **/
         public MCClickURL[] campaignClickStats(string cid){
             XmlRpcStruct s =  this.api.campaignClickStats(this.apikey, cid);
             MCClickURL[] urls = new MCClickURL[ ((Hashtable)s).Count ];
             int i = 0;
            foreach (string name in s.Keys){
                XmlRpcStruct tmp = (XmlRpcStruct)s[name];
                urls[i].url = name;
                urls[i].stats = new MCClickStats();
                foreach (string stat in tmp.Keys){
                    if (stat == "clicks"){
                        urls[i].stats.clicks = (int)tmp[stat];
                    }
                    if (stat == "unique"){
                        urls[i].stats.unique = (int)tmp[stat];
                    }                
                }
                i++;
            }
            return urls;
         }

        /**
         ** campaignHardBounces() method definition
         **/
        public string[] campaignHardBounces(string cid){
            return this.campaignHardBounces(cid, 0);
        }
        public string[] campaignHardBounces(string cid, int start){
            return this.campaignHardBounces(cid, start, 1000);
        }
        public string[] campaignHardBounces(string cid, int start, int limit){
            try {
                return this.api.campaignHardBounces(this.apikey, cid, start, limit);
            } catch(XmlRpcTypeMismatchException){
                return new string[0];
            }
        }

        /**
         ** campaignSoftBounces() method definition
         **/
        public string[] campaignSoftBounces(string cid){
            return this.campaignSoftBounces(cid, 0);
        }
        public string[] campaignSoftBounces(string cid, int start){
            return this.campaignSoftBounces(cid, start, 1000);
        }
        public string[] campaignSoftBounces(string cid, int start, int limit){
            try {
                return this.api.campaignSoftBounces(this.apikey, cid, start, limit);
            } catch(XmlRpcTypeMismatchException){
                return new string[0];
            }
        }
        
        /**
         ** campaignStats() method definition
         **/
        public MCCampaignStats campaignStats(string cid){
            return this.api.campaignStats(this.apikey, cid);
        }

        /**
         ** campaignUnsubscribes() method definition
         **/
        public string[] campaignUnsubscribes(string cid){
            return this.campaignUnsubscribes(cid, 0);
        }
        public string[] campaignUnsubscribes(string cid, int start){
            return this.campaignUnsubscribes(cid, start, 1000);
        }
        public string[] campaignUnsubscribes(string cid, int start, int limit){
            try {
                return this.api.campaignUnsubscribes(this.apikey, cid, start, limit);
            } catch(XmlRpcTypeMismatchException){
                return new string[0];
            }
        }
        
        /**
         ** campaignClickDetailAIM() method definition
         **/
        public MCAIMClickDetail[] campaignClickDetailAIM(string cid, string url){
            return this.campaignClickDetailAIM(cid, url, 0);
        }
        public MCAIMClickDetail[] campaignClickDetailAIM(string cid, string url, int start){
            return this.campaignClickDetailAIM(cid, url, start, 1000);
        }
        public MCAIMClickDetail[] campaignClickDetailAIM(string cid, string url, int start, int limit){
            try {
                return this.api.campaignClickDetailAIM(this.apikey, cid, url, start, limit);
            } catch(XmlRpcTypeMismatchException){
                return new MCAIMClickDetail[0];
            }
        }

        /**
         ** campaignEmailStatsAIM() method definition
         **/
        public MCAIMEmailDetail[] campaignEmailStatsAIM(string cid, string email){
            try {
                return this.api.campaignEmailStatsAIM(this.apikey, cid, email);
            } catch(XmlRpcTypeMismatchException){
                return new MCAIMEmailDetail[0];
            }                
        }
        
        /**
         ** campaignEmailStatsAIMAll() method definition
         **/
         public MCAIMEmail[] campaignEmailStatsAIMAll(string cid){
            return this.campaignEmailStatsAIMAll(cid, 0);
         }
         public MCAIMEmail[] campaignEmailStatsAIMAll(string cid, int start){
            return this.campaignEmailStatsAIMAll(cid, start, 1000);
         }
         public MCAIMEmail[] campaignEmailStatsAIMAll(string cid, int start, int limit){
            XmlRpcStruct s =  this.api.campaignEmailStatsAIMAll(this.apikey, cid, start, limit);
            MCAIMEmail[] emails = new MCAIMEmail[ ((Hashtable)s).Count ];
            int i = 0;
            foreach (string email in s.Keys){
                emails[i].email = email;
                Object[] details = (Object[])s[email];
                emails[i].details = new MCAIMEmailDetail[details.Length];
                int j = 0;
                foreach (XmlRpcStruct stat in details){
                    foreach(string name in stat.Keys){
                        if (name == "action"){
                            emails[i].details[j].action = (string)stat[name];
                        }
                        if (name == "timestamp"){
                            emails[i].details[j].timestamp = (string)stat[name];
                        }
                        if (name == "url"){
                            emails[i].details[j].url = (string)stat[name];
                        }
                    }
                    j++;
                }
                i++;
            }
            return emails;
         }

        /**
         ** campaignNotOpenedAIM() method definition
         **/
        public string[] campaignNotOpenedAIM(string cid){
            return this.campaignNotOpenedAIM(cid, 0);
        }
        public string[] campaignNotOpenedAIM(string cid, int start){
            return this.campaignNotOpenedAIM(cid, start, 1000);
        }
        public string[] campaignNotOpenedAIM(string cid, int start, int limit){
            try {
                return this.api.campaignNotOpenedAIM(this.apikey, cid, start, limit);
            } catch(XmlRpcTypeMismatchException){
                return new string[0];
            }
        }

        /**
         ** campaignEmailStatsAIM() method definition
         **/
        public MCAIMEmailOpen[] campaignOpenedAIM(string cid){
            return this.campaignOpenedAIM(cid, 0);
        }
        
        public MCAIMEmailOpen[] campaignOpenedAIM(string cid, int start){
            return this.campaignOpenedAIM(cid, start, 1000);
        }

        public MCAIMEmailOpen[] campaignOpenedAIM(string cid, int start, int limit){
            try {
                return this.api.campaignOpenedAIM(this.apikey, cid, start, limit);
            } catch(XmlRpcTypeMismatchException){
                return new MCAIMEmailOpen[0];
            }
        }
        
        /**
         ** campaignCreate() method definitions
         **/
        public string campaignCreate(string type, MCCampaignOpts options, MCCampaignContent content){
            MCSegmentOpts segment_opts = new MCSegmentOpts();
            return this.campaignCreate(type, options, content, segment_opts);
        }
        public string campaignCreate(string type, MCCampaignOpts options, MCCampaignContent content, MCSegmentOpts segment_opts){
            MCCampaignTypeOpts type_opts = new MCCampaignTypeOpts();
            return this.campaignCreate(type, options, content, segment_opts, type_opts );
        }
        public string campaignCreate(string type, MCCampaignOpts options, MCCampaignContent content, MCSegmentOpts segment_opts, MCCampaignTypeOpts type_opts){
            return this.api.campaignCreate(this.apikey, type, options, content, segment_opts, type_opts);
        }
        
        /**
         ** campaignUpdate() method definitions
         ** blargh, .Net, I hate you!
         **/
        public bool campaignUpdate(string cid, string name, string val){
            return this.api.campaignUpdateSV(this.apikey, cid, name, val);
        }
        public bool campaignUpdate(string cid, string name, int val){
            return this.api.campaignUpdateIV(this.apikey, cid, name, val);
        }        
        public bool campaignUpdate(string cid, string name, bool val){
            return this.api.campaignUpdateBV(this.apikey, cid, name, val);
        }        
        public bool campaignUpdate(string cid, string name, XmlRpcStruct val){
            return this.api.campaignUpdateXV(this.apikey, cid, name, val);
        }
        public bool campaignUpdate(string cid, string name, MCCampaignTracking val){
            return this.api.campaignUpdateMCCT(this.apikey, cid, name, val);
        }
        public bool campaignUpdate(string cid, string name, MCCampaignContent val){
            return this.api.campaignUpdateMCCC(this.apikey, cid, name, val);
        }
        public bool campaignUpdate(string cid, string name, MCSegmentOpts val){
            return this.api.campaignUpdateMCSO(this.apikey, cid, name, val);
        }
        public bool campaignUpdate(string cid, string name, MCCampaignTypeOpts val){
            return this.api.campaignUpdateMCTO(this.apikey, cid, name, val);
        }

        /**
         ** campaignSchedule() method definition
         **/
        public bool campaignSchedule(string cid, string schedule_time){
            return this.campaignSchedule(cid, schedule_time, "");
        }
        public bool campaignSchedule(string cid, string schedule_time, string schedule_time_b){
            return this.api.campaignSchedule(this.apikey, cid, schedule_time, schedule_time_b);
        }

        /**
         ** campaignUnschedule() method definition
         **/
        public bool campaignUnschedule(string cid){
            return this.api.campaignUnschedule(this.apikey, cid);
        }

        /**
         ** campaignSendTest() method definition
         **/
        public bool campaignSendTest(string cid, string[] test_emails){
            return this.campaignSendTest(cid, test_emails, "");
        }
        public bool campaignSendTest(string cid, string[] test_emails, string send_type){
            return this.api.campaignSendTest(this.apikey, cid, test_emails, send_type);
        }

        /**
         ** campaignSendNow() method definition
         **/
        public bool campaignSendNow(string cid){
            return this.api.campaignSendNow(this.apikey, cid);
        }

        /**
         ** campaignPause() method definition
         **/
        public bool campaignPause(string cid){
            return this.api.campaignPause(this.apikey, cid);
        }

        /**
         ** campaignResume() method definition
         **/
        public bool campaignResume(string cid){
            return this.api.campaignResume(this.apikey, cid);
        }

        /**
         ** campaignSegmentTest() method definition
         **/
        public int campaignSegmentTest(string id, MCSegmentOpts segment_opts){
            return this.api.campaignSegmentTest(this.apikey, id, segment_opts);
        }
        
        /**
         ** campaignReplicate() method definition
         **/
        public string campaignReplicate(string cid){
            return this.api.campaignReplicate(this.apikey, cid);
        }

        /**
         ** campaignDelete() method definition
         **/
        public bool campaignDelete(string cid){
            return this.api.campaignDelete(this.apikey, cid);
        }        

        /**
         ** inlineCss() method definitions
         **/
        public string inlineCss(string html){
            return this.inlineCss(html, false);
        }
        public string inlineCss(string html, bool strip_css){
            return this.api.inlineCss(this.apikey, html, strip_css);
        }
        
        /**
         ** generateText() method definition
         **/
        public string generateText(string type, string html){
            return this.api.generateText(this.apikey, type, html);
        }        
        
        /**
         ** ping() method definitions
         **/
        public string ping(){
            return this.api.ping(this.apikey);
        }        
        
        
        /**
         ** Misc Private Helper Methods
         **/
        private XmlRpcStruct mergeArrayToStruct(MCMergeVar[] merges){
            XmlRpcStruct mv = new XmlRpcStruct();
            foreach(MCMergeVar var in merges){
                if (var.tag != null)
                    mv.Add(var.tag, var.val);
            }
            return mv;
        }

    } //class

    public struct MCList
    {
      public string id;
      public int web_id;
      public string name;
      public string date_created;
      public double member_count; 
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]    
    public struct MCListMember
    {
      public string email;
      public string timestamp;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct MCMergeVar
    {
    [XmlRpcMissingMapping(MappingAction.Error)]
      public string tag;
      public string name;
      public bool   req;
      public string val;
    }

    public struct MCInterestGroups
    {
      public string name;
      public string form_field;
      public string[] groups;
    }

    public struct DummyMCMemberInfo
    {
      public string email;
      public string email_type;
      public System.Object merges;
      public string status;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public string timestamp;
    }
    
    public struct MCMemberInfo
    {
      public string email;
      public string email_type;
      public MCMergeVar[] merges;
      public string status;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public string timestamp;
    }

    public struct MCBatchResult
    {
      public int success_count;
      public int error_count;
      public MCEmailResult[] errors;
    }
    
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct MCEmailResult
    {
      public int code;
      public string message;
      public MCMemberInfo row;
    }
    
    public struct DummyMCBatchResult
    {
      public int success_count;
      public int error_count;
      public DummyMCEmailResult[] errors;
    }
    
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct DummyMCEmailResult
    {
      public int code;
      public string message;
      public System.Object row;
      public string email;
    }
    
    
    public struct MCCampaign
    {
         public string id;
         public string web_id;
         public string list_id;
         public double folder_id;
         public string title;
         public string type;
         public string create_time;
         public string send_time;
         public int emails_sent;
         public string status;
         public string from_name;
         public string from_email;
         public string subject;
         public string to_email;
         public string archive_url;
         public string inline_css;
    }

    public struct MCCampaignOpts
    {
        public string list_id;
        public string subject;
        public string from_email;
        public string from_name;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int template_id;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public MCCampaignTracking tracking;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string title;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool authenticate;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public XmlRpcStruct analytics;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool inline_css;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool generate_text;
    }    
        
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct MCSegmentOpts
    {
        public string match;
        public MCSegmentCond[] conditions;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct MCSegmentCond
    {
        public string field; 
        public string op;
        public string value;
    }
    
    //This is not well typed by any means.
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct MCCampaignTypeOpts
    {
        // RSS Only
        public string url;
        
        // ABSplit Only
        public string split_test;
        public string pick_winner;
        public int wait_units;
        public int wait_time;
        public int split_size;
        public string from_name_a;
        public string from_name_b;
        public string from_email_a;
        public string from_email_b;
        public string subject_a;
        public string subject_b;
    }
    
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct MCCampaignContent
    {
        [XmlRpcMissingMapping(MappingAction.Error)]
        public string text;
        public string html;
        public string html_main;
        public string html_sidecolumn;
        public string html_header;
        public string html_footer;
        public string url;
    }
    
    public struct MCCampaignTracking
    {
        public bool opens;
        public bool html_clicks;
        public bool text_clicks;
    }
    
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct MCCampaignAnalytics
    {
        public string type;
        public string key;
    }
    
    public struct MCCampaignFolder
    {
        public double folder_id;
        public string name;
    }

    public struct MCTemplate
    {
        public double id;
        public string name;
        public string layout;
        public string[] sections;    
    }
    public struct DummyMCTemplate
    {
        public double id;
        public string name;
        public string layout;
        public Object sections;
    }
    
    public struct MCClickURL
    {
        public string url;
        public MCClickStats stats;
    }
    public struct MCClickStats
    {
        public double clicks;
        public double unique;
    }

    public struct MCCampaignStats
    {
        public double syntax_errors;
        public double hard_bounces;
        public double soft_bounces;
        public double unsubscribes;
        public double abuse_reports;
        public double forwards;
        public double forwards_opens;
        public double opens;
        public string last_open;
        public int unique_opens;
        public int clicks;
        public int unique_clicks;
        public string last_click;
        public double users_who_clicked;
        public double emails_sent;
    }
    
    public struct MCAIMClickDetail
    {
        public string email;
        public string clicks;
    }
    
    public struct MCAIMEmail
    {
        public string email;
        [XmlRpcMissingMapping(MappingAction.Error)]
        public MCAIMEmailDetail detail;
        public MCAIMEmailDetail[] details;
    }

    public struct MCAIMEmailDetail
    {
        public string action;
        public string timestamp;
        public string url;
    }
    
    public struct MCAIMEmailOpen
    {
        public string email;
        public string open_count;
    }
    
    public struct MCApiKey
    {
        public string apikey;
        public string created_at;
        public string expired_at;
    }
    
    [XmlRpcUrl("http://api.mailchimp.com/1.1/")]
    public interface IMC : IXmlRpcProxy
    {
        [XmlRpcMethod("lists")]
        MCList[] lists(string apikey);

        [XmlRpcMethod("listMergeVars")]
        MCMergeVar[] listMergeVars(string apikey, string id);

        [XmlRpcMethod("listMergeVarAdd")]
        bool listMergeVarAdd(string apikey, string id, string tag, string name, bool req);

        [XmlRpcMethod("listMergeVarDel")]
        bool listMergeVarDel(string apikey, string id, string tag);

        [XmlRpcMethod("listInterestGroups")]
        MCInterestGroups listInterestGroups(string apikey, string id);

        [XmlRpcMethod("listInterestGroupAdd")]
        bool listInterestGroupAdd(string apikey, string id, string group_name);

        [XmlRpcMethod("listInterestGroupDel")]
        bool listInterestGroupDel(string apikey, string id, string group_name);


        [XmlRpcMethod("listMemberInfo")]
        DummyMCMemberInfo listMemberInfo(string apikey, string id, string email);

        [XmlRpcMethod("listMembers")]
        MCListMember[] listMembers(string apikey, string id, string status, int start, int limit);

        [XmlRpcMethod("listSubscribe")]
        bool listSubscribe(string apikey, string id, string email_address, Hashtable mv, string email_type, bool double_optin);

        [XmlRpcMethod("listUnsubscribe")]
        bool listUnsubscribe(string apikey, string id, string email_address, bool delete_member, bool send_goodbye, bool send_notify);

        [XmlRpcMethod("listBatchSubscribe")]
        DummyMCBatchResult listBatchSubscribe(string apikey, string id, Hashtable batch, bool double_optin, bool update_existing, bool replace_interests);

        [XmlRpcMethod("listBatchUnsubscribe")]        
        DummyMCBatchResult listBatchUnsubscribe(string apikey, string id, Hashtable batch, bool delete_member, bool send_goodbye, bool send_notify);

        [XmlRpcMethod("listUpdateMember")]
        bool listUpdateMember(string apikey, string id, string email, Hashtable mv, string email_type, bool replace_interests);

        [XmlRpcMethod("campaigns")]
        MCCampaign[] campaigns(string apikey, string filter_id, string filter_folder, string filter_fromname, string filter_fromemail, string filter_title, string filter_subject, string filter_sendtimestart, string filter_sendtimeend, bool filter_exact, int start, int limit);

        [XmlRpcMethod("campaignContent")]
        MCCampaignContent campaignContent(string apikey, string cid);

        [XmlRpcMethod("campaignFolders")]
        MCCampaignFolder[] campaignFolders(string apikey);

        [XmlRpcMethod("campaignTemplates")]
        DummyMCTemplate[] campaignTemplates(string apikey);

        [XmlRpcMethod("campaignAbuseReports")]
        string[] campaignAbuseReports(string apikey, string cid, int start, int limit);

        [XmlRpcMethod("campaignClickStats")]
        XmlRpcStruct campaignClickStats(string apikey, string cid);

        [XmlRpcMethod("campaignHardBounces")]
        string[] campaignHardBounces(string apikey, string cid, int start, int limit);

        [XmlRpcMethod("campaignSoftBounces")]
        string[] campaignSoftBounces(string apikey, string cid, int start, int limit);

        [XmlRpcMethod("campaignStats")]
        MCCampaignStats campaignStats(string apikey, string cid);

        [XmlRpcMethod("campaignUnsubscribes")]
        string[] campaignUnsubscribes(string apikey, string cid, int start, int limit);

        [XmlRpcMethod("campaignClickDetailAIM")]
        MCAIMClickDetail[] campaignClickDetailAIM(string apikey, string cid, string url, int start, int limit);

        [XmlRpcMethod("campaignEmailStatsAIM")]
        MCAIMEmailDetail[] campaignEmailStatsAIM(string apikey, string cid, string email);

        [XmlRpcMethod("campaignEmailStatsAIMAll")]
        XmlRpcStruct campaignEmailStatsAIMAll(string apikey, string cid, int start, int limit);

        [XmlRpcMethod("campaignNotOpenedAIM")]
        string[] campaignNotOpenedAIM(string apikey, string cid, int start, int limit);

        [XmlRpcMethod("campaignOpenedAIM")]
        MCAIMEmailOpen[] campaignOpenedAIM(string apikey, string cid, int start, int limit);

        [XmlRpcMethod("campaignCreate")]
        string campaignCreate(string apikey, string type, MCCampaignOpts opts, MCCampaignContent content, MCSegmentOpts segment_opts, MCCampaignTypeOpts type_opts);

        [XmlRpcMethod("campaignUpdate")]
        bool campaignUpdateSV(string apikey, string cid, string name, string val);

        [XmlRpcMethod("campaignUpdate")]
        bool campaignUpdateIV(string apikey, string cid, string name, int val);

        [XmlRpcMethod("campaignUpdate")]
        bool campaignUpdateBV(string apikey, string cid, string name, bool val);

        [XmlRpcMethod("campaignUpdate")]
        bool campaignUpdateXV(string apikey, string cid, string name, XmlRpcStruct val);

        [XmlRpcMethod("campaignUpdate")]
        bool campaignUpdateMCCT(string apikey, string cid, string name, MCCampaignTracking val);

        [XmlRpcMethod("campaignUpdate")]
        bool campaignUpdateMCCC(string apikey, string cid, string name, MCCampaignContent content);
        
        [XmlRpcMethod("campaignUpdate")]
        bool campaignUpdateMCSO(string apikey, string cid, string name, MCSegmentOpts segment_opts);

        [XmlRpcMethod("campaignUpdate")]
        bool campaignUpdateMCTO(string apikey, string cid, string name, MCCampaignTypeOpts type_opts);

        [XmlRpcMethod("campaignSchedule")]
        bool campaignSchedule(string apikey, string cid, string schedule_time, string schedule_time_b);

        [XmlRpcMethod("campaignUnschedule")]
        bool campaignUnschedule(string apikey, string cid);

        [XmlRpcMethod("campaignSendTest")]
        bool campaignSendTest(string apikey, string cid, string[] test_emails, string send_type);
    
        [XmlRpcMethod("campaignSendNow")]
        bool campaignSendNow(string apikey, string cid);

        [XmlRpcMethod("campaignPause")]
        bool campaignPause(string apikey, string cid);

        [XmlRpcMethod("campaignResume")]
        bool campaignResume(string apikey, string cid);
        
        [XmlRpcMethod("campaignSegmentTest")]
        int campaignSegmentTest(string apikey, string id, MCSegmentOpts segment_opts);

        [XmlRpcMethod("campaignReplicate")]
        string campaignReplicate(string apikey, string cid);

        [XmlRpcMethod("campaignDelete")]
        bool campaignDelete(string apikey, string cid);
        
        [XmlRpcMethod("inlineCss")]
        string inlineCss(string apikey, string html, bool strip_css);

        [XmlRpcMethod("generateText")]
        string generateText(string apikey, string type, string html);

        [XmlRpcMethod("closeOneOhSecurityHole")]
        bool closeOneOhSecurityHole(string username, string password, string apikey);

        [XmlRpcMethod("apikeys")]
        MCApiKey[] apikeys(string username, string password, string apikey, bool expired);

        [XmlRpcMethod("apikeyAdd")]
        string apikeyAdd(string username, string password, string apikey);

        [XmlRpcMethod("apikeyExpire")]
        bool apikeyExpire(string username, string password, string apikey);

        [XmlRpcMethod("ping")]
        string ping(string apikey);
    
    }

} //MailChimp()

