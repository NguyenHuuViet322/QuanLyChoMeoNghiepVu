using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DAS.Application.Models.MobileApiModel
{
    public class VMMobileCatalogingDoc
    {
        public int Id { get; set; }

        public long IdFile { get; set; }

        // public long IdDoc { get; set; }

        public int IdCatalogingProfile { get; set; }
        //ID bang ho so                
        public int IdDocType { get; set; } = 0; //IDLoai tai l
                                                //ieu
        public int Status { get; set; } = 1;
        [JsonIgnore]
        public DateTime? ApproveDate { get; set; }

        #region Result Column
        [JsonIgnore]
        public int IdOrgan { get; set; }
        public int NumbericalOrder { get; set; }
        public string ProfileName { get; set; }
        public string ProfileCode { get; set; }
        public string CodeElementProfile { get; set; }
        public string ElementProfile { get; set; }

        #endregion Result Column
    }
}
